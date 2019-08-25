//------------------------------------------------------------------------------
// <copyright file="XPathParser.cs" company="Microsoft">
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

namespace System.Xml.XPath
{
    // Howard
    // Changed from internal to public
    // Once the Query is added, it will become internal again.
    internal class XPathParser
    {
        #region Types

        private sealed class ParamInfo
        {
            #region Properties

            public Function.FunctionType FType { get; }
            public int Minargs { get; }
            public int Maxargs { get; }
            public XPathResultType[] ArgTypes { get; }

            #endregion

            #region Constructors

            internal ParamInfo(Function.FunctionType ftype, int minargs, int maxargs, XPathResultType[] argTypes)
            {
                FType = ftype;
                Minargs = minargs;
                Maxargs = maxargs;
                ArgTypes = argTypes;
            }

            #endregion
        } //ParamInfo

        #endregion

        #region Static

        // ----------------------------------------------------------------
        private static readonly XPathResultType[] _Temparray1 = { XPathResultType.Error };
        private static readonly XPathResultType[] _Temparray2 = { XPathResultType.NodeSet };
        private static readonly XPathResultType[] _Temparray3 = { XPathResultType.Any };
        private static readonly XPathResultType[] _Temparray4 = { XPathResultType.String };
        private static readonly XPathResultType[] _Temparray5 = { XPathResultType.String, XPathResultType.String };

        private static readonly XPathResultType[] _Temparray6 =
        {
            XPathResultType.String, XPathResultType.Number,
            XPathResultType.Number
        };

        private static readonly XPathResultType[] _Temparray7 =
        {
            XPathResultType.String, XPathResultType.String,
            XPathResultType.String
        };

        private static readonly XPathResultType[] _Temparray8 = { XPathResultType.Boolean };
        private static readonly XPathResultType[] _Temparray9 = { XPathResultType.Number };

        private static readonly Hashtable _FunctionTable = CreateFunctionTable();

        private static readonly Hashtable _AxesTable = CreateAxesTable();

        #endregion

        #region Fields

        private readonly XPathScanner _Scanner;

        #endregion

        #region Constructors

        private XPathParser(XPathScanner scanner) { _Scanner = scanner; }

        #endregion

        #region Methods

        /// <exception cref="XPathException"><see cref="XPathScanner.SourceText"/>  has an invalid token</exception>
        public static AstNode ParseXPathExpresion(string XpathExpresion)
        {
            var scanner = new XPathScanner(XpathExpresion);
            var parser = new XPathParser(scanner);
            var result = parser.ParseExpresion(null);
            if(scanner.Kind != XPathScanner.LexKind.Eof)
                throw new XPathException($"'{scanner.SourceText}' has an invalid token.");
            return result;
        }

        /// <exception cref="XPathException"><see cref="XPathScanner.SourceText"/>  has an invalid token</exception>
        public static AstNode ParseXPathPattern(string XpathPattern)
        {
            var scanner = new XPathScanner(XpathPattern);
            var parser = new XPathParser(scanner);
            var result = parser.ParsePattern(null);
            if(scanner.Kind != XPathScanner.LexKind.Eof)
                throw new XPathException($"'{scanner.SourceText}' has an invalid token.");
            return result;
        }

        // --------------- Expresion Parsing ----------------------

        private AstNode ParseExpresion(AstNode QyInput) => ParseOrExpr(QyInput);

        //>> OrExpr ::= ( OrExpr 'or' )? AndExpr 
        private AstNode ParseOrExpr(AstNode QyInput)
        {
            var opnd = ParseAndExpr(QyInput);

            do
            {
                if(!TestOp("or")) return opnd;
                NextLex();
                opnd = new Operator(Operator.Op.Or, opnd, ParseAndExpr(QyInput));
            } while(true);
        }

        //>> AndExpr ::= ( AndExpr 'and' )? EqualityExpr 
        private AstNode ParseAndExpr(AstNode QyInput)
        {
            var opnd = ParseEqualityExpr(QyInput);

            do
            {
                if(!TestOp("and")) return opnd;
                NextLex();
                opnd = new Operator(Operator.Op.And, opnd, ParseEqualityExpr(QyInput));
            } while(true);
        }

