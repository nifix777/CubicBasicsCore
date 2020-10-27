using System;
using System.Globalization;

namespace Cubic.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class CalendarWeekDin1355
    {
        private int _yearData;

        private int _weekData;

        public readonly static CalendarWeekDin1355 MinValue;

        public DateTime FirstDayDate
        {
            get
            {
                return this.GetDayOfWeek(DayOfWeek.Monday);
            }
        }

        public DateTime LastDayDate
        {
            get
            {
                return this.GetDayOfWeek(DayOfWeek.Sunday);
            }
        }

        public short ToInt16
        {
            get
            {
                return CalendarWeekDin1355.ConvertFromInt32(this.ToInt32);
            }
        }

        public int ToInt32
        {
            get
            {
                return this._yearData * 100 + this._weekData;
            }
        }

        public int Week
        {
            get
            {
                return this._weekData;
            }
        }

        public int Year
        {
            get
            {
                return this._yearData;
            }
        }

        static CalendarWeekDin1355()
        {
            CalendarWeekDin1355.MinValue = new CalendarWeekDin1355(0, 1);
        }

        public CalendarWeekDin1355(int yearAndWeek)
        {
            short num = (short)(yearAndWeek / 100);
            this.InternalConstruct(num, (short)(yearAndWeek % 100));
        }

        public CalendarWeekDin1355(DateTime date)
        {
            this.SetYearAndWeek(date);
        }

        public CalendarWeekDin1355(short year, short week)
        {
            this.InternalConstruct(year, week);
        }

        public static CalendarWeekDin1355 Add(CalendarWeekDin1355 calendarWeek, int weeks)
        {
            return calendarWeek + weeks;
        }

        public static int Compare(CalendarWeekDin1355 calendarWeek1, CalendarWeekDin1355 calendarWeek2)
        {
            if (object.Equals(calendarWeek1, null) && object.Equals(calendarWeek2, null))
            {
                return 0;
            }
            if (object.Equals(calendarWeek1, null) && !object.Equals(calendarWeek2, null))
            {
                return -1;
            }
            if (object.Equals(calendarWeek2, null))
            {
                return 1;
            }
            int toInt32 = calendarWeek1.ToInt32;
            int num = calendarWeek2.ToInt32;
            if (toInt32 == num)
            {
                return 0;
            }
            if (toInt32 > num)
            {
                return 1;
            }
            return -1;
        }

        public static short ConvertFromInt32(int yearAndWeek)
        {
            return (short)(yearAndWeek % 10000 / 100 * 100 + yearAndWeek % 100);
        }

        public static int DayOfWeekToWeekdayOrdinal(DayOfWeek dayOfWeek, DayOfWeek firstDayOfWeek)
        {
            return ((int)dayOfWeek - (int)firstDayOfWeek + (int)(DayOfWeek.Monday | DayOfWeek.Tuesday | DayOfWeek.Wednesday | DayOfWeek.Thursday | DayOfWeek.Friday | DayOfWeek.Saturday)) % (int)(DayOfWeek.Monday | DayOfWeek.Tuesday | DayOfWeek.Wednesday | DayOfWeek.Thursday | DayOfWeek.Friday | DayOfWeek.Saturday) + (int)DayOfWeek.Monday;
        }

        public bool Equals(CalendarWeekDin1355 calendarWeek)
        {
            if (calendarWeek == null)
            {
                return false;
            }
            if (this.Week != calendarWeek.Week)
            {
                return false;
            }
            return this.Year == calendarWeek.Year;
        }

        public override bool Equals(object obj)
        {
            if (object.Equals(obj, null))
            {
                return false;
            }
            CalendarWeekDin1355 calendarWeekDin1355 = obj as CalendarWeekDin1355;
            if (object.Equals(calendarWeekDin1355, null))
            {
                return Object.ReferenceEquals(this,obj);
            }
            return this.Equals(calendarWeekDin1355);
        }

        public static CalendarWeekDin1355 FromDate(DateTime date)
        {
            return new CalendarWeekDin1355(date);
        }

        public static CalendarWeekDin1355 FromInt16(short yearAndWeek)
        {
            return new CalendarWeekDin1355(yearAndWeek);
        }

        public static CalendarWeekDin1355 FromInt32(int yearAndWeek)
        {
            return new CalendarWeekDin1355(yearAndWeek);
        }

        public DateTime GetDayOfWeek(DayOfWeek dayOfWeek)
        {
            DateTime dateTime = new DateTime(this._yearData, 1, 1);
            dateTime = dateTime.AddDays((double)((8 - (int)dateTime.DayOfWeek) % (int)(DayOfWeek.Monday | DayOfWeek.Tuesday | DayOfWeek.Wednesday | DayOfWeek.Thursday | DayOfWeek.Friday | DayOfWeek.Saturday)));
            int num = ((new CalendarWeekDin1355(dateTime)).Week == 2 ? -2 : -1);
            int weekdayOrdinal = CalendarWeekDin1355.DayOfWeekToWeekdayOrdinal(dayOfWeek, DayOfWeek.Monday) - 1;
            return dateTime.AddDays((double)((this._weekData + num) * 7 + weekdayOrdinal));
        }

        public override int GetHashCode()
        {
            return this._yearData * 2 + this._weekData * 3;
        }

        private void InternalConstruct(short year, short week)
        {
            if (year == 0 && week == 0)
            {
                this._yearData = CalendarWeekDin1355.MinValue.Year;
                this._weekData = CalendarWeekDin1355.MinValue.Week;
                return;
            }
            year = (short)CultureInfo.InvariantCulture.Calendar.ToFourDigitYear(year);
            if (week > 0 && week < 53)
            {
                this._yearData = year;
                this._weekData = week;
                return;
            }
            CalendarWeekDin1355 calendarWeekDIN1355 = new CalendarWeekDin1355(year, 1);
            calendarWeekDIN1355 = calendarWeekDIN1355 + (week - 1);
            this._yearData = calendarWeekDIN1355.Year;
            this._weekData = calendarWeekDIN1355.Week;
        }

        public static CalendarWeekDin1355 operator +(CalendarWeekDin1355 calendarWeek, int weeks)
        {
            if (calendarWeek == null)
            {
                throw new ArgumentNullException(nameof(calendarWeek));
            }
            double num = (double)weeks * 7;
            return new CalendarWeekDin1355(calendarWeek.FirstDayDate.AddDays(num));
        }

        public static bool operator ==(CalendarWeekDin1355 calendarWeek1, CalendarWeekDin1355 calendarWeek2)
        {
            return CalendarWeekDin1355.Compare(calendarWeek1, calendarWeek2) == 0;
        }

        public static bool operator >(CalendarWeekDin1355 calendarWeek1, CalendarWeekDin1355 calendarWeek2)
        {
            return CalendarWeekDin1355.Compare(calendarWeek1, calendarWeek2) > 0;
        }

        public static bool operator >=(CalendarWeekDin1355 calendarWeek1, CalendarWeekDin1355 calendarWeek2)
        {
            return CalendarWeekDin1355.Compare(calendarWeek1, calendarWeek2) >= 0;
        }

        public static bool operator !=(CalendarWeekDin1355 calendarWeek1, CalendarWeekDin1355 calendarWeek2)
        {
            return CalendarWeekDin1355.Compare(calendarWeek1, calendarWeek2) != 0;
        }

        public static bool operator <(CalendarWeekDin1355 calendarWeek1, CalendarWeekDin1355 calendarWeek2)
        {
            return CalendarWeekDin1355.Compare(calendarWeek1, calendarWeek2) < 0;
        }

        public static bool operator <=(CalendarWeekDin1355 calendarWeek1, CalendarWeekDin1355 calendarWeek2)
        {
            return CalendarWeekDin1355.Compare(calendarWeek1, calendarWeek2) <= 0;
        }

        public static CalendarWeekDin1355 operator -(CalendarWeekDin1355 calendarWeek, int weeks)
        {
            if (calendarWeek == null)
            {
                throw new ArgumentNullException(nameof(calendarWeek));
            }
            double num = (double)weeks * -7;
            return new CalendarWeekDin1355(calendarWeek.FirstDayDate.AddDays(num));
        }

        private void SetYearAndWeek(DateTime date)
        {
            int num = 52;
            int num1 = 52;
            int year = date.Year;
            DateTime dateTime = new DateTime(year, 1, 1);
            if ((new DateTime(year - 1, 12, 31)).DayOfWeek == DayOfWeek.Thursday)
            {
                num1 = 53;
            }
            if ((new DateTime(year - 1, 1, 1)).DayOfWeek == DayOfWeek.Thursday)
            {
                num1 = 53;
            }
            if ((new DateTime(year, 12, 31)).DayOfWeek == DayOfWeek.Thursday)
            {
                num = 53;
            }
            if ((new DateTime(year, 1, 1)).DayOfWeek == DayOfWeek.Thursday)
            {
                num = 53;
            }
            int num2 = CalendarWeekDin1355.WeekdayOrdinal(dateTime, DayOfWeek.Monday);
            int num3 = (num2 < 5 ? 1 : 0);
            if (num2 != 1)
            {
                dateTime = dateTime.AddDays((double)(-1 * (num2 - 1)));
            }
            TimeSpan timeSpan = dateTime.Subtract(date);
            int num4 = Math.Abs(timeSpan.Days) / 7;
            if (num4 != 0)
            {
                num3 = num3 + num4;
                if (num3 > num)
                {
                    num3 = 1;
                    year++;
                }
            }
            else if (num3 == 0)
            {
                num3 = num1;
                year--;
            }
            this._yearData = year;
            this._weekData = num3;
        }

        public static CalendarWeekDin1355 Subtract(CalendarWeekDin1355 calendarWeek, int weeks)
        {
            return calendarWeek - weeks;
        }

        public override string ToString()
        {
            return this.ToInt32.ToString(CultureInfo.InvariantCulture);
        }

        public static int WeekdayOrdinal(DateTime dateValue, DayOfWeek firstDayOfWeek)
        {
            return CalendarWeekDin1355.DayOfWeekToWeekdayOrdinal(CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(dateValue), firstDayOfWeek);
        }
    }
}