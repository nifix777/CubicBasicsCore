using System;
using System.Dynamic;
using System.Reflection;
using System.Xml.Linq;

namespace Cubic.Core.Xml
{
  public class DynamicXmlNode : DynamicObject
  {
    XElement node;

    public DynamicXmlNode(XElement node)
    {
      this.node = node;
    }

    public DynamicXmlNode()
    {

    }

    public DynamicXmlNode(string name)
    {
      node = new XElement(name);
    }


    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
      XElement getNode = node.Element(binder.Name);
      if (getNode != null)
      {
        result = new DynamicXmlNode(getNode);
        return true;
      }
      else
      {
        result = null;
        return false;
      }
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
      XElement setNode = node.Element(binder.Name);
      if (setNode != null)
      {
        setNode.SetValue(binder.Name);
      }
      else
      {
        if (value.GetType() == typeof(DynamicXmlNode))
        {
          node.Add(new XElement(binder.Name));
        }
        else
        {
          node.Add(new XElement(binder.Name, value));
        }
      }
      return true;
    }

    // TryGetMember always returns an instance of DynamicXMLNode. How do I get the actual value of the XML node? For example, I want the following line to work, but now it throws an exception.  
    //   String state = contact.Address.State  
    // one option is to return the actual values for leaf nodes, but to explore another option: you can try the type conversion, just add the following method to the DynaxmicXMLNode class  
    public override bool TryConvert(ConvertBinder binder, out object result)
    {
      if (binder.Type == typeof(string))
      {
        result = node.Value;
        return true;
      }
      else
      {
        result = null;
        return false;
      }
    }

    // though we can get manipulate indirectly to XElement values wrapped by the DynamicXMLNode, we can even get the contained result   
    // out of the DynamicXMLNode, we cannot call methods which is suppose to work on XElement, here is what in addition you can   
    // to get Access to XElement methods  
    public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
    {
      Type xmlType = typeof(XElement);
      try
      {
        result = xmlType.InvokeMember(binder.Name, 
                                      BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                                      null,
                                      node,
                                      args);
        return true;
      }
      catch
      {
        result = null;
        return false;
      }
    }
  }
}