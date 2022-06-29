using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Cubic.Core.Runtime
{
  public class RuntimeExtensions
  {

    private const string FrameworkName = ".NET";
    private static string s_frameworkDescription;
    public static string FrameworkDescription
    {
      get
      {
        if (s_frameworkDescription == null)
        {
          var versionString = typeof(object).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

          // Strip the git hash if there is one
          int plusIndex = versionString.IndexOf('+');
          if (plusIndex >= 0)
          {
            versionString = versionString.Substring(0, plusIndex);
          }

          s_frameworkDescription = !string.IsNullOrEmpty(versionString.Trim()) ? $"{FrameworkName} {versionString}" : FrameworkName;
        }

        return s_frameworkDescription;
      }
    }
    public static string GetCurrentAppRoot()
    {
      try
      {
        var asm = Assembly.GetEntryAssembly();

        if (asm == null)
        {
          return Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        }
        return asm.Location;

      }
      catch (Exception)
      {
        return Environment.CurrentDirectory;
      }
    }

    public static string GetCurrentAppName()
    {
      var asm = Assembly.GetEntryAssembly();

      if (asm.IsNull())
      {
        return Process.GetCurrentProcess().ProcessName;
      }
      return asm.GetName().Name;
    }

    /// <summary>
    /// Ignores the errors.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns></returns>
    public static bool IgnoreErrors(Action action)
    {
      if (action == null)
      {
        return false;
      }

      try
      {
        action();
        return true;
      }
      catch
      {
        return false;
      }
    }

    /// <summary>
    /// Ignores the errors.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="operation">The operation.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns></returns>
    public static T IgnoreErrors<T>(Func<T> operation, T defaultValue = default(T))
    {
      if (operation == null)
        return defaultValue;

      T result;
      try
      {
        result = operation.Invoke();
      }
      catch
      {
        result = defaultValue;
      }

      return result;
    }

    public static TTarget TryGetTarget<TTarget>(WeakReference<TTarget> weakReference) where TTarget : class
    {
      TTarget target = null;
      if (weakReference.TryGetTarget(out target))
      {
        return target;
      }

      return default(TTarget);
    }

    public static bool Is64BitProcess => Marshal.SizeOf<IntPtr>() == 8;


  }
}