using Cubic.Core.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Cubic.Core.Cubic.Core.Text
{
  public class CsvReader : IEnumerable<string[]>
  {
    // Columns may be comma or tab seperated (Tsv)

    private readonly AdvancedTextReader reader;
    private readonly char delimiter = ',';

    public CsvReader(TextReader source, char seperator)
    {
      this.reader = new AdvancedTextReader(source);
      this.delimiter = seperator;

      this.reader.Next(); // Start things off
    }

    private IEnumerable<string[]> Enumerate()
    {
      while (!reader.IsEof)
      {
        yield return ReadRow();
      }
    }

    public bool IsEof => reader.IsEof;

    public string[] ReadRow()
    {
      if (reader.IsEof)
      {
        throw new EndOfStreamException("Cannot read past the end of file");
      }

      // Read comment lines
      while (reader.Current == '#')
      {
        ReadComment();
      }

      var values = new List<string>();

      while (!reader.IsEof)
      {
        values.Add(ReadColumnValue());

        if (reader.Current == '\r' || reader.Current == '\n')
        {
          break;
        }
      }

      reader.ReadNewLine();

      return values.ToArray();
    }



    public int ReadRow(string[] buffer)
    {
      if (reader.IsEof)
      {
        throw new EndOfStreamException("Cannot read past the end of file");
      }

      // Read comment lines
      while (reader.Current == '#')
      {
        ReadComment();
      }

      int i = 0;

      while (!reader.IsEof)
      {
        buffer[i] = ReadColumnValue();

        i++;

        if (reader.Current == '\r' || reader.Current == '\n')
        {
          break;
        }
      }

      reader.ReadNewLine();

      return i;
    }

    public void ReadComment()
    {
      while (!reader.IsEof)
      {
        reader.Next();

        // Read until a comma or tab seperator or a new line
        if (reader.Current == '\r' || reader.Current == '\n')
        {
          break;
        }
      }

      reader.ReadNewLine();
    }

    public string ReadColumnValue()
    {
      if (reader.IsEof)
      {
        throw new EndOfStreamException("Unexpected EOF reading column");
      }

      if (reader.Current == '"')
      {
        if (reader.Peek() != '"')
        {
          return ReadQuotedColumn();
        }

        reader.Skip(); // Skip the first double quote
      }

      if (reader.Current == delimiter)
      {
        reader.Next(); // Read the seperator

        return string.Empty;
      }

      reader.Mark();

      while (!reader.IsEof)
      {
        reader.Next();

        // Read until a comma or tab seperator or a new line
        if (reader.Current == delimiter || reader.Current == '\r' || reader.Current == '\n')
        {
          break;
        }
      }

      var text = this.reader.Unmark();

      if (reader.Current == delimiter)
      {
        reader.Next(); // Read the seperator
      }

      return text;

    }

    public string ReadQuotedColumn()
    {
      reader.Next(); // Read "

      reader.Mark();

      // ends with """

      while (!reader.IsEof)
      {
        reader.Next();

        while (reader.Current == '"' && reader.Peek() == '"')
        {
          reader.Skip(); // Skip the first  '"'
          reader.Next(); // Read the second '"'
        }

        if (reader.Current == '"') break;
      }

      var text = reader.Unmark();

      reader.Next(); // Read "

      if (reader.Current == delimiter)
      {
        reader.Next(); // Read the seperator
      }

      return text;
    }


    #region IEnumerable

    IEnumerator<string[]> IEnumerable<string[]>.GetEnumerator() => Enumerate().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Enumerate().GetEnumerator();

    #endregion
  }
}
