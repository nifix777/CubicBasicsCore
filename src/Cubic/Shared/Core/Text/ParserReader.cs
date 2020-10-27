using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Text
{
  public class Parser
  {
    private readonly TextReader reader;

    private StringBuilder buffer = new StringBuilder();

    public char Current { get; private set; }

    public string Capture => buffer.ToString();

    public void TrySkipWhiteSpace()
    {
      while(Current == Constants.Space)
      {
        this.Advance();
      }
    }

    public void SkipCurrentLine()
    {
      reader.ReadLine();
    }

    public void Advance(int count = 1, bool capture = false)
    {
      for(int index = 0; index <= count; index++)
      {
        var value = reader.Read();

        if (value == -1) return;

        Current = (char)value;

        if (capture) CaptureCurrent();
      }
    }

    public void CaptureCurrent()
    {
      buffer.Append(Current);
    }

    public void SkipUntilAny(params char[] values)
    {
      do
      {
        this.Advance();
      }
      while (!values.Contains(Current));
    }

    public void CaptureUntil(string value)
    {
      do
      {
        this.Advance(value.Length, true);
      }
      while (!Capture.Contains(value));
    }

    public bool Expecting(char value)
    {
      this.Advance();
      return value == Current;
    }

    public bool Expecting(params char[] values)
    {
      for(int index = 0; index < values.Length; index++ )
      {
        this.Advance();
        if (values[index] != Current) return false;
      }

      return true;
    }

    public void ClearCapture()
    {
      buffer.Clear();
    }
  }
}
