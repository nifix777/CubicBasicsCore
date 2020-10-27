using System;
using System.Diagnostics;
using Cubic.Core.Diagnostics;
using Cubic.Core.Net;
using Cubic.Core.Security;
using Cubic.Core.Net.Ftp;
using Xunit;

namespace Cubic.Core.Testing.Net
{
  
  public class FtpTests 
  {
    [Fact]
    public void FtpToHttpUriNoUserInfo()
    {
      var example = "ftp://expertenftp11@ftp.ft.sage-experten.de/filetransfer/Motec/OLAbfSAGAddOnsIII.rar";
      var input = "/filetransfer/Motec/OLAbfSAGAddOnsIII.rar";
      var expected = "http://www.sage-experten.de/filetransfer/Motec/OLAbfSAGAddOnsIII.rar";

      var options = new FtpOptions();
      options.Credentail = new System.Net.NetworkCredential("test","password");
      options.Host = "ftp://ftp.ft.sage-experten.de";
      options.HttpBaseUri = "www.sage-experten.de";

      var httpUri = FtpUtils.GetHttpUri(options, input).ToString();

      Trace.Write("Input:      ");
      example.Dump();

      Trace.Write("Expected: ");
      expected.Dump();

      Trace.Write("Result:     ");
      httpUri.Dump();

      Assert.Equal(expected, httpUri);
    }

  }
}
