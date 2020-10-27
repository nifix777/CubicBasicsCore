using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Cubic.Core.Text
{
    public static class BasicParsingFunctions
    {
        public static string GetCsvString<T>(T obj)
        {
            StringBuilder sb = new StringBuilder();

            if (obj == null)
            {
                throw new ArgumentNullException("obj", "Object for String can not be NULL!");
            }

            Type type = typeof (T);
            var data = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Select( pi => pi.GetValue(obj)).ToObjectArray();

            foreach (var o in data)
            {
                sb.AppendFormat("{0}", Constants.Semicolon);
            }

            return sb.ToString();
        }
    }
}