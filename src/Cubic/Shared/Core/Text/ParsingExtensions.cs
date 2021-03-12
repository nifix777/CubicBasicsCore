using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Cubic.Core.Annotations;
using Cubic.Core.Collections;
using Cubic.Core.Diagnostics;

namespace Cubic.Core.Text
{
  public static class ParsingExtensions
  {

    public static string Quote(this string text, char escape = '\\', char delimiter = '\"')
    {
      var sb = new StringBuilder();

      sb.Append(delimiter);
      foreach (var c in text)
      {
        if(escape == c)
        {
          sb.Append(escape, 2);
          sb.Append(c);
        }
        else if(delimiter == c)
        {
          sb.Append(escape);
          sb.Append(c);
        }
        else
        {
          sb.Append(c);
        }
      }
      sb.Append(delimiter);

      return sb.ToString();
    }

    /// <summary>
    /// Gets the culture information.
    /// </summary>
    /// <param name="name">The name. Like 'en-US'</param>
    /// <param name="refreshCultureSettings">if set to <c>true</c> it respects the changed Culture-Settings from the OS. Else it uses a cached culture-object</param>
    /// <returns></returns>
    public static CultureInfo GetCultureInfo(string name, bool refreshCultureSettings = false)
    {
      if (refreshCultureSettings)
      {
        return new CultureInfo(name);
      }

      return CultureInfo.GetCultureInfo(name);
    }

    public static string GetCultureSpecificString<T>(T value, CultureInfo culture)
    {
      return string.Format(culture, "{0}", value);
    }
    public static string GetInvariantString<T>(T value)
    {
      return GetCultureSpecificString(value, CultureInfo.InvariantCulture);
    }

    public static T GetValueFromInvariantString<T>(string value)
    {
      var converter = TypeDescriptor.GetConverter(typeof (T));
      return (T) converter.ConvertFromInvariantString(value);
    }

    public const string ReplaceStringDefaultParameter = @" ";

    /// <summary>
    /// Parse a string for a name value pair (name=value).
    /// </summary>
    /// <param name="nameEqualsValue">The (name = value) string to parse.</param>
    /// <returns>KeyValuePair name and value</returns>
    private static KeyValuePair<string, string> ParseNameValue(string nameEqualsValue)
    {
      var equals = nameEqualsValue.IndexOf(Constants.Equal.ToChar());
      var name = nameEqualsValue.Substring(0, equals);
      var value = nameEqualsValue.Substring(equals + 1);
      return new KeyValuePair<string, string>(name, value);
    }

    /// <summary>
    /// Formats the enhanced.
    /// </summary>
    /// <param name="anObject">An object.</param>
    /// <param name="aFormat">a format.</param>
    /// <returns></returns>
    public static string FormatEnhanced(this object anObject, string aFormat)
    {
      return ParsingExtensions.FormatEnhanced(anObject, aFormat, null);
    }

