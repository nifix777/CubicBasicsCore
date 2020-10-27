using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Cubic.Core.Security
{
  public interface IDataProtector
  {
    byte[] Protect(byte[] unprotectedBytes);
    byte[] Unprotect(byte[] protectedBytes);
  }

  //public class CustomDataProtector : IDataProtector
  //{
  //  private byte[] _passwordBytes;

  //  private byte[] _saltBytes;

  //  private SymmetricAlgorithm _algorithm;

  //  public byte[] Protect(byte[] unprotectedBytes)
  //  {
  //    throw new System.NotImplementedException();
  //  }

  //  public byte[] Unprotect(byte[] protectedBytes)
  //  {
  //    throw new System.NotImplementedException();
  //  }

  //  private byte[] Transform(byte[] data, ICryptoTransform cryptoTransform)
  //  {
  //    MemoryStream stream = new MemoryStream();
  //    CryptoStream cryptoStream = new CryptoStream(stream, cryptoTransform, CryptoStreamMode.Write);

  //    cryptoStream.Write(data, 0, data.Length);
  //    cryptoStream.FlushFinalBlock();

  //    return stream.ToArray();
  //  }
  //}
}