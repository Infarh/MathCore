//------------------------------------------------------------------------------
// <copyright file="Axis.cs" company="Microsoft">
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

// ReSharper disable once CheckNamespace
namespace System.Xml.XPath;

internal class Axis : AstNode
{
    #region Types

    internal enum AxisType
    {
        Ancestor = 0,
        AncestorOrSelf,
        Attribute,
        Child,
        Descendant,
        DescendantOrSelf,
        Following,
        FollowingSibling,
        Namespace,
        Parent,
        Preceding,
        PrecedingSibling,
        Self,
        None
    }

    #endregion

    #region Static

    internal static readonly string[] Str =
    [
        "Ancestor",
        "AncestorOrSelf",
        "Attribute",
        "Child",
        "Descendant",
        "DescendantOrSelf",
        "Following",
        "FollowingSibling",
        "Namespace",
        "Parent",
        "Preceding",
        "PrecedingSibling",
        "Self"
    ];

    #endregion

    #region Fields

    internal AstNode _Input;
    internal string _Name;
    internal string _Prefix;
    internal bool _AbbrAxis;

    internal AxisType _AxisType;
    internal string _Urn = string.Empty;
    internal XPathNodeType _NodeType;

    #endregion

    #region Properties

    internal override QueryType TypeOfAst => QueryType.Axis;

    internal override XPathResultType ReturnType => XPathResultType.NodeSet;

    internal AstNode Input { get => _Input; set => _Input = value; }

    internal string Urn => _Urn;

    internal string Prefix => _Prefix;

    internal string Name => _Name;

    internal XPathNodeType Type => _NodeType;

    internal AxisType TypeOfAxis => _AxisType;

    internal string AxisName => Str[(int)_AxisType];

    internal override double DefaultPriority => _Input != null
        ? 0.5
        : _AxisType != AxisType.Child && _AxisType != AxisType.Attribute
            ? 0.5
            : !string.IsNullOrEmpty(_Name) ? 0 : (!string.IsNullOrEmpty(_Prefix) ? -0.25 : -0.5);

    #endregion

    #region Constructors

    // constructor
    internal Axis(
        AxisType AxisType,
        AstNode input,
        string prefix,
        string name,
        XPathNodeType NodeType)
    {
        _AxisType      = AxisType;
        _Input         = input;
        _Prefix        = prefix;
        _Name          = name;
        this._NodeType = NodeType;
    }

    // constructor
    internal Axis(AxisType AxisType, AstNode input)
    {
        _AxisType = AxisType;
        _Input    = input;
        _Prefix   = string.Empty;
        _Name     = string.Empty;
        _NodeType = XPathNodeType.All;
        _AbbrAxis = true;
    }

    internal Axis() { }

    #endregion
}