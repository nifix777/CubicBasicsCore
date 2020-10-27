using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Text.Web.Html
{
  /// <summary>
  /// HTML content which can be written to a TextWriter.
  /// </summary>
  public interface IHtmlContent
  {
    /// <summary>
    /// Writes the content by encoding it with the specified <paramref name="encoder"/>
    /// to the specified <paramref name="writer"/>.
    /// </summary>
    /// <param name="writer">The <see cref="TextWriter"/> to which the content is written.</param>
    /// <param name="encoder">The <see cref="HtmlEncoder"/> which encodes the content to be written.</param>
    void WriteTo(TextWriter writer, IHtmlEncoder encoder);
  }
}
