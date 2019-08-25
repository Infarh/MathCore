//------------------------------------------------------------------------------
// <copyright file="XPathReader.cs" company="Microsoft">
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

using System.Collections;
using System.IO;

namespace System.Xml.XPath
{
    public class XPathReader : XmlReader
    {
        #region Fields

        private readonly XPathCollection _XPathCollection;

        #endregion

        #region Properties

        // Node Properties
        /// <devdoc>
        ///     <para>
        ///         Gets the type of the current node.
        ///     </para>
        /// </devdoc>
        public override XmlNodeType NodeType => BaseReader.NodeType;

        /// <devdoc>
        ///     <para>
        ///         Gets the name of
        ///         the current node, including the namespace prefix.
        ///     </para>
        /// </devdoc>
        public override string Name => BaseReader.Name;

        /// <devdoc>
        ///     <para>
        ///         Gets the name of the current node without the namespace prefix.
        ///     </para>
        /// </devdoc>
        public override string LocalName => BaseReader.LocalName;

        /// <devdoc>
        ///     <para>
        ///         Gets the namespace URN (as defined in the W3C Namespace Specification) of the current namespace scope.
        ///     </para>
        /// </devdoc>
        public override string NamespaceURI => BaseReader.NamespaceURI;

        /// <devdoc>
        ///     <para>
        ///         Gets the namespace prefix associated with the current node.
        ///     </para>
        /// </devdoc>
        public override string Prefix => BaseReader.Prefix;

        /// <devdoc>
        ///     <para>
        ///         Gets a value indicating whether
        ///         <see cref='System.Xml.XPath.XPathReader.Value' /> has a value to return.
        ///     </para>
        /// </devdoc>
        public override bool HasValue => BaseReader.HasValue;

        /// <devdoc>
        ///     <para>
        ///         Gets the text value of the current node.
        ///     </para>
        /// </devdoc>
        public override string Value => BaseReader.Value;

        /// <devdoc>
        ///     <para>
        ///         Gets the depth of the
        ///         current node in the XML element stack.
        ///     </para>
        /// </devdoc>
        public override int Depth => BaseReader.Depth;

        /// <devdoc>
        ///     <para>
        ///         Gets the base URI of the current node.
        ///     </para>
        /// </devdoc>
        public override string BaseURI => BaseReader.BaseURI;

        /// <devdoc>
        ///     <para>
        ///         Gets a value indicating whether
        ///         the current
        ///         node is an empty element (for example, &lt;MyElement/&gt;).
        ///     </para>
        /// </devdoc>
        public override bool IsEmptyElement => BaseReader.IsEmptyElement;

        /// <devdoc>
        ///     <para>
        ///         Gets a value indicating whether the current node is an
        ///         attribute that was generated from the default value defined
        ///         in the DTD or schema.
        ///     </para>
        /// </devdoc>
        public override bool IsDefault => BaseReader.IsDefault;

        /// <devdoc>
        ///     <para>
        ///         Gets the quotation mark character used to enclose the value of an attribute
        ///         node.
        ///     </para>
        /// </devdoc>
        public override char QuoteChar => BaseReader.QuoteChar;

        /// <devdoc>
        ///     <para>Gets the current xml:space scope.</para>
        /// </devdoc>
        public override XmlSpace XmlSpace => BaseReader.XmlSpace;

        /// <devdoc>
        ///     <para>Gets the current xml:lang scope.</para>
        /// </devdoc>
        public override string XmlLang => BaseReader.XmlLang;

        // Attribute Accessors
        /// <devdoc>
        ///     <para> The number of attributes on the current node.</para>
        /// </devdoc>
        public override int AttributeCount => BaseReader.AttributeCount;

        /// <devdoc>
        ///     <para>Gets the value of the attribute with the specified index.</para>
        /// </devdoc>
        public override string this[int i] => BaseReader[i];

