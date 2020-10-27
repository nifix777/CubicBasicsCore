using System.IO;

namespace Cubic.Core.Text.Web.Html
{
  public class SystemWebHtmlDecoder : IHtmlEncoder
  {
    public string Encode(string text)
    {
      return System.Net.WebUtility.HtmlEncode(text);
    }

    public void Encode(string text, TextWriter writer)
    {
      System.Net.WebUtility.HtmlEncode(text, writer);
    }
  }
}
