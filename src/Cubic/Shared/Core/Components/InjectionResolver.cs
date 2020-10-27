using System;
using System.Linq;
using System.Reflection;
using Cubic.Core.Diagnostics;

namespace Cubic.Core.Components
{
  public class InjectionResolver : BaseResolver
  {
    private Type _resolvingType;
    public InjectionResolver( IContainer container, Type serviceType , Type resolvingType ) : base(container, serviceType)
    {
      Guard.ArgumentNull( resolvingType, nameof( resolvingType ) );

      _resolvingType = resolvingType;

      if ( _resolvingType.IsInterface )
      {
        throw new InvalidOperationException( string.Format( "Interface Type {0} cant be resolved!" , _resolvingType.Name ) );
      }
    }

    public override object Resolve()
    {
      var ctor = _resolvingType.GetConstructors(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault();

      if (ctor != null)
      {
        var parameters = ctor.GetParameters();
        if (!parameters.Any()) // Default Construktor
        {
          return Activator.CreateInstance(_resolvingType);
        }
        else
        {
          var ctorParameters = parameters.Where(pi => !pi.HasDefaultValue && !pi.IsOptional).ToList();
          object[] callingParameters = new object[ctorParameters.Count];

          for (int i = 0; i < ctorParameters.Count; i++)
          {
            if (ctorParameters[i].ParameterType == typeof(IContainer))
            {
              callingParameters[i] = _container;
              continue;
            }
            else if (ctorParameters[i].ParameterType == typeof(IServiceProvider))
            {
              callingParameters[i] = _container;
              continue;
            }

            callingParameters[i] = _container.Resolve(ctorParameters[i].ParameterType);
          }

          return ctor.Invoke(callingParameters);

        } 
      }

      throw new CompositionException($"No matching Constructor found for Type '{_resolvingType}'");
    }
  }
}