    /// <summary>
    /// Formats the enhanced.
    /// </summary>
    /// <param name="anObject">An object.</param>
    /// <param name="aFormat">a format.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns></returns>
    /// <remarks>Source: https://www.hanselman.com/blog/ASmarterOrPureEvilToStringWithExtensionMethods.aspx </remarks>
    public static string FormatEnhanced(this object anObject, string aFormat, IFormatProvider formatProvider)
    {
      StringBuilder sb = new StringBuilder();
      Type type = anObject.GetType();
      Regex reg = new Regex(@"({)([^}]+)(})", RegexOptions.IgnoreCase);
      MatchCollection mc = reg.Matches(aFormat);
      int startIndex = 0;
      foreach (Match m in mc)
      {
        Group g = m.Groups[2]; //it's second in the match between { and }
        int length = g.Index - startIndex - 1;
        sb.Append(aFormat.Substring(startIndex, length));

        string toGet = String.Empty;
        string toFormat = String.Empty;
        int formatIndex = g.Value.IndexOf(":"); //formatting would be to the right of a :
        if (formatIndex == -1) //no formatting, no worries
        {
          toGet = g.Value;
        }
        else //pickup the formatting
        {
          toGet = g.Value.Substring(0, formatIndex);
          toFormat = g.Value.Substring(formatIndex + 1);
        }

        //first try properties
        PropertyInfo retrievedProperty = type.GetProperty(toGet);
        Type retrievedType = null;
        object retrievedObject = null;
        if (retrievedProperty != null)
        {
          retrievedType = retrievedProperty.PropertyType;
          retrievedObject = retrievedProperty.GetValue(anObject, null);
        }
        else //try fields
        {
          FieldInfo retrievedField = type.GetField(toGet);
          if (retrievedField != null)
          {
            retrievedType = retrievedField.FieldType;
            retrievedObject = retrievedField.GetValue(anObject);
          }
        }

        if (retrievedType != null) //Cool, we found something
        {
          string result = String.Empty;
          if (toFormat == String.Empty) //no format info
          {
            result = retrievedType.InvokeMember("ToString",
              BindingFlags.Public | BindingFlags.NonPublic |
              BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase
              , null, retrievedObject, null) as string;
          }
          else //format info
          {
            result = retrievedType.InvokeMember("ToString",
              BindingFlags.Public | BindingFlags.NonPublic |
              BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase
              , null, retrievedObject, new object[] { toFormat, formatProvider }) as string;
          }
          sb.Append(result);
        }
        else //didn't find a property with that name, so be gracious and put it back
        {
          sb.Append("{");
          sb.Append(g.Value);
          sb.Append("}");
        }
        startIndex = g.Index + g.Length + 1;
      }
      if (startIndex < aFormat.Length) //include the rest (end) of the string
      {
        sb.Append(aFormat.Substring(startIndex));
      }
      return sb.ToString();
    }

    public static bool IsNullOrEmpty(this string text)
    {
      return string.IsNullOrEmpty(text);
    }

    public static bool IsNullOrEmptyOrWhiteSpace(this string text)
    {
      return string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text);
    }

    public static string IfNullOrWhiteSpace(this string str, string defaultValue)
    {
      return str.IsNullOrEmptyOrWhiteSpace() ? defaultValue : str;
    }

    public static string ToAlphaNumeric(this string text)
    {
      return string.Concat(text.Where(c => IsAlphaNumeric(c)));
    }

    public static bool IsAlphaNumeric(this char cr)
    {
      return char.IsLetter(cr) || char.IsNumber(cr) || cr == ' ';
    }

    public static bool IsNumeric(this string expression)
    {
      double num;
      return double.TryParse(expression, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out num);
    }


    public static string ReplaceVariables(this string text, IDictionary<string, object> variables)
    {
      var pattern = "(\\{.*?\\})" + ".*?";

      var regex = new Regex(pattern, RegexOptions.IgnoreCase);

      var matches = regex.Matches(text);

      foreach (Match match in matches)
      {
        if (match.Success)
        {
          var trimmed = match.Value.Trim();
          string key = trimmed.Substring(1, trimmed.Length - 2);

          if (variables.ContainsKey(key))
          {
            text = text.Replace(trimmed, variables[key].ToString());
          }
        }
      }

      return text;
    }

    public static string Indent(this string text, int indent = 1)
    {
      if (text.IsNull()) return string.Empty;

      // This will never return an empty array.  The returned array will always
      // have at least one non-null element, even if "s" is totally empty.
      String[] subStrings = text.SplitStringOnNewLines();

      var result = new StringBuilder(
        (subStrings.Length * indent) +
        (subStrings.Length * Environment.NewLine.Length) +
        text.Length);

      for (int i = 0; i < subStrings.Length; i++)
      {
        result.Append(Constants.Space, indent).Append(subStrings[i]);
        result.AppendLine();
      }

      return result.ToString();
    }

    public static string[] SplitStringOnNewLines(this string text)
    {
      return text.Split(Constants.NewLines, StringSplitOptions.None);
    }