        //>> EqualityOp ::= '=' | '!='
        //>> EqualityExpr    ::= ( EqualityExpr EqualityOp )? RelationalExpr
        private AstNode ParseEqualityExpr(AstNode QyInput)
        {
            var opnd = ParseRelationalExpr(QyInput);

            do
            {
                var op = _Scanner.Kind == XPathScanner.LexKind.Eq
                    ? Operator.Op.Eq
                    : _Scanner.Kind == XPathScanner.LexKind.Ne
                        ? Operator.Op.Ne
                        : /*default :*/
                          Operator.Op.Invalid;
                if(op == Operator.Op.Invalid) return opnd;
                NextLex();
                opnd = new Operator(op, opnd, ParseRelationalExpr(QyInput));
            } while(true);
        }

        //>> RelationalOp ::= '<' | '>' | '<=' | '>='
        //>> RelationalExpr    ::= ( RelationalExpr RelationalOp )? AdditiveExpr  
        private AstNode ParseRelationalExpr(AstNode QyInput)
        {
            var opnd = ParseAdditiveExpr(QyInput);

            do
            {
                var op = _Scanner.Kind == XPathScanner.LexKind.Lt
                    ? Operator.Op.Lt
                    : _Scanner.Kind == XPathScanner.LexKind.Le
                        ? Operator.Op.Le
                        : _Scanner.Kind == XPathScanner.LexKind.Gt
                            ? Operator.Op.Gt
                            : _Scanner.Kind == XPathScanner.LexKind.Ge
                                ? Operator.Op.Ge
                                : /*default :*/
                                  Operator.Op.Invalid;
                if(op == Operator.Op.Invalid) return opnd;
                NextLex();
                opnd = new Operator(op, opnd, ParseAdditiveExpr(QyInput));
            } while(true);
        }

        //>> AdditiveOp   ::= '+' | '-'
        //>> AdditiveExpr ::= ( AdditiveExpr AdditiveOp )? MultiplicativeExpr
        private AstNode ParseAdditiveExpr(AstNode QyInput)
        {
            var opnd = ParseMultiplicativeExpr(QyInput);

            do
            {
                var op = _Scanner.Kind == XPathScanner.LexKind.Plus
                    ? Operator.Op.Plus
                    : _Scanner.Kind == XPathScanner.LexKind.Minus
                        ? Operator.Op.Minus
                        : /*default :*/
                          Operator.Op.Invalid;
                if(op == Operator.Op.Invalid) return opnd;
                NextLex();
                opnd = new Operator(op, opnd, ParseMultiplicativeExpr(QyInput));
            } while(true);
        }

        //>> MultiplicativeOp   ::= '*' | 'div' | 'mod'
        //>> MultiplicativeExpr ::= ( MultiplicativeExpr MultiplicativeOp )? UnaryExpr
        private AstNode ParseMultiplicativeExpr(AstNode QyInput)
        {
            var opnd = ParseUnaryExpr(QyInput);

            do
            {
                var op = _Scanner.Kind == XPathScanner.LexKind.Star
                    ? Operator.Op.Mul
                    : TestOp("div")
                        ? Operator.Op.Div
                        : TestOp("mod")
                            ? Operator.Op.Mod
                            : /*default :*/
                              Operator.Op.Invalid;
                if(op == Operator.Op.Invalid) return opnd;
                NextLex();
                opnd = new Operator(op, opnd, ParseUnaryExpr(QyInput));
            } while(true);
        }

        //>> UnaryExpr    ::= UnionExpr | '-' UnaryExpr
        private AstNode ParseUnaryExpr(AstNode QyInput)
        {
            if(_Scanner.Kind != XPathScanner.LexKind.Minus) return ParseUnionExpr(QyInput);
            NextLex();
            return new Operator(Operator.Op.Negate, ParseUnaryExpr(QyInput), null);
        }

