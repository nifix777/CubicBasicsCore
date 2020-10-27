
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Text.Web.Html
{
  public class HtmlWriter : IDisposable
  {
    private readonly TextWriter _writer;

    //public HtmlWriter() : this(new StreamWriter(new MemoryStream(), Encoding.UTF8))
    //{
    //}

    public HtmlWriter(Stream stream, bool leaveopen = false) : this(new StreamWriter(stream, Encoding.UTF8, Constants.LargeBufferSize, leaveopen))
    {
    }

    public HtmlWriter(TextWriter writer)
    {
      _writer = writer;
      _writer.NewLine = Constants.Lf.ToString();
    }

    public void Close()
    {
      _writer.Close();
    }

    public void Flush()
    {
      _writer.Flush();
    }

    public Task FlushAsync()
    {
      return _writer.FlushAsync();
    }

    public void Write(string value)
    {
      this.Write(new Utf8String(value));
    }

    public void WriteLine(string value)
    {
      this.WriteLine(new Utf8String(value));
    }

    public void Write(Utf8String value)
    {
      _writer.Write(value.BufferRaw);
    }

    public void WriteLine(Utf8String value)
    {
      _writer.WriteLine(value.BufferRaw);
    }

    public Task WriteAsync(string value)
    {
      return this.WriteAsync(value);
    }

    public Task WriteLineAsync(string value)
    {
      return this.WriteLineAsync(value);
    }

    public Task WriteAsync(Utf8String value)
    {
      return _writer.WriteAsync((char[])value);
    }

    public Task WriteLineAsync(Utf8String value)
    {
      return _writer.WriteLineAsync((char[])value);
    }

    public void Write(int value) 
    {
      _writer.Write(value);
    }

    public void WriteLine(int value)
    {
      _writer.WriteLine(value);
    }

    public void Dispose()
    {
      _writer.Dispose();
    }
  }
}
