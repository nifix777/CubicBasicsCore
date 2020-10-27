using System.IO;
using System.Text;

namespace Cubic.Core.Text
{
  public sealed class EncodedStringWriter : StringWriter
  {
    private readonly Encoding _stringWriterEncoding;

    public EncodedStringWriter(Encoding desiredEncoding)
    {
      this._stringWriterEncoding = desiredEncoding;
    }
    public EncodedStringWriter( StringBuilder builder , Encoding desiredEncoding ) : base( builder )
    {
      this._stringWriterEncoding = desiredEncoding;
    }

    public override Encoding Encoding
    {
      get
      {
        return this._stringWriterEncoding;
      }
    }

  }
}