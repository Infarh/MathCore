using MathCore.MathParser;
using MathCore.MathParser.ExpressionTrees.Nodes;

using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace MathCore.Tests.MathParser;

[TestClass]
public class ParseEquationsTests
{
    /// <summary>Тестирование процесса разбора выражения на блоки</summary>
    [TestMethod, Priority(0), Description("Тестирование процесса разбора выражения на блоки")]
    public void TermsParsing_Test()
    {
        var asm = typeof(ExpressionParser).Assembly;
        var BlockTerm_type = asm.GetType("MathCore.MathParser.BlockTerm");
        IsNotNull(BlockTerm_type);

        const string expression_str = "sin(5x)/(5x)+y-7cos(-2pi*(y-z)-3.2)";
        var block = BlockTerm_type.CreateObject(expression_str);
        IsNotNull(block);

        var Terms_property = BlockTerm_type.GetProperty("Terms");
        IsNotNull(Terms_property);

        var terms = Terms_property.GetValue(block) as object[];
        IsNotNull(terms);
        HasCount(8, terms);

        var check_term = terms[0];
        IsNotNull(check_term);
        AreEqual("MathCore.MathParser.FunctionTerm", check_term.GetType().FullName);
        var FunctionTerm_type = asm.GetType("MathCore.MathParser.FunctionTerm");
        IsNotNull(FunctionTerm_type);
        AreEqual("sin", (string)FunctionTerm_type.GetProperty("Name").GetValue(check_term)!);
        check_term = FunctionTerm_type.GetProperty("Block").GetValue(check_term);
        IsNotNull(check_term);
        AreEqual("(", (string)BlockTerm_type.GetProperty("OpenBracket").GetValue(check_term)!);
        AreEqual(")", (string)BlockTerm_type.GetProperty("CloseBracket").GetValue(check_term)!);
        var terms2 = Terms_property.GetValue(check_term) as object[];
        IsNotNull(terms2);
        HasCount(2, terms2);
        AreEqual("MathCore.MathParser.NumberTerm", terms2[0].GetType().FullName);
        AreEqual(5, (int)terms2[0].GetType().GetProperty("Value").GetValue(terms2[0])!);
        AreEqual("MathCore.MathParser.StringTerm", terms2[1].GetType().FullName);
        AreEqual("x", (string)terms2[1].GetType().GetProperty("Name").GetValue(terms2[1])!);

        IsNotNull(check_term = terms[1]);
        AreEqual("MathCore.MathParser.CharTerm", check_term.GetType().FullName);
        AreEqual('/', (char)check_term.GetType().GetProperty("Value").GetValue(check_term)!);

        IsNotNull(check_term = terms[2]);
        AreEqual(BlockTerm_type, check_term.GetType());
        AreEqual("(", (string)BlockTerm_type.GetProperty("OpenBracket").GetValue(check_term)!);
        AreEqual(")", (string)BlockTerm_type.GetProperty("CloseBracket").GetValue(check_term)!);
        terms2 = Terms_property.GetValue(check_term) as object[];
        IsNotNull(terms2);
        HasCount(2, terms2);
        AreEqual("MathCore.MathParser.NumberTerm", terms2[0].GetType().FullName);
        AreEqual(5, (int)terms2[0].GetType().GetProperty("Value").GetValue(terms2[0])!);
        AreEqual("MathCore.MathParser.StringTerm", terms2[1].GetType().FullName);
        AreEqual("x", (string)terms2[1].GetType().GetProperty("Name").GetValue(terms2[1])!);

        IsNotNull(check_term = terms[3]);
        AreEqual("MathCore.MathParser.CharTerm", check_term.GetType().FullName);
        AreEqual('+', (char)check_term.GetType().GetProperty("Value").GetValue(check_term)!);

        IsNotNull(check_term = terms[4]);
        AreEqual("MathCore.MathParser.StringTerm", check_term.GetType().FullName);
        AreEqual("y", (string)check_term.GetType().GetProperty("Name").GetValue(check_term)!);

        IsNotNull(check_term = terms[5]);
        AreEqual("MathCore.MathParser.CharTerm", check_term.GetType().FullName);
        AreEqual('-', (char)check_term.GetType().GetProperty("Value").GetValue(check_term)!);

        IsNotNull(check_term = terms[6]);
        AreEqual("MathCore.MathParser.NumberTerm", check_term.GetType().FullName);
        AreEqual(7, (int)check_term.GetType().GetProperty("Value").GetValue(check_term)!);

        IsNotNull(check_term = terms[7]);
        AreEqual("MathCore.MathParser.FunctionTerm", check_term.GetType().FullName);
        AreEqual("cos", (string)check_term.GetType().GetProperty("Name").GetValue(check_term)!);
        check_term = FunctionTerm_type.GetProperty("Block").GetValue(check_term);
        IsNotNull(check_term);
        AreEqual("(", (string)BlockTerm_type.GetProperty("OpenBracket").GetValue(check_term)!);
        AreEqual(")", (string)BlockTerm_type.GetProperty("CloseBracket").GetValue(check_term)!);

        //-2pi*(y-z)-3.2
        IsNotNull(terms2 = Terms_property.GetValue(check_term) as object[]);
        HasCount(9, terms2);
        AreEqual("MathCore.MathParser.CharTerm", terms2[0].GetType().FullName);
        AreEqual('-', (char)terms2[0].GetType().GetProperty("Value").GetValue(terms2[0])!);
        AreEqual("MathCore.MathParser.NumberTerm", terms2[1].GetType().FullName);
        AreEqual(2, (int)terms2[1].GetType().GetProperty("Value").GetValue(terms2[1])!);

        AreEqual("MathCore.MathParser.StringTerm", terms2[2].GetType().FullName);
        AreEqual("pi", (string)terms2[2].GetType().GetProperty("Name").GetValue(terms2[2])!);

        AreEqual("MathCore.MathParser.CharTerm", terms2[3].GetType().FullName);
        AreEqual('*', (char)terms2[3].GetType().GetProperty("Value").GetValue(terms2[3])!);

        AreEqual("MathCore.MathParser.BlockTerm", terms2[4].GetType().FullName);
        object[]? terms3;
        IsNotNull(terms3 = Terms_property.GetValue(terms2[4]) as object[]);
        HasCount(3, terms3);

        AreEqual("MathCore.MathParser.StringTerm", terms3[0].GetType().FullName);
        AreEqual("y", (string)terms3[0].GetType().GetProperty("Name").GetValue(terms3[0])!);

        AreEqual("MathCore.MathParser.CharTerm", terms3[1].GetType().FullName);
        AreEqual('-', (char)terms3[1].GetType().GetProperty("Value").GetValue(terms3[1])!);

        AreEqual("MathCore.MathParser.StringTerm", terms3[2].GetType().FullName);
        AreEqual("z", (string)terms3[2].GetType().GetProperty("Name").GetValue(terms3[2])!);

        AreEqual("MathCore.MathParser.CharTerm", terms2[5].GetType().FullName);
        AreEqual('-', (char)terms2[5].GetType().GetProperty("Value").GetValue(terms2[5])!);

        AreEqual("MathCore.MathParser.NumberTerm", terms2[6].GetType().FullName);
        AreEqual(3, (int)terms2[6].GetType().GetProperty("Value").GetValue(terms2[6])!);

        AreEqual("MathCore.MathParser.CharTerm", terms2[7].GetType().FullName);
        AreEqual('.', (char)terms2[7].GetType().GetProperty("Value").GetValue(terms2[7])!);

        AreEqual("MathCore.MathParser.NumberTerm", terms2[8].GetType().FullName);
        AreEqual(2, (int)terms2[8].GetType().GetProperty("Value").GetValue(terms2[8])!);
    }

