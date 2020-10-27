using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Cubic.Core;
using Cubic.Core.Collections;
using Cubic.Core.Text;

namespace Cubic.Core.Net.Ftp
{
  public class FtpConnection : IFtpConnection
  {
    private FtpOptions _options;


    public FtpConnection(FtpOptions options)
    {
      _options = options;
    }

    private Uri BuildUri(string path = "")
    {
      return FtpUtils.BuildFtpUri(_options, path);
    }

    private FtpWebRequest CreateRequest(string path = "")
    {

      var reqeustUri = BuildUri(path);

      if(reqeustUri.Scheme != Uri.UriSchemeFtp) throw new InvalidOperationException(string.Format("Uri '{0}' is not a valid FTP-Uri"));

      var request = (FtpWebRequest)FtpWebRequest.Create(reqeustUri);

      if (_options.Credentail != null)
      {
        request.Credentials = new NetworkCredential(_options.Credentail.UserName, _options.Credentail.Password);
      }

      if (_options.KeepAlive)
      {
        request.KeepAlive = true;
      }

      if (_options.UsePassive)
      {
        request.UsePassive = true;
      }

      if (_options.UseBinary)
      {
        request.UseBinary = true;
      }

      if (_options.UseSsl)
      {
        request.EnableSsl = true;
      }

      if (_options.Timeout != Timeout.Infinite)
      {
        request.Timeout = _options.Timeout;
      }

      return request;
    }

    public async Task UploadAsync(string localFile, string remoteFile, IProgress<int> progress = null, CancellationToken cancellationToken = new CancellationToken())
    {
      var request = CreateRequest(remoteFile);
      request.Method = WebRequestMethods.Ftp.UploadFile;

      using (var ftpStream = await request.GetRequestStreamAsync())
      {
        using (var file = new FileStream(localFile, FileMode.Open))
        {
          var buffer = SharedPools.ByteArray.Rent(_options.BufferSize);
          int bytesToSend = await file.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

          while (bytesToSend != 0 && !cancellationToken.IsCancellationRequested)
          {

            await ftpStream.WriteAsync(buffer, 0, bytesToSend, cancellationToken);

            if (progress != null)
            {
              progress.Report(bytesToSend);
            }

            bytesToSend = await file.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
          }
        }
      }
    }

    public async Task UploadAsync(Stream localStream, string remoteFile, IProgress<int> progress = null, CancellationToken cancellationToken = new CancellationToken())
    {
      var request = CreateRequest(remoteFile);
      request.Method = WebRequestMethods.Ftp.UploadFile;

      using (var ftpStream = await request.GetRequestStreamAsync())
      {
        var buffer = SharedPools.ByteArray.Rent(_options.BufferSize);
        int bytesToSend = await localStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

        while (bytesToSend != 0 && !cancellationToken.IsCancellationRequested)
        {

          await ftpStream.WriteAsync(buffer, 0, bytesToSend, cancellationToken);

          if (progress != null)
          {
            progress.Report(bytesToSend);
          }

          bytesToSend = await localStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
        }
      }
    }

    public async Task DownloadAsync(string localFile, string remoteFile, IProgress<int> progress = null, CancellationToken cancellationToken = new CancellationToken())
    {
      var request = CreateRequest(remoteFile);
      request.Method = WebRequestMethods.Ftp.DownloadFile;

      using (var ftpStream = await request.GetRequestStreamAsync())
      {
        using (var file = new FileStream(localFile, FileMode.Create))
        {
          var buffer = SharedPools.ByteArray.Rent(_options.BufferSize);
          int bytesToWrite = await ftpStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

          while (bytesToWrite != 0 && !cancellationToken.IsCancellationRequested)
          {

            await file.WriteAsync(buffer, 0, bytesToWrite, cancellationToken);
            bytesToWrite = await ftpStream.ReadAsync(buffer, 0, buffer.Length);
          }
        }
      }
    }

    public async Task DownloadAsync(Stream localStream, string remoteFile, IProgress<int> progress = null, CancellationToken cancellationToken = new CancellationToken())
    {
      var request = CreateRequest(remoteFile);
      request.Method = WebRequestMethods.Ftp.DownloadFile;

      using (var ftpStream = await request.GetRequestStreamAsync())
      {
        var buffer = SharedPools.ByteArray.Rent(_options.BufferSize);
        int bytesToSend = await ftpStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

        while (bytesToSend != 0 && !cancellationToken.IsCancellationRequested)
        {

          await localStream.WriteAsync(buffer, 0, bytesToSend, cancellationToken);

          if (progress != null)
          {
            progress.Report(bytesToSend);
          }

          bytesToSend = await ftpStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
        }
      }
    }

