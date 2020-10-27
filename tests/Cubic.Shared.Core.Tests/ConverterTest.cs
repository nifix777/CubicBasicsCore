using System;
using System.Text;
using System.Collections.Generic;
using Cubic.Core;
using Cubic.Core.Tools;
using Xunit;

namespace Cubic.Basics.Testing
{
  /// <summary>
  /// Tests für Convertierung von simplen DatenTypen per <see cref="Converter"/>
  /// </summary>
  
  public class ConverterTest
  {

    [Fact]
    public void TestIntToBool()
    {
      object source = 1;

      object target = null;

      target = Converter.Convert(typeof(bool), source);

      Assert.NotNull(target);

      Assert.True((bool)target);
    }
  }
}
