using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Data
{
  public static class DbNullFunctions
  {
    public static object DBNullValueToNull(object value) => value == DBNull.Value ? null : value;

    /// <summary>
    /// Any DBNull values are converted to null.
    /// </summary>
    /// <param name="values">The source values.</param>
    /// <returns>The converted enumerable.</returns>
    public static IEnumerable<object> DBNullToNull(this IEnumerable<object> values)
    {
      foreach (var v in values)
        yield return DBNullValueToNull(v);
    }

    /// <summary>
    /// Returns a copy of this array with any DBNull values converted to null.
    /// </summary>
    /// <param name="values">The source values.</param>
    /// <returns>A new array containing the results with.</returns>
    public static object[] DBNullToNull(this object[] values)
    {
      var len = values.Length;
      var result = new object[len];
      for (var i = 0; i < len; i++)
      {
        result[i] = DBNullValueToNull(values[i]);
      }
      return result;
    }

    /// <summary>
    /// Replaces any DBNull values in the array with null;
    /// </summary>
    /// <param name="values">The source values.</param>
    /// <returns>The converted enumerable.</returns>
    public static object[] ReplaceDBNullWithNull(this object[] values)
    {
      var len = values.Length;
      for (var i = 0; i < len; i++)
      {
        var value = values[i];
        if (value == DBNull.Value) values[i] = null;
      }
      return values;
    }
  }
}
