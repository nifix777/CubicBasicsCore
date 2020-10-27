using System.Collections.Generic;
using System.Globalization;

namespace Cubic.Core.Globalization.Localization
{
  public interface IStringLocalizer
  {
    IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures);

    CultureInfo CurrentCulture { get; }
    IStringLocalizer WithCulture(CultureInfo culture);

    LocalizedString this[string name, params object[] arguments] { get; }

    LocalizedString this[string name] { get; }
  }
}