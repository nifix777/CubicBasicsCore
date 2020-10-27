using System;
using Cubic.Core.Diagnostics;

namespace Cubic.Core.Numeric.Money
{
  /// <summary>
  /// Represents a Money object
  /// </summary>
  public struct Money : IEquatable<Money>, IComparable<Money>
  {
    public Money( Currency currency = Currency.Invalid , decimal amount = 0 )
    {
      this.Currency = currency;
      this.Amount = amount;
    }

    public Currency Currency { get; }

    public decimal Amount { get; }

    public override int GetHashCode()
    {
      return ( (Currency.ToInt32() * Amount).ToInt32() * 2);
    }

    public override string ToString()
    {
      return $"{this.Amount} {this.Currency.GetName<Currency>()}";
    }


    #region Operators
    public static Money operator + ( Money money1 , Money money2 )
    {
      Guard.EnsuresArgument( () => money1.Currency == money2.Currency , "Different Currencies." );

      //money1.Amount += money2.Amount;
      money1 = new Money(money1.Currency, money1.Amount + money2.Amount);
      return money1;
    }

    public static Money operator - ( Money money1 , Money money2 )
    {
      Guard.EnsuresArgument( () => money1.Currency == money2.Currency , "Different Currencies." );

      //money1.Amount -= money2.Amount;
      money1 = new Money(money1.Currency, money1.Amount - money2.Amount);
      return money1;
    }

    public static Money operator * ( Money money1 , Money money2 )
    {
      Guard.EnsuresArgument( () => money1.Currency == money2.Currency , "Different Currencies." );

      //money1.Amount *= money2.Amount;
      money1 = new Money(money1.Currency, money1.Amount * money2.Amount);
      return money1;
    }

    public static Money operator / ( Money money1 , Money money2 )
    {
      Guard.EnsuresArgument( () => money1.Currency == money2.Currency , "Different Currencies." );

      //money1.Amount -= money2.Amount;
      money1 = new Money(money1.Currency, money1.Amount / money2.Amount);
      return money1;
    }


    public static Money operator +( Money money1 , decimal money2 )
    {

      //money1.Amount += money2;
      money1 = new Money(money1.Currency, money1.Amount + money2);
      return money1;
    }

    public static Money operator -( Money money1 , decimal money2 )
    {

      //money1.Amount -= money2;
      money1 = new Money(money1.Currency, money1.Amount - money2);
      return money1;
    }

    public static Money operator *( Money money1 , decimal money2 )
    {

      //money1.Amount *= money2;
      money1 = new Money(money1.Currency, money1.Amount * money2);
      return money1;
    }

    public static Money operator /( Money money1 , decimal money2 )
    {

      //money1.Amount -= money2;
      money1 = new Money(money1.Currency, money1.Amount / money2);
      return money1;
    }
    #endregion


    public int CompareTo(Money other)
    {
      if ( this.Currency != other.Currency )
        throw new Exception($"Cannot compare {this.Currency} and {other.Currency}.");

      return this.Amount.CompareTo( other.Amount );
    }



    public bool Equals( Money other )
    {

      if (this.Currency != other.Currency) return false;

      return this.Amount == other.Amount;
    }


    public static Money Round( Money money , int decimals )
    {
      //money.Amount = decimal.Round( money.Amount , MidpointRounding.AwayFromZero );
      money = new Money(money.Currency, decimal.Round(money.Amount, MidpointRounding.AwayFromZero));
      return money;
    }
  }
}