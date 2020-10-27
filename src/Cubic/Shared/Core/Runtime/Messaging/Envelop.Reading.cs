﻿using Cubic.Core.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Runtime.Messaging
{

  public partial class Envelope
  {
    public void ReadPropertiesFromDictionary(IDictionary<string, object> dictionary)
    {
      foreach (var pair in dictionary)
        if (pair.Value is string)
          ReadData(pair.Key, pair.Value.As<string>());
    }

    private void ReadData(string key, string value)
    {
      try
      {
        switch (key)
        {
          case SourceKey:
            Source = value;
            break;

          case MessageTypeKey:
            MessageType = value;
            break;

          case ReplyUriKey:
            ReplyUri = value.ToUri();
            break;

          case ContentTypeKey:
            ContentType = value;
            break;

          case OriginalIdKey:
            if (Guid.TryParse(value, out var originalId)) OriginalId = originalId;
            break;

          case SagaIdKey:
            SagaId = value;
            break;

          case ParentIdKey:
            if (Guid.TryParse(value, out var parentId)) ParentId = parentId;
            break;

          case DestinationKey:
            Destination = value.ToUri();
            break;

          case AcceptedContentTypesKey:
            AcceptedContentTypes = value.Split(',');
            break;

          case IdKey:
            if (Guid.TryParse(value, out var id)) Id = id;
            break;

          case ReplyRequestedKey:
            ReplyRequested = value;
            break;

          case AckRequestedKey:
            AckRequested = value.ToBool();
            break;

          case ExecutionTimeKey:
            //ExecutionTime = XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Utc);
            ExecutionTime = value.ToDateTime();
            break;

          case AttemptsKey:
            Attempts = int.Parse(value);
            break;

          case DeliverByHeader:
            DeliverBy = DateTime.Parse(value);
            break;


          case SentAttemptsHeaderKey:
            SentAttempts = int.Parse(value);
            break;

          case ReceivedAtKey:
            ReceivedAt = value.ToUri();
            break;


          default:
            Headers.Add(key, value);
            break;
        }
      }
      catch (Exception e)
      {
        throw new InvalidOperationException($"Error trying to read data for {key} = '{value}'", e);
      }
    }
  }
}
