using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Cubic.Core.Globalization.Localization;
using Xunit;

namespace Cubic.Basics.Testing.Parsing
{
  
  public class LocalizationTest
  {
    private static CultureInfo _defaultStartCulture = CultureInfo.GetCultureInfo("en-US");

    [Fact]
    public void Test_Default_Localization()
    {
      var ressources = new List<Resource>();
      ressources.Add(new Resource("en-US", "Hello", "Hello"));
      ressources.Add(new Resource("de-DE", "Hello", "Hallo"));

      IStringLocalizer localizer = new InMemoryStringLocalizer(ressources);
      Thread.CurrentThread.CurrentUICulture = _defaultStartCulture;

      var key = "Hello";
      LocalizedString result = null;

      result = localizer[key];

      Assert.False(result.ResourceNotFound);
      Assert.Equal(key, result.Name);
      Assert.Equal("Hello", result.Value);

    }

    [Fact]
    public void Test_US_Localization()
    {
      var ressources = new List<Resource>();
      ressources.Add(new Resource("en-US", "Hello", "Hello"));
      ressources.Add(new Resource("de-DE", "Hello", "Hallo"));

      IStringLocalizer localizer = new InMemoryStringLocalizer(ressources, CultureInfo.GetCultureInfo("en-US"));
      Thread.CurrentThread.CurrentUICulture = _defaultStartCulture;

      var key = "Hello";
      LocalizedString result = null;

      result = localizer[key];

      Assert.False(result.ResourceNotFound);
      Assert.Equal(key, result.Name);
      Assert.Equal("Hello", result.Value);

    }

    [Fact]
    public void Test_German_Localization()
    {
      var ressources = new List<Resource>();
      ressources.Add(new Resource("en-US", "Hello", "Hello"));
      ressources.Add(new Resource("de-DE", "Hello", "Hallo"));

      IStringLocalizer localizer = new InMemoryStringLocalizer(ressources, CultureInfo.GetCultureInfo("de-DE"));
      Thread.CurrentThread.CurrentUICulture = _defaultStartCulture;

      var key = "Hello";
      LocalizedString result = null;

      result = localizer[key];

      Assert.False(result.ResourceNotFound);
      Assert.Equal(key, result.Name);
      Assert.Equal("Hallo", result.Value);

    }

    [Fact]
    public void Test_German_With_UI_Culture()
    {
      var ressources = new List<Resource>();
      ressources.Add(new Resource("en-US", "Hello", "Hello"));
      ressources.Add(new Resource("de-DE", "Hello", "Hallo"));

      IStringLocalizer localizer = new InMemoryStringLocalizer(ressources);
      Thread.CurrentThread.CurrentUICulture = _defaultStartCulture;

      Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("de-DE");
      var key = "Hello";
      LocalizedString result = null;

      result = localizer[key];

      Assert.False(result.ResourceNotFound);
      Assert.Equal(key, result.Name);
      Assert.Equal("Hallo", result.Value);

    }

    [Fact]
    public void Test_Culture_Change()
    {
      var ressources = new List<Resource>();
      ressources.Add(new Resource("en-US", "Hello", "Hello"));
      ressources.Add(new Resource("de-DE", "Hello", "Hallo"));

      IStringLocalizer localizer = new InMemoryStringLocalizer(ressources);
      Thread.CurrentThread.CurrentUICulture = _defaultStartCulture;

      var key = "Hello";
      LocalizedString result = null;

      result = localizer[key];

      Assert.False(result.ResourceNotFound);
      Assert.Equal(key, result.Name);
      Assert.Equal("Hello", result.Value);

      localizer = localizer.WithCulture(CultureInfo.GetCultureInfo("de-DE"));

      result = localizer[key];

      Assert.False(result.ResourceNotFound);
      Assert.Equal(key, result.Name);
      Assert.Equal("Hallo", result.Value);

    }
  }
}
