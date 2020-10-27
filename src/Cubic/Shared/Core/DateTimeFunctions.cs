using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Threading;
using Cubic.Core.Annotations;
using Cubic.Core.Collections;

namespace Cubic.Core
{
  /// <summary>
  /// Varius functions and variables for Date\Time workloads
  /// </summary>
  public static class DateTimeFunctions
  {
    /// <summary>
    /// The unix date time base as 01.01.1970 UTC
    /// </summary>
    internal static readonly DateTime UnixDateTimeBase = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// The epoch milliseconds: <see cref="UnixDateTimeBase"/> / 10000L 
    /// </summary>
    internal static readonly long EpochMilliseconds = UnixDateTimeBase.Ticks / 10000L;

    /// <summary>
    /// The time zone dictionary
    /// </summary>
    internal static readonly Dictionary<string, string> TimeZoneDictionary = new Dictionary<string, string>()
    {
      {"CEST","+02:00" }, //Central European Summer Time
      {"CET", "+01:00" }, //Central European Time
      {"EST", "-05:00" }, //Eastern Standard Time
      {"GMT", "+00:00" }, //Greenwich Mean Time
      {"BST", "+01:00" }, //British Summer Time
      {"CST", "-06:00" }, //Central Standard Time
      {"PST", "-08:00" }, //Pacific Standard Time
      {"PDT", "-07:00" } //Pacific Daylight Time
    }; 

    /// <summary>
    /// To the long date string in format "yyyy-MM-dd_HH:mm:ss".
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns></returns>
    public static string ToLongDateStringReverse( this DateTime date )
    {
      return date.ToString( "yyyy-MM-dd_HH:mm:ss" );
    }

    public static string DateToGerman(DateTime value)
    {
      DateTimeFormatInfo dateTimeFormat = (new CultureInfo("de-DE", false)).DateTimeFormat;
      if (value.Hour == 0 && value.Minute == 0 && value.Second == 0)
      {
        return value.ToString("d", dateTimeFormat);
      }
      return value.ToString("G", dateTimeFormat);
    }

    public static string DateToSqlServer(DateTime value)
    {
      CultureInfo invariantCulture = CultureInfo.InvariantCulture;
      object[] str = new object[] { value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) };
      return string.Format(invariantCulture, "CONVERT(DATETIME,'{0}',104)", str);
    }

    public static bool IsDate(object expression)
    {
      if (expression != null)
      {
        if (expression is DateTime)
        {
          return true;
        }

        if (expression is string str)
        {
          return DateTime.TryParse(str, out _);
        }
      }
      return false;
    }

    public static DateTime ToDateTime(object expression)
    {
      var typeCode = Type.GetTypeCode(expression.GetType());

      //Only Special Conversions here
      switch (typeCode)
      {
        case TypeCode.DBNull:
          return DateTime.MinValue;

        case TypeCode.String:
          return Convert.ToDateTime(expression.ToString(), CultureInfo.InvariantCulture);
      }

      return Convert.ToDateTime(expression, CultureInfo.InvariantCulture);
    }

    //public static bool IsDate(object expression)
    //{
    //  DateTime dateTime;
    //  if (expression != null)
    //  {
    //    if (expression is DateTime)
    //    {
    //      return true;
    //    }

    //    if (expression is string str)
    //    {
    //      return DateTime.TryParse(str, out dateTime);
    //    }
    //  }
    //  return false;
    //}

    public static DateTime ParseWithCulture(this string expression, CultureInfo cultureInfo)
    {
      return DateTime.Parse(expression, cultureInfo);
    }

    public static DateTime ParseInvariant(this string expression)
    {
      return ParseWithCulture(expression, CultureInfo.InvariantCulture);
    }

    public static DateTime Parse_enUS( this string text )
    {
      return DateTime.Parse( text , new CultureInfo( "en-US" ) );
    }

    public static DateTime Parse_enUK( this string text )
    {
      return DateTime.Parse( text , new CultureInfo( "en-UK" ) );
    }

    public static DateTime Parse_deDE(this string text)
    {
      return DateTime.Parse(text, new CultureInfo("de-DE"));
    }

    public static CalendarWeekDin1355 ToCalendarWeekDin1355( this DateTime date )
    {
      return CalendarWeekDin1355.FromDate( date );
    }

