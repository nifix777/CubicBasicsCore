using System.IO;

namespace Cubic.Core.Text.Web.Html
{
  public interface IHtmlEncoder
  {
    string Encode(string text);
    void Encode(string text, TextWriter writer);
  }

  public class HtmlEncoder
  {
    public static IHtmlEncoder Default = new SystemWebHtmlDecoder();
  }
}
