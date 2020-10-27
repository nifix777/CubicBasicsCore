//using System;
//using Cubic.Core.Net;
//using Cubic.Core.Security;
//using Cubic.Basics.Net;
//using Cubic.Basics.Security;
//using Xunit;

//namespace Cubic.Basics.Testing.Security
//{
//  
//  public class SecurityTests
//  {
//    [Fact]
//    public void ManagedSecureStringTest()
//    {
//      var expectedUnsecure = "my sensative data";
//      var unsecured = "my sensative data";
//      var key = "password";

//      var secureString = ManagedSecureString.CreateManagedSecureString(unsecured, key);

//      Assert.NotNull(secureString, nameof(ManagedSecureString));
//      Assert.AreNotEqual(unsecured, secureString);

//      var dangerous = secureString.GetInsecureString();

//      Assert.Equal(expectedUnsecure, dangerous);
//    }

//    [Fact]
//    public void ProtectStringTest()
//    {
//      var expectedUnsecure = "my sensative data";
//      var unsecured = "my sensative data";
//      var key = "password";

//      SecurityExtensions.EncryptWithAes(ref unsecured, key);

//      Assert.AreNotEqual(unsecured, expectedUnsecure);

//      SecurityExtensions.DencryptWithAes(ref unsecured, key);

//      Assert.Equal(expectedUnsecure, unsecured);
//    }

//    [Fact]
//    public void SecureCredentialTest()
//    {
//      var original = new NamePasswordCredential("user", "password");
//      var key = "secret";

//      var secure = SecureCredential.GetSecureCredential(original, key);

//      Assert.NotNull(secure, nameof(SecureCredential));

//      Assert.AreNotEqual(original.User, secure.User);
//      Assert.AreNotEqual(original.Password, secure.Password);

//      var unsecure = secure.GetUnsecuredCredential(key);

//      Assert.Equal(original.User, unsecure.User);
//      Assert.Equal(original.Password, unsecure.Password);

//    }
//  }
//}
