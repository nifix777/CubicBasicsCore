namespace Cubic.Core.Tools
{
  public class NoneProtectionProvider : IProtectionProvider
  {
    public static IProtectionProvider Instanze = new NoneProtectionProvider();

    public string Encrypt(string input, string obfuscation)
    {
      return obfuscation;
    }

    public string Decrypt(string input, string obfuscation)
    {
      return obfuscation;
    }
  }
}