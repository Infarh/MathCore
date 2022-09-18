#nullable enable
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

// ReSharper disable once CheckNamespace
namespace System.Xml.XPath;

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
        public int MinArgs { get; }
        public int MaxArgs { get; }
        public XPathResultType[] ArgTypes { get; }

        #endregion

        #region Constructors

        internal ParamInfo(Function.FunctionType FType, int MinArgs, int MaxArgs, XPathResultType[] ArgTypes)
        {
            this.FType    = FType;
            this.MinArgs  = MinArgs;
            this.MaxArgs  = MaxArgs;
            this.ArgTypes = ArgTypes;
        }

        #endregion
    } //ParamInfo

    #endregion

    #region Static

    // ----------------------------------------------------------------
    private static readonly XPathResultType[] __TempArray1 = { XPathResultType.Error };
    private static readonly XPathResultType[] __TempArray2 = { XPathResultType.NodeSet };
    private static readonly XPathResultType[] __TempArray3 = { XPathResultType.Any };
    private static readonly XPathResultType[] __TempArray4 = { XPathResultType.String };
    private static readonly XPathResultType[] __TempArray5 = { XPathResultType.String, XPathResultType.String };

    private static readonly XPathResultType[] __TempArray6 =
    {
        XPathResultType.String, XPathResultType.Number,
        XPathResultType.Number
    };

    private static readonly XPathResultType[] __TempArray7 =
    {
        XPathResultType.String, XPathResultType.String,
        XPathResultType.String
    };

    private static readonly XPathResultType[] __TempArray8 = { XPathResultType.Boolean };
    private static readonly XPathResultType[] __TempArray9 = { XPathResultType.Number };

    private static readonly Hashtable __FunctionTable = CreateFunctionTable();

    private static readonly Hashtable __AxesTable = CreateAxesTable();

    #endregion

    #region Fields

    private readonly XPathScanner _Scanner;

    #endregion

    #region Constructors

    private XPathParser(XPathScanner scanner) => _Scanner = scanner;

    #endregion

    #region Methods

    /// <exception cref="XPathException"><see cref="XPathScanner.SourceText"/>  has an invalid token</exception>
    public static AstNode ParseXPathExpression(string XPathExpression)
    {
        var scanner = new XPathScanner(XPathExpression);
        var parser  = new XPathParser(scanner);
        var result  = parser.ParseExpression(null);
        if (scanner.Kind != XPathScanner.LexKind.Eof)
            throw new XPathException($"'{scanner.SourceText}' has an invalid token.");
        return result;
    }

    /// <exception cref="XPathException"><see cref="XPathScanner.SourceText"/>  has an invalid token</exception>
    public static AstNode ParseXPathPattern(string XpathPattern)
    {
        var scanner = new XPathScanner(XpathPattern);
        var parser  = new XPathParser(scanner);
        var result  = parser.ParsePattern();
        if (scanner.Kind != XPathScanner.LexKind.Eof)
            throw new XPathException($"'{scanner.SourceText}' has an invalid token.");
        return result;
    }

    // --------------- Expression Parsing ----------------------

    private AstNode ParseExpression(AstNode? QyInput) => ParseOrExpr(QyInput);

    //>> OrExpr ::= ( OrExpr 'or' )? AndExpr 
    private AstNode ParseOrExpr(AstNode? QyInput)
    {
        var operand = ParseAndExpr(QyInput);

        do
        {
            if (!TestOp("or")) return operand;
            NextLex();
            operand = new Operator(Operator.Op.Or, operand, ParseAndExpr(QyInput));
        } while (true);
    }

    //>> AndExpr ::= ( AndExpr 'and' )? EqualityExpr 
    private AstNode ParseAndExpr(AstNode? QyInput)
    {
        var operand = ParseEqualityExpr(QyInput);

        do
        {
            if (!TestOp("and")) return operand;
            NextLex();
            operand = new Operator(Operator.Op.And, operand, ParseEqualityExpr(QyInput));
        } while (true);
    }

    //>> EqualityOp ::= '=' | '!='
    //>> EqualityExpr    ::= ( EqualityExpr EqualityOp )? RelationalExpr
    private AstNode ParseEqualityExpr(AstNode? QyInput)
    {
        var operand = ParseRelationalExpr(QyInput);

        do
        {
            var op = _Scanner.Kind == XPathScanner.LexKind.Eq
                ? Operator.Op.Eq
                : _Scanner.Kind == XPathScanner.LexKind.Ne
                    ? Operator.Op.Ne
                    : /*default :*/
                    Operator.Op.Invalid;
            if (op == Operator.Op.Invalid) return operand;
            NextLex();
            operand = new Operator(op, operand, ParseRelationalExpr(QyInput));
        } while (true);
    }

    //>> RelationalOp ::= '<' | '>' | '<=' | '>='
    //>> RelationalExpr    ::= ( RelationalExpr RelationalOp )? AdditiveExpr  
    private AstNode ParseRelationalExpr(AstNode? QyInput)
    {
        var operand = ParseAdditiveExpr(QyInput);

        do
        {
            var @operator = _Scanner.Kind switch
            {
                XPathScanner.LexKind.Lt => Operator.Op.Lt,
                XPathScanner.LexKind.Le => Operator.Op.Le,
                XPathScanner.LexKind.Gt => Operator.Op.Gt,
                XPathScanner.LexKind.Ge => Operator.Op.Ge,
                _                       => Operator.Op.Invalid
            };
            if (@operator == Operator.Op.Invalid) return operand;
            NextLex();
            operand = new Operator(@operator, operand, ParseAdditiveExpr(QyInput));
        } while (true);
    }

    //>> AdditiveOp   ::= '+' | '-'
    //>> AdditiveExpr ::= ( AdditiveExpr AdditiveOp )? MultiplicativeExpr
    private AstNode ParseAdditiveExpr(AstNode? QyInput)
    {
        var operand = ParseMultiplicativeExpr(QyInput);

        do
        {
            var @operator = _Scanner.Kind switch
            {
                XPathScanner.LexKind.Plus  => Operator.Op.Plus,
                XPathScanner.LexKind.Minus => Operator.Op.Minus,
                _                          => Operator.Op.Invalid
            };
            if (@operator == Operator.Op.Invalid) return operand;
            NextLex();
            operand = new Operator(@operator, operand, ParseMultiplicativeExpr(QyInput));
        } while (true);
    }

    //>> MultiplicativeOp   ::= '*' | 'div' | 'mod'
    //>> MultiplicativeExpr ::= ( MultiplicativeExpr MultiplicativeOp )? UnaryExpr
    private AstNode ParseMultiplicativeExpr(AstNode? QyInput)
    {
        var operand = ParseUnaryExpr(QyInput);

        do
        {
            var @operator = _Scanner.Kind == XPathScanner.LexKind.Star
                ? Operator.Op.Mul
                : TestOp("div")
                    ? Operator.Op.Div
                    : TestOp("mod")
                        ? Operator.Op.Mod
                        : /*default :*/
                        Operator.Op.Invalid;
            if (@operator == Operator.Op.Invalid) return operand;
            NextLex();
            operand = new Operator(@operator, operand, ParseUnaryExpr(QyInput));
        } while (true);
    }

    //>> UnaryExpr    ::= UnionExpr | '-' UnaryExpr
    private AstNode ParseUnaryExpr(AstNode? QyInput)
    {
        if (_Scanner.Kind != XPathScanner.LexKind.Minus) return ParseUnionExpr(QyInput);
        NextLex();
        return new Operator(Operator.Op.Negate, ParseUnaryExpr(QyInput), null);
    }

    //>> UnionExpr ::= ( UnionExpr '|' )? PathExpr  
    private AstNode ParseUnionExpr(AstNode? QyInput)
    {
        var operand = ParsePathExpr(QyInput);

        do
        {
            if (_Scanner.Kind != XPathScanner.LexKind.Union) return operand;
            NextLex();
            var operand2 = ParsePathExpr(QyInput);
            CheckNodeSet(operand.ReturnType);
            CheckNodeSet(operand2.ReturnType);
            operand = new Operator(Operator.Op.Union, operand, operand2);
        } while (true);
    }

    private static bool IsNodeType(XPathScanner Scanner) =>
        Scanner.Prefix.Length == 0
        && Scanner.Name is "node" or "text" or "processing-instruction" or "comment";

    //>> PathOp   ::= '/' | '//'
    //>> PathExpr ::= LocationPath | 
    //>>              FilterExpr ( PathOp  RelativeLocationPath )?
    private AstNode ParsePathExpr(AstNode? QyInput)
    {
        AstNode operand;
        if (!IsPrimaryExpr(_Scanner)) operand = ParseLocationPath(null);
        else
        {
            // in this moment we should distinct LocationPas vs FilterExpr (which starts from is PrimaryExpr)
            operand = ParseFilterExpr(QyInput);
            switch (_Scanner.Kind)
            {
                case XPathScanner.LexKind.Slash:
                    NextLex();
                    operand = ParseRelativeLocationPath(operand);
                    break;
                case XPathScanner.LexKind.SlashSlash:
                    NextLex();
                    operand = ParseRelativeLocationPath(new Axis(Axis.AxisType.DescendantOrSelf, operand));
                    break;
            }
        }

        return operand;
    }

    //>> FilterExpr ::= PrimaryExpr | FilterExpr Predicate 
    private AstNode ParseFilterExpr(AstNode? QyInput)
    {
        var operand = ParsePrimaryExpr(QyInput);
        while (_Scanner.Kind == XPathScanner.LexKind.LBracket)
            operand = new Filter(operand, ParsePredicate(operand)); // operand must be a query
        return operand;
    }

    //>> Predicate ::= '[' Expr ']'
    private AstNode ParsePredicate(AstNode QyInput)
    {
        // we have predicates. Check that input type is NodeSet
        CheckNodeSet(QyInput.ReturnType);

        PassToken(XPathScanner.LexKind.LBracket);
        var operand = ParseExpression(QyInput);
        PassToken(XPathScanner.LexKind.RBracket);

        return operand;
    }

    //>> LocationPath ::= RelativeLocationPath | AbsoluteLocationPath
    private AstNode ParseLocationPath(AstNode? QyInput)
    {
        if (_Scanner.Kind == XPathScanner.LexKind.Slash)
        {
            NextLex();
            AstNode operand = new Root();
            return IsStep(_Scanner.Kind) ? ParseRelativeLocationPath(operand) : operand;
        }
        if (_Scanner.Kind != XPathScanner.LexKind.SlashSlash) return ParseRelativeLocationPath(QyInput);
        NextLex();
        return ParseRelativeLocationPath(new Axis(Axis.AxisType.DescendantOrSelf, new Root()));
    } // ParseLocationPath

    //>> PathOp   ::= '/' | '//'
    //>> RelativeLocationPath ::= ( RelativeLocationPath PathOp )? Step 
    private AstNode ParseRelativeLocationPath(AstNode? QyInput)
    {
        var operand = ParseStep(QyInput);
        switch (_Scanner.Kind)
        {
            case XPathScanner.LexKind.SlashSlash:
                NextLex();
                operand = ParseRelativeLocationPath(new Axis(Axis.AxisType.DescendantOrSelf, operand));
                break;
            case XPathScanner.LexKind.Slash:
                NextLex();
                operand = ParseRelativeLocationPath(operand);
                break;
        }
        return operand;
    }

    private static bool IsStep(XPathScanner.LexKind LexKind) =>
        LexKind is XPathScanner.LexKind.Dot or XPathScanner.LexKind.DotDot or XPathScanner.LexKind.At or XPathScanner.LexKind.Axe or XPathScanner.LexKind.Star or XPathScanner.LexKind.Name;

    //>> Step ::= '.' | '..' | ( AxisName '::' | '@' )? NodeTest Predicate*
    private AstNode ParseStep(AstNode? QyInput)
    {
        AstNode operand;
        switch (_Scanner.Kind)
        {
            case XPathScanner.LexKind.Dot:
                //>> '.'
                NextLex();
                operand = new Axis(Axis.AxisType.Self, QyInput);
                break;
            case XPathScanner.LexKind.DotDot:
                //>> '..'
                NextLex();
                operand = new Axis(Axis.AxisType.Parent, QyInput);
                break;
            default:
                //>> ( AxisName '::' | '@' )? NodeTest Predicate*
                var axis_type = Axis.AxisType.Child;
                switch (_Scanner.Kind)
                {
                    case XPathScanner.LexKind.At: //>> '@'
                        axis_type = Axis.AxisType.Attribute;
                        NextLex();
                        break;
                    case XPathScanner.LexKind.Axe: //>> AxisName '::'
                        axis_type = GetAxis(_Scanner);
                        NextLex();
                        break;
                }
                var node_type = axis_type == Axis.AxisType.Attribute
                    ? XPathNodeType.Attribute
                    :
                    // axisType == Axis.AxisType.Namespace ? XPathNodeType.Namespace : // No Idea why it's this way but otherwise Axes doesn't work
                    /* default: */
                    XPathNodeType.Element;

                operand = ParseNodeTest(QyInput, axis_type, node_type);

                while (XPathScanner.LexKind.LBracket == _Scanner.Kind)
                    operand = new Filter(operand, ParsePredicate(operand));
                break;
        }
        return operand;
    }

    //>> NodeTest ::= NameTest | 'comment ()' | 'text ()' | 'node ()' | 'processing-instruction ('  Literal ? ')'
    private AstNode ParseNodeTest(AstNode? QyInput, Axis.AxisType AxisType, XPathNodeType NodeType)
    {
        string node_name, node_prefix;

        switch (_Scanner.Kind)
        {
            case XPathScanner.LexKind.Name:
                if (_Scanner.CanBeFunction && IsNodeType(_Scanner))
                {
                    node_prefix = string.Empty;
                    node_name   = string.Empty;
                    NodeType = _Scanner.Name switch
                    {
                        "comment"                => XPathNodeType.Comment,
                        "text"                   => XPathNodeType.Text,
                        "node"                   => XPathNodeType.All,
                        "processing-instruction" => XPathNodeType.ProcessingInstruction,
                        _                        => XPathNodeType.Root
                    };
                    NextLex();

                    PassToken(XPathScanner.LexKind.LParens);

                    if (NodeType == XPathNodeType.ProcessingInstruction)
                        if (_Scanner.Kind != XPathScanner.LexKind.RParens)
                        {
                            //>> 'processing-instruction (' Literal ')'
                            CheckToken(XPathScanner.LexKind.String);
                            node_name = _Scanner.StringValue;
                            NextLex();
                        }

                    PassToken(XPathScanner.LexKind.RParens);
                }
                else
                {
                    node_prefix = _Scanner.Prefix;
                    node_name   = _Scanner.Name;
                    NextLex();
                    if (node_name == "*") node_name = string.Empty;
                }
                break;
            case XPathScanner.LexKind.Star:
                node_prefix = string.Empty;
                node_name   = string.Empty;
                NextLex();
                break;
            default:
                throw new XPathException($"Expression {_Scanner.SourceText} must evaluate to a node-set.");
        }
        return new Axis(AxisType, QyInput, node_prefix, node_name, NodeType);
    }

    private static bool IsPrimaryExpr(XPathScanner scanner) =>
        scanner.Kind is XPathScanner.LexKind.String or XPathScanner.LexKind.Number or XPathScanner.LexKind.Dollar or XPathScanner.LexKind.LParens 
        || scanner.Kind == XPathScanner.LexKind.Name
        && scanner.CanBeFunction && !IsNodeType(scanner);

    //>> PrimaryExpr ::= Literal | Number | VariableReference | '(' Expr ')' | FunctionCall
    private AstNode ParsePrimaryExpr(AstNode? QyInput)
    {
        Debug.Assert(IsPrimaryExpr(_Scanner));
        AstNode? operand = null;
        switch (_Scanner.Kind)
        {
            case XPathScanner.LexKind.String:
                operand = new Operand(_Scanner.StringValue);
                NextLex();
                break;
            case XPathScanner.LexKind.Number:
                operand = new Operand(_Scanner.NumberValue);
                NextLex();
                break;
            case XPathScanner.LexKind.Dollar:
                NextLex();
                CheckToken(XPathScanner.LexKind.Name);
                operand = new Variable(_Scanner.Name, _Scanner.Prefix);
                NextLex();
                break;
            case XPathScanner.LexKind.LParens:
                NextLex();
                operand = ParseExpression(QyInput);
                if (operand.TypeOfAst != AstNode.QueryType.ConstantOperand) operand = new Group(operand);
                PassToken(XPathScanner.LexKind.RParens);
                break;
            case XPathScanner.LexKind.Name:
                if (_Scanner.CanBeFunction && !IsNodeType(_Scanner)) operand = ParseMethod(null);
                break;
        }
        return operand ?? throw new InvalidOperationException("IsPrimaryExpr() was true. We should recognize this lex");
    }

    private AstNode ParseMethod(AstNode? QyInput)
    {
        var arg_list = new ArrayList();
        var name     = _Scanner.Name;
        var prefix   = _Scanner.Prefix;
        PassToken(XPathScanner.LexKind.Name);
        PassToken(XPathScanner.LexKind.LParens);
        if (_Scanner.Kind != XPathScanner.LexKind.RParens)
            do
            {
                arg_list.Add(ParseExpression(QyInput));
                if (_Scanner.Kind == XPathScanner.LexKind.RParens) break;
                PassToken(XPathScanner.LexKind.Comma);
            } while (true);

        PassToken(XPathScanner.LexKind.RParens);
        if (prefix != string.Empty) return new Function(prefix, name, arg_list);
        var pi = (ParamInfo?)__FunctionTable[name];
        if (pi is null) return new Function(prefix, name, arg_list);
        var arg_count = arg_list.Count;
        if (arg_count < pi.MinArgs)
            throw new XPathException($"Function '{name}' in '{_Scanner.SourceText}' has invalid number of arguments.");
        if (pi.FType == Function.FunctionType.FuncConcat)
            for (var i = 0; i < arg_count; i++)
            {
                var arg = (AstNode)arg_list[i];
                if (arg.ReturnType != XPathResultType.String)
                    arg = new Function(Function.FunctionType.FuncString, arg);
                arg_list[i] = arg;
            }
        else
        {
            if (pi.MaxArgs < arg_count)
                throw new XPathException($"Function '{name}' in '{_Scanner.SourceText}' has invalid number of arguments.");
            if (pi.ArgTypes.Length < arg_count)
                arg_count = pi.ArgTypes.Length; // argument we have the type specified (can be < pi.MinArgs)
            for (var i = 0; i < arg_count; i++)
            {
                var arg = (AstNode)arg_list[i];
                if (pi.ArgTypes[i] == XPathResultType.Any || pi.ArgTypes[i] == arg.ReturnType) continue;
                switch (pi.ArgTypes[i])
                {
                    case XPathResultType.NodeSet:
                        if (arg is not Variable &&
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
                arg_list[i] = arg;
            }
        }
        return new Function(pi.FType, arg_list);
    }

    // --------------- Pattern Parsing ----------------------

    //>> Pattern ::= ( Pattern '|' )? LocationPathPattern
    private AstNode ParsePattern()
    {
        var operand = ParseLocationPathPattern();

        do
        {
            if (_Scanner.Kind != XPathScanner.LexKind.Union) return operand;
            NextLex();
            operand = new Operator(Operator.Op.Union, operand, ParseLocationPathPattern());
        } while (true);
    }

    //>> LocationPathPattern ::= '/' | RelativePathPattern | '//' RelativePathPattern  |  '/' RelativePathPattern
    //>>                       | IdKeyPattern (('/' | '//') RelativePathPattern)?  
    private AstNode ParseLocationPathPattern()
    {
        AstNode? operand = null;
        switch (_Scanner.Kind)
        {
            case XPathScanner.LexKind.Slash:
                NextLex();
                operand = new Root();
                if (_Scanner.Kind is XPathScanner.LexKind.Eof or XPathScanner.LexKind.Union)
                    return operand;
                break;
            case XPathScanner.LexKind.SlashSlash:
                NextLex();
                operand = new Axis(Axis.AxisType.DescendantOrSelf, new Root());
                break;
            case XPathScanner.LexKind.Name:
                if (_Scanner.CanBeFunction)
                    if ((operand = ParseIdKeyPattern()) != null)
                        switch (_Scanner.Kind)
                        {
                            case XPathScanner.LexKind.Slash:
                                NextLex();
                                break;
                            case XPathScanner.LexKind.SlashSlash:
                                NextLex();
                                operand = new Axis(Axis.AxisType.DescendantOrSelf, operand);
                                break;
                            default:
                                return operand;
                        }
                break;
        }
        return ParseRelativePathPattern(operand);
    }

    //>> IdKeyPattern ::= 'id' '(' Literal ')' | 'key' '(' Literal ',' Literal ')'  
    private AstNode? ParseIdKeyPattern()
    {
        Debug.Assert(_Scanner.CanBeFunction);
        var arg_list = new ArrayList();
        if (_Scanner.Prefix.Length != 0) return null;
        switch (_Scanner.Name)
        {
            case "id":
                var pi = (ParamInfo)__FunctionTable["id"];
                NextLex();
                PassToken(XPathScanner.LexKind.LParens);
                CheckToken(XPathScanner.LexKind.String);
                arg_list.Add(new Operand(_Scanner.StringValue));
                NextLex();
                PassToken(XPathScanner.LexKind.RParens);
                return new Function(pi.FType, arg_list);
            case "key":
                NextLex();
                PassToken(XPathScanner.LexKind.LParens);
                CheckToken(XPathScanner.LexKind.String);
                arg_list.Add(new Operand(_Scanner.StringValue));
                NextLex();
                PassToken(XPathScanner.LexKind.Comma);
                CheckToken(XPathScanner.LexKind.String);
                arg_list.Add(new Operand(_Scanner.StringValue));
                NextLex();
                PassToken(XPathScanner.LexKind.RParens);
                return new Function(string.Empty, "key", arg_list);
        }
        return null;
    }

    //>> PathOp   ::= '/' | '//'
    //>> RelativePathPattern ::= ( RelativePathPattern PathOp )? StepPattern
    private AstNode ParseRelativePathPattern(AstNode? QyInput)
    {
        var operand = ParseStepPattern(QyInput);
        switch (_Scanner.Kind)
        {
            case XPathScanner.LexKind.SlashSlash:
                NextLex();
                operand = ParseRelativePathPattern(new Axis(Axis.AxisType.DescendantOrSelf, operand));
                break;
            case XPathScanner.LexKind.Slash:
                NextLex();
                operand = ParseRelativePathPattern(operand);
                break;
        }
        return operand;
    }

    //>> StepPattern    ::=    ChildOrAttributeAxisSpecifier NodeTest Predicate*   
    //>> ChildOrAttributeAxisSpecifier    ::=    @ ? | ('child' | 'attribute') '::' 
    private AstNode ParseStepPattern(AstNode? QyInput)
    {
        var axis_type = Axis.AxisType.Child;
        switch (_Scanner.Kind)
        {
            case XPathScanner.LexKind.At: //>> '@'
                axis_type = Axis.AxisType.Attribute;
                NextLex();
                break;
            case XPathScanner.LexKind.Axe: //>> AxisName '::'
                axis_type = GetAxis(_Scanner);
                if (axis_type != Axis.AxisType.Child && axis_type != Axis.AxisType.Attribute)
                    throw new XPathException($"'{_Scanner.SourceText}' has an invalid token.");
                NextLex();
                break;
        }
        var node_type = axis_type == Axis.AxisType.Attribute
            ? XPathNodeType.Attribute
            : /* default: */
            XPathNodeType.Element;

        var operand = ParseNodeTest(QyInput, axis_type, node_type);

        while (XPathScanner.LexKind.LBracket == _Scanner.Kind)
            operand = new Filter(operand, ParsePredicate(operand));
        return operand;
    }


    // --------------- Helper methods ----------------------

    private void CheckToken(XPathScanner.LexKind t)
    {
        if (_Scanner.Kind != t)
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

    private bool TestOp(string op) =>
        _Scanner.Kind == XPathScanner.LexKind.Name
        && _Scanner.Prefix.Length == 0
        && _Scanner.Name.Equals(op);

    private void CheckNodeSet(XPathResultType t)
    {
        if (t != XPathResultType.NodeSet && t != XPathResultType.Error)
            throw new XPathException($"Expression {_Scanner.SourceText} must evaluate to a node-set");
    }

    private static Hashtable CreateFunctionTable()
    {
        var table = new Hashtable(36)
        {
            {"last", new ParamInfo(Function.FunctionType.FuncLast, 0, 0, __TempArray1)},
            {"position", new ParamInfo(Function.FunctionType.FuncPosition, 0, 0, __TempArray1)},
            {"name", new ParamInfo(Function.FunctionType.FuncName, 0, 1, __TempArray2)},
            {"namespace-uri", new ParamInfo(Function.FunctionType.FuncNameSpaceUri, 0, 1, __TempArray2)},
            {"local-name", new ParamInfo(Function.FunctionType.FuncLocalName, 0, 1, __TempArray2)},
            {"count", new ParamInfo(Function.FunctionType.FuncCount, 1, 1, __TempArray2)},
            {"id", new ParamInfo(Function.FunctionType.FuncID, 1, 1, __TempArray3)},
            {"string", new ParamInfo(Function.FunctionType.FuncString, 0, 1, __TempArray3)},
            {"concat", new ParamInfo(Function.FunctionType.FuncConcat, 2, 100, __TempArray4)},
            {"starts-with", new ParamInfo(Function.FunctionType.FuncStartsWith, 2, 2, __TempArray5)},
            {"contains", new ParamInfo(Function.FunctionType.FuncContains, 2, 2, __TempArray5)},
            {"substring-before", new ParamInfo(Function.FunctionType.FuncSubstringBefore, 2, 2, __TempArray5)},
            {"substring-after", new ParamInfo(Function.FunctionType.FuncSubstringAfter, 2, 2, __TempArray5)},
            {"substring", new ParamInfo(Function.FunctionType.FuncSubstring, 2, 3, __TempArray6)},
            {"string-length", new ParamInfo(Function.FunctionType.FuncStringLength, 0, 1, __TempArray4)},
            {"normalize-space", new ParamInfo(Function.FunctionType.FuncNormalize, 0, 1, __TempArray4)},
            {"translate", new ParamInfo(Function.FunctionType.FuncTranslate, 3, 3, __TempArray7)},
            {"boolean", new ParamInfo(Function.FunctionType.FuncBoolean, 1, 1, __TempArray3)},
            {"not", new ParamInfo(Function.FunctionType.FuncNot, 1, 1, __TempArray8)},
            {"true", new ParamInfo(Function.FunctionType.FuncTrue, 0, 0, __TempArray8)},
            {"false", new ParamInfo(Function.FunctionType.FuncFalse, 0, 0, __TempArray8)},
            {"lang", new ParamInfo(Function.FunctionType.FuncLang, 1, 1, __TempArray4)},
            {"number", new ParamInfo(Function.FunctionType.FuncNumber, 0, 1, __TempArray3)},
            {"sum", new ParamInfo(Function.FunctionType.FuncSum, 1, 1, __TempArray2)},
            {"floor", new ParamInfo(Function.FunctionType.FuncFloor, 1, 1, __TempArray9)},
            {"ceiling", new ParamInfo(Function.FunctionType.FuncCeiling, 1, 1, __TempArray9)},
            {"round", new ParamInfo(Function.FunctionType.FuncRound, 1, 1, __TempArray9)}
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

    private Axis.AxisType GetAxis(XPathScanner Scanner)
    {
        Debug.Assert(Scanner.Kind == XPathScanner.LexKind.Axe);
        var axis = __AxesTable[Scanner.Name];
        if (axis is null) throw new XPathException($"'{_Scanner.SourceText}' has an invalid token.");
        return (Axis.AxisType)axis;
    }

    #endregion
}