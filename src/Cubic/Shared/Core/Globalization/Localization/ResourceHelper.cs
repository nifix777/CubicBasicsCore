using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;

namespace Cubic.Core.Globalization.Localization
{
  public static class ResourceHelper
  {
    private const string seperator = "-";

    private static Hashtable resourceStringList;

    private static string _language;

    static ResourceHelper()
    {
      ResourceHelper.resourceStringList = new Hashtable();
      ResourceHelper._language = Thread.CurrentThread.CurrentUICulture.Name.Substring(0, (Thread.CurrentThread.CurrentUICulture.Name.IndexOf("-") > 0 ? Thread.CurrentThread.CurrentUICulture.Name.IndexOf("-") : Thread.CurrentThread.CurrentUICulture.Name.Length));
    }

    public static Task<string> GetFileContentAsync(Assembly assembly, string name)
    {
      var ressourceName = string.Format("{0}.{1}", assembly.GetName().Name, name);

      using (var manifest = assembly.GetManifestResourceStream(ressourceName))
      {
        using (var reader = new StreamReader(manifest))
        {
          return reader.ReadToEndAsync();
        }
      }
    }

    public static string FindResourceString(Assembly assembly, string resourceName)
    {
      string empty = string.Empty;
      empty = (ResourceHelper._language != "de" ? string.Concat(assembly.GetName().Name, ".Properties.Resource-", ResourceHelper._language) : string.Concat(assembly.GetName().Name, ".Properties.Resources"));
      string resourceString = ResourceHelper.GetResourceString(assembly, empty, resourceName);
      if (!string.IsNullOrEmpty(resourceString))
      {
        return resourceString;
      }
      return ResourceHelper.FindResourceStringByResourceName(assembly, resourceName);
    }

    public static string FindResourceString(Type type, string resourceName)
    {
      string empty = string.Empty;
      empty = (ResourceHelper._language != "de" ? string.Concat(type.Namespace, ".Properties.Resource-", ResourceHelper._language) : string.Concat(type.Namespace, ".Properties.Resources"));
      string resourceString = ResourceHelper.GetResourceString(type.Assembly, empty, resourceName);
      if (!string.IsNullOrEmpty(resourceString))
      {
        return resourceString;
      }
      return ResourceHelper.FindResourceStringByResourceName(type.Assembly, resourceName);
    }

    public static string FindResourceStringByResourceName(Assembly assembly, string resourceName)
    {
      string[] manifestResourceNames = assembly.GetManifestResourceNames();
      string empty = string.Empty;
      string[] strArrays = manifestResourceNames;
      for (int i = 0; i < (int)strArrays.Length; i++)
      {
        string str = strArrays[i];
        if (str.IndexOf(string.Concat("-", ResourceHelper._language)) > 0)
        {
          string use = ResourceHelper.ResourceToUse(str);
          string resourceString = ResourceHelper.GetResourceString(assembly, use, resourceName);
          if (!string.IsNullOrEmpty(resourceString))
          {
            return resourceString;
          }
        }
        if (str.IndexOf("Resources") > 0)
        {
          empty = ResourceHelper.ResourceToUse(str);
        }
      }
      if (string.IsNullOrEmpty(empty))
      {
        return string.Empty;
      }
      return ResourceHelper.GetResourceString(assembly, empty, resourceName);
    }

    public static string GetResourceString(Type type, string resourceName)
    {
      return ResourceHelper.FindResourceString(type.Assembly, resourceName);
    }

    public static string GetResourceString(Assembly assembly, string resourceName)
    {
      return ResourceHelper.FindResourceString(assembly, resourceName);
    }

    public static string GetResourceString(Assembly assembly, string baseName, string resourceName)
    {
      string empty;
      string str = string.Concat(baseName, resourceName);
      if (ResourceHelper.resourceStringList.Contains(str))
      {
        return ResourceHelper.resourceStringList[str].ToString();
      }
      try
      {
        string str1 = (new ResourceManager(baseName, assembly)).GetString(resourceName) ?? "[Missing Resource]";
        ResourceHelper.resourceStringList.Add(str, str1);
        empty = str1;
      }
      catch (MissingManifestResourceException missingManifestResourceException)
      {
        empty = string.Empty;
      }
      return empty;
    }

    private static string ResourceToUse(string resource)
    {
      if (!resource.EndsWith(".resources"))
      {
        return resource;
      }
      return resource.Replace(".resources", string.Empty);
    }
  }
}