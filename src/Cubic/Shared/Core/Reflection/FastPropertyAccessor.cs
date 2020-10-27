using System;
using System.Reflection;

namespace Cubic.Core.Reflection
{
  public static class FastPropertyAccessor
  {

    public static PropertySpec CreatePropertySpec(string propertyName)
    {
      return new PropertySpec(propertyName);
    }

    public static object GetPropertyValueFast(object declaredObject, string propertyName)
    {
      return new PropertySpec(propertyName).Fetch(declaredObject);
    }



  }

  #region private Readonly Fast Property 
  /// <summary>
  /// A PropertySpec represents information needed to fetch a property from 
  /// and efficiently.   Thus it represents a '.PROP' in a TransformSpec
  /// (and a transformSpec has a list of these).  
  /// </summary>
  public class PropertySpec
  {
    /// <summary>
    /// Make a new PropertySpec for a property named 'propertyName'. 
    /// For convenience you can set he 'next' field to form a linked
    /// list of PropertySpecs. 
    /// </summary>
    public PropertySpec(string propertyName, PropertySpec next = null)
    {
      Next = next;
      _propertyName = propertyName;
    }

    /// <summary>
    /// Given an object fetch the property that this PropertySpec represents.  
    /// </summary>
    public object Fetch(object obj)
    {
      Type objType = obj.GetType();
      if (objType != _expectedType)
      {
        var typeInfo = objType.GetTypeInfo();
        _fetchForExpectedType = PropertyFetch.FetcherForProperty(typeInfo.GetDeclaredProperty(_propertyName));
        _expectedType = objType;
      }
      return _fetchForExpectedType.Fetch(obj);
    }

    /// <summary>
    /// A public field that can be used to form a linked list.   
    /// </summary>
    public PropertySpec Next;

    #region private
    /// <summary>
    /// PropertyFetch is a helper class.  It takes a PropertyInfo and then knows how
    /// to efficiently fetch that property from a .NET object (See Fetch method).  
    /// It hides some slightly complex generic code.  
    /// </summary>
    public class PropertyFetch
    {
      /// <summary>
      /// Create a property fetcher from a .NET Reflection PropertyInfo class that
      /// represents a property of a particular type.  
      /// </summary>
      public static PropertyFetch FetcherForProperty(PropertyInfo propertyInfo)
      {
        if (propertyInfo == null)
          return new PropertyFetch();     // returns null on any fetch.

        var typedPropertyFetcher = typeof(TypedFetchProperty<,>);
        var instantiatedTypedPropertyFetcher = typedPropertyFetcher.GetTypeInfo().MakeGenericType(
            propertyInfo.DeclaringType, propertyInfo.PropertyType);
        return (PropertyFetch)Activator.CreateInstance(instantiatedTypedPropertyFetcher, propertyInfo);
      }

      /// <summary>
      /// Given an object, fetch the property that this propertyFech represents. 
      /// </summary>
      public virtual object Fetch(object obj) { return null; }

      #region private 

      private class TypedFetchProperty<TObject, TProperty> : PropertyFetch
      {
        public TypedFetchProperty(PropertyInfo property)
        {
          _propertyFetch = (Func<TObject, TProperty>)property.GetMethod.CreateDelegate(typeof(Func<TObject, TProperty>));
        }
        public override object Fetch(object obj)
        {
          return _propertyFetch((TObject)obj);
        }
        private readonly Func<TObject, TProperty> _propertyFetch;
      }
      #endregion
    }

    private string _propertyName;
    private Type _expectedType;
    private PropertyFetch _fetchForExpectedType;
    #endregion
  }
  #endregion
}