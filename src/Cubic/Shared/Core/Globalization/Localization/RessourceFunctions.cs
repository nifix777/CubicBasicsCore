using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;

namespace Cubic.Core.Globalization.Localization
{
  public static class RessourceFunctions
  {
    private static IDictionary<string, ResourceManager> _CachedReaders = new Dictionary<string, ResourceManager>();

    public static RessourceWrapper GetRessourceWrapper(Type sourceType)
    {
      return new RessourceWrapper(sourceType.Assembly);
    }

    public static string GetRessourceString(string name )
    {
      return GetRessourceString( Assembly.GetExecutingAssembly() , name );
    }

    public static string GetRessourceString(string name , params object[] args )
    {
      return GetRessourceString(Assembly.GetExecutingAssembly(), name, args);
    }

    public static string GetRessourceString( Type sourceType , string name )
    {
      return GetRessourceString(sourceType.Assembly, name);
    }

    public static string GetRessourceString(Assembly sourceAssembly, string name)
    {
      var asmKey = sourceAssembly.GetName().Name;

      if (_CachedReaders.ContainsKey(asmKey))
      {
        return _CachedReaders[asmKey].GetString(name);
      }

      var resMan = new ResourceManager(asmKey, sourceAssembly);
      _CachedReaders.Add(asmKey, resMan);
      return resMan.GetString(name);
    }

    public static string GetRessourceString( Assembly sourceAssembly , string name, params object[] args)
    {
      var asmKey = sourceAssembly.GetName().Name;
      var returnValue = string.Empty;
      if ( _CachedReaders.ContainsKey( asmKey ) )
      {
        returnValue = _CachedReaders[asmKey].GetString( name );
      }

      var resMan = new ResourceManager( asmKey , sourceAssembly );
      _CachedReaders.Add( asmKey , resMan );
      returnValue = resMan.GetString( name );

      returnValue = args == null ? returnValue : string.Format(returnValue, args);

      return returnValue;
    }


    public static Stream GetRessourceData( string name )
    {
      return GetRessourceData( Assembly.GetExecutingAssembly() , name );
    }

    public static Stream GetRessourceData( Type sourceType , string name )
    {
      return GetRessourceData( sourceType.Assembly , name );
    }

    public static Stream GetRessourceData( Assembly sourceAssembly , string name )
    {
      var asmKey = sourceAssembly.GetName().Name;

      if ( _CachedReaders.ContainsKey( asmKey ) )
      {
        return _CachedReaders[asmKey].GetStream( name );
      }

      var resMan = new ResourceManager( asmKey , sourceAssembly );
      _CachedReaders.Add( asmKey , resMan );
      return resMan.GetStream( name );
    }
  }
}