using System;
using System.Globalization;
using System.Threading;
using Cubic.Core.Execution.Cron;
using Xunit;

namespace Cubic.Basics.Testing.Execution
{
  
  public class CronTests
  {
    [Fact]
    public void Cron_EveryMinute_Test()
    {
      var cronExpression = "* * * * *"; // run every minute
      var datetimeDue = new DateTime(2018, 10, 17, 12, 1, 0);

      var cron = new CronExpression(cronExpression);

      Assert.True(cron.IsDue(datetimeDue));
    }

    [Fact]
    public void Cron_DailyTime_Test()
    {
      var cronExpression = "* 13 * * *"; // run at 1:00 pm daily
      var datetimeDue = new DateTime(2018, 10, 17, 13, 0, 0);
      var datetimeNotDue = new DateTime(2018, 10, 17, 0, 1, 0);

      var cron = new CronExpression(cronExpression);

      Assert.True(cron.IsDue(datetimeDue));
      Assert.False(cron.IsDue(datetimeNotDue));
    }

    [Fact]
    public void Cron_FirstDayMonth_Test()
    {
      var culture = CultureInfo.GetCultureInfo("de-DE");

      Thread.CurrentThread.CurrentCulture = culture;

      var cronExpression = "00 00 1 * *"; // run First day of the month at midnight.
      var datetimeDue = new DateTime(2018, 10, 1, 0, 0, 0);
      var datetimeNotDue = new DateTime(2018, 10, 17, 0, 1, 0);

      var cron = new CronExpression(cronExpression);

      Assert.True(cron.IsDue(datetimeDue));
      Assert.False(cron.IsDue(datetimeNotDue));
    }




    [Fact]
    public void Cron_PastEveryMinute_Test()
    {
      var cronExpression = "* * * * *"; // run every minute
      var datetimeDue = new DateTime(2018, 10, 17, 12, 4, 15);

      var cron = new CronExpression(cronExpression);

      Assert.True(cron.IsDueOrPast(datetimeDue));
    }

    [Fact]
    public void Cron_PastDailyTime_Test()
    {
      var cronExpression = "* 13 * * *"; // run at 1:00 pm daily
      var datetimeDue = new DateTime(2018, 10, 17, 14, 0, 0);
      var datetimeNotDue = new DateTime(2018, 10, 17, 0, 1, 0);

      var cron = new CronExpression(cronExpression);

      Assert.True(cron.IsDueOrPast(datetimeDue));
      Assert.False(cron.IsDueOrPast(datetimeNotDue));
    }

    [Fact]
    public void Cron_PastFirstDayMonth_Test()
    {
      var culture = CultureInfo.GetCultureInfo("de-DE");

      Thread.CurrentThread.CurrentCulture = culture;

      var cronExpression = "00 00 1-2 * *"; // run First day of the month (Jan-Feb) at midnight.
      var datetimeDue = new DateTime(2018, 10, 1, 0, 0, 0);
      var datetimeNotDue = new DateTime(2018, 10, 3, 0, 1, 0);

      var cron = new CronExpression(cronExpression);

      Assert.True(cron.IsDueOrPast(datetimeDue));
      Assert.False(cron.IsDueOrPast(datetimeNotDue));
    }
  }
}
