using System;
using System.Timers;

namespace Cubic.Core
{
  public class ExpirationPollTimer
  {
    private readonly Action _callbackMethod;

    private readonly Timer _pollTimer;

    private readonly object _timerLock;

    public bool Enabled
    {
      get
      {
        return this._pollTimer.Enabled;
      }
    }

    public TimeSpan PollInterval
    {
      get;
      private set;
    }

    public ExpirationPollTimer( TimeSpan pollInterval , Action callbackMethod )
    {
      this.PollInterval = pollInterval;
      this._callbackMethod = callbackMethod;
      this._pollTimer = new Timer();
      this._pollTimer.Elapsed += new ElapsedEventHandler( this.PollTimerElapsed );
      this._timerLock = new object();
    }

    private void PollTimerElapsed( object sender , ElapsedEventArgs args )
    {
      lock ( this._timerLock )
      {
        this.StopPolling();
        this._callbackMethod();
        this.StartPolling();
      }
    }

    public void ResetPolling( TimeSpan pollTime )
    {
      lock ( this._timerLock )
      {
        this.StopPolling();
        this.PollInterval = pollTime;
        this.StartPolling();
      }
    }

    public void StartPolling()
    {
      lock ( this._timerLock )
      {
        this._pollTimer.Interval = Convert.ToDouble( this.PollInterval.TotalMilliseconds );
        this._pollTimer.Start();
      }
    }

    public void StopPolling()
    {
      lock ( this._timerLock )
      {
        this._pollTimer.Stop();
      }
    }
  }
}