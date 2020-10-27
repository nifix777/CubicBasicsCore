using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using Cubic.Core.Diagnostics;

namespace Cubic.Core.Globalization.Localization
{
  public class ResourceStringLocalizer : IStringLocalizer
  {
    private ResourceManager _rsxManager;

    public ResourceStringLocalizer(ResourceManager resourceManager, CultureInfo culture = null)
    {
      Guard.ArgumentNull(resourceManager, nameof(resourceManager));

      _rsxManager = resourceManager;


      if(culture == null) culture = CultureInfo.CurrentUICulture;

      CurrentCulture = culture;
    }
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
      var rs = _rsxManager.GetResourceSet(CurrentCulture, true, includeParentCultures);

      foreach (DictionaryEntry entry in rs)
      {
        if(entry.Value is string) yield return  new LocalizedString(entry.Key.ToString(), entry.Value.ToString());
      }
    }

    public CultureInfo CurrentCulture { get; private set; }

    public IStringLocalizer WithCulture(CultureInfo culture)
    {
      CurrentCulture = culture;
      return this;
    }

    LocalizedString IStringLocalizer.this[string name, params object[] arguments]
    {
      get { return new LocalizedString( name, string.Format(_rsxManager.GetString(name, CurrentCulture) ?? throw new InvalidOperationException("No RessourceManager"), arguments)); }
    }

    LocalizedString IStringLocalizer.this[string name]
    {
      get { return new LocalizedString(name, _rsxManager.GetString(name, CurrentCulture));}
    }
  }
}