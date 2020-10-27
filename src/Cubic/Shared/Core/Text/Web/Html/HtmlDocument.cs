
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Text.Web.Html
{

  public class HtmlDocument : HtmlTag
  {
    private HtmlTag _title;

    public HtmlDocument() : base("html")
    {
      DocType = "<!DOCTYPE html>";
      Head = new HtmlTag("head");
      Body = new HtmlTag("body");

      _title = new HtmlTag("title");

      Head.Append(new HtmlTag("meta").Attr("viewport", "width=device-width"));
      Head.Append(_title);

      Append(Head, Body);
    }

    public string DocType { get; set; }
    public HtmlTag Head { get; }
    public HtmlTag Body { get; }

    public string Title
    {
      get { return _title.InnerText; }
      set { _title.Text(value); }
    }

    protected override async Task WriteBeginningTag(HtmlWriter writer)
    {
      await writer.WriteAsync(DocType);
      await base.WriteBeginningTag(writer);
    }
  }


}