        //>> UnionExpr ::= ( UnionExpr '|' )? PathExpr  
        private AstNode ParseUnionExpr(AstNode QyInput)
        {
            var opnd = ParsePathExpr(QyInput);

            do
            {
                if(_Scanner.Kind != XPathScanner.LexKind.Union) return opnd;
                NextLex();
                var opnd2 = ParsePathExpr(QyInput);
                CheckNodeSet(opnd.ReturnType);
                CheckNodeSet(opnd2.ReturnType);
                opnd = new Operator(Operator.Op.Union, opnd, opnd2);
            } while(true);
        }

        private static bool IsNodeType(XPathScanner scaner) => scaner.Prefix.Length == 0 && (
            scaner.Name == "node" ||
            scaner.Name == "text" ||
            scaner.Name == "processing-instruction" ||
            scaner.Name == "comment"
            );

        //>> PathOp   ::= '/' | '//'
        //>> PathExpr ::= LocationPath | 
        //>>              FilterExpr ( PathOp  RelativeLocationPath )?
        private AstNode ParsePathExpr(AstNode QyInput)
        {
            AstNode opnd;
            if(!IsPrimaryExpr(_Scanner)) opnd = ParseLocationPath(null);
            else
            {
                // in this moment we shoud distinct LocationPas vs FilterExpr (which starts from is PrimaryExpr)
                opnd = ParseFilterExpr(QyInput);
                switch(_Scanner.Kind)
                {
                    case XPathScanner.LexKind.Slash:
                        NextLex();
                        opnd = ParseRelativeLocationPath(opnd);
                        break;
                    case XPathScanner.LexKind.SlashSlash:
                        NextLex();
                        opnd = ParseRelativeLocationPath(new Axis(Axis.AxisType.DescendantOrSelf, opnd));
                        break;
                }
            }

            return opnd;
        }

        //>> FilterExpr ::= PrimaryExpr | FilterExpr Predicate 
        private AstNode ParseFilterExpr(AstNode QyInput)
        {
            var opnd = ParsePrimaryExpr(QyInput);
            while(_Scanner.Kind == XPathScanner.LexKind.LBracket)
                opnd = new Filter(opnd, ParsePredicate(opnd)); // opnd must be a query
            return opnd;
        }

        //>> Predicate ::= '[' Expr ']'
        private AstNode ParsePredicate(AstNode QyInput)
        {
            // we have predicates. Check that input type is NodeSet
            CheckNodeSet(QyInput.ReturnType);

            PassToken(XPathScanner.LexKind.LBracket);
            var opnd = ParseExpresion(QyInput);
            PassToken(XPathScanner.LexKind.RBracket);

            return opnd;
        }

        //>> LocationPath ::= RelativeLocationPath | AbsoluteLocationPath
        private AstNode ParseLocationPath(AstNode QyInput)
        {
            if(_Scanner.Kind == XPathScanner.LexKind.Slash)
            {
                NextLex();
                AstNode opnd = new Root();
                return IsStep(_Scanner.Kind) ? ParseRelativeLocationPath(opnd) : opnd;
            }
            if(_Scanner.Kind != XPathScanner.LexKind.SlashSlash) return ParseRelativeLocationPath(QyInput);
            NextLex();
            return ParseRelativeLocationPath(new Axis(Axis.AxisType.DescendantOrSelf, new Root()));
        } // ParseLocationPath

        //>> PathOp   ::= '/' | '//'
        //>> RelativeLocationPath ::= ( RelativeLocationPath PathOp )? Step 
        private AstNode ParseRelativeLocationPath(AstNode QyInput)
        {
            var opnd = ParseStep(QyInput);
            switch(_Scanner.Kind)
            {
                case XPathScanner.LexKind.SlashSlash:
                    NextLex();
                    opnd = ParseRelativeLocationPath(new Axis(Axis.AxisType.DescendantOrSelf, opnd));
                    break;
                case XPathScanner.LexKind.Slash:
                    NextLex();
                    opnd = ParseRelativeLocationPath(opnd);
                    break;
            }
            return opnd;
        }

