using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Expressions
{
  public abstract class NamedExpression : Expression
  {
    public string Name { get; set; }
  }
}
