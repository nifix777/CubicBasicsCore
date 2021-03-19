using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cubic.Core.Expressions;

namespace Cubic.Shared.Core.Data.Expressions
{
  public class SqlNameExpression : Expression
  {
    const int estimatetSize = 4;
    private readonly List<string> segments = new List<string>(estimatetSize);
    public SqlNameExpression(string source)
    {
      var fullname = source ?? throw new ArgumentNullException(nameof(source));
      segments.AddRange(SplitName(fullname));

      while (segments.Count < estimatetSize)
      {
        segments.Add(null);
      }
    }


    public string Property { get { return segments[0]; } set { segments[0] = value; } }

    public string Collection { get { return segments[1]; } set { segments[1] = value; } }

    public string Scheme { get { return segments[2]; } set { segments[2] = value; } }

    public string Synonym { get { return segments[3]; } set { segments[3] = value; } }

    public string FullName => string.Join(".", segments.Reverse<string>().Where(s => !string.IsNullOrEmpty(s)));

    public override string ToString()
    {
      //return HasAlias ? Source : $"({Source}) AS {Alias}";
      return FullName;
    }


    private IEnumerable<string> SplitName(string name)
    {
      return name.Split('.').Reverse();
    }

    public override IExpression Clone()
    {
      return new SqlNameExpression(this.FullName);
    }
  }
}
