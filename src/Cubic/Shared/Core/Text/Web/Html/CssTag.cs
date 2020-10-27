
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Text.Web.Html
{

  public class CssTag : HtmlTag
  {
    public CssTag(string href) : base("link")
    {
      Attr("rel", "stylesheet");
      Attr("href", href);
      Attr("type", "text/css");
      Attr("media", "all");
    }
  }


}
