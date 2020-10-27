namespace Cubic.Core.Execution
{
  public interface IExecutionParameter
  {
    string Name { get; set; }
    
    string Value { get; set; }

    string Type { get; set; }
    
    bool Required { get; set; } 
  }

  public class ExecutionParameter : IExecutionParameter
  {
    public ExecutionParameter()
    {
      Required = false;
    }

    public ExecutionParameter(string name, string value = null)
    {
      Name = name;
      Value = value;
      Type = typeof (string).Name;
    }

    public ExecutionParameter(string name, string type, string value = null)
    {
      Name = name;
      Value = value;
      Type = type;
    }
    public string Name { get; set; }
    public string Value { get; set; }
    public string Type { get; set; }
    public bool Required { get; set; }

    public override string ToString()
    {
      return string.Format("{0}:{1}", Name, Value);
    }
  }
}