namespace Cubic.Core.Tools
{
  public interface IProtectionProvider
  {
    string Encrypt(string input, string obfuscation);

    string Decrypt(string input, string obfuscation);
  }
}