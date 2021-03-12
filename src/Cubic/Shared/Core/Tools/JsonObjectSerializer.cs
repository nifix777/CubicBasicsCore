using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cubic.Shared.Core.Tools
{
  public class JsonObjectSerializer : ObjectSerializer
  {

    public override object Deserialize(Stream stream, Type returnType, CancellationToken cancellationToken)
    {
      return new DataContractJsonSerializer(returnType).ReadObject(stream);
    }

    public override Task<object> DeserializeAsync(Stream stream, Type returnType, CancellationToken cancellationToken)
    {
      return Task.Run(() => new DataContractJsonSerializer(returnType).ReadObject(stream));
    }

    public override void Serialize(Stream stream, object value, Type inputType, CancellationToken cancellationToken)
    {
      new DataContractJsonSerializer(inputType).WriteObject(stream, value);
    }

    public override Task SerializeAsync(Stream stream, object value, Type inputType, CancellationToken cancellationToken)
    {
      return Task.Run(() => new DataContractJsonSerializer(inputType).WriteObject(stream, value));
    }
  }
}
