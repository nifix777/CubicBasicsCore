using System;
using System.Collections.Generic;

namespace Cubic.Core.Execution
{
  public struct Optional<T> : IEquatable<Optional<T>>
  {

    private readonly T _value;

    internal Optional(T value)
    {
      if (ReferenceEquals(value, null))
      {
        HasValue = false;
        _value = default(T);
      }
      else
      {
        _value = value;
        HasValue = true;
      }
    }

    public bool HasValue { get; }

    public T Value
    {
      get
      {
        if (!HasValue)
        {
          throw new InvalidOperationException("Optional<T> has no value");
        }
        return _value;
      }
    }
    

    /// <summary>
    /// Implicit cast from the vale to the optional
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static implicit operator Optional<T>(T value)
    {
      return new Optional<T>(value);
    }

    /// <summary>
    /// Explicit cast from option to value
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static explicit operator T(Optional<T> value)
    {
      return value.Value;
    }

    #region Equality members

    /// <inheritdoc />
    public static bool operator ==(Optional<T> left, Optional<T> right)
    {
      return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(Optional<T> left, Optional<T> right)
    {
      return !left.Equals(right);
    }

    /// <inheritdoc />
    public bool Equals(Optional<T> other)
    {
      if (!HasValue) return !other.HasValue;
      if (!other.HasValue) return false;
      return HasValue.Equals(other.HasValue) && EqualityComparer<T>.Default.Equals(_value, other._value);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      return obj is Optional<T> && Equals((Optional<T>)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
      unchecked
      {
        return (HasValue.GetHashCode() * 397) ^ EqualityComparer<T>.Default.GetHashCode(_value);
      }
    }

    #endregion

    /// <inheritdoc />
    public override string ToString()
    {
      return !HasValue ? "<None>" : _value.ToString();
    }

    public static readonly Optional<T> None = default(Optional<T>);

    public static Optional<T> Create(T value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof(value));

      return new Optional<T>(value);
    }

    //public static Optional<T> None<T>()
    //{
    //  return None;
    //}

    public Optional<TResult> Select<TResult>(Func<T, TResult> selector)
    {
      if (selector == null) throw new ArgumentNullException(nameof(selector));

      if (this.HasValue) return Optional<TResult>.Create(selector(_value));

      return Optional<TResult>.None;
    }

    public Optional<TResult> SelectMany<TResult>(Func<T, Optional<TResult>> selector)
    {
      if (selector == null) throw new ArgumentNullException(nameof(selector));

      if (HasValue) return selector(_value);

      return Optional<TResult>.None;
    }

    public TResult Match<TResult>(TResult nothing, TResult just)
    {
      if (this.HasValue) return just;

      return nothing;
    }
  }
}