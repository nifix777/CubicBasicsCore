using System;
using System.Data;
using System.Globalization;
using System.Linq;

namespace Cubic.Core.Text
{
  public static class SqlStrings
  {
    public const string Null = "NULL";
    public const string And = "AND";
    public const string Or = "Or";
    public const string Select = "SELECT";
    public const string From = "FROM";
    public const string Where = "WHERE";

    public static string ClauseAnd(string clause, string argument)
    {
      return SqlStrings.ClauseAnd(clause, argument, true);
    }

    public static string ClauseAnd(string clause, string argument, bool isValidArgument)
    {
      string str = null;
      if (!isValidArgument || String.IsNullOrEmpty(argument))
      {
        str = clause;
      }
      else
      {
        str = (String.IsNullOrEmpty(clause) ? argument : string.Concat(clause, Constants.Space, And, Constants.Space, argument));
      }
      return str;
    }

    public static string ClauseOr(string clause, string argument)
    {
      return SqlStrings.ClauseOr(clause, argument, true);
    }

    public static string ClauseOr(string clause, string argument, bool isValidArgument)
    {
      string empty = String.Empty;
      if (isValidArgument && !String.IsNullOrEmpty(argument))
      {
        empty = (String.IsNullOrEmpty(clause) ? argument : string.Concat(clause, Constants.Space, Or, Constants.Space, argument));
      }
      else if (!String.IsNullOrEmpty(clause))
      {
        empty = clause;
      }
      return empty;
    }

    public static string SqlStringToString(string source)
    {
      string str = null;
      if (String.IsNullOrEmpty(source))
      {
        return null;
      }
      if (source == Null)
      {
        str = null;
      }
      else if ((source.Substring(0, 1) == "'") & (source.Substring(source.Length - 1) == "'") && source.Substring(1, source.Length - 2).Replace("''", "#").IndexOf("'", StringComparison.Ordinal) == -1)
      {
        str = source.Substring(1, source.Length - 2).Replace("''", "'");
      }
      return str;
    }

    public static string ToSqlString(this string source, bool nvarChar = false)
    {
      return SqlStrings.ToSqlString(source, false, nvarChar);
    }

    public static string ToSqlString(string source, bool allowEmptyString, bool nvarChar, int minLength = 0, int maxLenght = 0)
    {
      var sql = source;

      if(minLength > 0 && sql.Length < minLength)
      {
        sql = source.PadRight(minLength);
      }

      if (maxLenght > 0 && sql.Length > maxLenght)
      {
        sql = source.Substring(0, maxLenght);
      }

      if (string.IsNullOrEmpty(sql))
      {
        if (allowEmptyString)
        {
          return "''";
        }
        return Null;
      }
      if (nvarChar)
      {
        return string.Concat("N'", sql.Replace("'", "''"), "'"); 
      }
      else
      {
        return string.Concat("'", sql.Replace("'", "''"), "'");
      }
    }

    public static bool IsSqlSafeString(string source)
    {
      return !(source.Contains("'"));
    }

    public static string TrimQuery(string query)
    {
      query = query ?? string.Empty;
      string str = query.TrimEnd(new char[] { ' ' });
      str = str.Replace("%", "*");
      if (str.Length > 0 && str.Substring(str.Length - 1) == "%")
      {
        return str;
      }
      return string.Concat(str, "%");
    }

    public static string DateToSqlServer(DateTime? value)
    {
      if (!value.HasValue)
      {
        return "Null";
      }
      return DateTimeFunctions.DateToSqlServer(value.Value);
    }

    public static string DateToSqlServer(object dateTimeValue)
    {
      return DateTimeFunctions.DateToSqlServer(Convert.ToDateTime(dateTimeValue, CultureInfo.InvariantCulture));
    }

    public static string DateToSqlServer(DateTime dateTimeValue)
    {
      return dateTimeValue.ToString("s");
    }

    public static string ToSqlDate(this DateTime value)
    {
      return value.ToString("yyyyMMMMdd");
    }

    public static string ToSqlTime(this DateTime value)
    {
      return value.ToString("t");
    }

    public static string ToSql(this DateTime value)
    {
      return value.ToString("yyyyMMMMdd HH:mm:ss");
    }

