using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Cubic.Core.Runtime;

namespace Cubic.Core.Text
{
  public static class EncodingFunctions
  {
    private static EncodingPreambleInfo[] _encodingPreambles;

    private static int _maxPrembleLength = 0;

    public static readonly Encoding Utf8NoBOM = new UTF8Encoding(false);

    public static Encoding GetEncodingFromStream(Stream stream)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      GetEncodingsFromSystem();


      byte[] buffer = new byte[_maxPrembleLength];

      stream.Read(buffer, 0, (int) Math.Min(_maxPrembleLength, stream.Length));

      // Reset Position
      stream.Position = 0;

      return _encodingPreambles
        .Where(enc => enc.Preamble.SequenceEqual(buffer.Take(enc.Preamble.Length)))
        .Select(enc => enc.Encoding)
        .FirstOrDefault() ?? System.Text.Encoding.Default;
    }



    private static void GetEncodingsFromSystem()
    {
      if (_encodingPreambles == null)
      {
        _encodingPreambles = Enumerable.ToArray(System.Text.Encoding.GetEncodings()
          .Select(e => e.GetEncoding())
          //.Select(e => new { Encoding = e, Preamble = e.GetPreamble() })
          .Select(e => new EncodingPreambleInfo(e, e.GetPreamble()))
          .Where(e => e.Preamble.Any()));

        _maxPrembleLength = _encodingPreambles.Max(e => e.Preamble.Length);
      }
    }

    public static Encoding GetEncoding(string file)
    {
      using (var stream = File.OpenRead(file))
      {
        return EncodingFunctions.GetEncodingFromStream(stream);
      }

    }

    public static string ReadAllText(string filename, Encoding defaultEncoding, out Encoding usedEncoding)
    {
      byte[] bytes;
      using (var fileStream = File.OpenRead(filename))
      {
        // Detect the encoding of the file:
        usedEncoding = GetEncodingFromStream(fileStream);

        // read bytes
        fileStream.Position = 0;
        bytes = new byte[fileStream.Length];
        fileStream.Read(bytes, 0, bytes.Length);
      }

      // If none found, use the default encoding.
      // Otherwise, determine the length of the encoding markers in the file
      int offset;
      if (usedEncoding == null)
      {
        offset = 0;
        usedEncoding = defaultEncoding;
      }
      else
      {
        offset = usedEncoding.GetPreamble().Length;
      }

      // Now interpret the bytes according to the encoding,
      // skipping the preample (if any)
      return usedEncoding.GetString(bytes, offset, bytes.Length - offset);
    }

    public static bool IsBase64String(string base64String)
    {
      base64String = base64String.Trim();
      return (base64String.Length % 4 == 0) &&
             Regex.IsMatch(base64String, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);

    }

    public static string Base64Decode(this string data)
    {
      return data.Base64Decode(System.Text.Encoding.UTF8);
    }

    public static string Base64Decode(this string data, Encoding encoding)
    {
      return encoding.GetString(Convert.FromBase64String(data));
    }

    public static string Base64Encode(this string data)
    {
      return data.Base64Encode(System.Text.Encoding.UTF8);
    }

    public static byte[] FromBase64(this string data)
    {
      return Convert.FromBase64String(data);
    }

    public static string ToBase64(this byte[] data)
    {
      return Convert.ToBase64String(data);
    }

    public static string Base64Encode(this string data, Encoding encoding)
    {
      return Convert.ToBase64String(encoding.GetBytes(data));
    }

    public static Encoding ConvertToEncoding(string encodingNameOrId)
    {
      if (!encodingNameOrId.IsNullOrEmptyOrWhiteSpace())
      {
        if (encodingNameOrId.IsNumeric())
        {
          return
            new Func<Encoding>(() => Encoding.GetEncoding(encodingNameOrId.ToInt32())).Catch<Encoding, Exception>(
              Encoding.Default);
        }
        if (encodingNameOrId == "Default")
        {
          return Encoding.Default;
        }
        if (encodingNameOrId == "UTF7" | encodingNameOrId == "UTF-7")
        {
          return Encoding.UTF7;
        }
        if (encodingNameOrId == "UTF8" | encodingNameOrId == "UTF-8")
        {
          return Encoding.UTF8;
        }
        if (encodingNameOrId == "UTF8NoBOM" | encodingNameOrId == "UTF-8NoBOM")
        {
          return EncodingFunctions.Utf8NoBOM;
        }
        if (encodingNameOrId == "UTF32" | encodingNameOrId == "UTF-32")
        {
          return Encoding.UTF32;
        }
        if (encodingNameOrId == "ASCII")
        {
          return Encoding.ASCII;
        }
        if (encodingNameOrId == "BigEndianUnicode")
        {
          return Encoding.BigEndianUnicode;
        }
        if (encodingNameOrId == "Unicode" | encodingNameOrId == "UTF16" | encodingNameOrId == "UTF-16" | encodingNameOrId == "UTF-16LE")
        {
          return Encoding.Unicode;
        }

        return new Func<Encoding>(() => Encoding.GetEncoding(encodingNameOrId)).Catch<Encoding, Exception>(
              Encoding.Default);
      }

      return Encoding.Default;
    }

