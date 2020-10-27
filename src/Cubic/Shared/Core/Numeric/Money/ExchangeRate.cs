using System;

namespace Cubic.Core.Numeric.Money
{
  public class ExchangeRate
  {
    public Currency Currency { get; set; }

    public DateTime Date { get; set; }

    public decimal Rate { get; set; }
  }
}