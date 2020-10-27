using System;

namespace Cubic.Core.Reflection
{
  public class AssemblyVersionException : Exception
  {
    public AssemblyVersionException(string assemblyLoadErrorInfoFormat) : base(assemblyLoadErrorInfoFormat)
    {
      
    }
  }
}