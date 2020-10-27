using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Cubic.Core.Annotations;
using Cubic.Core.Reflection;
using Cubic.Core.Runtime;

namespace Cubic.Core.Diagnostics
{
  public static class Guard
  {

    public static AtomicBoolean GlobalUseTrace = new AtomicBoolean(false);
    public static AtomicBoolean GlobalUseThrow = new AtomicBoolean(false);

    public static void GlobalVerifyThrow(Exception error)
    {
      if (GlobalUseTrace.Value)
      {
        Trace.TraceError(error.ToString());
      }

      if (GlobalUseThrow.Value)
      {
        throw error;
      }
    }


    [DebuggerStepThrough]
    public static void TypeHasDefaultConstructor( Type type , [InvokerParameterName] string argumentName )
    {
      if ( type.HasDefaultConstructor() )
      {
        var error = $"Type '{type.FullName}' must have a default constructor.";
        throw new ArgumentException( error , argumentName );
      }
    }

    [DebuggerStepThrough]
    public static void AgainstNull( object value, [InvokerParameterName] string argumentName = null)
    {
      if (value == null)
      {
        throw new ArgumentNullException(argumentName);
      }
    }

    [DebuggerStepThrough]
    public static object NotNull(this object value, [InvokerParameterName] string argumentName = null)
    {
      if (!(value is null))
      {
        return value;
      }

      throw new ArgumentNullException(argumentName);
    }

    [DebuggerStepThrough]
    public static TValue NotNull<TValue>(this TValue value, string argumentName ) where TValue : class
    {
      if (!(value is null))
      {
        return value;
      }

      throw new ArgumentNullException(argumentName);
    }

    [DebuggerStepThrough]
    public static void ArgumentNull( object value, [InvokerParameterName] string argumentName)
    {
      if (value == null)
      {
        throw new ArgumentNullException(argumentName);
      }
    }

    [DebuggerStepThrough]
    public static void AgainstNullOrEmpty( [NotNull] string value, [InvokerParameterName] string argumentName )
    {
      if ( string.IsNullOrWhiteSpace( value ) )
      {
        throw new ArgumentNullException( argumentName );
      }
    }

    [DebuggerStepThrough]
    public static void AgainstNullOrEmpty(  [NotNull] ICollection value, [InvokerParameterName] string argumentName )
    {
      if ( value == null )
      {
        throw new ArgumentNullException( argumentName );
      }
      if ( value.Count == 0 )
      {
        throw new ArgumentOutOfRangeException( argumentName );
      }
    }

    [DebuggerStepThrough]
    public static void AgainstNullOrEmpty<T>( [NotNull] IEnumerable<T> value, [InvokerParameterName] string argumentName )
    {
      if ( value == null )
      {
        throw new ArgumentNullException( argumentName );
      }
      if ( !value.Any() )
      {
        throw new ArgumentOutOfRangeException( argumentName );
      }
    }

    [DebuggerStepThrough]
    public static void AgainstNegativeAndZero( [InvokerParameterName] string argumentName , int value )
    {
      if ( value <= 0 )
      {
        throw new ArgumentOutOfRangeException( argumentName );
      }
    }

    [DebuggerStepThrough]
    public static void AgainstNegative( [InvokerParameterName] string argumentName , int value )
    {
      if ( value < 0 )
      {
        throw new ArgumentOutOfRangeException( argumentName );
      }
    }

    [DebuggerStepThrough]
    public static void AgainstNegativeAndZero( [InvokerParameterName] string argumentName , TimeSpan value )
    {
      if ( value <= TimeSpan.Zero )
      {
        throw new ArgumentOutOfRangeException( argumentName );
      }
    }

    [DebuggerStepThrough]
    public static void AgainstNegative( [InvokerParameterName] string argumentName , TimeSpan value )
    {
      if ( value < TimeSpan.Zero )
      {
        throw new ArgumentOutOfRangeException( argumentName );
      }
    }

    [DebuggerStepThrough]
    public static int CheckInRangeInclusive(int val, string parameterName, int lower, int upper)
    {
      if (val < lower || val > upper)
        throw new ArgumentOutOfRangeException(parameterName, val, "Expected: " + lower.ToString(CultureInfo.InvariantCulture) + " <= " + parameterName + " <= " + upper.ToString(CultureInfo.InvariantCulture));
      return val;
    }

    [DebuggerStepThrough]
    public static void EnsuresArgument(Func<bool> condition, string argument, string message = "")
    {
      var msg = string.IsNullOrEmpty(message) ? string.Format("Argument {0} does not fit the Requirements.", argument) : message;
      if (!condition.Invoke())
      {
        throw new ArgumentException(msg);
      }
    }

    [DebuggerStepThrough]
    public static void EnsuresArgument(bool condition, string argument, string message = "")
    {
      var msg = string.IsNullOrEmpty(message) ? string.Format("Argument {0} does not fit the Requirements.", argument) : message;
      if (!condition)
      {
        throw new ArgumentException(msg);
      }
    }

    /// <summary>
    /// Ensures that the given expression is <see langword="true"/>.
    /// </summary>
    /// <typeparam name="TException">Type of exception to throw</typeparam>
    /// <param name="condition">Condition to test/ensure</param>
    /// <param name="message">Message for the exception</param>
    /// <exception>
    /// Thrown when <cref>TException</cref> <paramref name="condition"/> is <see langword="false"/>.
    /// </exception>
    [DebuggerStepThrough]
    public static void Ensure<TException>(bool condition, string message = "The given condition is false.") where TException : Exception
    {
      if (!condition)
      {
        Guard.Throw<TException>(message);
      }
    }

    [DebuggerStepThrough]
    public static void Ensure<TException>(Func<bool> condition, string message = "The given condition is false.") where TException : Exception
    {
      if (!condition())
      {
        Guard.Throw<TException>(message);
      }
    }

    [DebuggerStepThrough]
    public static void Throw<TException>(string message) where TException : Exception
    {
      throw (TException)Activator.CreateInstance(typeof(TException), message);
    }

    [DebuggerStepThrough]
    public static void ThrowWrap<TException>(string message, Exception inner) where TException : Exception
    {
      throw (TException)Activator.CreateInstance(typeof(TException), message, inner);
    }
  }
}