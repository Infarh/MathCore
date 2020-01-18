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
namespace System.Xml.XPath
{
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
        {
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
        };

        #endregion

        #region Fields

        internal AstNode _Input;
        internal string _name;
        internal string _prefix;
        internal bool AbbrAxis;

        internal AxisType Axistype;
        internal string _Urn = string.Empty;
        internal XPathNodeType Nodetype;

        #endregion

        #region Properties

        internal override QueryType TypeOfAst => QueryType.Axis;

        internal override XPathResultType ReturnType => XPathResultType.NodeSet;

        internal AstNode Input { get => _Input; set => _Input = value; }

        internal string Urn => _Urn;

        internal string Prefix => _prefix;

        internal string Name => _name;

        internal XPathNodeType Type => Nodetype;

        internal AxisType TypeOfAxis => Axistype;

        internal string AxisName => Str[(int)Axistype];

        internal override double DefaultPriority => _Input != null
            ? 0.5
            : Axistype != AxisType.Child && Axistype != AxisType.Attribute
                ? 0.5
                : !string.IsNullOrEmpty(_name) ? 0 : (!string.IsNullOrEmpty(_prefix) ? -0.25 : -0.5);

        #endregion

        #region Constructors

        // constructor
        internal Axis(
            AxisType axistype,
            AstNode input,
            string prefix,
            string name,
            XPathNodeType nodetype)
        {
            Axistype = axistype;
            _Input = input;
            _prefix = prefix;
            _name = name;
            Nodetype = nodetype;
        }

        // constructor
        internal Axis(AxisType axistype, AstNode input)
        {
            Axistype = axistype;
            _Input = input;
            _prefix = string.Empty;
            _name = string.Empty;
            Nodetype = XPathNodeType.All;
            AbbrAxis = true;
        }

        internal Axis() { }

        #endregion
    }
}