    public static DateTime NormalizeDate( this string text )
    {
      return DateTime.ParseExact( text , "DD:MM:YYYY" , CultureInfo.CurrentUICulture );
    }

    public static int Week( this DateTime date )
    {
      return date.ToCalendarWeekDin1355().Week;
    }

    public static DateTime ParseUniversal( string value )
    {
      return DateTime.ParseExact( value , "yyyy-MM-dd" , null , DateTimeStyles.AssumeUniversal ).ToUniversalTime();
    }

    public static string GetTimestampString( this DateTime value )
    {
      return value.ToString( "yyyyMMddHHmmssfff" );
    }

    /// <summary>
    /// Convert a date time object to Unix time representation.
    /// </summary>
    /// <param name="datetime">The datetime object to convert to Unix time stamp.</param>
    /// <returns>Returns a numerical representation (Unix time) of the DateTime object.</returns>
    public static long ConvertToUnixTime( DateTime datetime )
    {

      return ( long ) ( datetime - UnixDateTimeBase ).TotalSeconds;
    }

    public static long ToUnixTimeStamp(this DateTime dateTime)
    {
      return ConvertToUnixTime(dateTime);
    }

    /// <summary>
    /// Convert Unix time value to a DateTime object.
    /// </summary>
    /// <param name="unixtime">The Unix time stamp you want to convert to DateTime.</param>
    /// <returns>Returns a DateTime object that represents value of the Unix time.</returns>
    public static DateTime UnixTimeToDateTime( long unixtime )
    {
      return UnixDateTimeBase.AddSeconds( unixtime );
    }

    public static DateTime UnixDateTimeToDateTime(this long unixTimeStamp)
    {
      return UnixTimeToDateTime(unixTimeStamp);
    }

    public static int ToQuarter( this DateTime date )
    {
      if ( date.Month <= 3 )
        return 1;

      if ( date.Month <= 6 )
        return 2;

      if ( date.Month <= 9 )
        return 3;

      return 4;
    }



    /// <summary>
    /// Ruft den Namen des Tages in der aktuellen Kultur ab.
    /// </summary>
    /// <param name="dt">Das zu verarbeitende Datum.</param>
    /// <returns>Der Name des Tages in der aktuellen Kultur.</returns>
    public static string GetWeekDayName( this DateTime dt )
    {
      return CultureInfo.CurrentCulture.DateTimeFormat.DayNames[( int ) dt.DayOfWeek];
    }

    /// <summary>
    /// Ruft den Namen des Tages in der angegebenen Kultur ab.
    /// </summary>
    /// <param name="dt">Das zu verarbeitende Datum.</param>
    /// <param name="culture">Die zu verwendende Kultur</param>
    /// <returns>Der Name des Tages in der angegebenen Kultur.</returns>
    public static string GetWeekDayName( this DateTime dt , CultureInfo culture )
    {
      return culture.DateTimeFormat.DayNames[( int ) dt.DayOfWeek];
    }




    public static long ToEpochMilliseconds(this DateTime? time)
    {
      if (time.HasValue)
      {
        return (time.Value.Ticks / 10000L) - EpochMilliseconds;
      }

      return 0L;
    }

    public static CancellationToken ToCancellationToken(this TimeSpan timeout)
    {
      if (timeout == TimeSpan.Zero)
        return new CancellationToken(true);

      if (timeout.Ticks > 0)
        return new CancellationTokenSource(timeout).Token;

      return default(CancellationToken);
    }

    /// <summary>
    /// Parses the date time with time zone. Replaces the TimeZone-Name with the UTC-value
    /// <example>"24-10-2008 21:09:06 CEST" -> "24-10-2008 21:09:06 +02:00"</example>
    /// Attention: will only parse the TimeZoneInfo in <see cref="TimeZoneDictionary"/>
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="format">The format.</param>
    /// <param name="culture">The culture. if null -> CurremtCulture</param>
    /// <returns></returns>
    public static DateTime ParseDateTimeWithTimeZone(string value, string format = "dd-MM-yyyy HH:mm:ss zzz", CultureInfo culture = null)
    {
      if(culture == null) culture = CultureInfo.InvariantCulture;

      var dateTimeString = value;
      if (TimeZoneDictionary.Any(pair => value.Contains(pair.Key)))
      {
        foreach (var pair in TimeZoneDictionary)
        {
          if (dateTimeString.Contains(pair.Key))
          {
            dateTimeString = dateTimeString.Replace(pair.Key, pair.Value);
          }
        }
      }

      return DateTime.ParseExact(dateTimeString, format, culture );
    }