        private static bool IsStep(XPathScanner.LexKind LexKind) => LexKind == XPathScanner.LexKind.Dot ||
                                                                    LexKind == XPathScanner.LexKind.DotDot ||
                                                                    LexKind == XPathScanner.LexKind.At ||
                                                                    LexKind == XPathScanner.LexKind.Axe ||
                                                                    LexKind == XPathScanner.LexKind.Star ||
                                                                    LexKind == XPathScanner.LexKind.Name;

        //>> Step ::= '.' | '..' | ( AxisName '::' | '@' )? NodeTest Predicate*
        private AstNode ParseStep(AstNode QyInput)
        {
            AstNode opnd;
            switch(_Scanner.Kind)
            {
                case XPathScanner.LexKind.Dot:
                    //>> '.'
                    NextLex();
                    opnd = new Axis(Axis.AxisType.Self, QyInput);
                    break;
                case XPathScanner.LexKind.DotDot:
                    //>> '..'
                    NextLex();
                    opnd = new Axis(Axis.AxisType.Parent, QyInput);
                    break;
                default:
                    //>> ( AxisName '::' | '@' )? NodeTest Predicate*
                    var lv_AxisType = Axis.AxisType.Child;
                    switch(_Scanner.Kind)
                    {
                        case XPathScanner.LexKind.At: //>> '@'
                            lv_AxisType = Axis.AxisType.Attribute;
                            NextLex();
                            break;
                        case XPathScanner.LexKind.Axe: //>> AxisName '::'
                            lv_AxisType = GetAxis(_Scanner);
                            NextLex();
                            break;
                    }
                    var lv_NodeType = lv_AxisType == Axis.AxisType.Attribute
                        ? XPathNodeType.Attribute
                        :
                          // axisType == Axis.AxisType.Namespace ? XPathNodeType.Namespace : // No Idea why it's this way but othervise Axes doesn't work
                          /* default: */
                          XPathNodeType.Element;

                    opnd = ParseNodeTest(QyInput, lv_AxisType, lv_NodeType);

                    while(XPathScanner.LexKind.LBracket == _Scanner.Kind)
                        opnd = new Filter(opnd, ParsePredicate(opnd));
                    break;
            }
            return opnd;
        }

        //>> NodeTest ::= NameTest | 'comment ()' | 'text ()' | 'node ()' | 'processing-instruction ('  Literal ? ')'
        private AstNode ParseNodeTest(AstNode QyInput, Axis.AxisType AxisType, XPathNodeType NodeType)
        {
            string lv_NodeName, lv_NodePrefix;

            switch(_Scanner.Kind)
            {
                case XPathScanner.LexKind.Name:
                    if(_Scanner.CanBeFunction && IsNodeType(_Scanner))
                    {
                        lv_NodePrefix = string.Empty;
                        lv_NodeName = string.Empty;
                        NodeType = _Scanner.Name == "comment"
                            ? XPathNodeType.Comment
                            : _Scanner.Name == "text"
                                ? XPathNodeType.Text
                                : _Scanner.Name == "node"
                                    ? XPathNodeType.All
                                    : _Scanner.Name == "processing-instruction"
                                        ? XPathNodeType.ProcessingInstruction
                                        : /* default: */ XPathNodeType.Root;
                        Debug.Assert(NodeType != XPathNodeType.Root);
                        NextLex();

                        PassToken(XPathScanner.LexKind.LParens);

                        if(NodeType == XPathNodeType.ProcessingInstruction)
                            if(_Scanner.Kind != XPathScanner.LexKind.RParens)
                            {
                                //>> 'processing-instruction (' Literal ')'
                                CheckToken(XPathScanner.LexKind.String);
                                lv_NodeName = _Scanner.StringValue;
                                NextLex();
                            }

                        PassToken(XPathScanner.LexKind.RParens);
                    }
                    else
                    {
                        lv_NodePrefix = _Scanner.Prefix;
                        lv_NodeName = _Scanner.Name;
                        NextLex();
                        if(lv_NodeName == "*") lv_NodeName = string.Empty;
                    }
                    break;
                case XPathScanner.LexKind.Star:
                    lv_NodePrefix = string.Empty;
                    lv_NodeName = string.Empty;
                    NextLex();
                    break;
                default:
                    throw new XPathException($"Expression {_Scanner.SourceText} must evaluate to a node-set.");
            }
            return new Axis(AxisType, QyInput, lv_NodePrefix, lv_NodeName, NodeType);
        }

