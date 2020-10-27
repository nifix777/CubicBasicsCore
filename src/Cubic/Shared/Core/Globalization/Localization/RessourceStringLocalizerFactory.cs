using System;
using System.Reflection;
using System.Resources;

namespace Cubic.Core.Globalization.Localization
{
  public class RessourceStringLocalizerFactory : IStringLocalizerFactory
  {
    public IStringLocalizer Create(string baseName, string location)
    {
      var asmName = new AssemblyName(location);
      return new ResourceStringLocalizer(new ResourceManager(baseName, Assembly.Load(asmName)));
    }

    public IStringLocalizer Create(Type resourceSource)
    {
      return new ResourceStringLocalizer(new ResourceManager(resourceSource));
    }

    public IStringLocalizer Craete(Assembly assembly)
    {
      return new ResourceStringLocalizer(new ResourceManager("Ressources", assembly));
    }
  }
}