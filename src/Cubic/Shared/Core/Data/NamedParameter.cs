using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Data
{
  public class NamedParameter : DataContainer, INamedParameter
  {
    public string Name { get => GetValue<string>(nameof(Name)); set => base.Fill(nameof(Name), value); }
    public SimpleDataType DataType { get => GetValue<SimpleDataType>(nameof(DataType)); set => base.Fill(nameof(DataType), value); }
    public string Value { get => GetValue<string>(nameof(Value)); set => base.Fill(nameof(Value), value); }
    public Type SystemType { get => Conversion.DataTypeSimpleToSystem(DataType); set => DataType = Conversion.DataTypeSimpleFromSystem(value); }
  }
}