        private static bool IsPrimaryExpr(XPathScanner scanner) => scanner.Kind == XPathScanner.LexKind.String ||
                                                                   scanner.Kind == XPathScanner.LexKind.Number ||
                                                                   scanner.Kind == XPathScanner.LexKind.Dollar ||
                                                                   scanner.Kind == XPathScanner.LexKind.LParens ||
                                                                   scanner.Kind == XPathScanner.LexKind.Name &&
                                                                   scanner.CanBeFunction && !IsNodeType(scanner);

        //>> PrimaryExpr ::= Literal | Number | VariableReference | '(' Expr ')' | FunctionCall
        private AstNode ParsePrimaryExpr(AstNode QyInput)
        {
            Debug.Assert(IsPrimaryExpr(_Scanner));
            AstNode opnd = null;
            switch(_Scanner.Kind)
            {
                case XPathScanner.LexKind.String:
                    opnd = new Operand(_Scanner.StringValue);
                    NextLex();
                    break;
                case XPathScanner.LexKind.Number:
                    opnd = new Operand(_Scanner.NumberValue);
                    NextLex();
                    break;
                case XPathScanner.LexKind.Dollar:
                    NextLex();
                    CheckToken(XPathScanner.LexKind.Name);
                    opnd = new Variable(_Scanner.Name, _Scanner.Prefix);
                    NextLex();
                    break;
                case XPathScanner.LexKind.LParens:
                    NextLex();
                    opnd = ParseExpresion(QyInput);
                    if(opnd.TypeOfAst != AstNode.QueryType.ConstantOperand) opnd = new Group(opnd);
                    PassToken(XPathScanner.LexKind.RParens);
                    break;
                case XPathScanner.LexKind.Name:
                    if(_Scanner.CanBeFunction && !IsNodeType(_Scanner)) opnd = ParseMethod(null);
                    break;
            }
            Debug.Assert(opnd != null, "IsPrimaryExpr() was true. We should recognize this lex.");
            return opnd;
        }

        private AstNode ParseMethod(AstNode QyInput)
        {
            var lv_ArgList = new ArrayList();
            var name = _Scanner.Name;
            var prefix = _Scanner.Prefix;
            PassToken(XPathScanner.LexKind.Name);
            PassToken(XPathScanner.LexKind.LParens);
            if(_Scanner.Kind != XPathScanner.LexKind.RParens)
            {
                do
                {
                    lv_ArgList.Add(ParseExpresion(QyInput));
                    if(_Scanner.Kind == XPathScanner.LexKind.RParens) break;
                    PassToken(XPathScanner.LexKind.Comma);
                } while(true);
            }
            PassToken(XPathScanner.LexKind.RParens);
            if(prefix != string.Empty) return new Function(prefix, name, lv_ArgList);
            var pi = (ParamInfo)_FunctionTable[name];
            if(pi == null) return new Function(prefix, name, lv_ArgList);
            var lv_ArgCount = lv_ArgList.Count;
            if(lv_ArgCount < pi.Minargs)
                throw new XPathException($"Function '{name}' in '{_Scanner.SourceText}' has invalid number of arguments.");
            if(pi.FType == Function.FunctionType.FuncConcat)
            {
                for(var i = 0; i < lv_ArgCount; i++)
                {
                    var arg = (AstNode)lv_ArgList[i];
                    if(arg.ReturnType != XPathResultType.String)
                        arg = new Function(Function.FunctionType.FuncString, arg);
                    lv_ArgList[i] = arg;
                }
            }
            else
            {
                if(pi.Maxargs < lv_ArgCount)
                    throw new XPathException($"Function '{name}' in '{_Scanner.SourceText}' has invalid number of arguments.");
                if(pi.ArgTypes.Length < lv_ArgCount)
                    lv_ArgCount = pi.ArgTypes.Length; // argument we have the type specified (can be < pi.Minargs)
                for(var i = 0; i < lv_ArgCount; i++)
                {
                    var arg = (AstNode)lv_ArgList[i];
                    if(pi.ArgTypes[i] == XPathResultType.Any || pi.ArgTypes[i] == arg.ReturnType) continue;
                    switch(pi.ArgTypes[i])
                    {
                        case XPathResultType.NodeSet:
                            if(!(arg is Variable) &&
                                !(arg is Function && arg.ReturnType == XPathResultType.Error))
                                throw new XPathException(
                                    $"Function '{name}' in '{_Scanner.SourceText}' has invalid number of arguments.");
                            break;
                        case XPathResultType.String:
                            arg = new Function(Function.FunctionType.FuncString, arg);
                            break;
                        case XPathResultType.Number:
                            arg = new Function(Function.FunctionType.FuncNumber, arg);
                            break;
                        case XPathResultType.Boolean:
                            arg = new Function(Function.FunctionType.FuncBoolean, arg);
                            break;
                    }
                    lv_ArgList[i] = arg;
                }
            }
            return new Function(pi.FType, lv_ArgList);
        }

