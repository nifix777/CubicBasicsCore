using Cubic.Core.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Runtime.Messaging
{
  public static class MessagingExtensions
  {
    public static T As<T>(this object value)
    {
      return (T)value;
    }

    public static string ToMessageTypeName(this Type type)
    {
      return type.FullName;
    }

    public static bool IsEmpty(this Guid guid)
    {
      return Guid.Empty == guid;
    }

    public static string QueueName(this Uri uri)
    {
      if (uri == null) return null;

      if (uri.Scheme == TransportConstants.Loopback && uri.Host != TransportConstants.Durable) return uri.Host;

      var lastSegment = uri.Segments.Skip(1).LastOrDefault();
      if (lastSegment == TransportConstants.Durable) return TransportConstants.Default;

      return lastSegment ?? TransportConstants.Default;
    }

    public static bool IsDurable(this Uri uri)
    {
      if (uri.Scheme == TransportConstants.Loopback && uri.Host == TransportConstants.Durable) return true;

      var firstSegment = uri.Segments.Skip(1).FirstOrDefault();
      if (firstSegment == null) return false;

      return TransportConstants.Durable == firstSegment.TrimEnd('/');
    }


    public static Uri ToCanonicalTcpUri(this Uri uri)
    {
      if (uri.Scheme != TransportConstants.Durable)
        throw new ArgumentOutOfRangeException(nameof(uri),
            "This only applies to Uri's with the scheme 'durable'");

      var queueName = uri.QueueName();

      return queueName == TransportConstants.Default
          ? $"tcp://{uri.Host}:{uri.Port}/{TransportConstants.Durable}".ToUri()
          : $"tcp://{uri.Host}:{uri.Port}/{TransportConstants.Durable}/{queueName}".ToUri();
    }

    public static Uri ToCanonicalUri(this Uri uri)
    {
      switch (uri.Scheme)
      {
        case "tcp":
          return uri.IsDurable()
              ? $"tcp://{uri.Host}:{uri.Port}/{TransportConstants.Durable}".ToUri()
              : $"tcp://{uri.Host}:{uri.Port}".ToUri();

        case "durable":
          return $"tcp://{uri.Host}:{uri.Port}/{TransportConstants.Durable}".ToUri();

        default:
          return uri;
      }
    }
  }
}

