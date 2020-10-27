namespace Cubic.Core.Execution.Transient
{
  public class TestEnviroment : TransientContext
  {
    public TransientDirectory CreateDirectory(string name)
    {
      var dir = new TransientDirectory(name);
      base.Register(dir);
      return dir;
    }
  }
}