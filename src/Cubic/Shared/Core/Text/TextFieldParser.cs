using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.ComponentModel;

namespace TestProject.Microsoft.VisualBasic.FileIO
{
  public class TextFieldParser : IDisposable
  {
    private TextReader m_Reader;
    private bool m_LeaveOpen = false;
    private string[] m_CommentTokens = new string[] { };
    private string[] m_Delimiters = null;
    private string m_ErrorLine = string.Empty;
    private long m_ErrorLineNumber = -1;
    private int[] m_FieldWidths = null;
    private bool m_HasFieldsEnclosedInQuotes = true;
    private long m_LineNumber = -1;
    private FieldType m_TextFieldType = FieldType.Delimited;
    private bool m_TrimWhiteSpace = true;

    private Queue<string> m_PeekedLine = new Queue<string>();
    private int m_MinFieldLength;

    public TextFieldParser(Stream stream)
    {
      m_Reader = new StreamReader(stream);
    }

    public TextFieldParser(TextReader reader)
    {
      m_Reader = reader;
    }

    public TextFieldParser(string path)
    {
      m_Reader = new StreamReader(path);
    }

    public TextFieldParser(Stream stream, Encoding defaultEncoding)
    {
      m_Reader = new StreamReader(stream, defaultEncoding);
    }

    public TextFieldParser(string path, Encoding defaultEncoding)
    {
      m_Reader = new StreamReader(path, defaultEncoding);
    }

    public TextFieldParser(Stream stream, Encoding defaultEncoding, bool detectEncoding)
    {
      m_Reader = new StreamReader(stream, defaultEncoding, detectEncoding);
    }

    public TextFieldParser(string path, Encoding defaultEncoding, bool detectEncoding)
    {
      m_Reader = new StreamReader(path, defaultEncoding, detectEncoding);
    }

    public TextFieldParser(Stream stream, Encoding defaultEncoding, bool detectEncoding, bool leaveOpen)
    {
      m_Reader = new StreamReader(stream, defaultEncoding, detectEncoding);
      m_LeaveOpen = leaveOpen;
    }

    private string[] GetDelimitedFields()
    {
      if (m_Delimiters == null || m_Delimiters.Length == 0)
        throw new InvalidOperationException("Unable to read delimited fields because Delimiters is Nothing or empty.");

      List<string> result = new List<string>();
      string line;
      int currentIndex = 0;
      int nextIndex = 0;

      line = GetNextLine();

      if (line == null)
        return null;

      while (!(nextIndex >= line.Length))
      {
        result.Add(GetNextField(line, currentIndex, ref nextIndex));
        currentIndex = nextIndex;
      }

      return result.ToArray();
    }

    private string GetNextField(string line, int startIndex, ref int nextIndex)
    {
      bool inQuote = false;
      int currentindex = 0;

      if (nextIndex == int.MinValue)
      {
        nextIndex = int.MaxValue;
        return string.Empty;
      }

      if (m_HasFieldsEnclosedInQuotes && line[currentindex] == '"')
      {
        inQuote = true;
        startIndex += 1;
      }

      currentindex = startIndex;

      bool mustMatch = false;
      for (int j = startIndex; j <= line.Length - 1; j++)
      {
        if (inQuote)
        {
          if (line[j] == '"')
          {
            inQuote = false;
            mustMatch = true;
          }
          continue;
        }

        for (int i = 0; i <= m_Delimiters.Length - 1; i++)
        {
          if (string.Compare(line, j, m_Delimiters[i], 0, m_Delimiters[i].Length) == 0)
          {
            nextIndex = j + m_Delimiters[i].Length;
            if (nextIndex == line.Length)
              nextIndex = int.MinValue;
            if (mustMatch)
              return line.Substring(startIndex, j - startIndex - 1);
            else
              return line.Substring(startIndex, j - startIndex);
          }
        }

        if (mustMatch)
          RaiseDelimiterEx(line);
      }

      if (inQuote)
        RaiseDelimiterEx(line);

      nextIndex = line.Length;
      if (mustMatch)
        return line.Substring(startIndex, nextIndex - startIndex - 1);
      else
        return line.Substring(startIndex);
    }

    private void RaiseDelimiterEx(string Line)
    {
      m_ErrorLineNumber = m_LineNumber;
      m_ErrorLine = Line;
      throw new MalformedLineException("Line " + m_ErrorLineNumber.ToString() + " cannot be parsed using the current Delimiters.", m_ErrorLineNumber);
    }

    private void RaiseFieldWidthEx(string Line)
    {
      m_ErrorLineNumber = m_LineNumber;
      m_ErrorLine = Line;
      throw new MalformedLineException("Line " + m_ErrorLineNumber.ToString() + " cannot be parsed using the current FieldWidths.", m_ErrorLineNumber);
    }