        /// <devdoc>
        ///     <para>
        ///         Gets the value of the attribute with the specified
        ///         <see cref='System.Xml.XPath.XPathReader.Name' /> .
        ///     </para>
        /// </devdoc>
        /// <exception cref="InvalidOperationException">
        ///     An <see cref="T:System.Xml.XmlReader" /> method was called before a
        ///     previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown
        ///     with the message “An asynchronous operation is already in progress.”
        /// </exception>
        public override string this[string name] => BaseReader[name];

        /// <devdoc>
        ///     <para>
        ///         Gets the value of the attribute with the
        ///         specified <see cref='System.Xml.XPath.XPathReader.LocalName' /> and
        ///         <see cref='System.Xml.XPath.XPathReader.NamespaceURI' /> .
        ///     </para>
        /// </devdoc>
        public override string this[string name, string NamespaceUri] => BaseReader[name, NamespaceUri];

        //UE Atention
        public override bool CanResolveEntity => BaseReader.CanResolveEntity;

        /// <devdoc>
        ///     <para>
        ///         Gets
        ///         a value indicating whether XmlReader is positioned at the end of the
        ///         stream.
        ///     </para>
        /// </devdoc>
        public override bool EOF => BaseReader.EOF;

        /// <devdoc>
        ///     <para>
        ///         Returns
        ///         the read state of the stream.
        ///     </para>
        /// </devdoc>
        public override ReadState ReadState => BaseReader.ReadState;


        // Nametable and Namespace Helpers
        /// <devdoc>
        ///     <para>
        ///         Gets the XmlNameTable associated with this
        ///         implementation.
        ///     </para>
        /// </devdoc>
        public override XmlNameTable NameTable => BaseReader.NameTable;

        //----------------------------------------------------
        // internal methods
        //

        internal XmlReader BaseReader { get; }

        internal int ProcessAttribute { get; set; } = -1;

        internal ReadMethods ReadMethod { get; private set; } = ReadMethods.None;

        #endregion

        #region Constructors

        private XPathReader()
        {
        }

        /// <internalonly />
        /// <devdoc>
        ///     <para>
        ///         Initializes a new instance of the XPathReader class with the specified XmlNameTable.
        ///         This constructor is used when creating reader with "new XPathReader(..)"
        ///     </para>
        /// </devdoc>
        public XPathReader(XmlReader reader, XPathCollection xc) : this()
        {
            _XPathCollection = xc;
            xc.SetReader = this;
            BaseReader = reader;
        }

        /// <internalonly />
        /// <devdoc>
        ///     <para>
        ///         Initializes a new instance of the XPathReader class with the specified XmlNameTable.
        ///         This constructor is used when creating reader with "new XPathReader(..)"
        ///     </para>
        /// </devdoc>

        //
        // For class function provides the url, we will construct the
        // XmlTextReader to process the URL
        //
        public XPathReader(string url, string xpath) : this()
        {
            BaseReader = new XmlTextReader(url);
            _XPathCollection = new XPathCollection { xpath };
        }

        /// <internalonly />
        /// <devdoc>
        ///     <para>
        ///         Initializes a new instance of the XPathReader class with the specified XmlNameTable.
        ///         This constructor is used when creating reader with "new XPathReader(..)"
        ///     </para>
        /// </devdoc>

        //
        // For class function provides the url, we will construct the
        // XmlTextReader to process the URL
        //
        public XPathReader(TextReader reader, string xpath) : this()
        {
            BaseReader = new XmlTextReader(reader);
            _XPathCollection = new XPathCollection { xpath };
        }


        /// <internalonly />
        /// <devdoc>
        ///     <para>
        ///         Initializes a new instance of the XPathReader class with the specified XmlNameTable.
        ///         This constructor is used when creating reader with "new XPathReader(..)"
        ///     </para>
        /// </devdoc>

