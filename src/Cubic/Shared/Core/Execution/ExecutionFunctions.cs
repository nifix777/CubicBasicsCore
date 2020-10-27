using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cubic.Core.Execution
{
  public static class ExecutionFunctions
  {
    public static string GetParameterString(IEnumerable<IExecutionParameter> parameters)
    {
      return string.Join(";", parameters);
    }

    public static IEnumerable<IExecutionParameter> GetParameters(string args)
    {
      var parameters = new List<IExecutionParameter>();
      var parameterStrings = args.Split(';');

      foreach (var parameterString in parameterStrings)
      {
        var parameterStringArray = parameterString.Split('=');
        var parameterTypeValue = parameterStringArray[1].Split(':');
        var parameterValue = parameterTypeValue[1];
        var parametertype = parameterTypeValue[2];
        parameters.Add(new ExecutionParameter(parameterStringArray[0], parametertype, parameterValue));
      }

      return parameters;
    }

    public static bool ParameterExists(this IEnumerable<IExecutionParameter> parameters, string name)
    {
      return parameters.Any(p => p.Name == name);
    }

    public static bool VerifyRequierdParameters( this IEnumerable<IExecutionParameter> parameters)
    {
      return parameters.Where( p => p.Required ).Any( para => string.IsNullOrEmpty(para.Value));
    }
  }
}