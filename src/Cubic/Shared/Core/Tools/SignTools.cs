using System;
using System.Security.Cryptography;
using System.Text;

namespace Cubic.Core.Tools
{
  public static class SignTools
  {
    public static string SignData(string message, RSAParameters privateKey)
    {
      return Convert.ToBase64String(SignTools.SignData(Encoding.UTF8.GetBytes(message), privateKey));
    }

    public static byte[] SignData(byte[] data, RSAParameters privateKey)
    {
      //// The array to store the signed message in bytes
      byte[] signedBytes;
      using (var rsa = new RSACryptoServiceProvider())
      {

        try
        {
          //// Import the private key used for signing the message
          rsa.ImportParameters(privateKey);

          //// Sign the data, using SHA512 as the hashing algorithm 
          signedBytes = rsa.SignData(data, CryptoConfig.MapNameToOID("SHA512"));
        }
        finally
        {
          //// Set the keycontainer to be cleared when rsa is garbage collected.
          rsa.PersistKeyInCsp = false;
        }
      }
      //// Convert the a base64 string before returning
      return signedBytes;
    }

    public static bool VerifyData(string originalMessage, string signedMessage, RSAParameters publicKey, Encoding encoding)
    {
      return SignTools.VerifyData(encoding.GetBytes(originalMessage), Convert.FromBase64String(signedMessage),
        publicKey);
    }

    public static bool VerifyData(byte[] originalBytes, byte[] signedBytes, RSAParameters publicKey)
    {
      bool success = false;
      using (var rsa = new RSACryptoServiceProvider())
      {
        try
        {
          rsa.ImportParameters(publicKey);

          SHA512Managed hash = new SHA512Managed();

          byte[] hashedData = hash.ComputeHash(signedBytes);

          success = rsa.VerifyData(originalBytes, CryptoConfig.MapNameToOID("SHA512"), signedBytes);
        }
        finally
        {
          rsa.PersistKeyInCsp = false;
        }
      }
      return success;
    }
  }
}