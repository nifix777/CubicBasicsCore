using System.Text;

namespace Cubic.Core
{
  public class StatusToken : ProgressToken
  {
    private readonly StringBuilder _msgbuffer = new StringBuilder(300);

    public StatusToken()
    {
      
    }

    public StatusToken(string name) : base(name)
    {
      
    }

    public string Message
    {
      get
      {
        lock (_msgbuffer)
        {
          return _msgbuffer.ToString();
        }
      }
    }

    public void Report(string message)
    {
      lock (_msgbuffer)
      {
        _msgbuffer.Append(message);
      }
      base.NotifyPropertyChanged(nameof(Message));
      FireOnChange();
    }

    public void ReportLine( string message )
    {
      lock (_msgbuffer)
      {
        _msgbuffer.AppendLine(message);
      }
      base.NotifyPropertyChanged(nameof(Message));
      FireOnChange();
    }

    public override void Clear()
    {
      lock (_msgbuffer)
      {
        _msgbuffer.Clear();
      }
      base.NotifyPropertyChanged(nameof(Message));
      FireOnChange();
      base.Clear();
    }
  }
}