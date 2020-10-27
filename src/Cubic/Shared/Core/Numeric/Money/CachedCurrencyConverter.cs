using System;
using System.Collections.Generic;

namespace Cubic.Core.Numeric.Money
{
  public class CachedCurrencyConverter : ICurrencyConverter
  {
    public ICurrencyConverter CurrencyConverter { get; private set; }
    private readonly IDictionary<string , decimal> rateCache;

    private decimal value;
    public CachedCurrencyConverter( ICurrencyConverter currencyConverter )
    {
      if ( currencyConverter == null )
        throw new ArgumentNullException( "currencyConverter" );

      this.CurrencyConverter = currencyConverter;
      this.rateCache = new Dictionary<string , decimal>();
    }

    private void PopulateRate( Currency fromCurrency , Currency toCurrency )
    {
      var rate = this.CurrencyConverter.Convert( this.value , fromCurrency , toCurrency );
      var key = GenerateKey( fromCurrency , toCurrency );
      this.rateCache[key] = rate;
    }

    private static string GenerateKey( Currency fromCurrency , Currency toCurrency )
    {
      return string.Format( "{0}-{1}" , fromCurrency , toCurrency );
    }

    public Money Convert(Money fromMoney, Currency toCurrency)
    {
      Money newMoney = new Money(toCurrency, this.Convert(fromMoney.Amount, fromMoney.Currency, toCurrency));
      
      return newMoney;
    }

    public decimal Convert(decimal fromAmount, Currency fromCurrency, Currency toCurrency)
    {

      var key = GenerateKey(fromCurrency, toCurrency);
      value = fromAmount;
      if (!rateCache.ContainsKey(key))
      {
        PopulateRate(fromCurrency, toCurrency);
      }
      return rateCache[key];
    }
  }
}