        //
        // For class function provides the url, we will construct the
        // XmlTextReader to process the URL
        //
        public XPathReader(string url, XPathCollection xc) : this(new XmlTextReader(url), xc) { }

        #endregion

        #region Methods

        /// <internalonly />
        /// <devdoc>
        ///     <para>
        ///         Initializes a new instance of the XPathReader class with the specified XmlNameTable.
        ///         This constructor is used when creating reader with "new XPathReader(..)"
        ///     </para>
        /// </devdoc>

        //
        // check the query index in the query
        // collections match the result that we are
        // looking for.
        public bool Match(int QueryIndex)
        {
#if DEBUG1
            Console.WriteLine("queryIndex: {0}", queryIndex);
#endif
            return _XPathCollection[QueryIndex] != null && _XPathCollection[QueryIndex].Match();
        }

        /// <internalonly />
        /// <devdoc>
        ///     <para>
        ///         Initializes a new instance of the XPathReader class with the specified XmlNameTable.
        ///         This constructor is used when creating reader with "new XPathReader(..)"
        ///     </para>
        /// </devdoc>
        public bool Match(string XpathQuery) => true;

        /// <internalonly />
        /// <devdoc>
        ///     <para>return true when the </para>
        /// </devdoc>
        public bool Match(XPathQuery XpathExpr) => _XPathCollection.Contains(XpathExpr) && XpathExpr.Match();

        /// <internalonly />
        /// <devdoc>
        ///     <para> return true if one of the queries matches with the XmlReader context. </para>
        /// </devdoc>
        public bool MatchesAny(ArrayList QueryList) => _XPathCollection.MatchesAny(QueryList, BaseReader.Depth);

        /// <internalonly />
        /// <devdoc>
        ///     <para> return true if one of the queries matches with the XmlReader context. </para>
        /// </devdoc>
        public bool ReadUntilMatch()
        {
            while(true)
            {
                if(ProcessAttribute > 0)
                {
                    //need to process the attribute one at time

                    if(MoveToNextAttribute())
                    {
                        //attributeIndex < AttributeCount) {
                        //MoveToAttribute(attributeIndex++);
                        if(_XPathCollection.MatchAnyQuery())
                            return true;
                    }
                    else
                        ProcessAttribute = -1; //stop attributes processing.
                }
                else if(!BaseReader.Read())
                    return false;
                else
                {
                    _XPathCollection.AdvanceUntil(this);
                    if(_XPathCollection.MatchAnyQuery()) return true;
                }
            }
        }

        /// <devdoc>
        ///     <para>
        ///         Reads the next
        ///         node from the stream.
        ///     </para>
        /// </devdoc>
        public override bool Read()
        {
            ReadMethod = ReadMethods.Read;

            var ret = BaseReader.Read();
            if(ret) _XPathCollection.Advance(this);
            return ret;
        }


        /// <devdoc>
        ///     <para>Moves to the attribute with the specified <see cref='XPathReader.Name' /> .</para>
        /// </devdoc>
        public override bool MoveToAttribute(string name)
        {
            ReadMethod = ReadMethods.MoveToAttribute;

            var ret = BaseReader.MoveToAttribute(name);
            if(ret) _XPathCollection.Advance(this);
            return ret;
        }


        /// <devdoc>
        ///     <para>
        ///         Moves to the attribute with the specified <see cref='System.Xml.XPath.XPathReader.LocalName' />
        ///         and <see cref='NamespaceURI' /> .
        ///     </para>
        /// </devdoc>
        public override bool MoveToAttribute(string name, string ns)
        {
            var ret = BaseReader.MoveToAttribute(name, ns);
            if(ret) _XPathCollection.Advance(this);
            return ret;
        }

        /// <devdoc>
        ///     <para>Moves to the attribute with the specified index.</para>
        /// </devdoc>
        public override void MoveToAttribute(int i)
        {
            ReadMethod = ReadMethods.MoveToAttribute;
            BaseReader.MoveToAttribute(i);
            _XPathCollection.Advance(this);
        }

