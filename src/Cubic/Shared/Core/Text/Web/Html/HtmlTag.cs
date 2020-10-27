
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Text.Web.Html
{
  public class HtmlTag
  {
    private readonly Dictionary<string, HtmlAttribute> _attributes = new Dictionary<string, HtmlAttribute>();
    private readonly List<HtmlTag> _children = new List<HtmlTag>();
    private string _innerText;

    public HtmlTag(string tag)
    {
      Tag = tag.ToLower();
    }

    public string Tag { get; }
    public string InnerText => _innerText;

    public IEnumerable<HtmlTag> Children => _children;

    public string Attr(string key)
    {
      HtmlAttribute attr;

      if (_attributes.TryGetValue(key, out attr))
      {
        return attr.Value;
      }

      return null;
    }

    public HtmlTag Attr(string key, string value)
    {
      var attr = new HtmlAttribute(key, value);
      _attributes[key] = attr;
      return this;
    }

    public HtmlTag Append(HtmlTag tag)
    {
      _children.Add(tag);
      return this;
    }

    public HtmlTag Append(params HtmlTag[] tags)
    {
      foreach (var tag in tags)
      {
        Append(tag);
      }

      return this;
    }

    public HtmlTag Text(string text)
    {
      _innerText = text;
      return this;
    }

    public virtual async Task WriteHtml(HtmlWriter writer)
    {
      await WriteBeginningTag(writer);
      await WriteContent(writer);
      await WriteEndingTag(writer);
    }

    public virtual void AppendHtml(Utf8StringBuilder builder)
    {
      AppendBeginningTag(builder);
      AppendContent(builder);
      AppendEndingTag(builder);
    }

    protected virtual async Task WriteBeginningTag(HtmlWriter writer)
    {
      await writer.WriteAsync($"<{Tag}");
      foreach (var attr in _attributes.Values)
      {
        await writer.WriteAsync(" ");
        await attr.WriteHtml(writer);
      }
      await writer.WriteAsync(">");
    }

    protected virtual void AppendBeginningTag(Utf8StringBuilder writer)
    {
      writer.Append($"<{Tag}");
      foreach (var attr in _attributes.Values)
      {
        writer.Append(" ");
        attr.AppendHtml(writer);
      }
      writer.Append(">");
    }

    protected virtual async Task WriteContent(HtmlWriter writer)
    {
      if (_innerText != null)
      {
        await writer.WriteAsync(_innerText);
      }

      foreach (var child in Children)
      {
        await child.WriteHtml(writer);
      }
    }

    protected virtual void AppendContent(Utf8StringBuilder builder)
    {
      if (_innerText != null)
      {
        builder.Append(_innerText);
      }

      foreach (var child in Children)
      {
        child.AppendHtml(builder);
      }
    }

    protected virtual async Task WriteEndingTag(HtmlWriter writer)
    {
      await writer.WriteAsync($"</{Tag}>");
    }

    protected virtual void AppendEndingTag(Utf8StringBuilder builder)
    {
      builder.Append($"</{Tag}>");
    }

    public override string ToString()
    {
      //using (var stream = new MemoryStream())
      //{
      //  var writer = new HtmlWriter(stream);

      //  WriteHtml(writer).GetAwaiter().GetResult();

      //  stream.Position = 0;
      //  var result = Encoding.UTF8.GetString(stream.ToArray());
      //  return result;
      //}

      var builder = new Utf8StringBuilder();

      AppendHtml(builder);

      return builder.ToString();
    }
  }


}
