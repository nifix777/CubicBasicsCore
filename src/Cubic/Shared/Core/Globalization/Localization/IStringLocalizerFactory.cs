using System;
using System.Reflection;

namespace Cubic.Core.Globalization.Localization
{
  public interface IStringLocalizerFactory
  {
    IStringLocalizer Create(string baseName, string location);

    IStringLocalizer Create(Type resourceSource);

    IStringLocalizer Craete(Assembly assembly);
  }
}