        /// <devdoc>
        ///     <para>
        ///         Moves to the first attribute.
        ///     </para>
        /// </devdoc>
        public override bool MoveToFirstAttribute()
        {
            var ret = BaseReader.MoveToFirstAttribute();
            if(ret) _XPathCollection.Advance(this);
            return ret;
        }

        /// <devdoc>
        ///     <para>
        ///         Moves to the next attribute.
        ///     </para>
        /// </devdoc>
        public override bool MoveToNextAttribute()
        {
            var ret = BaseReader.MoveToNextAttribute();
            if(ret) _XPathCollection.Advance(this);
            return ret;
        }

        /// <devdoc>
        ///     <para>
        ///         Moves to the element that contains the current attribute node.
        ///     </para>
        /// </devdoc>
        public override bool MoveToElement()
        {
            ReadMethod = ReadMethods.MoveToElement;
            var ret = BaseReader.MoveToElement();
            if(ret) _XPathCollection.Advance(this);
            return ret;
        }

        /// <devdoc>
        ///     <para>
        ///         Gets the value of the attribute with the specified
        ///         <see cref='XPathReader.Name' /> .
        ///     </para>
        /// </devdoc>
        public override string GetAttribute(string name) => BaseReader.GetAttribute(name);

        /// <devdoc>
        ///     <para>
        ///         Gets the value of the attribute with the
        ///         specified <see cref='XPathReader.LocalName' /> and
        ///         <see cref='XPathReader.NamespaceURI' /> .
        ///     </para>
        /// </devdoc>
        public override string GetAttribute(string name, string NamespaceUri)
            => BaseReader.GetAttribute(name, NamespaceUri);

        /// <devdoc>
        ///     <para>Gets the value of the attribute with the specified index.</para>
        /// </devdoc>
        public override string GetAttribute(int i) => BaseReader.GetAttribute(i);

        /// <devdoc>
        ///     <para>
        ///         Closes the stream, changes the <see cref='XPathReader.ReadState' />
        ///         to Closed, and sets all the properties back to zero.
        ///     </para>
        /// </devdoc>
        public override void Close() => BaseReader.Close();

        /// <devdoc>
        ///     <para>Reads the contents of an element as a string.</para>
        /// </devdoc>
        public override string ReadString() => BaseReader.ReadString();

        /// <devdoc>
        ///     <para>
        ///         Resolves a namespace prefix in the current element's scope.
        ///     </para>
        /// </devdoc>
        public override string LookupNamespace(string prefix) => BaseReader.LookupNamespace(prefix);

        /// <devdoc>
        ///     <para>Resolves the entity reference for nodes of NodeType EntityReference.</para>
        /// </devdoc>
        public override void ResolveEntity() => BaseReader.ResolveEntity();

        /// <devdoc>
        ///     <para>
        ///         Parses the attribute value into one or more Text and/or EntityReference node
        ///         types.
        ///     </para>
        /// </devdoc>
        public override bool ReadAttributeValue() => BaseReader.ReadAttributeValue();

        /// <devdoc>
        ///     <para>Reads all the content (including markup) as a string.</para>
        /// </devdoc>
        public override string ReadInnerXml() => BaseReader.ReadInnerXml();

        /// <devdoc>
        ///     <para>[To be supplied.]</para>
        /// </devdoc>
        public override string ReadOuterXml() => BaseReader.ReadOuterXml();

        internal bool MapPrefixWithNamespace(string prefix)
        {
            var lv_NsMgr = _XPathCollection.NamespaceManager;
            return lv_NsMgr != null && lv_NsMgr.LookupNamespace(prefix) == NamespaceURI;
        }

        #endregion
    }

    class XmlReaderWrapper : XmlReader
    {
        private bool _IsInFuture;

        private readonly XmlReader _Reader;