        // --------------- Pattern Parsing ----------------------

        //>> Pattern ::= ( Pattern '|' )? LocationPathPattern
        private AstNode ParsePattern(AstNode QyInput)
        {
            var opnd = ParseLocationPathPattern(QyInput);

            do
            {
                if(_Scanner.Kind != XPathScanner.LexKind.Union) return opnd;
                NextLex();
                opnd = new Operator(Operator.Op.Union, opnd, ParseLocationPathPattern(QyInput));
            } while(true);
        }

        //>> LocationPathPattern ::= '/' | RelativePathPattern | '//' RelativePathPattern  |  '/' RelativePathPattern
        //>>                       | IdKeyPattern (('/' | '//') RelativePathPattern)?  
        private AstNode ParseLocationPathPattern(AstNode QyInput)
        {
            AstNode opnd = null;
            switch(_Scanner.Kind)
            {
                case XPathScanner.LexKind.Slash:
                    NextLex();
                    opnd = new Root();
                    if(_Scanner.Kind == XPathScanner.LexKind.Eof || _Scanner.Kind == XPathScanner.LexKind.Union)
                        return opnd;
                    break;
                case XPathScanner.LexKind.SlashSlash:
                    NextLex();
                    opnd = new Axis(Axis.AxisType.DescendantOrSelf, new Root());
                    break;
                case XPathScanner.LexKind.Name:
                    if(_Scanner.CanBeFunction)
                        if((opnd = ParseIdKeyPattern(QyInput)) != null)
                            switch(_Scanner.Kind)
                            {
                                case XPathScanner.LexKind.Slash:
                                    NextLex();
                                    break;
                                case XPathScanner.LexKind.SlashSlash:
                                    NextLex();
                                    opnd = new Axis(Axis.AxisType.DescendantOrSelf, opnd);
                                    break;
                                default:
                                    return opnd;
                            }
                    break;
            }
            return ParseRelativePathPattern(opnd);
        }

        //>> IdKeyPattern ::= 'id' '(' Literal ')' | 'key' '(' Literal ',' Literal ')'  
        private AstNode ParseIdKeyPattern(AstNode QyInput)
        {
            Debug.Assert(_Scanner.CanBeFunction);
            var lv_ArgList = new ArrayList();
            if(_Scanner.Prefix.Length != 0) return null;
            switch(_Scanner.Name)
            {
                case "id":
                    var pi = (ParamInfo)_FunctionTable["id"];
                    NextLex();
                    PassToken(XPathScanner.LexKind.LParens);
                    CheckToken(XPathScanner.LexKind.String);
                    lv_ArgList.Add(new Operand(_Scanner.StringValue));
                    NextLex();
                    PassToken(XPathScanner.LexKind.RParens);
                    return new Function(pi.FType, lv_ArgList);
                case "key":
                    NextLex();
                    PassToken(XPathScanner.LexKind.LParens);
                    CheckToken(XPathScanner.LexKind.String);
                    lv_ArgList.Add(new Operand(_Scanner.StringValue));
                    NextLex();
                    PassToken(XPathScanner.LexKind.Comma);
                    CheckToken(XPathScanner.LexKind.String);
                    lv_ArgList.Add(new Operand(_Scanner.StringValue));
                    NextLex();
                    PassToken(XPathScanner.LexKind.RParens);
                    return new Function("", "key", lv_ArgList);
            }
            return null;
        }

