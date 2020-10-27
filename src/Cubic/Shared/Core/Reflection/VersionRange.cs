using System;

namespace Cubic.Core.Reflection
{
  public class VersionRange
  {
    public static Version EmptyVersion = new Version(0, 0, 0, 0);

    public Version MinVersion { get; }

    public Version MaxVersion { get; }

    public VersionRange(Version minVersion, Version maxVersion)
    {
      MinVersion = minVersion;
      MaxVersion = maxVersion;
    }

    public bool IsInRange(Version version)
    {
      return version >= MinVersion && (MaxVersion == null || version <= MaxVersion);
    }
  }
}