    /// <summary>
    /// Gets the days of a week starting with the cultures first day of week.
    /// </summary>
    /// <param name="cultureInfo">The culture information.</param>
    /// <returns>The days of a week starting with the cultures first day of week.</returns>
    [NotNull]
    public static DayOfWeek[] GetDaysOfWeek([NotNull] this CultureInfo cultureInfo)
    {
      var values = EnumExtensions.GetAllItems<DayOfWeek>();

      var firstDayOfWeek = cultureInfo.DateTimeFormat.FirstDayOfWeek;

      return values.OrderBy(d => d < firstDayOfWeek ? (int)d + 7 : (int)d).ToArray();
    }

    /// <summary>
    /// Returns the smaller of two dates.
    /// </summary>
    /// <param name="value1">The first of two dates.</param>
    /// <param name="value2">The second of two dates.</param>
    /// <returns>Parameter value1 or value2, whichever is smaller.</returns>
    public static DateTime Min(DateTime value1, DateTime value2)
    {
      return value1 < value2 ? value1 : value2;
    }

    /// <summary>
    /// Returns the larger of two dates.
    /// </summary>
    /// <param name="value1">The first of two dates.</param>
    /// <param name="value2">The second of two dates.</param>
    /// <returns>Parameter value1 or value2, whichever is larger.</returns>
    public static DateTime Max(DateTime value1, DateTime value2)
    {
      return value1 > value2 ? value1 : value2;
    }

    /// <summary>
    /// Returns the smaller of two time spans.
    /// </summary>
    /// <param name="value1">The first of two time spans.</param>
    /// <param name="value2">The second of two time spans.</param>
    /// <returns>Parameter value1 or value2, whichever is smaller.</returns>
    public static TimeSpan Min(TimeSpan value1, TimeSpan value2)
    {
      return value1 < value2 ? value1 : value2;
    }

    /// <summary>
    /// Returns the larger of two time spans.
    /// </summary>
    /// <param name="value1">The first of two time spans.</param>
    /// <param name="value2">The second of two time spans.</param>
    /// <returns>Parameter value1 or value2, whichever is larger.</returns>
    public static TimeSpan Max(TimeSpan value1, TimeSpan value2)
    {
      return value1 > value2 ? value1 : value2;
    }

    /// <summary>
    /// Rounds the time span so it does not contain any fractional seconds.
    /// </summary>
    /// <param name="timeSpan">The time span.</param>
    /// <returns>The time span with no fractional seconds.</returns>
    public static TimeSpan RoundToSeconds(this TimeSpan timeSpan)
    {
      return RoundToSeconds(timeSpan, Math.Round);
    }

    /// <summary>
    /// Rounds the time span so it does not contain any fractional seconds.
    /// </summary>
    /// <param name="timeSpan">The time span.</param>
    /// <param name="roundingOperation">The rounding operation that rounds the seconds.</param>
    /// <returns> The time span with no fractional seconds.</returns>
    public static TimeSpan RoundToSeconds(this TimeSpan timeSpan, [NotNull] Func<double, double> roundingOperation)
    {

      return TimeSpan.FromSeconds(roundingOperation(timeSpan.TotalSeconds));
    }

    /// <summary>
    /// Rounds the time span so it does not contain any fractional seconds.
    /// </summary>
    /// <param name="time">The time.</param>
    /// <returns>The time with no fractional seconds.</returns>
    public static DateTime RoundToSeconds(this DateTime time)
    {
      return RoundToSeconds(time, Math.Round);
    }

    /// <summary>
    /// Rounds the time span so it does not contain any fractional seconds.
    /// </summary>
    /// <param name="time">The time.</param>
    /// <param name="roundingOperation">The rounding operation that rounds the seconds.</param>
    /// <returns>The time with no fractional seconds.</returns>
    public static DateTime RoundToSeconds(this DateTime time, [NotNull] Func<double, double> roundingOperation)
    {

      return time.Date + TimeSpan.FromSeconds(roundingOperation(time.TimeOfDay.TotalSeconds));
    }

