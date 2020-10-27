using Cubic.Core.Components;
using Xunit;

namespace Cubic.Basics.Testing.Reflection
{
  
  public class DpTests
  {
    [Fact]
    public void TestDpRegister()
    {
      var parent = new Parent();
      parent.State = true;

      var doObj = parent as DependencyObject;
      var trueValue = (bool)doObj.GetValue(Parent.StateProperty);
      Assert.True(trueValue);
    }
  }

  public class Parent : DependencyObject
  {
    public bool State
    {
      get { return (bool) this.GetValue(StateProperty); }
      set { this.SetValue(StateProperty, value);}
    }

    public static readonly DependencyProperty StateProperty = DependencyProperty.Register("State", typeof(bool), typeof(Parent));


  }

}
