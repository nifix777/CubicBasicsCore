using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Diagnostics
{
  [Serializable]
  public class ExceptionInfo
  {
    public DateTimeOffset When { get; set; }

    public string Message { get; set; }

    public int ErrorCode { get; set; }

    public List<CallStackInfo> CallStack { get; set; } = new List<CallStackInfo>();

    public ExceptionInfo Inner { get; set; }
  }
   

}
