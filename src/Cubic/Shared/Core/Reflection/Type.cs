//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Cubic.Core.Reflection
//{
//  public static class Type<T>
//  {
//    private static readonly PropertyDescriptorCollection properties;


//    static Type()
//    {
//      if(RuntimeType.IsValueType)
//      {

//      }
//    }
      


//    /// <summary>
//    /// Gets reflected type.
//    /// </summary>
//    public static Type RuntimeType => typeof(T);

//    /// <summary>
//    /// Returns default value for this type.
//    /// </summary>
//    public static T Default => Intrinsics.DefaultOf<T>();

//    ///// <summary>
//    ///// Provides smart hash code computation.
//    ///// </summary>
//    ///// <remarks>
//    ///// For reference types, this delegate always calls <see cref="object.GetHashCode"/> virtual method.
//    ///// For value type, it calls <see cref="object.GetHashCode"/> if it is overridden by the value type; otherwise,
//    ///// it calls <see cref="BitwiseComparer{T}.GetHashCode(T, bool)"/>.
//    ///// </remarks>
//    //public static new readonly Operator<T, int> GetHashCode;

//    ///// <summary>
//    ///// Provides smart equality check.
//    ///// </summary>
//    ///// <remarks>
//    ///// If type <typeparamref name="T"/> has equality operator then use it.
//    ///// Otherwise, for reference types, this delegate always calls <see cref="object.Equals(object, object)"/> method.
//    ///// For value type, it calls equality operator or <see cref="IEquatable{T}.Equals(T)"/> if it is implemented by the value type; else,
//    ///// it calls <see cref="BitwiseComparer{T}.Equals{G}"/>.
//    ///// </remarks>
//    //public static new readonly Operator<T, T, bool> Equals;

//    /// <summary>
//    /// Determines whether an instance of a specified type can be assigned to an instance of the current type.
//    /// </summary>
//    /// <typeparam name="U">The type to compare with the current type.</typeparam>
//    /// <returns><see langword="true"/>, if instance of type <typeparamref name="U"/> can be assigned to type <typeparamref name="T"/>.</returns>
//    public static bool IsAssignableFrom<U>() => RuntimeType.IsAssignableFrom(typeof(U));

//    /// <summary>
//    /// Determines whether an instance of the current type can be assigned to an instance of the specified type.
//    /// </summary>
//    /// <typeparam name="U">The type to compare with the current type.</typeparam>
//    /// <returns><see langword="true"/>, if instance of type <typeparamref name="T"/> can be assigned to type <typeparamref name="U"/>.</returns>
//    public static bool IsAssignableTo<U>() => Type<U>.IsAssignableFrom<T>();

//    #region Property
//    /// <summary>
//    /// Provides access to property declared in type <typeparamref name="T"/>.
//    /// </summary>
//    /// <typeparam name="V">Type of property.</typeparam>
//    public static class Property<V>
//    {
//      /// <summary>
//      /// Reflects instance property.
//      /// </summary>
//      /// <param name="propertyName">Name of property.</param>
//      /// <param name="nonPublic"><see langword="true"/> to reflect non-public property.</param>
//      /// <returns>Property field; or <see langword="null"/>, if property doesn't exist.</returns>
//      public static Property<T, V> Get(string propertyName, bool nonPublic = false)
//          => Property<T, V>.GetOrCreate(propertyName, nonPublic);

//      /// <summary>
//      /// Reflects instance property.
//      /// </summary>
//      /// <param name="propertyName">Name of property.</param>
//      /// <param name="nonPublic"><see langword="true"/> to reflect non-public property.</param>
//      /// <returns>Instance property.</returns>
//      /// <exception cref="MissingPropertyException">Property doesn't exist.</exception>
//      public static Property<T, V> Require(string propertyName, bool nonPublic = false)
//          => Get(propertyName, nonPublic) ?? throw MissingPropertyException.Create<T, V>(propertyName);

//      /// <summary>
//      /// Reflects instance property getter method.
//      /// </summary>
//      /// <param name="propertyName">The name of the property.</param>
//      /// <param name="nonPublic"><see langword="true"/> to reflect non-public property.</param>
//      /// <returns>The reflected getter method; or <see langword="null"/>, if getter method doesn't exist.</returns>
//      public static Reflection.Method<MemberGetter<T, V>> GetGetter(string propertyName, bool nonPublic = false)
//                => Get(propertyName, nonPublic)?.GetMethod;

//      /// <summary>
//      /// Reflects instance property getter method.
//      /// </summary>
//      /// <param name="propertyName">The name of the property.</param>
//      /// <param name="nonPublic"><see langword="true"/> to reflect non-public property.</param>
//      /// <returns>The reflected getter method.</returns>
//      /// <exception cref="MissingMethodException">The getter doesn't exist.</exception>
//      public static Reflection.Method<MemberGetter<T, V>> RequireGetter(string propertyName, bool nonPublic = false)
//          => GetGetter(propertyName, nonPublic) ?? throw MissingMethodException.Create<T, MemberGetter<T, V>>(propertyName.ToGetterName());

