using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Data
{
  public interface INamedParameter
  {
    string Name { get; set; }

    SimpleDataType DataType { get; set; }

    string Value { get; set; }

    Type SystemType { get; set; }
  }
}
