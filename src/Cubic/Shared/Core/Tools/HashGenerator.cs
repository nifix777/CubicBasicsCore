using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Cubic.Core.Tools
{
  public static class HashGenerator
  {
    private static string GenerateHash(string value, HashAlgorithm hasher)
    {
      byte[] bytes = Encoding.ASCII.GetBytes(value);
      return Convert.ToBase64String(hasher.ComputeHash(bytes));
    }

    private static string GenerateHash(Stream stream, HashAlgorithm hasher)
    {
      if (stream.CanSeek)
      {
        stream.Seek((long)0, SeekOrigin.Begin);
      }
      return Convert.ToBase64String(hasher.ComputeHash(stream));
    }

    public static string GenerateMD5(string value)
    {
      return HashGenerator.GenerateHash(value, new MD5CryptoServiceProvider());
    }

    public static string GenerateMD5(Stream stream)
    {
      return HashGenerator.GenerateHash(stream, new MD5CryptoServiceProvider());
    }

    public static string GenerateSHA1(string value)
    {
      return HashGenerator.GenerateHash(value, new SHA1Managed());
    }

    public static string GenerateSHA1(Stream stream)
    {
      return HashGenerator.GenerateHash(stream, new SHA1Managed());
    }
  }
}