//      /// <summary>
//      /// Reflects instance property setter method.
//      /// </summary>
//      /// <param name="propertyName">The name of the property.</param>
//      /// <param name="nonPublic"><see langword="true"/> to reflect non-public property.</param>
//      /// <returns>The reflected setter method; or <see langword="null"/>, if setter method doesn't exist.</returns>
//      public static Reflection.Method<MemberSetter<T, V>> GetSetter(string propertyName, bool nonPublic = false)
//          => Get(propertyName, nonPublic)?.SetMethod;

//      /// <summary>
//      /// Reflects instance property setter method.
//      /// </summary>
//      /// <param name="propertyName">The name of the property.</param>
//      /// <param name="nonPublic"><see langword="true"/> to reflect non-public property.</param>
//      /// <returns>The reflected setter method.</returns>
//      /// <exception cref="MissingMethodException">The setter doesn't exist.</exception>
//      public static Reflection.Method<MemberSetter<T, V>> RequireSetter(string propertyName, bool nonPublic = false)
//          => GetSetter(propertyName, nonPublic) ?? throw MissingMethodException.Create<T, MemberSetter<T, V>>(propertyName.ToSetterName());

//      /// <summary>
//      /// Reflects static property.
//      /// </summary>
//      /// <param name="propertyName">Name of property.</param>
//      /// <param name="nonPublic">True to reflect non-public property.</param>
//      /// <returns>Instance property; or <see langword="null"/>, if property doesn't exist.</returns>
//      public static Reflection.Property<V> GetStatic(string propertyName, bool nonPublic = false)
//          => Reflection.Property<V>.GetOrCreate<T>(propertyName, nonPublic);

//      /// <summary>
//      /// Reflects static property.
//      /// </summary>
//      /// <param name="propertyName">Name of property.</param>
//      /// <param name="nonPublic">True to reflect non-public property.</param>
//      /// <returns>Instance property.</returns>
//      /// <exception cref="MissingPropertyException">Property doesn't exist.</exception>
//      public static Reflection.Property<V> RequireStatic(string propertyName, bool nonPublic = false)
//          => GetStatic(propertyName, nonPublic) ?? throw MissingFieldException.Create<T, V>(propertyName);

//      /// <summary>
//      /// Reflects static property getter method.
//      /// </summary>
//      /// <param name="propertyName">The name of the property.</param>
//      /// <param name="nonPublic"><see langword="true"/> to reflect non-public property.</param>
//      /// <returns>The reflected getter method; or <see langword="null"/>, if getter method doesn't exist.</returns>
//      public static Reflection.Method<MemberGetter<V>> GetStaticGetter(string propertyName, bool nonPublic = false)
//          => GetStatic(propertyName, nonPublic)?.GetMethod;

//      /// <summary>
//      /// Reflects static property setter method.
//      /// </summary>
//      /// <param name="propertyName">The name of the property.</param>
//      /// <param name="nonPublic"><see langword="true"/> to reflect non-public property.</param>
//      /// <returns>The reflected setter method.</returns>
//      /// <exception cref="MissingMethodException">The setter doesn't exist.</exception>
//      public static Reflection.Method<MemberGetter<V>> RequireStaticGetter(string propertyName, bool nonPublic = false)
//          => GetStaticGetter(propertyName, nonPublic) ?? throw MissingMethodException.Create<T, MemberGetter<V>>(propertyName.ToGetterName());

//      /// <summary>
//      /// Reflects static property setter method.
//      /// </summary>
//      /// <param name="propertyName">The name of the property.</param>
//      /// <param name="nonPublic"><see langword="true"/> to reflect non-public property.</param>
//      /// <returns>The reflected setter method; or <see langword="null"/>, if setter method doesn't exist.</returns>
//      public static Reflection.Method<MemberSetter<V>> GetStaticSetter(string propertyName, bool nonPublic = false)
//          => GetStatic(propertyName, nonPublic)?.SetMethod;

//      /// <summary>
//      /// Reflects static property setter method.
//      /// </summary>
//      /// <param name="propertyName">The name of the property.</param>
//      /// <param name="nonPublic"><see langword="true"/> to reflect non-public property.</param>
//      /// <returns>The reflected setter method.</returns>
//      /// <exception cref="MissingMethodException">The setter doesn't exist.</exception>
//      public static Reflection.Method<MemberSetter<V>> RequireStaticSetter(string propertyName, bool nonPublic = false)
//          => GetStaticSetter(propertyName, nonPublic) ?? throw MissingMethodException.Create<T, MemberSetter<V>>(propertyName.ToSetterName());
//    } 
//    #endregion
//  }

//  internal static class TypeExtensions
//  {
//    private const BindingFlags PublicInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;

//    internal static MethodInfo GetHashCodeMethod(this Type type)
//        => type.GetMethod(nameof(GetHashCode), PublicInstance, Array.Empty<Type>());

//    internal static string ToGetterName(this string propertyName) => string.Concat("get_", propertyName);

//    internal static string ToSetterName(this string propertyName) => string.Concat("set_", propertyName);
//  }
//}
