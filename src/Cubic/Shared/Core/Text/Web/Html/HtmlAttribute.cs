
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Text.Web.Html
{

  public class HtmlAttribute
  {
    public HtmlAttribute(string key, string value)
    {
      Key = key;
      Value = value;
    }

    public string Key { get; }
    public string Value { get; }

    public async Task WriteHtml(HtmlWriter writer)
    {
      if (Value != null)
      {
        await writer.WriteAsync($"{Key}=\"{Value}\"");
      }
      else
      {
        await writer.WriteAsync($"{Key}");
      }
    }

    public void AppendHtml(Utf8StringBuilder writer)
    {
      if (Value != null)
      {
        writer.Append($"{Key}=\"{Value}\"");
      }
      else
      {
        writer.Append($"{Key}");
      }
    }
  }


}
