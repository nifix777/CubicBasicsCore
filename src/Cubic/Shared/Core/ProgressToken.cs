using System;
using System.Text;
using System.Threading;

namespace Cubic.Core
{
  public class ProgressToken : BindableBase
  {
    private int _progress;

    private bool _isComplete;
    public ProgressToken()
    {
      
    }

    public ProgressToken(string name)
    {
      Name = name;
    }

    public event EventHandler<ProgressToken> OnChange;

    public int Progress
    {
      get
      {
        return _progress;
      }
    }

    public string Name { get; }

    public bool IsComplete => _isComplete;

    public void Report(int value)
    {
      Interlocked.Add( ref _progress , value );
      base.NotifyPropertyChanged(nameof(Progress));
      FireOnChange();
    }

    public virtual void Complete()
    {
      _isComplete = true;
      base.NotifyPropertyChanged(nameof(IsComplete));
      FireOnChange();
    }

    protected void FireOnChange()
    {
      OnChange?.Invoke(this, this);
    }

    public void Inkrement()
    {
      Interlocked.Add( ref _progress , 1 );
      base.NotifyPropertyChanged(nameof(Progress));
      FireOnChange();
    }

    public virtual void Clear()
    {
      Interlocked.Exchange(ref _progress, 0);
      base.NotifyPropertyChanged(nameof(Progress));
      FireOnChange();
    }
  }
}