    public static string ToSql(this DateTimeOffset value)
    {
      return value.ToString("yyyy-MM-dd HH:mm:ss.fffffff zzz");
    }

    public static string ToSql(bool value)
    {
      return value ? 1.ToString() : 0.ToString();
    }

    private static string ToSql(byte[] source)
    {
      return $"0x{BitConverter.ToString(source).Replace("-", "")}";
    }

    public static string ToSql(object expression)
    {
      //if (expression == DBNull.Value) return Null;

      if (expression.IsNullOrDbNull()) return Null;

      if (expression is string)
      {
        return SqlStrings.ToSqlString((string)expression);
      }
      else if (expression is bool)
      {
        return SqlStrings.ToSql((bool)expression);
      }
      else if (expression is int || expression is short || expression is long)
      {
        return expression.ToInvariant();
      }
      else if (expression is decimal || expression is float || expression is double)
      {
        return expression.ToInvariant();
      }
      else if (expression is DateTime)
      {
        return SqlStrings.ToSql((DateTime)expression);
      }
      else if (expression is Guid)
      {
        return SqlStrings.ToSqlString(expression.ToGuid().ToString("D"));
      }
      else if (expression is byte[] bytes)
      {
        return SqlStrings.ToSql(bytes);
      }
      else
      {
        return SqlStrings.ToSqlString(expression.ToString());
      }
    }

    public static string ToSql(object expression, DbType dbtype, int minLength = 0, int maxLenght = 0)
    {

      if (expression.IsNullOrDbNull()) return SqlStrings.Null;

      switch (dbtype)
      {
        case DbType.AnsiString:
        case DbType.AnsiStringFixedLength:
          return SqlStrings.ToSqlString(expression.ToString(), false, false, minLength, maxLenght);

        case DbType.String:
        case DbType.StringFixedLength:
          return SqlStrings.ToSqlString(expression.ToString(), false, true, minLength, maxLenght);

        case DbType.Binary:
          return SqlStrings.BinaryToString(expression.ToBytes(), minLength, maxLenght);
        case DbType.Boolean:
        case DbType.Int16:
        case DbType.Int32:
        case DbType.Int64:
        case DbType.UInt16:
        case DbType.UInt32:
        case DbType.UInt64:
        case DbType.Guid:
        case DbType.Decimal:
        case DbType.Single:
        case DbType.SByte:
        case DbType.Byte:
        case DbType.Currency:
        case DbType.Double:
        case DbType.Xml:
        case DbType.VarNumeric:
        case DbType.Object:
          return ToSql(expression);

        case DbType.Time:
          return SqlStrings.ToSqlTime(expression.ToDateTime());
        case DbType.Date:
          return SqlStrings.ToSqlDate(expression.ToDateTime());
        case DbType.DateTime:
        case DbType.DateTime2:
          return SqlStrings.ToSql(expression.ToDateTime());

        case DbType.DateTimeOffset:
          return SqlStrings.ToSql(expression.ToDateTimeOffset());

        default:
          throw new InvalidOperationException();
      }
    }

    private static string BinaryToString(byte[]bytes, int minLength, int maxLenght)
    {
      var sql = SqlStrings.Null;

      if(minLength > 0)
      {
        sql = bytes.ToSql();
      }
      if(maxLenght > 0 && bytes.Length >  maxLenght)
      {
        sql = bytes.Take(maxLenght).ToArray().ToSql();
      }

      return sql;
    }

    /// <summary>Encodes the name of an SQL-Object.</summary>
    /// <param name="name">The name of the SQL-Object.</param>
    /// <returns>The encoded string if necessary.</returns>
    /// <example>"mytable." as an SQL-Name converts to "mytable%2E".
    /// <code></code></example>
    public static string EncodeSqlName(string name)
    {
      System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(255);
      for (int i = 0; i < name.Length; i++)
      {
        if (char.IsControl(name[i]) || name[i] == '.' || "\\/:%<>*?[]|".Contains(name[i].ToString()))
        {
          stringBuilder.Append('%');
          stringBuilder.Append(string.Format("{0:X2}", Convert.ToByte(name[i])));
        }
        else
        {
          stringBuilder.Append(name[i]);
        }
      }
      return stringBuilder.ToString();
    }
  }
}