        public XmlReaderWrapper(XmlReader reader) { _Reader = reader; }

        private XmlNodeType _OldNodeType;
        private string _OldLocalName;
        private string _OldPrefix;
        private string _OldName;
        private string _OldValue;
        private Type _OldValueType;
        private int _OldDepth;
        private char _OldQuoteChar;
        private string _OldBaseURI;
        private bool _OldIsEmptyElement;
        private int _OldAttributeCount;
        private bool _OldEOF;
        private ReadState _OldReadState;
        private bool _OldHasAttributes;
        private bool _OldHasValue;
        private bool _OldIsDefault;

        public override bool Read()
        {
            if(!_IsInFuture)
            {
                var next = _Reader.Read();
                _OldNodeType = _Reader.NodeType;
                _OldLocalName = _Reader.LocalName;
                _OldPrefix = _Reader.Prefix;
                _OldName = _Reader.Name;
                _OldValue = _Reader.Value;
                _OldValueType = _Reader.ValueType;
                _OldDepth = _Reader.Depth;
                _OldQuoteChar = _Reader.QuoteChar;
                _OldBaseURI = _Reader.BaseURI;
                _OldIsEmptyElement = _Reader.IsEmptyElement;
                _OldAttributeCount = _Reader.AttributeCount;
                _OldEOF = _Reader.EOF;
                _OldReadState = _Reader.ReadState;
                _OldHasAttributes = _Reader.HasAttributes;
                _OldHasValue = _Reader.HasValue;
                _OldIsDefault = _Reader.IsDefault;
                return next;
            }
            _IsInFuture = false;
            return _NextReadValue;
        }

        private bool _NextReadValue = true;
        public bool PeekNextNode()
        {
            if(!_NextReadValue) return false;
            if(_IsInFuture) return _NextReadValue;
            _IsInFuture = true;
            _NextReadValue = _Reader.Read();
            return _NextReadValue;
        }

        public override XmlReaderSettings Settings => _Reader.Settings;

        public override bool IsDefault => _IsInFuture ? _OldIsDefault : _Reader.IsDefault;

        public bool? NextNodeIsDefault => _IsInFuture
            ? _Reader.IsDefault
            : (PeekNextNode()
                ? _Reader.IsDefault
                : (bool?)null);

        public override XmlNodeType NodeType => _IsInFuture ? _OldNodeType : _Reader.NodeType;

        public XmlNodeType? NextNodeType
        {
            get
            {
                if(_IsInFuture) return _Reader.NodeType;
                return PeekNextNode() ? _Reader.NodeType : (XmlNodeType?)null;
            }
        }

        public override string LocalName => _IsInFuture ? _OldLocalName : _Reader.LocalName;

        public string NextNodeLocalName => _IsInFuture
            ? _Reader.LocalName
            : (PeekNextNode()
                ? _Reader.LocalName
                : null);

        public override string NamespaceURI => _Reader.NamespaceURI;

        public override string Prefix => _IsInFuture ? _OldPrefix : _Reader.Prefix;
        public string NextNodePrefix => _IsInFuture
            ? _Reader.Prefix
            : (PeekNextNode()
                ? _Reader.Prefix
                : null);

        public override string Name => _IsInFuture ? _OldName : _Reader.Name;
        public string NextNodeName => _IsInFuture
            ? _Reader.Name
            : (PeekNextNode()
                ? _Reader.Name
                : null);

        public override string Value => _IsInFuture ? _OldValue : _Reader.Value;
        public string NextNodeValue => _IsInFuture
            ? _Reader.Value
            : (PeekNextNode()
                ? _Reader.Value
                : null);

        public override Type ValueType => _IsInFuture ? _OldValueType : _Reader.ValueType;
        public Type NextNodeValueType => _IsInFuture
            ? _Reader.ValueType
            : (PeekNextNode()
                ? _Reader.ValueType
                : null);

