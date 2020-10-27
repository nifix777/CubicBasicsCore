using System;

namespace Cubic.Core.Expressions
{
  public abstract class MemberExpression : NamedExpression
  {
    public string BaseName { get; }

    public MemberExpression(string baseName, string memberName)
    {
      this.BaseName = baseName ?? throw new ArgumentNullException(nameof(baseName));
      this.Name = memberName ?? throw new ArgumentNullException(nameof(memberName)); ;
    }
  }

  public class PropertyExpression : MemberExpression
  {
    public PropertyExpression(string baseName, string propertyName, Type proertyType) : base(baseName, propertyName)
    {
      PropertyType = proertyType ?? throw new ArgumentNullException(nameof(proertyType));
    }

    public Type PropertyType { get; set; }
    public override IExpression Clone()
    {
      return new PropertyExpression(BaseName, Name, PropertyType);
    }
  }
}
