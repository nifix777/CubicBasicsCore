using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Runtime.Messaging
{

  public class MessageTypeModel
  {
    public MessageTypeModel(Type type)
    {
      Name = type?.Name;
      FullName = type?.FullName;
    }

    public string Name { get; }
    public string FullName { get; }
  }
}
