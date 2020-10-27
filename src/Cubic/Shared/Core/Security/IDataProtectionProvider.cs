

namespace Cubic.Core.Security
{
  public interface IDataProtectionProvider
  {
    IDataProtector CreateProtector(string purpose);
  }
}