    /// <summary>
    /// Returns a stream from a string
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    internal static Stream ToStream(string s)
    {
      var stream = new MemoryStream();
      var writer = new StreamWriter(stream);
      writer.Write(s);
      writer.Flush();
      stream.Position = 0;
      return stream;
    }

    public static string GetOnlyNumbers(this string text)
    {
      StringBuilder builder = new StringBuilder();

      foreach (char c in text)
      {

        if (char.IsDigit(c))
        {
          builder.Append(c);
        }
      }

      return builder.ToString();
    }

    private static Regex emailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.IgnoreCase);

    public static bool IsEmailAddress(this string emailAddress)
    {
      return emailRegex.IsMatch(emailAddress);
    }

    public static IList<string> GetEmailAddresses(this string text)
    {
      var matches = emailRegex.Matches(text);

      IList<string> emailAddresses = (from object match in matches select match.ToString()).ToList();

      return emailAddresses;
    }


    /// <summary>
    /// Returns the number of steps required to transform the source string
    /// into the target string.
    /// </summary>
    public static int ComputeLevenshteinDistance(string source, string target)
    {
      if ((source == null) || (target == null)) return 0;
      if ((source.Length == 0) || (target.Length == 0)) return 0;
      if (source == target) return source.Length;

      int sourceWordCount = source.Length;
      int targetWordCount = target.Length;

      // Step 1
      if (sourceWordCount == 0)
        return targetWordCount;

      if (targetWordCount == 0)
        return sourceWordCount;

      int[,] distance = new int[sourceWordCount + 1, targetWordCount + 1];

      // Step 2
      for (int i = 0; i <= sourceWordCount; distance[i, 0] = i++) ;
      for (int j = 0; j <= targetWordCount; distance[0, j] = j++) ;

      for (int i = 1; i <= sourceWordCount; i++)
      {
        for (int j = 1; j <= targetWordCount; j++)
        {
          // Step 3
          int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

          // Step 4
          distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
        }
      }

      return distance[sourceWordCount, targetWordCount];
    }


    /// <summary>
    /// Calculate percentage similarity of two strings
    /// <param name="source">Source String to Compare with</param>
    /// <param name="target">Targeted String to Compare</param>
    /// <returns>Return Similarity between two strings from 0 to 1.0</returns>
    /// </summary>
    public static double CalculateSimilarity(string source, string target)
    {
      if ((source == null) || (target == null)) return 0.0;
      if ((source.Length == 0) || (target.Length == 0)) return 0.0;
      if (source == target) return 1.0;

      int stepsToSame = ComputeLevenshteinDistance(source, target);
      return (1.0 - ((double)stepsToSame / (double)Math.Max(source.Length, target.Length)));
    }

    public static string EscapeUri(this string uri)
    {
      return Uri.EscapeUriString(uri);
    }

    public static Uri ToUri(this string uri)
    {
      return new Uri(uri);
    }

    public static IEnumerable<string> GetWordsStartingWith(this string text, string startString)
    {
      var testSoruce = text;
      while (testSoruce.Contains(startString))
      {
        var index = testSoruce.IndexOf(startString, StringComparison.InvariantCulture);
        var search = testSoruce.Substring(index).Substring(startString.Length);
        var result = search.ParseUntil(Constants.Space);
        testSoruce = text.Remove(index, result.Length);
        yield return result;
      }
    }

    public static string Append(this string value, string append, string joinString = ",")
    {
      return string.Concat(value, append, joinString);
    }

    public static string Append(this string value, IEnumerable<string> append, string joinString = ",")
    {
      StringBuilder builder = new StringBuilder(value);

      foreach (var s in append)
      {
        builder.AppendFormat(string.Concat(s, joinString));
      }

      return builder.ToString();
    }

    ///<summary>
    /// Parse
    ///
    /// System.Version.TryParse (and therefore the constructor that takes a string) fails with
    /// an exception if the version string is not formatted correctly.  We need a more forgiving
    /// Parse that takes strings like "12" and "12.2"
    /// </summary>
    /// <param name="raw"></param>
    /// <returns></returns>
    public static Version ToVersion(this string raw)
    {
      int major = 0, minor = 0, build = 0;

      string[] tokens = raw.Split(Constants.Dot.ToChar());

      if (tokens.Length > 0)
      {
        int.TryParse(tokens[0], out major);

        if (tokens.Length > 1)
        {
          int.TryParse(tokens[1], out minor);

          if (tokens.Length > 2)
          {
            int.TryParse(tokens[2], out build);
          }
        }
      }

      return new Version(major, minor, build);
    }

    public static int BinarySearch(this string source, string query)
    {
      var searchSource = System.Text.Encoding.Unicode.GetBytes(source);
      var searchKey = System.Text.Encoding.Unicode.GetBytes(query);

      return ArrayExtensions.SearchBytes(searchSource, searchKey);
    }

    public static bool RegexSearchExits(this string source, string query)
    {
      return !string.IsNullOrEmpty(source.RegexSearchFirst(query));
    }

    public static string RegexSearchFirst(this string source, string query)
    {
      var match = source.RegexSearch(query).First() as Match;

      if (match != null)
      {
        return match.Value;
      }

      return string.Empty;
    }

    public static MatchCollection RegexSearch(this string source, string query)
    {
      return System.Text.RegularExpressions.Regex.Matches(source, query, RegexOptions.IgnoreCase);
    }

    public static bool ContainsIgnoreCase(this string source, string query)
    {
      return source.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
    }



    public static string Multiply(this string text, int count)
    {
      StringBuilder builder = new StringBuilder(text);
      for (int i = 0; i < count; i++)
      {
        builder.Append(text);
      }

      return builder.ToString();
    }

    public static string Assemble(string seperator, params string[] values)
    {
      return string.Join(seperator, values);
    }

    public static string ToQuotationMarks(this string text)
    {
      return text.ToStringEnhanced("'");
    }

    public static string ParseUntil(this string text, params char[] until)
    {
      return text.Split(until).FirstOrDefault();
    }

    public static string ParseFrom(this string text, params char[] from)
    {
      return text.Split(from).LastOrDefault();
    }

    public static string RemoveLeft(this string text, string remove)
    {
      var index = text.IndexOf(remove, 0, StringComparison.InvariantCultureIgnoreCase);
      return text.Remove(index, remove.Length);
    }

    public static string ReplaceIgnoreCase(this string expression, string search, string replace)
    {
      int foundIndex;
      return ReplaceStringInternal(expression, replace, search, StringComparison.InvariantCultureIgnoreCase, out foundIndex);
    }

    public static string ReplaceCultureIgnoreCase(this string expression, string search, string replace)
    {
      int foundIndex;
      return ReplaceStringInternal(expression, replace, search, StringComparison.CurrentCultureIgnoreCase, out foundIndex);
    }

    private static string ReplaceStringInternal(string exp,
                                             string replaceText,
                                             string find,
                                             StringComparison comparisonType,
                                             out int returnFoundPosition)
    {
      returnFoundPosition = -1;

      //// IMPORTANT: Use "IsNullOrEmpty" not "IsNullOrWhiteSpace"
      if (string.IsNullOrEmpty(exp))
      {
        return exp;
      }

      //// IMPORTANT: Use "IsNullOrEmpty" not "IsNullOrWhiteSpace"
      find = string.IsNullOrEmpty(find) ? ReplaceStringDefaultParameter : find;

      try
      {
        var helper = new StringBuilder();

        int previousIndex = 0;
        int index = exp.IndexOf(find, comparisonType);
        if (index != -1 && returnFoundPosition == -1)
        {
          returnFoundPosition = index;
        }
        while (index != -1)
        {
          helper.Append(exp.Substring(previousIndex, index - previousIndex));
          helper.Append(replaceText);
          index += find.Length;

          previousIndex = index;
          index = exp.IndexOf(find, index, comparisonType);
        }

        helper.Append(exp.Substring(previousIndex));

        return helper.ToString();
      }
      catch
      {
        return exp;
      }
    }



    public static IEnumerable<string> GetParameterValues(this string text)
    {
      return text.Split(Constants.Semicolon);
    }

    public static string ToParameterValues(this IEnumerable<string> parameters)
    {
      return string.Join(Constants.Semicolon.ToString(), parameters);
    }

    public static IDictionary<string, object> GetParameters(this string text)
    {
      var parameters = new System.Collections.Generic.Dictionary<string, object>();

      var pv = text.GetParameterValues();

      foreach (var parameter in pv)
      {
        var key = parameter.ParseUntil(Constants.Semicolon);
        var value = parameter.ParseFrom(Constants.Semicolon);

        parameters.Add(key, value);
      }

      return parameters;
    }

    public static string ParseParameters(this IDictionary<string, object> parameters)
    {
      StringBuilder builder = new StringBuilder();

      foreach (KeyValuePair<string, object> keyValuePair in parameters)
      {
        builder.AppendFormat("{0}:{1};", keyValuePair.Key, keyValuePair.Value);
      }

      return builder.ToString();
    }



    #region StringSegments
    public static StringSegment Segment(this string source, int offset = 0, int length = 1)
    {
      return new StringSegment(source, offset, length);
    }

    public static IEnumerable<StringSegment> SplitInSegments(this string source, char delemiter)
    {
      //if (source.IsNullOrEmpty() || !source.Contains(delemiter)) return Enumerable.Empty<StringSegment>();
      IList<StringSegment> segments = new List<StringSegment>();

      int offset = 0;
      for (int i = 0; i < source.Length; i++)
      {
        if (source[i] == delemiter)
        {
          segments.Add(new StringSegment(source, offset, i));
          offset = i + 1;
        }
      }

      if (offset < source.Length && segments.Any())
      {
        segments.Add(new StringSegment(source, offset, source.Length - offset));
      }

      return segments;
    }

    public static IEnumerable<StringSegment> SplitInSegments(this string source, int defaultSegmentLenght = 64)
    {
      IList<StringSegment> segments = new List<StringSegment>();

      for (int offset = 0; offset < source.Length; offset = (offset + defaultSegmentLenght))
      {
        if ((offset + defaultSegmentLenght) <= source.Length)
        {
          segments.Add(new StringSegment(source, offset, defaultSegmentLenght));
        }
        else if (offset == 0)
        {
          segments.Add(new StringSegment(source, offset, source.Length));
        }
        else
        {
          segments.Add(new StringSegment(source, offset, ((offset + defaultSegmentLenght) - source.Length)));
        }
      }

      return segments;
    }

    public static StringSegment TrimToSegment(this string source)
    {
      var sourceString = source.Trim();
      return new StringSegment(sourceString, 0, sourceString.Length);
    }
    #endregion

    public static bool IsWhiteSpace(this char c)
    {
      return (c == Constants.Space);
    }


    /// <summary>
    /// Gets the collection string. Uses <see cref="StringBuilder"/> and append this with item.ToString().
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The source.</param>
    /// <param name="header"> Optional: The header.</param>
    /// <returns>string with mutliple Lines</returns>
    public static string GetCollectionString<T>(this IEnumerable<T> source, string header = "")
    {
      StringBuilder builder = new StringBuilder();

      if (!string.IsNullOrEmpty(header)) builder.AppendLine(header);

      foreach (T item in source)
      {
        builder.AppendLine(item.ToString());
      }

      return builder.ToString();
    }

    /// <summary>
    /// Removes the curly braces in a string like " {dummy} ".
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    public static string RemoveCurlyBraces(this string source)
    {
      return source.Trim().Replace("{", "").Replace("}", "").Trim();
    }

    /// <summary>
    /// Formats the string double quoted.
    /// </summary>
    /// <param name="s">The s.</param>
    /// <returns></returns>
    public static string FormatDoubleQuoted([CanBeNull] string s)
    {
      s = s ?? string.Empty;
      StringBuilder stringBuilder = new StringBuilder(s.Length + 2);
      stringBuilder.Append("\"");
      stringBuilder.Append(s.Replace("\"", "\"\""));
      stringBuilder.Append("\"");
      return stringBuilder.ToString();
    }


    public static bool EqualsIgnoreCase(this string source, string other)
    {
      return string.Equals(source, other, StringComparison.InvariantCultureIgnoreCase);
    }

    public static int Count(this string source, char pattern)
    {
      int count = 0, current = 0;
      while ((current = source.IndexOf(pattern, current + 1)) != -1)
        count++;

      return count + 1;
    }

    public static int CountInvariant(this string source, string pattern)
    {
      int count = 0, current = 0;
      while ((current = source.IndexOf(pattern, current + 1, StringComparison.InvariantCultureIgnoreCase)) != -1)
        count++;

      return count + 1;
    }

    public static IEnumerable<T> Parse<T>(this string source, Func<string, T> parseingFunc, char splitChar = ',')
    {
      // allocate response list with pre-defined capacity
      // walk the entire input string counting commas to get that capacity
      // it's better to do that extra walk than reallocate underlying Guid[] as elements get added
      int count = 0, current = 0;
      while ((current = source.IndexOf(',', current + 1)) != -1)
        count++;

      List<T> results = new List<T>(count);

      // manually walk the input string and parsing elements out of it as they come
      string substring;
      int last = 0;
      while ((current = source.IndexOf(splitChar, last)) != -1)
      {
        substring = source.Substring(last, current - last);
        results.Add(parseingFunc(substring));
        last = current + 1;
      }

      substring = source.Substring(last);
      results.Add(parseingFunc(substring));

      return results;
    }

    public static string BytesToString(this byte[] bytes)
    {
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < bytes.Length; i++)
      {
        sb.Append(bytes[i].ToString("x2"));
      }
      return sb.ToString();
    }

    public static IEnumerable<T> Split<T>([NotNull] this Regex regex, [NotNull] string input, [NotNull] Func<string, bool, T> itemGenerator)
    {

      // ReSharper disable once AssignNullToNotNullAttribute
      var textParts = regex.Matches(input).Cast<Match>().ToArray();

      Match previousTextPart = null;

      foreach (var textPart in textParts)
      {
        if (textPart == null)
          continue;

        var startIndex = previousTextPart?.Index + previousTextPart?.Length ?? 0;

        yield return itemGenerator(input.Substring(startIndex, textPart.Index - startIndex), false);
        yield return itemGenerator(textPart.Value, true);

        previousTextPart = textPart;
      }

      yield return itemGenerator(input.Substring(previousTextPart?.Index + previousTextPart?.Length ?? 0), false);
    }

    private static readonly Lazy<Regex> Hexcode = new Lazy<Regex>( () => new Regex("%[0-9A-F][0-9A-F]"));

    //private static readonly Lazy<Regex> PhoneNumbersAndPlus = new Lazy<Regex>(() => new Regex("s/[^0-9+]//g;s/[+]49/0/g"));

    //public static string GetUniversalPhoneNumber(string foramttedPhonenumber)
    //{
    //  return PhoneNumbersAndPlus.Value.Replace(foramttedPhonenumber, "");
    //}

    private static string LowerHex(Match m)
    {
      return m.ToString().ToLower();
    }

    public static string UnescapeForEncodeUriCompatability(string source)
    {
      var builder = new StringBuilder(source);
      builder.Replace("%20", " ").Replace("%21", "!").Replace("%2A", "*")
        .Replace("%27", "'").Replace("%28", "(").Replace("%29", ")")
        .Replace("%3B", ";").Replace("%2F", "/").Replace("%3F", "?")
        .Replace("%3A", ":").Replace("%40", "@").Replace("%26", "&")
        .Replace("%3D", "=").Replace("%2B", "+").Replace("%24", "$")
        .Replace("%2C", ",").Replace("%23", "#");
      return Hexcode.Value.Replace(builder.ToString(), LowerHex);
    }
  }
}