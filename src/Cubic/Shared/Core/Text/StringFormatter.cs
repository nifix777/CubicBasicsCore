using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cubic.Core.Text
{
  public static class StringFormatter
  {
    private static Regex reg = new Regex( @"({)([^}]+)(})" , RegexOptions.IgnoreCase );

    public static bool ContainsFarmatting(this string text)
    {
      return reg.IsMatch(text);
    }
    public static string FormatEx(this string text, IDictionary<string, object> values)
    {
      return text.FormatEx(delegate(string s)
      {
        object value = null;
        if (!values.TryGetValue(s, out value)) value = string.Empty;
        return value;
      }
      );
    }

    public static string FormatEx(this string text, Func<string, object> getValueFunc)
    {
      StringBuilder sb = new StringBuilder();
      int startIndex = 0;
      var matches = reg.Matches(text);
      foreach (Match m in matches)
      {
        Group g = m.Groups[2]; //it's second in the match between { and }
        int length = g.Index - startIndex - 1;

        sb.Append( text.Substring( startIndex , length ) );
        startIndex = g.Index + g.Length + 1;

        string toGet = String.Empty;
        string toFormat = String.Empty;
        int formatIndex = g.Value.IndexOf( ":" ); //formatting would be to the right of a :
        if ( formatIndex == -1 ) //no formatting, no worries
        {
          toGet = g.Value;
        }
        else //pickup the formatting
        {
          toGet = g.Value.Substring( 0 , formatIndex );
          toFormat = g.Value.Substring( formatIndex + 1 );
        }

        if (toFormat.IsNullOrEmpty())
        {
          sb.Append(getValueFunc(toGet.ToUpperInvariant()));
        }
        else
        {
          //sb.AppendFormat(g.Value.Replace( g.Value , getValueFunc(toGet.ToUpperInvariant()).ToString()));
          sb.AppendFormat(string.Format( g.Value.Replace( toGet,"{0}") , getValueFunc(toGet.ToUpperInvariant())));
        }

      }

      var maxIndex = matches.Cast<Match>().Max(m => m.Index);
      var endIndex = matches.Cast<Match>().Where(m => m.Index == maxIndex).Select(m => m.Index + m.Length).First();

      if ( endIndex < text.Length ) //include the rest (end) of the string
      {
        sb.Append( text.Substring( endIndex ) );
      }
      return sb.ToString();
    }
  }
}