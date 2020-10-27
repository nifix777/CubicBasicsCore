using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Cubic.Core.Annotations;
using Cubic.Core.Collections;
using Cubic.Core.Diagnostics;

namespace Cubic.Core.Reflection
{
  /// <summary>
  /// Contains Funtctions and Extensionmethods for Assemblies and Refelction
  /// </summary>
  public static class AssemblyExtensions
  {

    /// <summary>
    /// Generates the pack URI according to <see href="https://msdn.microsoft.com/library/aa970069.aspx"/> for the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly containing the resource.</param>
    /// <returns>The pack URI.</returns>
    /// <remarks>
    /// The URI is in the format "pack://application:,,,/ReferencedAssembly;component/"
    /// </remarks>
    [NotNull]
    public static Uri GeneratePackUri([NotNull] this Assembly assembly)
    {

      var name = new AssemblyName(assembly.FullName).Name;

      return new Uri(string.Format(CultureInfo.InvariantCulture, "pack://application:,,,/{0};component/", name), UriKind.Absolute);
    }


    /// <summary>
    /// Generates the pack URI according to <see href="https://msdn.microsoft.com/library/aa970069.aspx" /> for the resource in the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly containing the resource.</param>
    /// <param name="relativeUri">The relative URI of the resource.</param>
    /// <returns>
    /// The pack URI.
    /// </returns>
    /// <remarks>
    /// The URI is in the format "pack://application:,,,/ReferencedAssembly;component/RelativeUri"
    /// </remarks>
    public static Uri GeneratePackUri([NotNull] this Assembly assembly, [NotNull] Uri relativeUri)
    {

      return new Uri(assembly.GeneratePackUri(), relativeUri);
    }

    /// <summary>
    /// Get the GUID of the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <returns></returns>
    public static string Guid(this Assembly assembly)
    {
      return assembly.GetCustomAttributes().Cast<GuidAttribute>().First().Value;
    }

    /// <summary>
    /// Returns all the Type objects with match the given <paramref name="typeFilter"/>
    /// from the executing assembly and any assemblies in its or lower directories.
    /// </summary>
    /// <param name="assembly">The Assembly on which the method is called.</param>
    /// <param name="assemblyFilter">
    /// The filter which should be satisfied to consider an Assembly for the returned
    /// set of Types, if applicable.
    /// </param>
    /// <param name="typeFilter">
    /// The filter which should be satisfied to include the Type in the returned
    /// set of Types, if applicable.
    /// </param>
    /// <returns>Any Types which match the <paramref name="typeFilter"/>.</returns>
    public static IEnumerable<Type> GetAvailableTypes(
        this Assembly assembly ,
        Func<Assembly , bool> assemblyFilter = null ,
        Func<Type , bool> typeFilter = null )
    {
      string assemblyDirectory = Path.GetDirectoryName( assembly.Location );

      List<Type> matchingTypes = new List<Type>();

      IEnumerable<string> availableAssemblies = GetAvailableAssemblies( assembly , assemblyDirectory , assemblyFilter );

      foreach ( var a in availableAssemblies )
      {
        Assembly availableAssembly = Assembly.LoadFrom( a );

        if ( ( assemblyFilter == null ) || assemblyFilter.Invoke( availableAssembly ) )
        {
          IEnumerable<Type> matchingTypesFromThisAssembly = availableAssembly.GetTypes();

          if ( typeFilter != null )
          {
            matchingTypesFromThisAssembly = Enumerable.ToArray(matchingTypesFromThisAssembly.Where( typeFilter ));
          }

          matchingTypes.AddRange( matchingTypesFromThisAssembly );
        }
      }

      Type[] distinctMatchingTypes = Enumerable.ToArray(matchingTypes.Distinct().OrderBy( t => t.Name ));

      return distinctMatchingTypes;
    }

