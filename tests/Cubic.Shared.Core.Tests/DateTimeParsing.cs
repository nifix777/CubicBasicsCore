using System;
using Xunit;
using Cubic.Core;

namespace Cubic.Basics.Testing
{
  
  public class DateTimeParsingTests
  {
    [Fact]
    public void TestParseDateTimeWithTimeZone()
    {
      var input = "24-10-2008 21:09:06 CEST";
      var output = Cubic.Core.DateTimeFunctions.ParseDateTimeWithTimeZone(input);

    }
  }
}