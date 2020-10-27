namespace Cubic.Core.Numeric.Money
{
  public interface ICurrencyConverter
  {
    Money Convert( Money fromMoney , Currency toCurrency );
    decimal Convert( decimal fromAmount , Currency fromCurrency , Currency toCurrency );
  }
}