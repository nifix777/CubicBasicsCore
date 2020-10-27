using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Cubic.Core.Globalization
{
  public static class Tools
  {
    /// <summary>
    /// Converts the name of the three letter name (ISO 3166-1 alpha-3) to two letter (ISO 3166-1 alpha-2).
    /// </summary>
    /// <param name="name">The name.</param>
    /// <remarks>Example: deu -> de</remarks>
    /// <returns></returns>
    /// <exception cref="ArgumentException">name must be three letters.</exception>
    /// <exception cref="Exception"></exception>
    public static string ConvertThreeLetterNameToTwoLetterName(string name)
    {
      if (name.Length != 3)
      {
        throw new ArgumentException("name must be three letters.");
      }

      name = name.ToUpper();

      CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
      foreach (CultureInfo culture in cultures)
      {
        RegionInfo region = new RegionInfo(culture.LCID);
        if (region.ThreeLetterISORegionName.ToUpper() == name)
        {
          return region.TwoLetterISORegionName;
        }
      }

      throw new Exception($"No matching Culture Found for Value '{name}'");
    }

    /// <summary>
    /// Builds the region information dictionary for (ISO 3166-1 alpha-3) -> <see cref="RegionInfo"/>.
    /// </summary>
    /// <returns></returns>
    public static IDictionary<string, RegionInfo> BuildRegionInfoDictionary()
    {
      return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
        .Select(ci => new RegionInfo(ci.LCID))
        .GroupBy(ri => ri.ThreeLetterISORegionName)
        .ToDictionary(g => g.Key, g => g.First());
    }

    /// <summary>
    /// Finds the candidate cultures.
    /// </summary>
    /// <param name="region">The region.</param>
    /// <returns></returns>
    public static IEnumerable<CultureInfo> FindCandidateCultures(RegionInfo region)
    {
      return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
        .Where(x => (new RegionInfo(x.Name)).GeoId == region.GeoId);
    }

    /// <summary>
    /// Finds the candidate cultures.
    /// </summary>
    /// <param name="countryCode">The country code.</param>
    /// <returns></returns>
    public static IEnumerable<CultureInfo> FindCandidateCultures(string countryCode)
    {
      return CultureInfo.GetCultures(CultureTypes.AllCultures)
        .Where(c => c.Name.EndsWith($"-{countryCode.ToUpperInvariant()}"));
    }
  }
}