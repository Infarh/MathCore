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
using MathCore.Annotations;
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Xml.XPath;

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
    ///         <see cref='XPathReader.Value' /> has a value to return.
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
    ///         <see cref='XPathReader.Name' /> .
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
    ///         specified <see cref='XPathReader.LocalName' /> and
    ///         <see cref='XPathReader.NamespaceURI' /> .
    ///     </para>
    /// </devdoc>
    public override string this[string name, string NamespaceUri] => BaseReader[name, NamespaceUri];

    //UE Attention
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


    // NameTable and Namespace Helpers
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

    private XPathReader() { }

    /// <internalonly />
    /// <devdoc>
    ///     <para>
    ///         Initializes a new instance of the XPathReader class with the specified XmlNameTable.
    ///         This constructor is used when creating reader with "new XPathReader(..)"
    ///     </para>
    /// </devdoc>
    public XPathReader(XmlReader Reader, [NotNull] XPathCollection XPathCollection) : this()
    {
        _XPathCollection          = XPathCollection;
        XPathCollection.SetReader = this;
        BaseReader                = Reader;
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
    public XPathReader([NotNull] string Url, string XPath) : this()
    {
        BaseReader       = new XmlTextReader(Url);
        _XPathCollection = new XPathCollection { XPath };
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
    public XPathReader(TextReader Reader, string XPath) : this()
    {
        BaseReader       = new XmlTextReader(Reader);
        _XPathCollection = new XPathCollection { XPath };
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
    public XPathReader([NotNull] string Url, [NotNull] XPathCollection XPathCollection) : this(new XmlTextReader(Url), XPathCollection) { }

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
    public bool Match(int QueryIndex) => _XPathCollection[QueryIndex] != null && _XPathCollection[QueryIndex].Match();

    /// <internalonly />
    /// <devdoc>
    ///     <para>
    ///         Initializes a new instance of the XPathReader class with the specified XmlNameTable.
    ///         This constructor is used when creating reader with "new XPathReader(..)"
    ///     </para>
    /// </devdoc>
    public bool Match(string XPathQuery) => true;

    /// <internalonly />
    /// <devdoc>
    ///     <para>return true when the </para>
    /// </devdoc>
    public bool Match(XPathQuery XPathExpr) => _XPathCollection.Contains(XPathExpr) && XPathExpr.Match();

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
            if(ProcessAttribute > 0)
            {
                //need to process the attribute one at time

                if(MoveToNextAttribute())
                {
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
    public override bool MoveToAttribute(string AttributeName)
    {
        ReadMethod = ReadMethods.MoveToAttribute;

        var ret = BaseReader.MoveToAttribute(AttributeName);
        if(ret) _XPathCollection.Advance(this);
        return ret;
    }


    /// <devdoc>
    ///     <para>
    ///         Moves to the attribute with the specified <see cref='XPathReader.LocalName' />
    ///         and <see cref='NamespaceURI' /> .
    ///     </para>
    /// </devdoc>
    public override bool MoveToAttribute(string AttributeName, string ns)
    {
        var result = BaseReader.MoveToAttribute(AttributeName, ns);
        if(result) _XPathCollection.Advance(this);
        return result;
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
        var result = BaseReader.MoveToFirstAttribute();
        if(result) _XPathCollection.Advance(this);
        return result;
    }

    /// <devdoc>
    ///     <para>
    ///         Moves to the next attribute.
    ///     </para>
    /// </devdoc>
    public override bool MoveToNextAttribute()
    {
        var result = BaseReader.MoveToNextAttribute();
        if(result) _XPathCollection.Advance(this);
        return result;
    }

    /// <devdoc>
    ///     <para>
    ///         Moves to the element that contains the current attribute node.
    ///     </para>
    /// </devdoc>
    public override bool MoveToElement()
    {
        ReadMethod = ReadMethods.MoveToElement;
        var result = BaseReader.MoveToElement();
        if(result) _XPathCollection.Advance(this);
        return result;
    }

    /// <devdoc>
    ///     <para>
    ///         Gets the value of the attribute with the specified
    ///         <see cref='XPathReader.Name' /> .
    ///     </para>
    /// </devdoc>
    public override string GetAttribute(string AttributeName) => BaseReader.GetAttribute(AttributeName);

    /// <devdoc>
    ///     <para>
    ///         Gets the value of the attribute with the
    ///         specified <see cref='XPathReader.LocalName' /> and
    ///         <see cref='XPathReader.NamespaceURI' /> .
    ///     </para>
    /// </devdoc>
    public override string GetAttribute(string AttributeName, string NamespaceUri)
        => BaseReader.GetAttribute(AttributeName, NamespaceUri);

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
    public override string LookupNamespace(string Prefix) => BaseReader.LookupNamespace(Prefix);

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

    internal bool MapPrefixWithNamespace(string prefix) => 
        _XPathCollection.NamespaceManager?.LookupNamespace(prefix) == NamespaceURI;

    #endregion
}