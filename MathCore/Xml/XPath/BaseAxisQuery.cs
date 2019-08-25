//------------------------------------------------------------------------------
// <copyright file="BaseQuery.cs" company="Microsoft">
//     
//      Copyright (c) 2002 Microsoft Corporation.  All rights reserved.
//     
//      The use and distribution terms for this software are contained in the file
//      named license.txt, which can be found in the root of this distribution.
//      By using this software in any fashion, you are agreeing to be bound by the
//      terms of this license.
//     
//      You must not remove this notice, or any other, from this software.
//     
// </copyright>                                                                
//------------------------------------------------------------------------------

namespace System.Xml.XPath
{
    internal class BaseAxisQuery : Query
    {
        #region Properties

        internal string Name { get; } = string.Empty;

        internal string Prefix { get; } = string.Empty;

        internal XPathNodeType NodeType { get; }

        internal int Depth { get; set; } = -1;

        internal Query QueryInput { get; set; }

        #endregion

        #region Constructors

        internal BaseAxisQuery() { }

        internal BaseAxisQuery(Query queryInput) { QueryInput = queryInput; }

        internal BaseAxisQuery(Query queryInput, string name, string prefix, XPathNodeType nodeType)
        {
            Prefix = prefix;
            QueryInput = queryInput;
            Name = name;
            NodeType = nodeType;
        }

        #endregion

        #region Methods

        internal override XPathResultType ReturnType() => XPathResultType.NodeSet;

        internal bool MatchType(XPathNodeType XType, XmlNodeType type)
        {
            var ret = false;

            switch(XType)
            {
                case XPathNodeType.Element:
                    if(type == XmlNodeType.Element || type == XmlNodeType.EndElement)
                        ret = true;
                    break;

                case XPathNodeType.Attribute:
                    if(type == XmlNodeType.Attribute)
                        ret = true;
                    break;

                case XPathNodeType.Text:
                    if(type == XmlNodeType.Text)
                        ret = true;
                    break;

                case XPathNodeType.ProcessingInstruction:
                    if(type == XmlNodeType.ProcessingInstruction)
                        ret = true;
                    break;

                case XPathNodeType.Comment:
                    if(type == XmlNodeType.Comment)
                        ret = true;
                    break;

                default:
                    throw new XPathReaderException("Unknown nodeType");
            }
            return ret;
        }

        #endregion
    }
}