        public override int Depth => _IsInFuture ? _OldDepth : _Reader.Depth;
        public int? NextNodeDepth => _IsInFuture
            ? _Reader.Depth
            : (PeekNextNode()
                ? _Reader.Depth
                : (int?)null);

        public override char QuoteChar => _IsInFuture ? _OldQuoteChar : _Reader.QuoteChar;
        public char? NextNodeQuoteChar => _IsInFuture
            ? _Reader.QuoteChar
            : (PeekNextNode()
                ? _Reader.QuoteChar
                : (char?)null);

        public override string BaseURI => _IsInFuture ? _OldBaseURI : _Reader.BaseURI;
        public string NextNodeBaseURI => _IsInFuture
            ? _Reader.BaseURI
            : (PeekNextNode()
                ? _Reader.BaseURI
                : null);

        public override bool IsEmptyElement => _IsInFuture ? _OldIsEmptyElement : _Reader.IsEmptyElement;
        public bool? NextNodeIsEmptyElement => _IsInFuture
            ? _Reader.IsEmptyElement
            : (PeekNextNode()
                ? _Reader.IsEmptyElement
                : (bool?)null);

        public override bool HasAttributes => _IsInFuture ? _OldHasAttributes : _Reader.HasAttributes;
        public bool? NextNodeHasAttributes => _IsInFuture
            ? _Reader.HasAttributes
            : (PeekNextNode()
                ? _Reader.HasAttributes
                : (bool?)null);

        public override bool HasValue => _IsInFuture ? _OldHasValue : _Reader.HasValue;
        public bool? NextNodeHasValue => _IsInFuture
            ? _Reader.HasValue
            : (PeekNextNode()
                ? _Reader.HasValue
                : (bool?)null);

        public override int AttributeCount => _IsInFuture ? _OldAttributeCount : _Reader.AttributeCount;
        public int? NextNodeAttributeCount => _IsInFuture
            ? _Reader.AttributeCount
            : (PeekNextNode()
                ? _Reader.AttributeCount
                : (int?)null);

        public override bool EOF => _IsInFuture ? _OldEOF : _Reader.EOF;
        public bool? NextNodeEOF => _IsInFuture
            ? _Reader.EOF
            : (PeekNextNode()
                ? _Reader.EOF
                : (bool?)null);

        public override ReadState ReadState => _IsInFuture ? _OldReadState : _Reader.ReadState;
        public ReadState? NextNodeReadState => _IsInFuture
            ? _Reader.ReadState
            : (PeekNextNode()
                ? _Reader.ReadState
                : (ReadState?)null);

        public override XmlNameTable NameTable => _Reader.NameTable;

        public override string GetAttribute(string name)
        {
            return _Reader.GetAttribute(name);
        }

        public override string GetAttribute(string name, string namespaceURI)
        {
            return _Reader.GetAttribute(name, namespaceURI);
        }

        public override string GetAttribute(int i)
        {
            return _Reader.GetAttribute(i);
        }

        public override bool MoveToAttribute(string name)
        {
            return _Reader.MoveToAttribute(name);
        }

        public override bool MoveToAttribute(string name, string ns)
        {
            return _Reader.MoveToAttribute(name, ns);
        }

        public override bool MoveToFirstAttribute()
        {
            return _Reader.MoveToFirstAttribute();
        }

        public override bool MoveToNextAttribute()
        {
            return _Reader.MoveToNextAttribute();
        }

        public override bool MoveToElement()
        {
            return _Reader.MoveToElement();
        }

        public override bool ReadAttributeValue()
        {
            return _Reader.ReadAttributeValue();
        }

        public override string LookupNamespace(string prefix)
        {
            return _Reader.LookupNamespace(prefix);
        }

        public override void ResolveEntity()
        {
            _Reader.ResolveEntity();
        }

        /// <inheritdoc />
        public override void Close()
        {
            _Reader.Close();
        }
    }
}