        //>> PathOp   ::= '/' | '//'
        //>> RelativePathPattern ::= ( RelativePathPattern PathOp )? StepPattern
        private AstNode ParseRelativePathPattern(AstNode QyInput)
        {
            var opnd = ParseStepPattern(QyInput);
            switch(_Scanner.Kind)
            {
                case XPathScanner.LexKind.SlashSlash:
                    NextLex();
                    opnd = ParseRelativePathPattern(new Axis(Axis.AxisType.DescendantOrSelf, opnd));
                    break;
                case XPathScanner.LexKind.Slash:
                    NextLex();
                    opnd = ParseRelativePathPattern(opnd);
                    break;
            }
            return opnd;
        }

        //>> StepPattern    ::=    ChildOrAttributeAxisSpecifier NodeTest Predicate*   
        //>> ChildOrAttributeAxisSpecifier    ::=    @ ? | ('child' | 'attribute') '::' 
        private AstNode ParseStepPattern(AstNode QyInput)
        {
            var lv_AxisType = Axis.AxisType.Child;
            switch(_Scanner.Kind)
            {
                case XPathScanner.LexKind.At: //>> '@'
                    lv_AxisType = Axis.AxisType.Attribute;
                    NextLex();
                    break;
                case XPathScanner.LexKind.Axe: //>> AxisName '::'
                    lv_AxisType = GetAxis(_Scanner);
                    if(lv_AxisType != Axis.AxisType.Child && lv_AxisType != Axis.AxisType.Attribute)
                        throw new XPathException($"'{_Scanner.SourceText}' has an invalid token.");
                    NextLex();
                    break;
            }
            var lv_NodeType = lv_AxisType == Axis.AxisType.Attribute
                ? XPathNodeType.Attribute
                : /* default: */
                  XPathNodeType.Element;

            var opnd = ParseNodeTest(QyInput, lv_AxisType, lv_NodeType);

            while(XPathScanner.LexKind.LBracket == _Scanner.Kind)
                opnd = new Filter(opnd, ParsePredicate(opnd));
            return opnd;
        }


        // --------------- Helper methods ----------------------

        private void CheckToken(XPathScanner.LexKind t)
        {
            if(_Scanner.Kind != t)
                throw new XPathException($"'{_Scanner.SourceText}' has an invalid token.");
        }

        private void PassToken(XPathScanner.LexKind t)
        {
            CheckToken(t);
            NextLex();
        }

        private void NextLex(XPathScanner.LexKind t)
        {
            Debug.Assert(_Scanner.Kind == t);
            NextLex();
        }

        private void NextLex() => _Scanner.NextLex();

        private bool TestOp(string op) => _Scanner.Kind == XPathScanner.LexKind.Name &&
                                          _Scanner.Prefix.Length == 0 &&
                                          _Scanner.Name.Equals(op);

        private void CheckNodeSet(XPathResultType t)
        {
            if(t != XPathResultType.NodeSet && t != XPathResultType.Error)
                throw new XPathException($"Expression {_Scanner.SourceText} must evaluate to a node-set");
        }

