using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cubic.Core.Reflection;
using Cubic.Core.Text;

namespace Cubic.Core.Execution
{
  public abstract class ExecutionTask
  {
    private readonly Dictionary<string, PropertyInfo> _inputProperties;

    private readonly Dictionary<string, PropertyInfo> _outputProperties;

    protected ExecutionTask()
    {
      _inputProperties = new Dictionary<string, PropertyInfo>();
      _outputProperties = new Dictionary<string, PropertyInfo>();

      Name = this.GetType().Name;

      CollectInputProperties();

      CollectOutputProperties();
    }

    public string Name { get; }

    public object GetOutput(string key)
    {
      if (!_outputProperties.ContainsKey(key)) return null;

      return _outputProperties[key].GetValue(this);
    }

    public IEnumerable<string> InputNames => _inputProperties.Keys;

    public IEnumerable<string> OutputNames => _inputProperties.Keys;

    public void SetInput(string key, object value)
    {
      if (!_outputProperties.ContainsKey(key)) return;

      _outputProperties[key].SetValue(this, value);
    }

    protected abstract Task<bool> Execute();

    public async Task<bool> ExecuteTask()
    {
      CollectInputProperties();
      var result = await this.Execute();
      CollectOutputProperties();

      return result;
    }

    private void CollectInputProperties()
    {
      foreach (var propertyInfo in this.GetType().GetPublicProperties().Where( p => p.GetCustomAttribute<InputAttribute>() != null && p.CanRead && p.CanWrite))
      {
        var name = propertyInfo.GetCustomAttribute<InputAttribute>().Name;

        if (name.IsNullOrEmpty())
        {
          name = propertyInfo.Name;
        }

        _inputProperties[name] = propertyInfo;
      }
    }

    private void CollectOutputProperties()
    {
      foreach (var propertyInfo in this.GetType().GetPublicProperties().Where(p => p.GetCustomAttribute<OutputAttribute>() != null && p.CanRead && p.CanWrite))
      {
        var name = propertyInfo.GetCustomAttribute<OutputAttribute>().Name;

        if (name.IsNullOrEmpty())
        {
          name = propertyInfo.Name;
        }

        _inputProperties[name] = propertyInfo;
      }
    }
  }
}