using System;

namespace Cubic.Core.Diagnostics
{
  public static class EnviromentExtensions
  {
    /// <summary>
    /// The <see cref="OperatingSystem.Version"/> for Windows 8.
    /// </summary>
    private static readonly Version Windows8Version = new Version(6, 2, 9200);

    /// <summary>
    /// Gets a value indicating whether the current operating system is Windows 8 or later.
    /// </summary>
    public static bool IsWindows8OrLater
    {
      get
      {
        return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version >= Windows8Version;
      }
    }

    public static bool IsWindows => Environment.OSVersion.Platform == PlatformID.Win32NT;
  }
}