    public async Task DeleteAsync(string file, CancellationToken cancellationToken = new CancellationToken())
    {
      var request = CreateRequest(file);
      request.Method = WebRequestMethods.Ftp.DeleteFile;

      var response = (FtpWebResponse)await request.GetResponseAsync();

      if (response.StatusCode != FtpStatusCode.CommandOK)
      {
        throw new FtpException(response.StatusDescription, response.StatusCode.ToInt32());
      }
    }

    public async Task RenameAsync(string fileName, string newFileName, CancellationToken cancellationToken = new CancellationToken())
    {
      var request = CreateRequest(fileName);
      request.Method = WebRequestMethods.Ftp.Rename;
      request.RenameTo = newFileName;
      var response = (FtpWebResponse)await request.GetResponseAsync();

      if (response.StatusCode != FtpStatusCode.CommandOK)
      {
        throw new FtpException(response.StatusDescription, response.StatusCode.ToInt32());
      }
    }

    public async Task CreateAsync(string directory, CancellationToken cancellationToken = new CancellationToken())
    {
      var request = CreateRequest(directory);
      request.Method = WebRequestMethods.Ftp.MakeDirectory;
      var response = (FtpWebResponse)await request.GetResponseAsync();

      if (response.StatusCode != FtpStatusCode.CommandOK)
      {
        throw new FtpException(response.StatusDescription, response.StatusCode.ToInt32());
      }
    }

    public async Task<bool> ExistsAsync(string directory, CancellationToken cancellationToken = new CancellationToken())
    {
      var request = CreateRequest(directory);
      request.Method = WebRequestMethods.Ftp.ListDirectory;
      var response = (FtpWebResponse)await request.GetResponseAsync();

      if (response.StatusCode == FtpStatusCode.CommandOK)
      {
        return true;
      }

      return false;
    }

    public async Task<FtpResult> TryCreateAsync(string directory, CancellationToken cancellationToken = new CancellationToken())
    {
      var request = CreateRequest(directory);
      request.Method = WebRequestMethods.Ftp.MakeDirectory;
      var response = (FtpWebResponse)await request.GetResponseAsync();

      if (response.StatusCode == FtpStatusCode.CommandOK)
      {
        return FtpResult.Ok;
      }

      return FtpResult.Error(new FtpException(response.StatusDescription, response.StatusCode.ToInt32()));
    }

    public async Task<DateTime> GetTimestampAsync(string file, CancellationToken cancellationToken = new CancellationToken())
    {
      var request = CreateRequest(file);
      request.Method = WebRequestMethods.Ftp.GetDateTimestamp;
      var response = (FtpWebResponse)await request.GetResponseAsync();

      if (response.StatusCode != FtpStatusCode.CommandOK)
      {
        throw new FtpException(response.StatusDescription, response.StatusCode.ToInt32());
      }

      return response.LastModified;
    }

    public async Task<long> GetFileSizeAsync(string file, CancellationToken cancellationToken = new CancellationToken())
    {
      var request = CreateRequest(file);
      request.Method = WebRequestMethods.Ftp.GetFileSize;
      var response = (FtpWebResponse)await request.GetResponseAsync();

      if (response.StatusCode != FtpStatusCode.CommandOK)
      {
        throw new FtpException(response.StatusDescription, response.StatusCode.ToInt32());
      }

      return response.ContentLength;
    }

    public async Task<IEnumerable<string>> ListDirectoryAsync(string directory, CancellationToken cancellationToken = new CancellationToken())
    {
      var request = CreateRequest(directory);
      request.Method = WebRequestMethods.Ftp.ListDirectory;

      var list = new List<string>();

      using (var ftpStream = await request.GetRequestStreamAsync())
      {
        using (var reader = new StreamReader(ftpStream))
        {
          var dir = await reader.ReadToEndAsync();
          list.AddRange(dir.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries));
        }
      }

      return list;
    }

    public string GetHttpUri(string path, bool includeCredentials = false)
    {
      var ftpUri = BuildUri(path);

      var httpUri = ftpUri.ToString().Replace(FtpUtils.FtpScheme, FtpUtils.HttpScheme);

      if (!includeCredentials && !ftpUri.UserInfo.IsNullOrEmpty())
      {
        httpUri = httpUri.Replace(ftpUri.UserInfo, "");
      }

      if (!_options.HttpBaseUri.IsNullOrEmpty())
      {
        httpUri = httpUri.Replace(ftpUri.Host, _options.HttpBaseUri);
      }

      return httpUri;
    }
  }
}