    /// <summary>
    /// Gets the available assemblies.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <param name="assemblyDirectory">The assembly directory.</param>
    /// <param name="assemblyFilter">The assembly filter.</param>
    /// <returns></returns>
    private static IEnumerable<string> GetAvailableAssemblies(
        Assembly assembly ,
        string assemblyDirectory ,
        Func<Assembly , bool> assemblyFilter )
    {
      IEnumerable<string> availableAssemblies = GetAssembliesWithinDirectory( assemblyDirectory );

      if ( Enumerable.Count( availableAssemblies ) > 1 )
      {
        return availableAssemblies;
      }

      // The currently-executing assembly is the only one it its
      // directory; this happens in debugging and deployment
      // scenarios where each assembly lives in a separate folder.
      // We need to find the deployment root and hunt down the other
      // assemblies!

      // Get a reference to another assembly referenced by the
      // executing assembly which is in the same namespace:
      string assemblyTopLevelNamespace =
          assembly.FullName.Substring( 0 , assembly.FullName.IndexOf( '.' ) + 1 );

      AssemblyName referencedAssemblyName = assembly.GetReferencedAssemblies()
          .FirstOrDefault( an => an.FullName.StartsWith( assemblyTopLevelNamespace ) );

      if ( referencedAssemblyName == null )
      {
        return availableAssemblies;
      }

      Assembly referencedAssembly = Assembly.Load( referencedAssemblyName );
      string referencedAssemblyDirectory = Path.GetDirectoryName( referencedAssembly.Location );

      string commonRootDirectory = GetCommonDirectoryPath( assemblyDirectory , referencedAssemblyDirectory );

      IEnumerable<string> allDeployedAssemblies = GetAssembliesWithinDirectory( commonRootDirectory );

      ForceAssemblyLoadCompletion( allDeployedAssemblies , assemblyFilter );

      return allDeployedAssemblies;
    }

    private static void ForceAssemblyLoadCompletion(
        IEnumerable<string> assemblyPaths ,
        Func<Assembly , bool> assemblyFilter )
    {
      // In some debugging scenarios, assemblies are copied locally only
      // when requested; we can force all the referenced assemblies to be
      // copied locally and therefore available by calling Assembly.GetTypes():

      foreach ( var p in assemblyPaths )
      {
        Assembly assembly = Assembly.LoadFrom( p );

        if ( ( assemblyFilter == null ) || assemblyFilter.Invoke( assembly ) )
        {
          assembly.GetTypes();
        }
      }
    }

    /// <summary>
    /// Gets the assemblies within directory.
    /// </summary>
    /// <param name="directory">The directory.</param>
    /// <returns></returns>
    private static IEnumerable<string> GetAssembliesWithinDirectory( string directory )
    {
      return Directory.EnumerateFiles( directory , "*.dll" , SearchOption.TopDirectoryOnly );
    }

    /// <summary>
    /// Gets the common directory path.
    /// </summary>
    /// <param name="path1">The path1.</param>
    /// <param name="path2">The path2.</param>
    /// <returns></returns>
    private static string GetCommonDirectoryPath( string path1 , string path2 )
    {
      string[] path1Directories = path1.Split( Path.DirectorySeparatorChar );
      string[] path2Directories = path2.Split( Path.DirectorySeparatorChar );

      for ( int i = 0 ; i < path1Directories.Length ; i++ )
      {
        if ( path2Directories[i] != path1Directories[i] )
        {
          StringBuilder rootDirectoryBuilder = new StringBuilder();

          for ( int j = 0 ; j < i ; j++ )
          {
            rootDirectoryBuilder
                .Append( path1Directories[j] )
                .Append( Path.DirectorySeparatorChar );
          }

          return rootDirectoryBuilder.ToString();
        }
      }

      // If we've got here then apparently the two paths
      // are the same:
      return path1;
    }

    /// <summary>
    /// Gets the instances.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="types">The types.</param>
    /// <returns></returns>
    public static IEnumerable<T> GetInstances<T>( IEnumerable<Type> types ) where T : class
    {
      IList<T> instances = new List<T>();

      foreach ( var type in types )
      {
        try
        {
          T obj = Activator.CreateInstance( type ) as T;

          if ( obj != null )
          {
            instances.Add( obj );
          }
        }
        catch
        {

        }
      }

      return instances;
    }

    /// <summary>
    /// Gets all derieved types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="asm">The asm.</param>
    /// <returns></returns>
    public static IList<Type> GetAllDerievedTypes<T>( this Assembly asm )
    {
      IList<Type> types = new List<Type>();

      var foundTypes = asm.GetTypes().Where( t => t.IsClass && !t.IsAbstract && t.IsAssignableFrom( typeof( T ) ) );

      foreach ( var foundType in foundTypes )
      {
        types.Add( foundType );
      }

      return types;
    }

    /// <summary>
    /// Gets all derieved types.
    /// </summary>
    /// <param name="asm">The asm.</param>
    /// <param name="searchedType">Type of the searched.</param>
    /// <returns></returns>
    public static IList<Type> GetAllDerievedTypes( this Assembly asm , Type searchedType )
    {
      IList<Type> types = new List<Type>();

      var foundTypes = asm.GetTypes().Where( t => t.IsClass && !t.IsAbstract && t.IsAssignableFrom( searchedType ) );

      foreach ( var foundType in foundTypes )
      {
        types.Add( foundType );
      }

      return types;
    }

