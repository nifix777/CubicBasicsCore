using System;
using System.Net;
using Cubic.Core.Net;
using Cubic.Core.Security;

namespace Cubic.Shared.Core.Net.Ftp
{
  public class FtpOptions
  {
    public string Host { get; set; } = string.Empty;

    public int Port { get; set; } = 21;

    public NetworkCredential Credentail { get; set; }

    public int BufferSize { get; set; } = 2048;

    public bool KeepAlive { get; set; }

    public bool UseBinary { get; set; }

    public bool UsePassive { get; set; }

    public bool UseSsl { get; set; }

    public int Timeout { get; set; } = System.Threading.Timeout.Infinite;

    public string HttpBaseUri { get; set; }
  }
}