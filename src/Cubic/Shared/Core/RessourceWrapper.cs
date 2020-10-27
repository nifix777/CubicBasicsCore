using System;
using System.Reflection;
using System.Resources;
using Cubic.Core.Diagnostics;

namespace Cubic.Core
{
  public class RessourceWrapper : IDisposable
  {
    private ResourceManager _manager;

    public RessourceWrapper() : this(Assembly.GetCallingAssembly().GetName().Name, Assembly.GetCallingAssembly())
    {
      
    }

    public RessourceWrapper( Assembly assembly ) : this(assembly.GetName().Name, assembly)
    {

    }

    public RessourceWrapper(string baseName, Assembly assembly)
    {
      Guard.ArgumentNull( assembly, nameof( assembly ) );
      Guard.AgainstNullOrEmpty( baseName, nameof( baseName ) );

      _manager = new ResourceManager(baseName, assembly);
    }

    public RessourceWrapper(ResourceManager resourceManager)
    {
      Guard.ArgumentNull(resourceManager, nameof(resourceManager));

      _manager = resourceManager;
    }

    public ResourceManager ResourceManager { get { return _manager; } }


    public void Dispose()
    {
      if ( _manager != null )
      {
        _manager.ReleaseAllResources();
        _manager = null; 
      }
    }
  }
}