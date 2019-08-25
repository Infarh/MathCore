using System.Xml;

namespace MathCore.DataGenericSouces
{
    public class XmlDataHost : DataHost<XmlNode>
    {
        private XmlDocument _XmlDocument;



        public XmlDataHost(XmlDocument data) { _XmlDocument = data; }



    }
}