    /// <summary>
    /// Gets all derieved types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="asm">The asm.</param>
    /// <param name="ignoreTypes">The ignore types.</param>
    /// <returns></returns>
    public static IEnumerable<Type> GetAllDerievedTypes<T>( this Assembly asm , IEnumerable<Type> ignoreTypes )
    {
      List<Type> types = new List<Type>();

      bool useFilter = ignoreTypes != null && ignoreTypes.Any();

      var foundTypes = asm.GetAllDerievedTypes<T>();

      if ( useFilter )
      {
        types.AddRange( foundTypes.Where( t => !ignoreTypes.Contains( t ) ) );
      }
      else
      {
        types.AddRange( foundTypes );
      }

      return types;
    }

    /// <summary>
    /// Gets all derieved types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="asm">The asm.</param>
    /// <param name="searchPath">The search path.</param>
    /// <returns></returns>
    public static IEnumerable<Type> GetAllDerievedTypes<T>( this Assembly asm , string searchPath )
    {
      IList<Type> foundTypes = new List<Type>();

      if ( Directory.Exists( searchPath ) )
      {
        var files = AssemblyExtensions.GetAssembliesWithinDirectory( searchPath );

        foreach ( var file in files )
        {
          Assembly assembly = null;

          try
          {
            assembly = Assembly.LoadFile( file );
          }
          catch
          {

          }

          if ( assembly != null )
          {
            var localtypes = assembly.GetAllDerievedTypes<T>();

            foreach ( var localtype in localtypes )
            {
              foundTypes.Add( localtype );
            }
          }
        }
      }

      return foundTypes;
    }

    /// <summary>
    /// Gets all derieved types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="asm">The asm.</param>
    /// <param name="searchPath">The search path.</param>
    /// <param name="ignoreTypes">The ignore types.</param>
    /// <returns></returns>
    public static IEnumerable<Type> GetAllDerievedTypes<T>( this Assembly asm , string searchPath , IEnumerable<Type> ignoreTypes )
    {
      IList<Type> foundTypes = new List<Type>();

      if ( Directory.Exists( searchPath ) )
      {
        var files = AssemblyExtensions.GetAssembliesWithinDirectory( searchPath );

        foreach ( var file in files )
        {
          Assembly assembly = null;

          try
          {
            assembly = Assembly.LoadFile( file );
          }
          catch
          {

          }

          if ( assembly != null )
          {
            var types = ignoreTypes as IList<Type> ?? ignoreTypes.ToList();
            var localtypes = assembly.GetAllDerievedTypes<T>( types );

            foreach ( var localtype in localtypes )
            {
              foundTypes.Add( localtype );
            }
          }
        }
      }

      return foundTypes;
    }

    /// <summary>
    /// Gets the directory.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <returns></returns>
    public static string GetDirectory( this Assembly assembly )
    {
      return Path.GetDirectoryName( assembly.Location );
    }



    /// <summary>
    /// Gets the first attribute of a specified type from the current assembly.
    /// </summary>
    /// <typeparam name="TAttribute">Specifies an attribute type.</typeparam>
    /// <param name="assembly">Specifies the current assembly.</param>
    /// <returns>Returns the first attribute of the specified type, or null, if no attributes of the specified type
    /// were found.</returns>
    private static TAttribute GetAttribute<TAttribute>(this Assembly assembly) where TAttribute : Attribute
    {
      object[] attributes = assembly.GetCustomAttributes(typeof(TAttribute), false);

      if (attributes.Length == 0)
        return (null);

      else
        return ((TAttribute)attributes[0]);
    }

    /// <summary>
    /// Gets the attributes of a specified type from the current assembly.
    /// </summary>
    /// <typeparam name="TAttribute">Specifies an attribute type.</typeparam>
    /// <param name="assembly">Specifies the current assembly.</param>
    /// <returns>Returns the attributes of the specified type, or null, if no attributes of the specified type were
    /// found.</returns>
    private static TAttribute[] GetAttributes<TAttribute>(this Assembly assembly) where TAttribute : Attribute
    {
      object[] attributes = assembly.GetCustomAttributes(typeof(TAttribute), false);

      if (attributes.Length == 0)
        return (Enumerable.Empty<TAttribute>().ToArray());

      else
        return (Array.ConvertAll(attributes, x => (TAttribute)x));
    }

    /// <summary>
    /// Gets the value of the current assembly's Product attribute.
    /// </summary>
    /// <param name="assembly">Specifies the current assembly.</param>
    /// <returns>Returns the value of the Product attribute, or null, if the attibute was not found.</returns>
    public static string GetProduct(this Assembly assembly)
    {
      AssemblyProductAttribute productAttribute = GetAttribute<AssemblyProductAttribute>(assembly);

      if (productAttribute != null)
        return (productAttribute.Product);
      else
        return string.Empty;
    }

