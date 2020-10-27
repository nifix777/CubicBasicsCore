
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Text.Web.Html
{

  public class ScriptTag : HtmlTag
  {
    public ScriptTag(string src) : base("script")
    {
      Attr("type", "text/javascript");
      Attr("src", src);
    }
  }


}
