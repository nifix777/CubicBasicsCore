using System;
using System.IO;
using System.Text;

namespace Cubic.Core.Text
{
  public sealed class AdvancedTextReader
  {
    private const char EofChar = '\0';

    private readonly TextReader textReader;

    private readonly StringBuilder buffer;

    private char current;
    private int position;

 
    public AdvancedTextReader(TextReader textReader)
    {
      this.textReader = textReader ?? throw new ArgumentNullException(nameof(textReader));
      buffer = new StringBuilder();
    }

    public char Current => current;

    public string Text => buffer.ToString();

    public bool IsEof => current == EofChar;

    public int Position => position;

    public char Peek()
    {
      int charCode = textReader.Peek();

      return (charCode > 0) ? (char)charCode : EofChar;
    }

    public string ReadUntil(char stopChar)
    {
      buffer.Clear();

      while (stopChar != Peek())
      {
        Next();
      }

      return buffer.ToString();
    }

    /// <summary>
    /// Advances to the next character
    /// </summary>
    public void Next()
    {
      if (marked != -1 && (marked <= this.position) && !IsEof)
      {
        buffer.Append(current);
      }

      int charCode = textReader.Read(); // -1 if end of stream

      this.current = (charCode > 0) ? (char)charCode : EofChar;

      position++;
    }

    /// <summary>
    /// Advances to the next character without appending it to the buffer
    /// </summary>
    public void Skip()
    {
      int charCode = textReader.Read(); // -1 if there are no more chars to read (e.g. stream has ended)

      this.current = (charCode > 0) ? (char)charCode : EofChar;

      position++;
    }

    public void ReadWhitespace()
    {
      while (char.IsWhiteSpace(current))
      {
        Next();
      }
    }

    public void ReadNewLine()
    {
      while (current == '\r' || current == '\n')
      {
        Next();
      }
    }

    public void SkipWhitespace()
    {
      while (char.IsWhiteSpace(current))
      {
        Skip();
      }
    }

    #region Mark

    private int marked = -1;

    public void Mark(bool appendCurrent = true)
    {
      marked = this.position;

      if (appendCurrent == false)
      {
        marked++;
      }
    }

    public string Unmark()
    {
      marked = -1;

      var text = buffer.ToString();

      buffer.Clear();

      return text;
    }

    #endregion
  }
}