    /// <summary>
    /// Rounds the time span so it does not contain any fractional minutes.
    /// </summary>
    /// <param name="timeSpan">The time span.</param>
    /// <returns>The time span with no fractional minutes.</returns>
    public static TimeSpan RoundToMinutes(this TimeSpan timeSpan)
    {
      return RoundToMinutes(timeSpan, Math.Round);
    }

    /// <summary>
    /// Rounds the time span so it does not contain any fractional minutes.
    /// </summary>
    /// <param name="timeSpan">The time span.</param>
    /// <param name="roundingOperation">The rounding operation that rounds the minutes.</param>
    /// <returns> The time span with no fractional minutes.</returns>
    public static TimeSpan RoundToMinutes(this TimeSpan timeSpan, [NotNull] Func<double, double> roundingOperation)
    {

      return TimeSpan.FromMinutes(roundingOperation(timeSpan.TotalMinutes));
    }

    /// <summary>
    /// Rounds the time span so it does not contain any fractional minutes.
    /// </summary>
    /// <param name="time">The time.</param>
    /// <returns>The time with no fractional minutes.</returns>
    public static DateTime RoundToMinutes(this DateTime time)
    {
      return RoundToMinutes(time, Math.Round);
    }

    /// <summary>
    /// Rounds the time span so it does not contain any fractional minutes.
    /// </summary>
    /// <param name="time">The time.</param>
    /// <param name="roundingOperation">The rounding operation that rounds the minutes.</param>
    /// <returns>The time with no fractional minutes.</returns>
    public static DateTime RoundToMinutes(this DateTime time, [NotNull] Func<double, double> roundingOperation)
    {

      return time.Date + TimeSpan.FromMinutes(roundingOperation(time.TimeOfDay.TotalMinutes));
    }



    #region StartOfPeriod

    /// <summary>
    ///     First day of the week of the passed in date
    /// </summary>
    /// <remarks>
    ///     Uses <see cref="CultureInfo.CurrentCulture" /> to determine first day of week
    /// </remarks>
    public static DateTime StartOfWeek(this DateTime referenceDate)
    {
      DayOfWeek currentDOW = CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(referenceDate.Date);
      return referenceDate.Date.Subtract(new TimeSpan((int)currentDOW, 0, 0, 0));
    }

    /// <summary>
    ///     First day of the month of the passed in date
    /// </summary>
    public static DateTime StartOfMonth(this DateTime referenceDate)
    {
      return new DateTime(referenceDate.Year, referenceDate.Month, 1);
    }

    /// <summary>
    ///     First day of the year of the passed in date
    /// </summary>
    public static DateTime StartOfYear(this DateTime referenceDate)
    {
      return new DateTime(referenceDate.Year, 1, 1);
    }

    #endregion

    #region EndOfPeriod

    /// <summary>
    ///     Last day of the week of the passed in date
    /// </summary>
    /// <remarks>
    ///     Uses <see cref="CultureInfo.CurrentCulture" /> to determine <i>first</i> day of week
    ///     and then assumes 7 day week
    /// </remarks>
    public static DateTime EndOfWeek(this DateTime referenceDate)
    {
      return StartOfWeek(referenceDate).AddDays(6);
    }

    /// <summary>
    ///     Last day of the month of the passed in date
    /// </summary>
    public static DateTime EndOfMonth(this DateTime referenceDate)
    {
      return StartOfMonth(referenceDate.Date.AddMonths(1)).AddDays(-1);
    }

    /// <summary>
    ///     Last day of the year of the passed in date
    /// </summary>
    public static DateTime EndOfYear(this DateTime referenceDate)
    {
      return StartOfYear(referenceDate.Date.AddYears(1)).AddDays(-1);
    }

    #endregion


    #region CompareToToday

    /// <summary>
    ///     Check if date is after today ignoring time
    /// </summary>
    public static bool IsAfterToday(this DateTime referenceDate)
    {
      return referenceDate.Date > DateTime.Now.Date;
    }

    /// <summary>
    ///     Check if date is before today ignoring time
    /// </summary>
    public static bool IsBeforeToday(this DateTime referenceDate)
    {
      return referenceDate.Date < DateTime.Now.Date;
    }

