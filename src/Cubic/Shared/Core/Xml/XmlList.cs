using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Cubic.Core.Xml
{
  public class XmlList<T> : List<T>, IXmlSerializable
  {
    private readonly string _collectionNode;
    private readonly string _elementNode;

    public XmlList() : base()
    {
      _elementNode = typeof(T).Name;
      _collectionNode = "XmlList";
    }

    public System.Xml.Schema.XmlSchema GetSchema() { return null; }

    public void ReadXml(XmlReader reader)
    {
      reader.ReadStartElement(_collectionNode);
      while (reader.IsStartElement(_elementNode))
      {
        Type type = Type.GetType(reader.GetAttribute("AssemblyQualifiedName"));
        XmlSerializer serial = new XmlSerializer(type);

        reader.ReadStartElement(_elementNode);
        this.Add((T)serial.Deserialize(reader));
        reader.ReadEndElement();
      }
      reader.ReadEndElement();
    }

    public void WriteXml(XmlWriter writer)
    {
      foreach (T item in this)
      {
        if (item != null)
        {
          writer.WriteStartElement(_elementNode);
          writer.WriteAttributeString("AssemblyQualifiedName", item.GetType().AssemblyQualifiedName);
          XmlSerializer xmlSerializer = new XmlSerializer(item.GetType());
          xmlSerializer.Serialize(writer, item);
          writer.WriteEndElement(); 
        }
      }
    }
  }
}