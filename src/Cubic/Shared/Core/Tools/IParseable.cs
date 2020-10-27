using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Tools
{
  interface IParseable<T>
  {
    T Parse(string value);
    T Parse(string value, IFormatProvider formatProvider);
    T Parse(string value, string format, IFormatProvider formatProvider);

    bool TryParse(string value, out T result);
    bool TryParse(string value, IFormatProvider formatProvider, out T result);
    bool TryParse(string value, string format, IFormatProvider formatProvider, out T result);
  }
}
