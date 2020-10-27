using System;
using System.Reflection;
using Cubic.Core.Collections;

namespace Cubic.Core.Reflection
{
  public class AssemblyAttributesInfo
  {
    public AssemblyAttributesInfo(Assembly assembly)
    {
      this.FullName = assembly.GetName().FullName;
      this.Version = assembly.GetName().Version;
      this.FileVersion = assembly.GetProductVersion();
      this.Title = assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title;
      this.Copyright = assembly.GetCopyright();
      this.Product = assembly.GetProduct();
      this.Company = assembly.GetCompany();
      this.Culture = assembly.GetCustomAttribute<AssemblyCultureAttribute>().Culture;
      this.Description = assembly.GetDescription();

    }

    public string FileVersion { get; private set; }

    public string Title { get; private set; }
    public string FullName { get; private set; }
    public Version Version { get; private set; }
    public string Company { get; private set; }
    public string Product { get; private set; }
    public string Culture { get; private set; }
    public string Copyright { get; private set; }
    public string Description { get; private set; }
  }
}