    /// <summary>Тестирование процесса генерации дерева выражения</summary>
    [TestMethod, Priority(10), Description("Тестирование процесса генерации дерева выражения")]
    public void ExpressionTreeGeneration_Test()
    {
        var asm = typeof(ExpressionParser).Assembly;
        var BlockTerm_type = asm.GetType("MathCore.MathParser.BlockTerm");
        IsNotNull(BlockTerm_type);

        const string expression_str = "sin(5x)/(5x)+y-7cos(arg:-2pi*{-y^2+sin(x:z>5?z:-z)*atan2(y:5x,x:7-y)}-3.2)";
        var block = BlockTerm_type.CreateObject(expression_str);
        IsNotNull(block);

        var expression = new MathExpression();
        var parser = new ExpressionParser();

        var root = block
               .GetType()
               .GetMethod("GetSubTree")
               .Invoke(block, new object[] { parser, expression })
            as ExpressionTreeNode;
        IsNotNull(root);

        IsInstanceOfType<AdditionOperatorNode>(root);
        IsTrue(root.IsRoot); AreEqual(0, root.Depth);
        AreEqual(AdditionOperatorNode.NodeName, ((AdditionOperatorNode)root).Name);
        IsNotNull(root.Left); IsNotNull(root.Right);
        IsInstanceOfType<DivisionOperatorNode>(root.Left);
        IsInstanceOfType<subtractionOperatorNode>(root.Right);

        var node = root.Left;
        IsFalse(node.IsRoot); AreEqual(1, node.Depth);
        AreEqual(DivisionOperatorNode.NodeName, ((DivisionOperatorNode)node).Name);
        IsNotNull(node.Left); IsNotNull(node.Right);
        IsInstanceOfType<FunctionNode>(node.Left);
        IsInstanceOfType<ComputedBracketNode>(node.Right);

        node = node.Left;
        IsFalse(node.IsRoot); AreEqual(2, node.Depth);
        AreEqual("sin", ((FunctionNode)node).Name);
        var func = ((FunctionNode)node).Function;
        IsNotNull(func);
        AreEqual("sin", func.Name);
        IsNotNull(func.Arguments);
        HasCount(1, ((FunctionNode)node).Arguments);
        IsNull(node.Left); IsNotNull(node.Right);
        IsInstanceOfType<FunctionArgumentNode>(node.Right);

        node = node.Right;
        IsFalse(node.IsRoot); AreEqual(3, node.Depth);
        IsNotNull(node.Left); IsNull(node.Right);
        IsInstanceOfType<FunctionArgumentNameNode>(node.Left);
        IsNull(((FunctionArgumentNode)node).ArgumentName);
        IsNotNull(node.Left?.Right);
        AreEqual(node.Left?.Right, ((FunctionArgumentNode)node).ArgumentSubtree);

        node = node.Left;
        IsFalse(node.IsRoot); AreEqual(4, node.Depth);
        IsNull(node.Left); IsNotNull(node.Right);
        IsInstanceOfType<MultiplicationOperatorNode>(node.Right);
        IsNull(((FunctionArgumentNameNode)node).ArgumentName);
        AreEqual(node.Right, ((FunctionArgumentNameNode)node).ArgumentNode);

        node = node.Right;
        IsFalse(node.IsRoot); AreEqual(5, node.Depth);
        AreEqual(MultiplicationOperatorNode.NodeName, ((MultiplicationOperatorNode)node).Name);
        IsNotNull(node.Left); IsNotNull(node.Right);
        IsInstanceOfType<ConstValueNode>(node.Left);
        IsInstanceOfType<VariableValueNode>(node.Right);

        node = node.Left;
        IsFalse(node.IsRoot); AreEqual(6, node.Depth);
        IsNull(node.Left); IsNull(node.Right);
        AreEqual(5, ((ConstValueNode)node).Value);

        node = node["./r"];
        IsNotNull(node);
        IsInstanceOfType<VariableValueNode>(node);
        IsFalse(node.IsRoot); AreEqual(6, node.Depth);
        IsNull(node.Left); IsNull(node.Right);
        AreEqual("x", ((VariableValueNode)node).Name);
        AreEqual(0, ((VariableValueNode)node).Value);
        var var_x_1 = ((VariableValueNode)node).Variable;
        IsNotNull(var_x_1);
        AreEqual("x", var_x_1.Name);
        IsFalse(var_x_1.IsConstant);
        AreEqual(0, var_x_1.Value);

        node = node.Parents[4];
        IsNotNull(node);
        IsInstanceOfType<DivisionOperatorNode>(node);
        IsFalse(node.IsRoot); AreEqual(1, node.Depth);

        node = node.Right;
        IsNotNull(node);
        IsInstanceOfType<ComputedBracketNode>(node);
        IsFalse(node.IsRoot); AreEqual(2, node.Depth);
        IsNotNull(node.Left); IsNull(node.Right);
        IsInstanceOfType<MultiplicationOperatorNode>(node.Left);

        node = node.Left;
        IsFalse(node.IsRoot); AreEqual(3, node.Depth);
        IsNotNull(node.Left); IsNotNull(node.Right);
        IsInstanceOfType<ConstValueNode>(node.Left);
        IsInstanceOfType<VariableValueNode>(node.Right);

        node = node.Left;
        IsFalse(node.IsRoot); AreEqual(4, node.Depth);
        IsNull(node.Left); IsNull(node.Right);
        AreEqual(5, ((ConstValueNode)node).Value);

        node = node["./r"];
        IsNotNull(node);
        IsFalse(node.IsRoot); AreEqual(4, node.Depth);
        IsNull(node.Left); IsNull(node.Right);
        var var_x_2 = ((VariableValueNode)node).Variable;
        IsNotNull(var_x_2);
        AreEqual("x", var_x_2.Name);
        IsFalse(var_x_2.IsConstant);
        AreEqual(0, var_x_2.Value);
        AreEqual(var_x_1, var_x_2);

        var_x_2.Value = 3;
        AreEqual(3, var_x_2.Value);
        AreEqual(3, var_x_1.Value);
        AreEqual(3, ((VariableValueNode)node).Value);
        AreEqual(3, ((VariableValueNode)node["./././l/r/l/r/r"]!).Value);

        node = node.Root.Right;
        IsNotNull(node);
        IsInstanceOfType<subtractionOperatorNode>(node);
        IsNotNull(node.Left); IsNotNull(node.Right);
        IsInstanceOfType<VariableValueNode>(node.Left);
        IsInstanceOfType<MultiplicationOperatorNode>(node.Right);

        node = node.Left;
        var var_y_1_node = (VariableValueNode)node;
        AreEqual(0, var_y_1_node.Value);
        var var_y_1 = var_y_1_node.Variable;
        IsNotNull(var_y_1);
        AreEqual(0, var_y_1.Value);
        AreEqual("y", var_y_1.Name);

        node = node.Parent?.Right;
        IsNotNull(node);
        IsFalse(node.IsRoot); AreEqual(2, node.Depth);
        IsInstanceOfType<MultiplicationOperatorNode>(node);
        IsNotNull(node.Left); IsNotNull(node.Right);
        IsInstanceOfType<ConstValueNode>(node.Left);
        IsInstanceOfType<FunctionNode>(node.Right);

        node = node.Left;
        IsFalse(node.IsRoot); AreEqual(3, node.Depth);
        IsNull(node.Left); IsNull(node.Right);
        AreEqual(7, ((ConstValueNode)node).Value);

        node = node.Parent?.Right;
        IsNotNull(node);
        IsFalse(node.IsRoot); AreEqual(3, node.Depth);
        IsInstanceOfType<FunctionNode>(node);
        IsNull(node.Left); IsNotNull(node.Right);
        IsInstanceOfType<FunctionArgumentNode>(node.Right);
        AreEqual("cos", ((FunctionNode)node).Name);
        AreEqual("cos", ((FunctionNode)node).Function.Name);
        HasCount(1, ((FunctionNode)node).ArgumentsNames);
        AreEqual("arg", ((FunctionNode)node).ArgumentsNames[0]);
        HasCount(1, ((FunctionNode)node).Function.Arguments);
        AreEqual("arg", ((FunctionNode)node).Function.Arguments[0]);

        node = node.Right;
        IsFalse(node.IsRoot); AreEqual(4, node.Depth);
        IsNotNull(node.Left); IsNull(node.Right);
        IsInstanceOfType<FunctionArgumentNameNode>(node.Left);

        node = node.Left;
        IsFalse(node.IsRoot); AreEqual(5, node.Depth);
        IsNotNull(node.Left); IsNotNull(node.Right);
        IsInstanceOfType<StringNode>(node.Left);
        IsInstanceOfType<subtractionOperatorNode>(node.Right);

        node = node.Left;
        IsFalse(node.IsRoot); AreEqual(6, node.Depth);
        IsNull(node.Left); IsNull(node.Right);
        AreEqual("arg", ((StringNode)node).Value);

        node = node.Parent?.Right;
        IsNotNull(node);
        IsFalse(node.IsRoot); AreEqual(6, node.Depth);
        IsInstanceOfType<subtractionOperatorNode>(node);
        IsNotNull(node.Left); IsNotNull(node.Right);
        IsInstanceOfType<subtractionOperatorNode>(node.Left);
        IsInstanceOfType<ConstValueNode>(node.Right);

        node = node.Right;
        IsFalse(node.IsRoot); AreEqual(7, node.Depth);
        IsNull(node.Left); IsNull(node.Right);
        AreEqual(3.2, ((ConstValueNode)node).Value);

        node = node.Parent?.Left;
        IsNotNull(node);
        IsFalse(node.IsRoot); AreEqual(7, node.Depth);
        IsInstanceOfType<subtractionOperatorNode>(node);
        IsNull(node.Left); IsNotNull(node.Right);
        IsInstanceOfType<MultiplicationOperatorNode>(node.Right);

        node = node.Right;
        IsFalse(node.IsRoot); AreEqual(8, node.Depth);
        IsNotNull(node.Left); IsNotNull(node.Right);
        IsInstanceOfType<MultiplicationOperatorNode>(node.Left);
        IsInstanceOfType<ComputedBracketNode>(node.Right);

        node = node.Left;
        IsFalse(node.IsRoot); AreEqual(9, node.Depth);
        IsNotNull(node.Left); IsNotNull(node.Right);
        IsInstanceOfType<ConstValueNode>(node.Left);
        IsInstanceOfType<VariableValueNode>(node.Right);

        node = node.Left;
        IsFalse(node.IsRoot); AreEqual(10, node.Depth);
        IsNull(node.Left); IsNull(node.Right);
        AreEqual(2, ((ConstValueNode)node).Value);

        node = node["./r"];
        IsNotNull(node);
        IsInstanceOfType<VariableValueNode>(node);
        IsFalse(node.IsRoot); AreEqual(10, node.Depth);
        IsNull(node.Left); IsNull(node.Right);
        AreEqual("pi", ((VariableValueNode)node).Name);
        AreEqual(0, ((VariableValueNode)node).Value);
        AreEqual("pi", ((VariableValueNode)node).Variable.Name);
        AreEqual(0, ((VariableValueNode)node).Variable.Value);
        IsFalse(((VariableValueNode)node).Variable.IsConstant);

        node = node["././r"];
        IsNotNull(node);
        IsInstanceOfType<ComputedBracketNode>(node);
        IsFalse(node.IsRoot); AreEqual(9, node.Depth);
        AreEqual("{", ((ComputedBracketNode)node).Bracket.Start);
        AreEqual("}", ((ComputedBracketNode)node).Bracket.Stop);
        IsNotNull(node.Left); IsNull(node.Right);
        IsInstanceOfType<AdditionOperatorNode>(node.Left);

        node = node.Left;
        IsFalse(node.IsRoot); AreEqual(10, node.Depth);
        IsNotNull(node.Left); IsNotNull(node.Right);
        IsInstanceOfType<PowerOperatorNode>(node.Left);
        IsInstanceOfType<MultiplicationOperatorNode>(node.Right);

        node = node.Left;
        IsFalse(node.IsRoot); AreEqual(11, node.Depth);
        IsNotNull(node.Left); IsNotNull(node.Right);
        IsInstanceOfType<subtractionOperatorNode>(node.Left);
        IsInstanceOfType<ConstValueNode>(node.Right);

        node = node.Left;
        IsFalse(node.IsRoot); AreEqual(12, node.Depth);
        IsNull(node.Left); IsNotNull(node.Right);
        IsInstanceOfType<VariableValueNode>(node.Right);

        node = node.Right;
        IsFalse(node.IsRoot); AreEqual(13, node.Depth);
        IsNull(node.Left); IsNull(node.Right);
        AreEqual("y", ((VariableValueNode)node).Name);
        AreEqual(0, ((VariableValueNode)node).Value);
        var var_y_2 = ((VariableValueNode)node).Variable;
        AreEqual(((VariableValueNode)node).Value, var_y_2.Value);
        AreEqual(((VariableValueNode)node).Name, var_y_2.Name);
        AreEqual(var_y_1_node.Variable, ((VariableValueNode)node).Variable);

        node = node[n => n.Parent].First(n => n is AdditionOperatorNode).Right;
        IsNotNull(node);
        IsInstanceOfType<MultiplicationOperatorNode>(node);
        IsFalse(node.IsRoot); AreEqual(11, node.Depth);
        IsNotNull(node.Left); IsNotNull(node.Right);
        IsInstanceOfType<FunctionNode>(node.Left);
        IsInstanceOfType<FunctionNode>(node.Right);

        node = node.Left;
        IsFalse(node.IsRoot); AreEqual(12, node.Depth);
        IsNull(node.Left); IsNotNull(node.Right);
        AreEqual("sin", ((FunctionNode)node).Name);
        AreEqual("sin", ((FunctionNode)node).Function.Name);
        AreNotEqual(((FunctionNode)node["../l/l"]!).Function, ((FunctionNode)node).Function);
        HasCount(1, ((FunctionNode)node).ArgumentsNames);
        AreEqual("x", ((FunctionNode)node).ArgumentsNames[0]);
        IsInstanceOfType<FunctionArgumentNode>(node.Right);

        node = node.Right;
        IsFalse(node.IsRoot); AreEqual(13, node.Depth);
        AreEqual("x", ((FunctionArgumentNode)node).ArgumentName);
        IsNotNull(node.Left); IsNull(node.Right);
        AreEqual(node.Left.Right, ((FunctionArgumentNode)node).ArgumentSubtree);
        IsInstanceOfType<FunctionArgumentNameNode>(node.Left);

        node = node.Left;
        IsFalse(node.IsRoot); AreEqual(14, node.Depth);
        IsNotNull(node.Left); IsNotNull(node.Right);
        AreEqual("x", ((FunctionArgumentNameNode)node).ArgumentName);
        AreEqual(node.Right, ((FunctionArgumentNameNode)node).ArgumentNode);
        IsInstanceOfType<StringNode>(node.Left);
        IsInstanceOfType<SelectorOperatorNode>(node.Right);

        node = node.Left;
        IsFalse(node.IsRoot); AreEqual(15, node.Depth);
        AreEqual("x", ((StringNode)node).Value);
        IsNull(node.Left); IsNull(node.Right);

        node = node.Parent?.Right;
        IsNotNull(node);
        IsInstanceOfType<SelectorOperatorNode>(node);
        IsFalse(node.IsRoot); AreEqual(15, node.Depth);
        IsNotNull(node.Left); IsNotNull(node.Right);
        IsInstanceOfType<GreaterThenOperatorNode>(node.Left);
        IsInstanceOfType<VariantOperatorNode>(node.Right);

        node = node.Left;
        IsFalse(node.IsRoot); AreEqual(16, node.Depth);
        IsNotNull(node.Left); IsNotNull(node.Right);
        IsInstanceOfType<VariableValueNode>(node.Left);
        IsInstanceOfType<ConstValueNode>(node.Right);

        node = node.Left;
        IsFalse(node.IsRoot); AreEqual(17, node.Depth);
        IsNull(node.Left); IsNull(node.Right);
        AreEqual("z", ((VariableValueNode)node).Name);
        var var_z_1 = ((VariableValueNode)node).Variable;
        AreEqual(((VariableValueNode)node).Name, var_z_1.Name);
        AreEqual(((VariableValueNode)node).Value, var_z_1.Value);
        AreEqual(0, var_z_1.Value);

        node = node["./r"];
        IsNotNull(node);
        IsInstanceOfType<ConstValueNode>(node);
        IsFalse(node.IsRoot); AreEqual(17, node.Depth);
        AreEqual(5, ((ConstValueNode)node).Value);
        IsNull(node.Left); IsNull(node.Right);

        node = node["././r"];
        IsNotNull(node);
        IsInstanceOfType<VariantOperatorNode>(node);
        IsFalse(node.IsRoot); AreEqual(16, node.Depth);
        IsNotNull(node.Left); IsNotNull(node.Right);
        IsInstanceOfType<VariableValueNode>(node.Left);
        IsInstanceOfType<subtractionOperatorNode>(node.Right);

        node = node.Left;
        IsFalse(node.IsRoot); AreEqual(17, node.Depth);
        IsNull(node.Left); IsNull(node.Right);
        AreEqual(var_z_1, ((VariableValueNode)node).Variable);

        node = node["./r"];
        IsNotNull(node);
        IsFalse(node.IsRoot); AreEqual(17, node.Depth);
        IsInstanceOfType<subtractionOperatorNode>(node);
        IsNull(node.Left); IsNotNull(node.Right);
        IsInstanceOfType<VariableValueNode>(node.Right);

        node = node[n => n.Parent].First(n => n is MultiplicationOperatorNode).Right;
        IsNotNull(node);
        IsInstanceOfType<FunctionNode>(node);
        IsFalse(node.IsRoot); AreEqual(12, node.Depth);
        IsNull(node.Left); IsNotNull(node.Right);
        AreEqual("atan2", ((FunctionNode)node).Name);
        AreEqual("atan2", ((FunctionNode)node).Function.Name);
        HasCount(2, ((FunctionNode)node).Function.Arguments);
        AreEqual("y", ((FunctionNode)node).Function.Arguments[0]);
        AreEqual("x", ((FunctionNode)node).Function.Arguments[1]);
        IsInstanceOfType<FunctionArgumentNode>(node.Right);

        node = node.Right;
        IsFalse(node.IsRoot); AreEqual(13, node.Depth);
        IsNotNull(node.Left); IsNotNull(node.Right);
        AreEqual("y", ((FunctionArgumentNode)node).ArgumentName);
        AreEqual(node.Left.Right, ((FunctionArgumentNode)node).ArgumentSubtree);
        IsInstanceOfType<FunctionArgumentNameNode>(node.Left);
        IsInstanceOfType<FunctionArgumentNode>(node.Right);

        node = node.Left;
        IsFalse(node.IsRoot); AreEqual(14, node.Depth);
        IsNotNull(node.Left); IsNotNull(node.Right);
        AreEqual("y", ((FunctionArgumentNameNode)node).ArgumentName);
        AreEqual(node.Right, ((FunctionArgumentNameNode)node).ArgumentNode);
        IsInstanceOfType<StringNode>(node.Left);
        IsInstanceOfType<MultiplicationOperatorNode>(node.Right);

        node = node.Left;
        IsFalse(node.IsRoot); AreEqual(15, node.Depth);
        IsNull(node.Left); IsNull(node.Right);
        AreEqual("y", ((StringNode)node).Value);

        node = node["./r"];
        IsNotNull(node);
        IsInstanceOfType<MultiplicationOperatorNode>(node);
        IsFalse(node.IsRoot); AreEqual(15, node.Depth);
        IsNotNull(node.Left); IsNotNull(node.Right);
        IsInstanceOfType<ConstValueNode>(node.Left);
        IsInstanceOfType<VariableValueNode>(node.Right);

        node = node.Left;
        IsFalse(node.IsRoot); AreEqual(16, node.Depth);
        IsNull(node.Left); IsNull(node.Right);
        AreEqual(5, ((ConstValueNode)node).Value);

        node = node["./r"];
        IsNotNull(node);
        IsInstanceOfType<VariableValueNode>(node);
        IsFalse(node.IsRoot); AreEqual(16, node.Depth);
        IsNull(node.Left); IsNull(node.Right);
        AreEqual(var_x_1, ((VariableValueNode)node).Variable);

        node = node["./././r"];
        IsNotNull(node);
        IsInstanceOfType<FunctionArgumentNode>(node);
        IsFalse(node.IsRoot); AreEqual(14, node.Depth);
        IsNotNull(node.Left); IsNull(node.Right);
        AreEqual("x", ((FunctionArgumentNode)node).ArgumentName);
        AreEqual(node.Left.Right, ((FunctionArgumentNode)node).ArgumentSubtree);
        IsInstanceOfType<FunctionArgumentNameNode>(node.Left);

        node = node.Left;
        IsFalse(node.IsRoot); AreEqual(15, node.Depth);
        IsNotNull(node.Left); IsNotNull(node.Right);
        AreEqual("x", ((FunctionArgumentNameNode)node).ArgumentName);
        AreEqual(node.Right, ((FunctionArgumentNameNode)node).ArgumentNode);
        IsInstanceOfType<StringNode>(node.Left);
        IsInstanceOfType<subtractionOperatorNode>(node.Right);

        node = node.Left;
        IsFalse(node.IsRoot); AreEqual(16, node.Depth);
        AreEqual("x", ((StringNode)node).Value);
        IsNull(node.Left); IsNull(node.Right);

        node = node["./r"];
        IsNotNull(node);
        IsInstanceOfType<subtractionOperatorNode>(node);
        IsFalse(node.IsRoot); AreEqual(16, node.Depth);
        IsNotNull(node.Left); IsNotNull(node.Right);
        IsInstanceOfType<ConstValueNode>(node.Left);
        IsInstanceOfType<VariableValueNode>(node.Right);

        node = node.Left;
        IsFalse(node.IsRoot); AreEqual(17, node.Depth);
        IsNull(node.Left); IsNull(node.Right);
        AreEqual(7, ((ConstValueNode)node).Value);

        node = node["./r"];
        IsNotNull(node);
        IsInstanceOfType<VariableValueNode>(node);
        IsFalse(node.IsRoot); AreEqual(17, node.Depth);
        AreEqual(var_y_1, ((VariableValueNode)node).Variable);
    }
}