    public static string GetUnicodeSymbol(this string unicodeString)
    {
      return char.ConvertFromUtf32(int.Parse(unicodeString, System.Globalization.NumberStyles.HexNumber));
    }

    public static int GetFromUnicode(this string unicodeString)
    {
      if (unicodeString.StartsWith("U+"))
      {
        return int.Parse(unicodeString.Substring(2), System.Globalization.NumberStyles.HexNumber);
      }
      return int.Parse(unicodeString, System.Globalization.NumberStyles.HexNumber);
    }

    public static byte[] GetBytes(string text, Encoding encoding)
    {
      return encoding.GetBytes(text);
    }

    public static string GetString(byte[] data, Encoding encoding)
    {
      return encoding.GetString(data);
    }

    public static byte[] ToUtf8(this string text)
    {
      return EncodingFunctions.GetBytes(text, Encoding.UTF8);
    }

    public static byte[] ToUtf8NoBOM(this string text)
    {
      return EncodingFunctions.GetBytes(text, EncodingFunctions.Utf8NoBOM);
    }

    public static byte[] ToUnicode(this string text)
    {
      return EncodingFunctions.GetBytes(text, Encoding.Unicode);
    }

    public static byte[] ToAscii(this string text)
    {
      return EncodingFunctions.GetBytes(text, Encoding.ASCII);
    }

    public static byte[] FromUnicode(this string text)
    {
      return GetBytes(text, Encoding.Unicode);
    }

    public static byte[] FromUtf8(this string text)
    {
      return GetBytes(text, Encoding.UTF8);
    }

    public static byte[] FromAscii(this string text)
    {
      return GetBytes(text, Encoding.ASCII);
    }

    public static string ChangeEncoding(this string text, Encoding source, Encoding target)
    {
      byte[] utfBytes = source.GetBytes(text);
      byte[] isoBytes = Encoding.Convert(source, target, utfBytes);
      return target.GetString(isoBytes);
    }

    public static byte[] GetRawBytes(this string text)
    {
      byte[] bytes = new byte[text.Length * sizeof(char)];
      System.Buffer.BlockCopy(text.ToCharArray(), 0, bytes, 0, bytes.Length);
      return bytes;
    }

    public static string GetRawString(this byte[] data)
    {
      char[] chars = new char[data.Length / sizeof(char)];
      System.Buffer.BlockCopy(data, 0, chars, 0, data.Length);
      return new string(chars);
    }

    internal class EncodingPreambleInfo
    {
      /// <summary>
      /// System AG: Internal field with encoding.
      /// </summary>
      protected Encoding EncodingInfo;

      /// <summary>
      /// System AG: Internal field with preamble.
      /// </summary>
      protected byte[] PreambleInfo;

      /// <summary>
      /// System AG: Property Encoding (Encoding).
      /// </summary>
      /// <value>The encoding.</value>
      public Encoding Encoding
      {
        get { return this.EncodingInfo; }
      }

      /// <summary>
      /// System AG: Property Preamble (byte[]).
      /// </summary>
      /// <value>The preamble.</value>
      public byte[] Preamble
      {
        get { return this.PreambleInfo; }
      }

      /// <summary>
      /// System AG: Constructor with preamble and encoding
      /// </summary>
      /// <param name="encoding">The encoding.</param>
      /// <param name="preamble">The preamble.</param>
      public EncodingPreambleInfo(Encoding encoding,
        byte[] preamble)
      {
        this.EncodingInfo = encoding;
        this.PreambleInfo = preamble;
      }
    }
  }


}