        private static Hashtable CreateFunctionTable()
        {
            var table = new Hashtable(36)
            {
                {"last", new ParamInfo(Function.FunctionType.FuncLast, 0, 0, _Temparray1)},
                {"position", new ParamInfo(Function.FunctionType.FuncPosition, 0, 0, _Temparray1)},
                {"name", new ParamInfo(Function.FunctionType.FuncName, 0, 1, _Temparray2)},
                {"namespace-uri", new ParamInfo(Function.FunctionType.FuncNameSpaceUri, 0, 1, _Temparray2)},
                {"local-name", new ParamInfo(Function.FunctionType.FuncLocalName, 0, 1, _Temparray2)},
                {"count", new ParamInfo(Function.FunctionType.FuncCount, 1, 1, _Temparray2)},
                {"id", new ParamInfo(Function.FunctionType.FuncID, 1, 1, _Temparray3)},
                {"string", new ParamInfo(Function.FunctionType.FuncString, 0, 1, _Temparray3)},
                {"concat", new ParamInfo(Function.FunctionType.FuncConcat, 2, 100, _Temparray4)},
                {"starts-with", new ParamInfo(Function.FunctionType.FuncStartsWith, 2, 2, _Temparray5)},
                {"contains", new ParamInfo(Function.FunctionType.FuncContains, 2, 2, _Temparray5)},
                {"substring-before", new ParamInfo(Function.FunctionType.FuncSubstringBefore, 2, 2, _Temparray5)},
                {"substring-after", new ParamInfo(Function.FunctionType.FuncSubstringAfter, 2, 2, _Temparray5)},
                {"substring", new ParamInfo(Function.FunctionType.FuncSubstring, 2, 3, _Temparray6)},
                {"string-length", new ParamInfo(Function.FunctionType.FuncStringLength, 0, 1, _Temparray4)},
                {"normalize-space", new ParamInfo(Function.FunctionType.FuncNormalize, 0, 1, _Temparray4)},
                {"translate", new ParamInfo(Function.FunctionType.FuncTranslate, 3, 3, _Temparray7)},
                {"boolean", new ParamInfo(Function.FunctionType.FuncBoolean, 1, 1, _Temparray3)},
                {"not", new ParamInfo(Function.FunctionType.FuncNot, 1, 1, _Temparray8)},
                {"true", new ParamInfo(Function.FunctionType.FuncTrue, 0, 0, _Temparray8)},
                {"false", new ParamInfo(Function.FunctionType.FuncFalse, 0, 0, _Temparray8)},
                {"lang", new ParamInfo(Function.FunctionType.FuncLang, 1, 1, _Temparray4)},
                {"number", new ParamInfo(Function.FunctionType.FuncNumber, 0, 1, _Temparray3)},
                {"sum", new ParamInfo(Function.FunctionType.FuncSum, 1, 1, _Temparray2)},
                {"floor", new ParamInfo(Function.FunctionType.FuncFloor, 1, 1, _Temparray9)},
                {"ceiling", new ParamInfo(Function.FunctionType.FuncCeiling, 1, 1, _Temparray9)},
                {"round", new ParamInfo(Function.FunctionType.FuncRound, 1, 1, _Temparray9)}
            };
            return table;
        }

        private static Hashtable CreateAxesTable()
        {
            var table = new Hashtable(13)
            {
                {"ancestor", Axis.AxisType.Ancestor},
                {"ancestor-or-self", Axis.AxisType.AncestorOrSelf},
                {"attribute", Axis.AxisType.Attribute},
                {"child", Axis.AxisType.Child},
                {"descendant", Axis.AxisType.Descendant},
                {"descendant-or-self", Axis.AxisType.DescendantOrSelf},
                {"following", Axis.AxisType.Following},
                {"following-sibling", Axis.AxisType.FollowingSibling},
                {"namespace", Axis.AxisType.Namespace},
                {"parent", Axis.AxisType.Parent},
                {"preceding", Axis.AxisType.Preceding},
                {"preceding-sibling", Axis.AxisType.PrecedingSibling},
                {"self", Axis.AxisType.Self}
            };
            return table;
        }

        private Axis.AxisType GetAxis(XPathScanner scaner)
        {
            Debug.Assert(scaner.Kind == XPathScanner.LexKind.Axe);
            var axis = _AxesTable[scaner.Name];
            if(axis == null) throw new XPathException($"'{_Scanner.SourceText}' has an invalid token.");
            return (Axis.AxisType)axis;
        }

        #endregion
    }
}