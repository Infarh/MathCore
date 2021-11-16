//------------------------------------------------------------------------------
// <copyright file="XPathException.cs" company="Microsoft">
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
using System.Diagnostics;
using MathCore.Annotations;
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local

// ReSharper disable once CheckNamespace
namespace System.Xml.XPath
{
    using FT = Function.FunctionType;

    internal class QueryBuilder
    {
        #region Methods

        //public enum XmlNodeType {
        //    None,
        //    Element,
        //    Attribute,
        //    Text,
        //    CDATA,
        //    EntityReference,
        //    Entity,
        //    ProcessingInstruction,
        //    Comment,
        //    Document,
        //    DocumentType,
        //    DocumentFragment,
        //    Notation,
        //    Whitespace,
        //    SignificantWhitespace,
        //    EndElement,
        //    EndEntity,
        //    XmlDeclaration
        //}

        //public enum XPathNodeType {
        //    Root,
        //    Element,
        //    Attribute,
        //    Namespace,
        //    Text,
        //    SignificantWhitespace,
        //    Whitespace,
        //    ProcessingInstruction,
        //    Comment,
        //    All,
        //}

        // xpath defines its own NodeType
        // it should use the XmlNodeType instead
        // we just map between them for now
        // so that the construct query will have
        // the XmlNodeType instead of XPathNodeType

        private XmlNodeType MapNodeType(XPathNodeType type)
        {
            var ret = XmlNodeType.None;
            switch(type)
            {
                case XPathNodeType.Element:
                    ret = XmlNodeType.Element;
                    break;
                case XPathNodeType.Attribute:
                    ret = XmlNodeType.Attribute;
                    break;
                case XPathNodeType.Text:
                    ret = XmlNodeType.Text;
                    break;
                case XPathNodeType.ProcessingInstruction:
                    ret = XmlNodeType.ProcessingInstruction;
                    break;
                case XPathNodeType.Comment:
                    ret = XmlNodeType.Comment;
                    break;
            }

            return ret;
        }

        [NotNull]
        private Query ProcessFilter([NotNull] Filter root)
        {
            //condition
            var operand = ProcessNode(root.Condition, null);

            //axis
            var qy_input = ProcessNode(root.Input, null);

            return new FilterQuery(qy_input, operand);
        }

        [NotNull] private static Query ProcessOperand([NotNull] Operand root) => new OperandQuery(root.OperandValue, root.ReturnType);

        [CanBeNull]
        private Query ProcessFunction([NotNull] Function root, Query InputQuery)
        {
            Query query;

            switch(root.TypeOfFunction)
            {
                case FT.FuncPosition:
                    query = new MethodOperand(null, root.TypeOfFunction);
                    return query;

                // we should be able to count how many attributes
                case FT.FuncCount:
                    query = ProcessNode((AstNode)root.ArgumentList[0], null);

                    if(query is AttributeQuery)
                        return new MethodOperand(query, FT.FuncCount);
                    //none attribute count function result in error.

                    break;

                case FT.FuncLocalName:
                case FT.FuncNameSpaceUri:
                case FT.FuncName:
                    if(root.ArgumentList != null && root.ArgumentList.Count > 0)
                        return new MethodOperand(ProcessNode((AstNode)root.ArgumentList[0], null),
                            root.TypeOfFunction);
                    return new MethodOperand(null, root.TypeOfFunction);

                case FT.FuncString:
                case FT.FuncConcat:
                case FT.FuncStartsWith:
                case FT.FuncContains:
                case FT.FuncSubstringBefore:
                case FT.FuncSubstringAfter:
                case FT.FuncSubstring:
                case FT.FuncStringLength:
                case FT.FuncNormalize:
                case FT.FuncTranslate:
                    if (root.ArgumentList is null) return new StringFunctions(new ArrayList(), root.TypeOfFunction);
                    var count = 0;
                    var arg_list = new ArrayList();
                    while(count < root.ArgumentList.Count)
                        arg_list.Add(ProcessNode((AstNode)root.ArgumentList[count++], null));
                    return new StringFunctions(arg_list, root.TypeOfFunction);

                case FT.FuncNumber:
                //case FT.FuncSum:
                case FT.FuncFloor:
                case FT.FuncCeiling:
                case FT.FuncRound:
                    if(root.ArgumentList != null)
                        return new NumberFunctions(ProcessNode((AstNode)root.ArgumentList[0], null),
                            root.TypeOfFunction);
                    return new NumberFunctions(null);

                case FT.FuncTrue:
                case FT.FuncFalse:
                    return new BooleanFunctions(null, root.TypeOfFunction);

                case FT.FuncNot:
                case FT.FuncLang:
                case FT.FuncBoolean:
                    return new BooleanFunctions(ProcessNode((AstNode)root.ArgumentList[0], null),
                        root.TypeOfFunction);


                // Unsupported functions
                //case FT.FuncID:

                // Last Function is not supported, because we don't know
                // how many we get in the list
                // <Root> <e a="1"/> <e a="2"/></Root>
                // /Root/e[last()=2]
                // we will not return the first one because
                // we don't if we have two e elements.
                //case FT.FuncLast:
                //qy = new MethodOperand(null, root.TypeOfFunction);
                //return qy;

                default:
                    throw new XPathReaderException("The XPath query is not supported.");
            }

            return null;
        }

