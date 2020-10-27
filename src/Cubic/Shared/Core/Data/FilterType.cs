using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Data
{

  public enum FilterType
  {
    Unavailable = -1,
    Disabled = 0,
    Equal = 1,
    Unequal = 2,
    Greater = 3,
    GreaterOrEqual = 4,
    Less = 5,
    LessOrEqual = 6,
    BeginsWith = 7,
    Between = 8,
    Calculation = 9,
    Multiselect = 10,
    Contains = 11,
    IsEmpty = 12,
    IsNotEmpty = 13,
    ContainsNot = 14,
    EndsWith = 15,
    Like = 16,
    NotLike = 17
  }
}