    private string[] GetWidthFields()
    {
      if (m_FieldWidths == null || m_FieldWidths.Length == 0)
        throw new InvalidOperationException("Unable to read fixed width fields because FieldWidths is Nothing or empty.");

      string[] result = new string[m_FieldWidths.Length - 1 + 1];
      int currentIndex = 0;
      string line;

      line = GetNextLine();

      if (line.Length < m_MinFieldLength)
        RaiseFieldWidthEx(line);

      for (int i = 0; i <= result.Length - 1; i++)
      {
        if (m_TrimWhiteSpace)
          result[i] = line.Substring(currentIndex, m_FieldWidths[i]).Trim();
        else
          result[i] = line.Substring(currentIndex, m_FieldWidths[i]);
        currentIndex += m_FieldWidths[i];
      }

      return result;
    }

    private bool IsCommentLine(string Line)
    {
      if (m_CommentTokens == null)
        return false;

      foreach (string str in m_CommentTokens)
      {
        if (Line.StartsWith(str))
          return true;
      }

      return false;
    }

    private string GetNextRealLine()
    {
      string nextLine;

      do
        nextLine = ReadLine();
      while (!(nextLine == null || IsCommentLine(nextLine) == false));

      return nextLine;
    }

    private string GetNextLine()
    {
      if (m_PeekedLine.Count > 0)
        return m_PeekedLine.Dequeue();
      else
        return GetNextRealLine();
    }

    public void Close()
    {
      if (m_Reader != null && m_LeaveOpen == false)
        m_Reader.Close();
      m_Reader = null;
    }

    public string PeekChars(int numberOfChars)
    {
      if (numberOfChars < 1)
        throw new ArgumentException("numberOfChars has to be a positive, non-zero number", "numberOfChars");

      string[] peekedLines;
      string theLine = null;
      if (m_PeekedLine.Count > 0)
      {
        peekedLines = m_PeekedLine.ToArray();
        for (int i = 0; i <= m_PeekedLine.Count - 1; i++)
        {
          if (IsCommentLine(peekedLines[i]) == false)
          {
            theLine = peekedLines[i];
            break;
          }
        }
      }

      if (theLine == null)
      {
        do
        {
          theLine = m_Reader.ReadLine();
          m_PeekedLine.Enqueue(theLine);
        }
        while (!(theLine == null || IsCommentLine(theLine) == false));
      }

      if (theLine != null)
      {
        if (theLine.Length <= numberOfChars)
          return theLine;
        else
          return theLine.Substring(0, numberOfChars);
      }
      else
        return null;
    }

    public string[] ReadFields()
    {
      switch (m_TextFieldType)
      {
        case FieldType.Delimited:
          {
            return GetDelimitedFields();
          }

        case FieldType.FixedWidth:
          {
            return GetWidthFields();
          }

        default:
          {
            return GetDelimitedFields();
          }
      }
    }

    public string ReadLine()
    {
      if (m_PeekedLine.Count > 0)
        return m_PeekedLine.Dequeue();
      if (m_LineNumber == -1)
        m_LineNumber = 1;
      else
        m_LineNumber += 1;
      return m_Reader.ReadLine();
    }


    public string ReadToEnd()
    {
      return m_Reader.ReadToEnd();
    }

    public void SetDelimiters(params string[] delimiters)
    {
      this.Delimiters = delimiters;
    }

    public void SetFieldWidths(params int[] fieldWidths)
    {
      this.FieldWidths = fieldWidths;
    }

    public string[] CommentTokens
    {
      get
      {
        return m_CommentTokens;
      }
      set
      {
        m_CommentTokens = value;
      }
    }

    public string[] Delimiters
    {
      get
      {
        return m_Delimiters;
      }
      set
      {
        m_Delimiters = value;
      }
    }

    public bool EndOfData
    {
      get
      {
        return PeekChars(1) == null;
      }
    }

    public string ErrorLine
    {
      get
      {
        return m_ErrorLine;
      }
    }

    public long ErrorLineNumber
    {
      get
      {
        return m_ErrorLineNumber;
      }
    }

    public int[] FieldWidths
    {
      get
      {
        return m_FieldWidths;
      }
      set
      {
        m_FieldWidths = value;
        if (m_FieldWidths != null)
        {
          m_MinFieldLength = 0;
          for (int i = 0; i <= m_FieldWidths.Length - 1; i++)
            m_MinFieldLength += value[i];
        }
      }
    }

    public bool HasFieldsEnclosedInQuotes
    {
      get
      {
        return m_HasFieldsEnclosedInQuotes;
      }
      set
      {
        m_HasFieldsEnclosedInQuotes = value;
      }
    }

    public long LineNumber
    {
      get
      {
        return m_LineNumber;
      }
    }

    public FieldType TextFieldType
    {
      get
      {
        return m_TextFieldType;
      }
      set
      {
        m_TextFieldType = value;
      }
    }

    public bool TrimWhiteSpace
    {
      get
      {
        return m_TrimWhiteSpace;
      }
      set
      {
        m_TrimWhiteSpace = value;
      }
    }

    private bool disposedValue = false;        // To detect redundant calls

    // IDisposable
    protected virtual void Dispose(bool disposing)
    {
      if (!this.disposedValue)
        Close();
      this.disposedValue = true;
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
  }
}

