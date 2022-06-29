using System;

namespace Cubic.Core.Runtime
{
  public class SystemInformation
  {
    private static DateTimeOffset _appStarTime = DateTimeOffset.UtcNow;


    public SystemInformation(string applicationName, Version applicationVersion)
    {
      ApplicationName = applicationName;
      ApplicationVersion = applicationVersion;
    }

    public string ApplicationName { get; }

    public Version ApplicationVersion { get; }

    public string OperatingSystem => Environment.OSVersion.VersionString;

    public Version OperatingSystemVersion => Environment.OSVersion.Version;

    public DateTimeOffset LaunchTime => _appStarTime;

    public TimeSpan AppUptime => DateTimeOffset.UtcNow.Subtract(_appStarTime);

  }
}