        //
        // Operator: Or, and, |
        //           +, -, *, div,
        //           >, >=, <, <=, =, !=
        //
        [CanBeNull]
        private Query ProcessOperator([NotNull] Operator root, Query InputQuery)
        {
            switch(root.OperatorType)
            {
                case Operator.Op.Or:
                    return new OrExpr(ProcessNode(root.Operand1, null),
                        ProcessNode(root.Operand2, null));

                case Operator.Op.And:
                    return new AndExpr(ProcessNode(root.Operand1, null),
                        ProcessNode(root.Operand2, null));
            }

            switch(root.ReturnType)
            {
                case XPathResultType.Number:
                    return new NumericExpr(root.OperatorType,
                        ProcessNode(root.Operand1, null),
                        ProcessNode(root.Operand2, null));

                case XPathResultType.Boolean:
                    return new LogicalExpr(root.OperatorType,
                        ProcessNode(root.Operand1, null),
                        ProcessNode(root.Operand2, null));
            }

            return null;
        }

        //
        ///
        [NotNull]
        private static Query ProcessAxis([NotNull] Axis root, Query QyInput) =>
            root.TypeOfAxis switch
            {
                Axis.AxisType.Attribute => new AttributeQuery(QyInput, root.Name, root.Prefix, root.Type),
                Axis.AxisType.Self => new XPathSelfQuery(QyInput, root.Name, root.Prefix, root.Type),
                Axis.AxisType.Child => new ChildQuery(QyInput, root.Name, root.Prefix, root.Type),
                Axis.AxisType.Descendant => new DescendantQuery(QyInput, root.Name, root.Prefix, root.Type),
                Axis.AxisType.DescendantOrSelf => new DescendantQuery(QyInput, root.Name, root.Prefix, root.Type),
                _ => throw new XPathReaderException("xpath is not supported!")
            };

        [CanBeNull]
        private Query ProcessNode([CanBeNull] AstNode root, Query QyInput)
        {
            Query result = null;

            if(root is null)
                return null;

            switch(root.TypeOfAst)
            {
                case AstNode.QueryType.Axis:
                    var axis = (Axis)root;
                    result = ProcessAxis(axis, ProcessNode(axis.Input, QyInput));
                    break;

                case AstNode.QueryType.Operator:
                    result = ProcessOperator((Operator)root, null);
                    break;

                case AstNode.QueryType.Filter:
                    result = ProcessFilter((Filter)root);
                    break;

                case AstNode.QueryType.ConstantOperand:
                    result = ProcessOperand((Operand)root);
                    break;

                case AstNode.QueryType.Function:
                    result = ProcessFunction((Function)root, QyInput);
                    break;

                case AstNode.QueryType.Root:
                    result = new AbsoluteQuery();
                    break;

                case AstNode.QueryType.Group:
                    result = new GroupQuery(ProcessNode(((Group)root).GroupNode, QyInput));
                    break;
                default:
                    Debug.Assert(false, "Unknown QueryType encountered!!");
                    break;
            }
            return result;
        }

        public void Build(string xpath, [NotNull] ArrayList CompiledXPath, int depth)
        {
            //
            // build the AST node first
            //
            var root = XPathParser.ParseXPathExpression(xpath);

            var stack = new Stack();

            var query = ProcessNode(root, null);

            while(query != null)
                if(query is BaseAxisQuery axis_query)
                {
                    stack.Push(query);
                    query = axis_query.QueryInput;
                }
                else
                // these queries are not supported
                // for example, the primary expression not in the predicate.
                    throw new XPathReaderException("XPath query is not supported!");

            query = (Query)stack.Peek();

            if(query is AbsoluteQuery)
                stack.Pop(); //AbsoluteQuery at root means nothing. Throw it away.

            // reverse the query
            // compute the query depth table
            while(stack.Count > 0)
            {
                CompiledXPath.Add(stack.Pop());
                var current_query = (BaseAxisQuery)CompiledXPath[CompiledXPath.Count - 1];

                FilterQuery filter_query = null;

                if(current_query is FilterQuery f)
                {
                    filter_query = f;
                    current_query = f.Axis;
                }

                switch (current_query)
                {
                    case ChildQuery _:
                    case AttributeQuery _:
                    case DescendantQuery _:
                        depth++;
                        break;
                    case AbsoluteQuery _: depth = 0;
                        break;
                }

                current_query.Depth = depth;

                if(filter_query != null)
                    filter_query.Depth = depth;
            }

            //
            // matchIndex always point to the next query to match.
            // We use the MatchIndex to retrieve the query depth info,
            // without this added Null query, we need to check the
            // condition all the time in the Expression Advance method.
            //
            CompiledXPath.Add(new NullQuery());
        }

        #endregion
    }
}