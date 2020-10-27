using Cubic.Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cubic.Core.Text
{
  public static class StringExtensions
  {

    public static string FormatWith(this string format, IFormatProvider provider, params object[] args)
    {
      // leave this a private to force code to use an explicit overload
      // avoids stack memory being reserved for the object array
      Guard.ArgumentNull(format, nameof(format));

      return string.Format(provider, format, args);
    }

    public static bool StartsWithOrdinal(this string expression, string template)
    {
      return expression.StartsWith(template, StringComparison.OrdinalIgnoreCase);
    }

    public static bool AnyOrdinalIgnoreCase(this IEnumerable<string> enumerable, string value)
    {
      return enumerable.Any(source => string.Equals(source, value, StringComparison.OrdinalIgnoreCase));
    }

    public static bool EqualsOrdinalIgnoreCase(this string s, string value)
    {
      return string.Equals(s, value, StringComparison.OrdinalIgnoreCase);
    }

    public static string Left(this string value, int length)
    {
      if (length <= 0 || string.IsNullOrEmpty(value))
      {
        return string.Empty;
      }
      if (length >= value.Length)
      {
        return value;
      }
      return value.Substring(0, length);
    }

    public static string Right(this string value, int length)
    {
      if (length <= 0 || string.IsNullOrEmpty(value))
      {
        return string.Empty;
      }
      int num = value.Length;
      if (length >= num)
      {
        return value;
      }
      return value.Substring(num - length, length);
    }

    public static int[] GetLineLengths(this string text)
    {
      if (text == null)
      {
        throw new ArgumentNullException();
      }

      if (text.Length == 0)
      {
        return new int[0];
      }

      var result = new List<int>();
      int currentLineLength = 0;
      bool previousWasCarriageReturn = false;

      for (int i = 0; i < text.Length; i++)
      {
        if (text[i] == '\r')
        {
          if (previousWasCarriageReturn)
          {
            result.Add(currentLineLength);
            currentLineLength = 1;
          }
          else
          {
            currentLineLength++;
            previousWasCarriageReturn = true;
          }
        }
        else if (text[i] == '\n')
        {
          previousWasCarriageReturn = false;
          currentLineLength++;
          result.Add(currentLineLength);
          currentLineLength = 0;
        }
        else
        {
          currentLineLength++;
          previousWasCarriageReturn = false;
        }
      }

      result.Add(currentLineLength);

      if (previousWasCarriageReturn)
      {
        result.Add(0);
      }

      return result.ToArray();
    }

    public static bool IsLineBreakChar(this char c)
    {
      return c == '\r' || c == '\n';
    }

    public static ValueTuple<int, int> GetLineFromPosition(int position, string sourceText)
    {
      int lineStart = position;
      int lineEnd = position;

      for (; lineStart > 0; lineStart--)
      {
        if (IsLineBreakChar(sourceText[lineStart - 1]))
        {
          break;
        }
      }

      for (; lineEnd < sourceText.Length - 1; lineEnd++)
      {
        if (IsLineBreakChar(sourceText[lineEnd + 1]))
        {
          break;
        }
      }

      return new ValueTuple<int, int>(lineStart, lineEnd - lineStart + 1);
    }

    public static int GetLineNumber(int start, int[] lineLengths)
    {
      for (int i = 0; i < lineLengths.Length; i++)
      {
        if (start < lineLengths[i])
        {
          return i;
        }

        start -= lineLengths[i];
      }

      return 0;
    }

    public static string WithThousandSeparators(this object i)
    {
      return string.Format("{0:#,0}", i);
    }

    public static string StripQuotes(this string text)
    {
      if (string.IsNullOrEmpty(text))
      {
        return text;
      }

      if (text.StartsWith("\"") && text.EndsWith("\"") && text.Length > 2)
      {
        text = text.Substring(1, text.Length - 2);
      }

      if (text.StartsWith("'") && text.EndsWith("'") && text.Length > 2)
      {
        text = text.Substring(1, text.Length - 2);
      }

      return text;
    }
  }
}