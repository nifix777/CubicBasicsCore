using System;
using Cubic.Core.Text;

namespace Cubic.Shared.Core.Net.Ftp
{
  public static class FtpUtils
  {
    public const string FtpScheme = @"ftp://";
    public const string HttpScheme = @"http://";

    public const int FtpDefaultPort = 21;

    public static Uri BuildFtpUri(FtpOptions options, string path)
    {
      //var uri = options.Port == FtpUtils.FtpDefaultPort
      //? options.Host
      //: string.Format("{0}:{1}", options.Host, options.Port);

      var uri = options.Host.StartsWith(FtpScheme) ? options.Host : string.Format("{0}{1}", FtpScheme, options.Host);

      if (!path.IsNullOrEmpty())
      {
        uri = string.Format("{0}{1}", uri, path.EscapeUri());
      }

      return new Uri(uri);
    }

    public static Uri GetHttpUri(FtpOptions options, string path, bool includeCredentials = false)
    {
      var ftpUri = FtpUtils.BuildFtpUri(options, path);

      var httpUri = ftpUri.ToString().Replace(FtpScheme, HttpScheme);

      if (!includeCredentials && !ftpUri.UserInfo.IsNullOrEmpty())
      {
        httpUri = httpUri.Replace(ftpUri.UserInfo, "");
      }

      if (!options.HttpBaseUri.IsNullOrEmpty())
      {
        httpUri = httpUri.Replace(ftpUri.Host, options.HttpBaseUri);
      }

      return new Uri(httpUri);
    }
  }
}