    /// <summary>
    ///     Check if date is today ignoring time
    /// </summary>
    public static bool IsToday(this DateTime referenceDate)
    {
      return referenceDate.Date == DateTime.Now.Date;
    }

    /// <summary>
    ///     Check if date is after today ignoring time
    /// </summary>
    public static bool IsAfterToday(this DateTime? referenceDate)
    {
      return referenceDate.HasValue && referenceDate.Value.IsAfterToday();
    }

    /// <summary>
    ///     Check if date is before today ignoring time
    /// </summary>
    public static bool IsBeforeToday(this DateTime? referenceDate)
    {
      return referenceDate.HasValue && referenceDate.Value.IsBeforeToday();
    }

    /// <summary>
    ///     Check if date is today ignoring time
    /// </summary>
    public static bool IsToday(this DateTime? referenceDate)
    {
      return referenceDate.HasValue && referenceDate.Value.IsToday();
    }

    #endregion

    #region IsSameDayAs

    /// <summary>
    ///     Check if two dates are the same ignoring time
    /// </summary>
    public static bool IsSameDayAs(this DateTime referenceDate, DateTime? otherDate)
    {
      return otherDate.HasValue && referenceDate.Date == otherDate.Value.Date;
    }

    /// <summary>
    ///     Check if two dates are the same ignoring time
    /// </summary>
    public static bool IsSameDayAs(this DateTime? referenceDate, DateTime? otherDate)
    {
      return referenceDate.HasValue && referenceDate.Value.IsSameDayAs(otherDate);
    }

    #endregion

    #region IsSameWeekAs

    /// <summary>
    ///     Check if two dates are in the same week ignoring time
    /// </summary>
    /// <remarks>
    ///     Uses <see cref="CultureInfo.CurrentCulture" /> to determine week
    /// </remarks>
    public static bool IsSameWeekAs(this DateTime referenceDate, DateTime? otherDate)
    {
      return otherDate.HasValue && StartOfWeek(referenceDate) == StartOfWeek(otherDate.Value);
    }

    /// <summary>
    ///     Check if two dates are in the same week ignoring time
    /// </summary>
    /// <remarks>
    ///     Uses <see cref="CultureInfo.CurrentCulture" /> to determine week
    /// </remarks>
    public static bool IsSameWeekAs(this DateTime? referenceDate, DateTime? otherDate)
    {
      return referenceDate.HasValue && referenceDate.Value.IsSameWeekAs(otherDate);
    }

    #endregion

    #region IsSameMonthAs

    /// <summary>
    ///     Check if two dates are in the same month ignoring time
    /// </summary>
    public static bool IsSameMonthAs(this DateTime referenceDate, DateTime? otherDate)
    {
      return otherDate.HasValue && StartOfMonth(referenceDate) == StartOfMonth(otherDate.Value);
    }

    /// <summary>
    ///     Check if two dates are in the same month ignoring time
    /// </summary>
    public static bool IsSameMonthAs(this DateTime? referenceDate, DateTime? otherDate)
    {
      return referenceDate.HasValue && referenceDate.Value.IsSameMonthAs(otherDate);
    }

    #endregion

    #region IsSameYearAs

    /// <summary>
    ///     Check if two dates are in the same year ignoring time
    /// </summary>
    public static bool IsSameYearAs(this DateTime referenceDate, DateTime? otherDate)
    {
      return otherDate.HasValue && referenceDate.Date.Year == otherDate.Value.Date.Year;
    }

    /// <summary>
    ///     Check if two dates are in the same year ignoring time
    /// </summary>
    public static bool IsSameYearAs(this DateTime? referenceDate, DateTime? otherDate)
    {
      return referenceDate.HasValue && referenceDate.Value.IsSameYearAs(otherDate);
    }

    #endregion

    public static bool IsAtLeastBefore(this DateTime referenceDate, DateTime? otherDate, TimeSpan? beforeTime)
    {
      return otherDate.HasValue && (referenceDate - otherDate) >= beforeTime;
    }

    public static bool IsAtLeastAfter(this DateTime referenceDate, DateTime? otherDate, TimeSpan? afterTime)
    {
      return otherDate.HasValue && (otherDate - referenceDate) >= afterTime;
    }

  }


}