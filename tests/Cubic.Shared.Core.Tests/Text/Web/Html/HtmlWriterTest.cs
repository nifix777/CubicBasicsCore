using System;
using System.Diagnostics;
using System.IO;
using Cubic.Core.Execution.Transient;
using Cubic.Core.Text;
using Cubic.Core.Text.Web.Html;
using Xunit;

namespace Cubic.Basics.Testing.Text.Web.Html
{
  
  public class HtmlWriterTest
  {
    private string htmlTitle = "TestDocument";

    private string Html;

    private HtmlDocument CreateHtml()
    {
      var document = new HtmlDocument() { Title = htmlTitle };
      document.Head.Append(new CssTag("https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.3.7/css/bootstrap.min.css"));
      document.Head.Append(new CssTag("https://cdnjs.cloudflare.com/ajax/libs/prism/1.6.0/themes/prism.min.css"));
      document.Head.Append(new CssTag("https://cdnjs.cloudflare.com/ajax/libs/prism/1.6.0/themes/prism-okaidia.min.css"));
      document.Head.Append(new CssTag("https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css"));
      document.Body.Append(new HtmlTag("div").Attr("id", "root"));

      var initialData = new HtmlTag("script")
          .Attr("type", "text/javascript")
          .Text("{ var DiagnosticsSettings = { websocketAddress: '127.0.0.1' } }");

      document.Body.Append(initialData);

      document.Body.Append(new ScriptTag($"https://cdnjs.cloudflare.com/ajax/libs/prism/1.6.0/prism.js"));

      return document;
    }

    //[Fact]
    public void TestHtmlWriter()
    {
      using (var enviroment = new TestEnviroment())
      {
        var dir = enviroment.CreateDirectory("html");

        var htmlFile = dir.CreateFile("test.hmtl");

        using (var writer = new HtmlWriter(htmlFile.WriteableStream))
        {
          var doc = CreateHtml();
          doc.WriteHtml(writer).GetAwaiter().GetResult();

        }

        var info = new FileInfo(htmlFile.FilePath);
        Assert.True(info.Length > 0);
      }
    }


    [Fact]
    public void TestHtmlDocument()
    {
      var doc = CreateHtml();

      Html = doc.ToString();

      Trace.Write(Html);

      Assert.True(Html.Contains(htmlTitle));
      Assert.True(Html.Contains("text/javascript"));
      Assert.True(Html.Contains("127.0.0.1"));
    }

    [Fact]
    public void TestUtf8StringBuilder()
    {
      var builder = new Utf8StringBuilder();

      var doc = CreateHtml();

      doc.AppendHtml(builder);

      var utf8 = builder.ToUtf8String();

      Trace.Write(utf8);

      Html = builder.ToString();

      Assert.True(Html.Contains(htmlTitle));
      Assert.True(Html.Contains("text/javascript"));
      Assert.True(Html.Contains("127.0.0.1"));
    }
  }
}
