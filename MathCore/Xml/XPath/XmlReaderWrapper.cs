// ReSharper disable once CheckNamespace
namespace System.Xml.XPath;

internal class XmlReaderWrapper(XmlReader reader) : XmlReader
{
    private bool _IsInFuture;

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
            var next = reader.Read();
            _OldNodeType       = reader.NodeType;
            _OldLocalName      = reader.LocalName;
            _OldPrefix         = reader.Prefix;
            _OldName           = reader.Name;
            _OldValue          = reader.Value;
            _OldValueType      = reader.ValueType;
            _OldDepth          = reader.Depth;
            _OldQuoteChar      = reader.QuoteChar;
            _OldBaseURI        = reader.BaseURI;
            _OldIsEmptyElement = reader.IsEmptyElement;
            _OldAttributeCount = reader.AttributeCount;
            _OldEOF            = reader.EOF;
            _OldReadState      = reader.ReadState;
            _OldHasAttributes  = reader.HasAttributes;
            _OldHasValue       = reader.HasValue;
            _OldIsDefault      = reader.IsDefault;
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
        _IsInFuture    = true;
        _NextReadValue = reader.Read();
        return _NextReadValue;
    }

    public override XmlReaderSettings Settings => reader.Settings;

    public override bool IsDefault => _IsInFuture ? _OldIsDefault : reader.IsDefault;

    public bool? NextNodeIsDefault => _IsInFuture
        ? reader.IsDefault
        : (PeekNextNode()
            ? reader.IsDefault
            : null);

    public override XmlNodeType NodeType => _IsInFuture ? _OldNodeType : reader.NodeType;

    public XmlNodeType? NextNodeType => _IsInFuture ? reader.NodeType : PeekNextNode() ? reader.NodeType : null;

    public override string LocalName => _IsInFuture ? _OldLocalName : reader.LocalName;

    public string NextNodeLocalName => _IsInFuture
        ? reader.LocalName
        : (PeekNextNode()
            ? reader.LocalName
            : null);

    public override string NamespaceURI => reader.NamespaceURI;

    public override string Prefix => _IsInFuture ? _OldPrefix : reader.Prefix;
    public string NextNodePrefix => _IsInFuture
        ? reader.Prefix
        : (PeekNextNode()
            ? reader.Prefix
            : null);

    public override string Name => _IsInFuture ? _OldName : reader.Name;
    public string NextNodeName => _IsInFuture
        ? reader.Name
        : (PeekNextNode()
            ? reader.Name
            : null);

    public override string Value => _IsInFuture ? _OldValue : reader.Value;
    public string NextNodeValue => _IsInFuture
        ? reader.Value
        : (PeekNextNode()
            ? reader.Value
            : null);

    public override Type ValueType => _IsInFuture ? _OldValueType : reader.ValueType;
    public Type NextNodeValueType => _IsInFuture
        ? reader.ValueType
        : (PeekNextNode()
            ? reader.ValueType
            : null);

    public override int Depth => _IsInFuture ? _OldDepth : reader.Depth;
    public int? NextNodeDepth => _IsInFuture
        ? reader.Depth
        : (PeekNextNode()
            ? reader.Depth
            : null);

    public override char QuoteChar => _IsInFuture ? _OldQuoteChar : reader.QuoteChar;
    public char? NextNodeQuoteChar => _IsInFuture
        ? reader.QuoteChar
        : (PeekNextNode()
            ? reader.QuoteChar
            : null);

    public override string BaseURI => _IsInFuture ? _OldBaseURI : reader.BaseURI;
    public string NextNodeBaseURI => _IsInFuture
        ? reader.BaseURI
        : (PeekNextNode()
            ? reader.BaseURI
            : null);

    public override bool IsEmptyElement => _IsInFuture ? _OldIsEmptyElement : reader.IsEmptyElement;
    public bool? NextNodeIsEmptyElement => _IsInFuture
        ? reader.IsEmptyElement
        : (PeekNextNode()
            ? reader.IsEmptyElement
            : null);

    public override bool HasAttributes => _IsInFuture ? _OldHasAttributes : reader.HasAttributes;
    public bool? NextNodeHasAttributes => _IsInFuture
        ? reader.HasAttributes
        : (PeekNextNode()
            ? reader.HasAttributes
            : null);

    public override bool HasValue => _IsInFuture ? _OldHasValue : reader.HasValue;
    public bool? NextNodeHasValue => _IsInFuture
        ? reader.HasValue
        : (PeekNextNode()
            ? reader.HasValue
            : null);

    public override int AttributeCount => _IsInFuture ? _OldAttributeCount : reader.AttributeCount;
    public int? NextNodeAttributeCount => _IsInFuture
        ? reader.AttributeCount
        : (PeekNextNode()
            ? reader.AttributeCount
            : null);

    public override bool EOF => _IsInFuture ? _OldEOF : reader.EOF;
    public bool? NextNodeEOF => _IsInFuture
        ? reader.EOF
        : (PeekNextNode()
            ? reader.EOF
            : null);

    public override ReadState ReadState => _IsInFuture ? _OldReadState : reader.ReadState;
    public ReadState? NextNodeReadState => _IsInFuture
        ? reader.ReadState
        : (PeekNextNode()
            ? reader.ReadState
            : null);

    public override XmlNameTable NameTable => reader.NameTable;

    public override string GetAttribute(string name) => reader.GetAttribute(name);

    public override string GetAttribute(string name, string NamespaceURI) => reader.GetAttribute(name, NamespaceURI);

    public override string GetAttribute(int i) => reader.GetAttribute(i);

    public override bool MoveToAttribute(string name) => reader.MoveToAttribute(name);

    public override bool MoveToAttribute(string name, string ns) => reader.MoveToAttribute(name, ns);

    public override bool MoveToFirstAttribute() => reader.MoveToFirstAttribute();

    public override bool MoveToNextAttribute() => reader.MoveToNextAttribute();

    public override bool MoveToElement() => reader.MoveToElement();

    public override bool ReadAttributeValue() => reader.ReadAttributeValue();

    public override string LookupNamespace(string prefix) => reader.LookupNamespace(prefix);

    public override void ResolveEntity() => reader.ResolveEntity();

    /// <inheritdoc />
    public override void Close() => reader.Close();
}