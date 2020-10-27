using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Runtime
{
  public static class SerialisationHelper
  {
    public static object CreateWithSerialisationCtor(Type type, SerializationInfo info, StreamingContext context)
    {
      var uninitializedObject = FormatterServices.GetUninitializedObject(type);

      var ctorInfo = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { typeof(SerializationInfo), typeof(StreamingContext) }, null);
      //var infoParameter = Expression.Parameter(typeof(SerializationInfo), "info");
      //var contextParamter = Expression.Parameter(typeof(StreamingContext), "context");

      //var callTheCtor = Expression.New(ctorInfo, infoParameter, contextParamter);

      return ctorInfo.Invoke(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { info, context }, CultureInfo.CurrentCulture);

    }
  }
}
