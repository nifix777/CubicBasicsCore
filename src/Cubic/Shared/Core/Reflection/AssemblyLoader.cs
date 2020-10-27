using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Cubic.Core.Reflection
{
  public static class AssemblyLoader
  {
    public static Assembly LoadAssembly(string assemblyIdentifier, string assemblyPath, bool checkVersion, Version expecktedVersion)
    {
      Assembly assembly = AssemblyLoader.LoadAssembly(assemblyIdentifier, assemblyPath);
      if (checkVersion && assembly.GetName().Version.Major != expecktedVersion.Major && assembly.GetName().Version.Minor != expecktedVersion.Minor)
      {
        throw new AssemblyVersionException($" Assembly {assemblyIdentifier} has the wrong Version Number.");
      }
      return assembly;
    }

    private static Assembly LoadAssembly(string assemblyIdentifier, string assemblyDirectory)
    {
      if (assemblyIdentifier == null)
      {
        throw new ArgumentNullException(nameof(assemblyIdentifier));
      }
      assemblyIdentifier = assemblyIdentifier.Trim(new char[] { ' ' });
      if (assemblyIdentifier.IndexOf(":", StringComparison.Ordinal) != -1 || assemblyIdentifier.IndexOf("//", StringComparison.Ordinal) != -1 || assemblyIdentifier.IndexOf("\\", StringComparison.Ordinal) != -1)
      {
        return Assembly.LoadFrom(assemblyIdentifier);
      }
      try
      {
        return Assembly.Load(assemblyIdentifier);
      }
      catch (FileNotFoundException)
      {
      }
      catch (FileLoadException)
      {
      }
      catch (BadImageFormatException)
      {
      }

      return Assembly.LoadFrom(Path.Combine(assemblyDirectory, string.Concat(assemblyIdentifier, ".dll")));
    }
  }
}