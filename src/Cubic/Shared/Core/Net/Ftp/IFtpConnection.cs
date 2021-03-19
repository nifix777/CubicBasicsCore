using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Cubic.Shared.Core.Net.Ftp
{
  public interface IFtpConnection
  {
    Task UploadAsync(string localFile, string remoteFile, IProgress<int> progress = null, CancellationToken cancellationToken = default(CancellationToken));
    Task UploadAsync(Stream localStream, string remoteFile, IProgress<int> progress = null, CancellationToken cancellationToken = default(CancellationToken));
    Task DownloadAsync(string localFile, string remoteFile, IProgress<int> progress = null, CancellationToken cancellationToken = default(CancellationToken));
    Task DeleteAsync(string file,  CancellationToken cancellationToken = default(CancellationToken));
    Task RenameAsync(string fileName, string newFileName,  CancellationToken cancellationToken = default(CancellationToken));
    Task CreateAsync(string directory,  CancellationToken cancellationToken = default(CancellationToken));
    Task<FtpResult> TryCreateAsync(string directory, CancellationToken cancellationToken = default(CancellationToken));
    Task<bool> ExistsAsync(string directory,  CancellationToken cancellationToken = default(CancellationToken));
    Task<DateTime> GetTimestampAsync(string file,  CancellationToken cancellationToken = default(CancellationToken));
    Task<long> GetFileSizeAsync(string file,  CancellationToken cancellationToken = default(CancellationToken));
    Task<IEnumerable<string>> ListDirectoryAsync(string directory,  CancellationToken cancellationToken = default(CancellationToken));

    string GetHttpUri(string path, bool includeCredentials = false);
  }
}