    /// <summary>
    /// Gets the value of the current assembly's Company attribute.
    /// </summary>
    /// <param name="assembly">Specifies the current assembly.</param>
    /// <returns>Returns the value of the Company attribute, or null, if the attibute was not found.</returns>
    public static string GetCompany(this Assembly assembly)
    {
      AssemblyCompanyAttribute companyAttribute = GetAttribute<AssemblyCompanyAttribute>(assembly);

      if (companyAttribute != null)
        return (companyAttribute.Company);
      else
        return (null);
    }

    /// <summary>
    /// Gets the value of the current assembly's Copyright attribute.
    /// </summary>
    /// <param name="assembly">Specifies the current assembly.</param>
    /// <returns>Returns the value of the Copyright attribute, or null, if the attibute was not found.</returns>
    public static string GetCopyright(this Assembly assembly)
    {
      AssemblyCopyrightAttribute copyrightAttribute = GetAttribute<AssemblyCopyrightAttribute>(assembly);

      if (copyrightAttribute != null)
        return (copyrightAttribute.Copyright);
      else
        return (null);
    }

    /// <summary>
    /// Gets the value of the current assembly's Description attribute.
    /// </summary>
    /// <param name="assembly">Specifies the current assembly.</param>
    /// <returns>Returns the value of the Description attribute, or null, if the attibute was not found.</returns>
    public static string GetDescription(this Assembly assembly)
    {
      AssemblyDescriptionAttribute descriptionAttribute = GetAttribute<AssemblyDescriptionAttribute>(assembly);

      if (descriptionAttribute != null)
        return (descriptionAttribute.Description);
      else
        return (null);
    }

    /// <summary>
    /// Gets the product version associated with the current assembly.
    /// </summary>
    /// <param name="assembly">Specifies the current assembly.</param>
    /// <returns>Returns the product version associated with the current assembly.</returns>
    public static string GetProductVersion(this Assembly assembly)
    {
      FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

      return (fileVersionInfo.ProductVersion);
    }

    /// <summary>
    /// Gets the value of the current assembly's Title attribute.
    /// </summary>
    /// <param name="assembly">Specifies the current assembly.</param>
    /// <returns>Returns the value of the Title attribute, or null, if the attibute was not found.</returns>
    public static string GetTitle(this Assembly assembly)
    {
      AssemblyTitleAttribute titleAttribute = GetAttribute<AssemblyTitleAttribute>(assembly);

      if (titleAttribute != null)
        return (titleAttribute.Title);
      else
        return string.Empty;
    }

    /// <summary>
    /// Gets the value of the current assembly's Version attribute.
    /// </summary>
    /// <param name="assembly">Specifies the current assembly.</param>
    /// <returns>Returns the value of the Version attribute, or null, if the attibute was not found.</returns>
    public static string GetVersion(this Assembly assembly)
    {
      Version version;

      version = assembly.GetName().Version;

      if (version != null)
        return (version.ToString());
      else
        return string.Empty;
    }

    public static string BuildAssamblyName(CultureInfo culture, string name, string publicKeyToken)
    {
      return string.Format(CultureInfo.InvariantCulture,
        $"{name}, Version=0.0.0.0, Culture={culture}, PublicKeyToken={publicKeyToken}");
    }

    /// <summary>
    /// Returns the public key token of a type's assembly.
    /// </summary>
    /// <param name="type">The <see cref="T:System.Type" /> whose assembly is checked.</param>
    /// <returns>The public key token value. Either "null" or a 16 byte string.</returns>
    public static string GetPublicKeyToken(Type type)
    {
      Guard.ArgumentNull(type, nameof(type));

      return AssemblyExtensions.GetPublicKeyToken(Assembly.GetAssembly(type));
    }

    /// <summary>
    /// Returns the public key token of an assembly.
    /// </summary>
    /// <param name="assemblyToCheck">The assembly to check.</param>
    /// <returns>The public key token value. Either "null" or a 16 byte string.</returns>
    public static string GetPublicKeyToken(Assembly assemblyToCheck)
    {
      Guard.ArgumentNull(assemblyToCheck, nameof(assemblyToCheck));

      string[] strArrays = assemblyToCheck.FullName.Split(new char[] { '=' });
      for (int i = (int)strArrays.Length - 1; i > 0; i--)
      {
        if (strArrays[i - 1].Length >= "PublicKeyToken".Length && strArrays[i - 1].Substring(strArrays[i - 1].Length - "PublicKeyToken".Length).Equals("PublicKeyToken"))
        {
          if (strArrays[i].Length < 17)
          {
            return strArrays[i];
          }
          return strArrays[i].Substring(0, 16);
        }
      }
      return string.Empty;
    }
  }
}