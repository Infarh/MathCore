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
                default:
                    break;
            }

            return ret;
        }

        private Query ProcessFilter(Filter root)
        {
            //condition
            var opnd = ProcessNode(root.Condition, null);

            //axis
            var lv_QyInput = ProcessNode(root.Input, null);

            return new FilterQuery(lv_QyInput, opnd);
        }

        private Query ProcessOperand(Operand root) => new OperandQuery(root.OperandValue, root.ReturnType);

        private Query ProcessFunction(Function root, Query QyInput)
        {
            Query qy = null;

            switch(root.TypeOfFunction)
            {
                case FT.FuncPosition:
                    qy = new MethodOperand(null, root.TypeOfFunction);
                    return qy;

                // we should be able to count how many attributes
                case FT.FuncCount:
                    qy = ProcessNode((AstNode)root.ArgumentList[0], null);

                    if(qy is AttributeQuery)
                        return new MethodOperand(qy, FT.FuncCount);
                    //none attribute count funciton result in error.

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
                    ArrayList lv_ArgList = null;
                    if(root.ArgumentList != null)
                    {
                        var count = 0;
                        lv_ArgList = new ArrayList();
                        while(count < root.ArgumentList.Count)
                            lv_ArgList.Add(ProcessNode((AstNode)root.ArgumentList[count++], null));
                    }
                    return new StringFunctions(lv_ArgList, root.TypeOfFunction);

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


                // Unsupport functions
                case FT.FuncID:

                // Last Function is not supported, because we don't know
                // how many we get in the list
                // <Root> <e a="1"/> <e a="2"/></Root>
                // /Root/e[last()=2]
                // we will not return the first one because
                // we don't if we have two e elements.
                case FT.FuncLast:
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
        private Query ProcessOperator(Operator root, Query QyInput)
        {
            Query ret = null;

            switch(root.OperatorType)
            {
                case Operator.Op.Or:
                    ret = new OrExpr(ProcessNode(root.Operand1, null),
                        ProcessNode(root.Operand2, null));
                    return ret;

                case Operator.Op.And:
                    ret = new AndExpr(ProcessNode(root.Operand1, null),
                        ProcessNode(root.Operand2, null));
                    return ret;
            }

            switch(root.ReturnType)
            {
                case XPathResultType.Number:
                    ret = new NumericExpr(root.OperatorType,
                        ProcessNode(root.Operand1, null),
                        ProcessNode(root.Operand2, null));
                    return ret;

                case XPathResultType.Boolean:
                    ret = new LogicalExpr(root.OperatorType,
                        ProcessNode(root.Operand1, null),
                        ProcessNode(root.Operand2, null));
                    return ret;
            }

            return ret;
        }

        //
        ///
        private Query ProcessAxis(Axis root, Query QyInput) =>
            root.TypeOfAxis switch
            {
                Axis.AxisType.Attribute => (Query) new AttributeQuery(QyInput, root.Name, root.Prefix, root.Type),
                Axis.AxisType.Self => new XPathSelfQuery(QyInput, root.Name, root.Prefix, root.Type),
                Axis.AxisType.Child => new ChildQuery(QyInput, root.Name, root.Prefix, root.Type),
                Axis.AxisType.Descendant => new DescendantQuery(QyInput, root.Name, root.Prefix, root.Type),
                Axis.AxisType.DescendantOrSelf => new DescendantQuery(QyInput, root.Name, root.Prefix, root.Type),
                _ => throw new XPathReaderException("xpath is not supported!")
            };

        private Query ProcessNode(AstNode root, Query QyInput)
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

        public void Build(string xpath, ArrayList CompiledXPath, int depth)
        {
            //
            // build the AST node first
            //
            var root = XPathParser.ParseXPathExpresion(xpath);

            var stack = new Stack();

            var query = ProcessNode(root, null);

            while(query != null)
            {
                if(query is BaseAxisQuery axis_query)
                {
                    stack.Push(query);
                    query = axis_query.QueryInput;
                }
                else
                {
                    // these queries are not supported
                    // for example, the primary exprission not in the predicate.
                    throw new XPathReaderException("XPath query is not supported!");
                }
            }

            query = (Query)stack.Peek();

            if(query is AbsoluteQuery)
                stack.Pop(); //AbsoluteQuery at root means nothing. Throw it away.

            // reverse the query
            // compute the query depth table
            while(stack.Count > 0)
            {
                CompiledXPath.Add(stack.Pop());
                var current_query = (BaseAxisQuery)CompiledXPath[CompiledXPath.Count - 1];

                FilterQuery lv_FilterQuery = null;

                if(current_query is FilterQuery filter_query)
                {
                    lv_FilterQuery = filter_query;
                    current_query = filter_query.Axis;
                }

                if(current_query is ChildQuery || current_query is AttributeQuery || current_query is DescendantQuery)
                    ++depth;
                else if(current_query is AbsoluteQuery)
                    depth = 0;

                current_query.Depth = depth;

                if(lv_FilterQuery != null)
                    lv_FilterQuery.Depth = depth;
            }

            //
            // matchIndex always point to the next query to match.
            // We use the matchIndex to retriev the query depth info,
            // without this added Null query, we need to check the
            // condition all the time in the Expression Advance method.
            //
            CompiledXPath.Add(new NullQuery());
        }

        #endregion
    }
}