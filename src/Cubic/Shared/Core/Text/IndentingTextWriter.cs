using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Text
{
  public class IndentingTextWriter : TextWriter
  {
    private readonly string _indend;

    private readonly StringBuilder _indendBuilder = new StringBuilder();

    private readonly TextWriter _inner;

    public override IFormatProvider FormatProvider => _inner.FormatProvider;

    public override string NewLine { get => _inner.NewLine; set => _inner.NewLine = value; }

    public IndentingTextWriter(TextWriter inner) : this(Constants.Space.ToString(), inner)
    {

    }

    public IndentingTextWriter(string intend, TextWriter inner)
    {
      _inner = inner ?? throw new ArgumentNullException(nameof(inner));
      _indend = intend ?? throw new ArgumentNullException(nameof(intend));
    }



    #region TextWriter Implementation
    public override void Close()
    {
      _inner.Close();
    }

    public override void Flush()
    {
      _inner.Flush();
    }

    public override void Write(char value)
    {
      _inner.Write(value);
    }

    public override void Write(char[] buffer)
    {
      _inner.Write(buffer);
    }

    public override void Write(char[] buffer, int index, int count)
    {
      _inner.Write(buffer, index, count);
    }

    public override void Write(bool value)
    {
      _inner.Write(value);
    }

    public override void Write(int value)
    {
      _inner.Write(value);
    }

    public override void Write(uint value)
    {
      _inner.Write(value);
    }

    public override void Write(long value)
    {
      _inner.Write(value);
    }

    public override void Write(ulong value)
    {
      _inner.Write(value);
    }

    public override void Write(float value)
    {
      _inner.Write(value);
    }

    public override void Write(double value)
    {
      _inner.Write(value);
    }

    public override void Write(decimal value)
    {
      _inner.Write(value);
    }

    public override void Write(string value)
    {
      _inner.Write(value);
    }

    public override void Write(object value)
    {
      _inner.Write(value);
    }

    public override void Write(string format, object arg0)
    {
      _inner.Write(format, arg0);
    }

    public override void Write(string format, object arg0, object arg1)
    {
      _inner.Write(format, arg0, arg1);
    }

    public override void Write(string format, object arg0, object arg1, object arg2)
    {
      _inner.Write(format, arg0, arg1, arg2);
    }

    public override void Write(string format, params object[] arg)
    {
      _inner.Write(format, arg);
    }

    public override void WriteLine()
    {
      _inner.Write(CurrentIntend);
      _inner.WriteLine();
    }

    public override void WriteLine(char value)
    {
      _inner.Write(CurrentIntend);
      _inner.WriteLine(value);
    }

    public override void WriteLine(char[] buffer)
    {
      _inner.Write(CurrentIntend);
      _inner.WriteLine(buffer);
    }

    public override void WriteLine(char[] buffer, int index, int count)
    {
      _inner.Write(CurrentIntend);
      _inner.WriteLine(buffer, index, count);
    }

    public override void WriteLine(bool value)
    {
      _inner.Write(CurrentIntend);
      _inner.WriteLine(value);
    }

    public override void WriteLine(int value)
    {
      _inner.Write(CurrentIntend);
      _inner.WriteLine(value);
    }

    public override void WriteLine(uint value)
    {
      _inner.Write(CurrentIntend);
      _inner.WriteLine(value);
    }

    public override void WriteLine(long value)
    {
      _inner.Write(CurrentIntend);
      _inner.WriteLine(value);
    }

    public override void WriteLine(ulong value)
    {
      _inner.Write(CurrentIntend);
      _inner.WriteLine(value);
    }

    public override void WriteLine(float value)
    {
      _inner.Write(CurrentIntend);
      _inner.WriteLine(value);
    }

    public override void WriteLine(double value)
    {
      _inner.Write(CurrentIntend);
      _inner.WriteLine(value);
    }

    public override void WriteLine(decimal value)
    {
      _inner.Write(CurrentIntend);
      _inner.WriteLine(value);
    }

    public override void WriteLine(string value)
    {
      _inner.Write(CurrentIntend);
      _inner.WriteLine(value);
    }

    public override void WriteLine(object value)
    {
      _inner.Write(CurrentIntend);
      _inner.WriteLine(value);
    }

    public override void WriteLine(string format, object arg0)
    {
      _inner.Write(CurrentIntend);
      _inner.WriteLine(format, arg0);
    }

    public override void WriteLine(string format, object arg0, object arg1)
    {
      _inner.Write(CurrentIntend);
      _inner.WriteLine(format, arg0, arg1);
    }

    public override void WriteLine(string format, object arg0, object arg1, object arg2)
    {
      _inner.Write(CurrentIntend);
      _inner.WriteLine(format, arg0, arg1, arg2);
    }

    public override void WriteLine(string format, params object[] arg)
    {
      _inner.Write(CurrentIntend);
      _inner.WriteLine(format, arg);
    }

    public override Task WriteAsync(char value)
    {
      return _inner.WriteAsync(value);
    }

    public override Task WriteAsync(string value)
    {
      return _inner.WriteAsync(value);
    }

    public override Task WriteAsync(char[] buffer, int index, int count)
    {
      return _inner.WriteAsync(buffer, index, count);
    }

    public override async Task WriteLineAsync(char value)
    {
      await _inner.WriteAsync(CurrentIntend);
      await _inner.WriteLineAsync(value);
    }

    public override async Task WriteLineAsync(string value)
    {
      await _inner.WriteAsync(CurrentIntend);
      await _inner.WriteLineAsync(value);
    }

    public override async Task WriteLineAsync(char[] buffer, int index, int count)
    {
      await _inner.WriteAsync(CurrentIntend);
      await _inner.WriteLineAsync(buffer, index, count);
    }

    public override async Task WriteLineAsync()
    {
      await _inner.WriteAsync(CurrentIntend);
      await _inner.WriteLineAsync();
    }

    public override Task FlushAsync()
    {
      return _inner.FlushAsync();
    }

    public override Encoding Encoding => _inner.Encoding;
    #endregion

    private string CurrentIntend => _indend.ToString();



    internal void IncreaseIntend()
    {
      _indendBuilder.Append(_indend);
    }

    internal void DecreaseIntend()
    {
      _indendBuilder.Remove(_indendBuilder.Length - _indend.Length, _indend.Length);
    }

    public IDisposable Intend()
    {
      return new IntendToken(this);
    }

    private class IntendToken : IDisposable
    {
      private readonly IndentingTextWriter indentingTextWriter;

      public IntendToken(IndentingTextWriter indentingTextWriter)
      {
        this.indentingTextWriter = indentingTextWriter ?? throw new ArgumentNullException(nameof(indentingTextWriter));
        indentingTextWriter.IncreaseIntend();
      }

      public void Dispose()
      {
        indentingTextWriter?.DecreaseIntend();
      }
    }
  }
}
