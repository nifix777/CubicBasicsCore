using System;
using System.IO;
using System.Xml;

namespace Cubic.Core.Xml
{
  public static class XmlHelper
  {
    public static XmlQualifiedName CreateQualifiedName(string name, string namespaceUri)
    {
      if (string.IsNullOrEmpty(namespaceUri))
      {
        return new XmlQualifiedName(name);
      }
      return new XmlQualifiedName(name, namespaceUri);
    }

    public static bool Read(string tag, Action readAction, XmlReader reader)
    {
      if (reader.Name != tag)
      {
        return false;
      }
      readAction();
      return true;
    }

    public static void ReadXml(XmlReader reader, Func<XmlReader, bool> readAttribute, Func<XmlReader, bool> readSubElements)
    {
      using (XmlReader xmlReader = reader.ReadSubtree())
      {
        xmlReader.MoveToContent();
        if (readAttribute != null && xmlReader.HasAttributes)
        {
          while (xmlReader.MoveToNextAttribute())
          {
            readAttribute(xmlReader);
          }
        }
        if (readSubElements != null)
        {
          bool flag = xmlReader.Read();
          while (flag)
          {
            if (xmlReader.IsStartElement() && readSubElements(xmlReader))
            {
              continue;
            }
            flag = xmlReader.Read();
          }
        }
      }
      reader.Read();
    }

    public static string ToXmlString(Action<XmlWriter> writeTo)
    {
      string end;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        XmlWriterSettings xmlWriterSetting = new XmlWriterSettings()
        {
          ConformanceLevel = ConformanceLevel.Fragment,
          Indent = true,
          OmitXmlDeclaration = true
        };
        using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSetting))
        {
          writeTo(xmlWriter);
        }
        memoryStream.Seek((long)0, SeekOrigin.Begin);
        using (StreamReader streamReader = new StreamReader(memoryStream))
        {
          end = streamReader.ReadToEnd();
        }
      }
      return end;
    }

    public static void WriteStartElement(this XmlWriter writer, XmlQualifiedName qualifiedName)
    {
      if (string.IsNullOrEmpty(qualifiedName.Namespace))
      {
        writer.WriteStartElement(qualifiedName.Name);
        return;
      }
      writer.WriteStartElement(qualifiedName.Name, qualifiedName.Namespace);
    }
  }
}