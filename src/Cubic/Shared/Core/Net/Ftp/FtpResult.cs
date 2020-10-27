using Cubic.Core.Net.Ftp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Net.Ftp
{
  public class FtpResult
  {
    private static FtpResult _success = new FtpResult(true, null);
    protected FtpResult(bool success, FtpException exception)
    {
      Success = success;
      Exception = exception ?? throw new ArgumentNullException(nameof(exception));
    }

    public bool Success { get; }

    public FtpException Exception { get; }

    public static FtpResult Ok => _success;
    public static FtpResult Error(FtpException ftpException)
    {
      return new FtpResult(false, ftpException);
    }
  }
}
