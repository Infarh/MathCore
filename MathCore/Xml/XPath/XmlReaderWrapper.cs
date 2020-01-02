namespace System.Xml.XPath
{
    internal class XmlReaderWrapper : XmlReader
    {
        private bool _IsInFuture;

        private readonly XmlReader _Reader;

        public XmlReaderWrapper(XmlReader reader) => _Reader = reader;

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

        public override string GetAttribute(string name) => _Reader.GetAttribute(name);

        public override string GetAttribute(string name, string namespaceURI) => _Reader.GetAttribute(name, namespaceURI);

        public override string GetAttribute(int i) => _Reader.GetAttribute(i);

        public override bool MoveToAttribute(string name) => _Reader.MoveToAttribute(name);

        public override bool MoveToAttribute(string name, string ns) => _Reader.MoveToAttribute(name, ns);

        public override bool MoveToFirstAttribute() => _Reader.MoveToFirstAttribute();

        public override bool MoveToNextAttribute() => _Reader.MoveToNextAttribute();

        public override bool MoveToElement() => _Reader.MoveToElement();

        public override bool ReadAttributeValue() => _Reader.ReadAttributeValue();

        public override string LookupNamespace(string prefix) => _Reader.LookupNamespace(prefix);

        public override void ResolveEntity() => _Reader.ResolveEntity();

        /// <inheritdoc />
        public override void Close() => _Reader.Close();
    }
}