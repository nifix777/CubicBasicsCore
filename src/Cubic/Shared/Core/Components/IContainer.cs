using System;
using System.Collections.Generic;

namespace Cubic.Core.Components
{
  public interface IContainer : IServiceProvider, IDisposable
  {
    void Use(IResolver typeResolver);
    void Replace(IResolver typeResolver);
    void Register(string key, IResolver keyResolver);

    T Resolve<T>() where T : class;

    IEnumerable<T> ResolveMany<T>() where T : class; 
    object Resolve(Type interfacType);

    object Resolve( string key );
    T Resolve<T>( string key ) where T : class;
  }
}
