using System;

namespace Cubic.Core.Runtime
{
  public class SystemInformation
  {
    private static DateTime _appStarTime = DateTime.UtcNow;


    public SystemInformation(string applicationName, Version applicationVersion)
    {
      ApplicationName = applicationName;
      ApplicationVersion = applicationVersion;
    }

    public string ApplicationName { get; }

    public Version ApplicationVersion { get; }

    public string OperatingSystem => Environment.OSVersion.VersionString;

    public Version OperatingSystemVersion => Environment.OSVersion.Version;

    public DateTime LaunchTime => _appStarTime;

    public TimeSpan AppUptime => DateTime.UtcNow.Subtract(_appStarTime);
  }
}