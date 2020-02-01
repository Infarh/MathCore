using System;
using System.Collections.Generic;
using System.Linq;
using MathCore.MathParser;
using MathCore.MathParser.ExpressionTrees.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathCore.Tests.MathParser
{
    [TestClass]
    public class ExpressionParserTests
    {
        public TestContext TestContext { get; set; }

        #region Additional test attributes

        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion

        /// <summary>Общий тест парсера</summary>
        [TestMethod, Priority(10), Description("Общий тест парсера")]
        public void MathExpression_Parse_Test()
        {
            var parser = new ExpressionParser();
            const int a = 1;
            const int b = 2;
            var exp = parser.Parse($"{a}+{b}");
            var tree = exp.Tree;
            var root = tree.Root;
            Assert.IsTrue(root is AdditionOperatorNode);

            var left = root.Left;
            Assert.IsTrue(left is ConstValueNode);
            var right = root.Right;
            Assert.IsTrue(right is ConstValueNode);

            var left_value = left as ConstValueNode;
            Assert.AreEqual(a, left_value.Value);
            var right_value = right as ConstValueNode;
            Assert.AreEqual(b, right_value.Value);

            var result = exp.Compute();
            Assert.AreEqual(a + b, result);

            void Test(string str, double value)
            {
                var expr = parser.Parse(str);
                Assert.AreEqual(value, expr.Compute(), expr.ToString());
            }

            Test("2+2", 4);
            Test("2*2", 4);

            Test("(2+2)", 4);
            Test("((2+2))", 4);

            Test("2*2+2", 6);
            Test("2+2*2", 6);

            Test("(2+2)*2", 8);
            Test("2*(2+2)", 8);


            Test("3-5-7-9", 3 - 5 - 7 - 9);
            Test("3-5+7-9", 3 - 5 + 7 - 9);
            Test("3+5-7+9", 3 + 5 - 7 + 9);
            Test("3+5-7-9", 3 + 5 - 7 - 9);

            Test("2^2", 4);
            Test("2^2+1", 5);
            Test("1+2^2", 5);
            Test("2^2*3", 12);
            Test("3*2^2", 12);
            Test("2^(1+2)", 8);
            Test("2^(2*5)", 1024);
            Test("(1+1)^(2*5)", 1024);

            Test("2.5 * 2", 5);
            Test("2.5 - 0.5", 2);
            Test("0.1 / 2", 0.05);
            Test("0.9 / 0.1", 9);

            Test("0 + 0.0", 0);
            Test("2 * 0.0", 0);

            Test("-1 + 1", -1 + 1);
            Test("2 + -1", 2 + -1);
            Test("2 * -1", 2 * -1);
            Test("2 * (-1)", 2 * -1);
            Test("2 + -1 - -5", 2 + -1 - -5);
            Test("+2 - -3 * -5", +2 - -3 * -5);
            Test("-2/-1", 2);
            Test("(-4-4*-2)^(2^-1)", 2);

            Test("(2+3)(7-2)", 25);
            Test("5(7-2)", 25);
        }

        /// <summary>Тестирование приоритета операторов</summary>
        [TestMethod, Priority(5), Description("Тестирование приоритета операторов")]
        public void MathExpression_OperatorPriority_Test()
        {
            var parser = new ExpressionParser();
            var expr = parser.Parse("2+2*2");
            var tree = expr.Tree;
            var root = tree.Root;
            Assert.IsTrue(root is AdditionOperatorNode);
            Assert.IsTrue(root.Left is ConstValueNode);
            Assert.IsTrue(root.Right is MultiplicationOperatorNode);
            Assert.IsTrue(root.Right.Left is ConstValueNode);
            Assert.IsTrue(root.Right.Right is ConstValueNode);

            var result = expr.Compute();
            Assert.AreEqual(6, result);
        }

        /// <summary>Тестирование скобок</summary>
        [TestMethod, Priority(5), Description("Тестирование скобок")]
        public void MathExpression_Bracket_Test()
        {
            var parser = new ExpressionParser();
            var expr = parser.Parse("(2+2)*2");
            var tree = expr.Tree;
            var root = tree.Root;
            Assert.IsTrue(root is MultiplicationOperatorNode);
            Assert.IsTrue(root.Left is ComputedBracketNode);
            Assert.IsTrue(root.Left.Left is AdditionOperatorNode);
            Assert.IsTrue(root.Left.Left.Left is ConstValueNode);
            Assert.IsTrue(root.Left.Left.Right is ConstValueNode);
            Assert.IsTrue(root.Right is ConstValueNode);

            var result = expr.Compute();
            Assert.AreEqual(8, result);
        }

        /// <summary>Тестирование значащих узлов</summary>
        [TestMethod, Description("Тестирование значащих узлов")]
        public void ExpressionTree_ValueTestNode_Test()
        {
            var parser = new ExpressionParser();
            var expr = parser.Parse("2");
            var root = expr.Tree.Root;
            Assert.IsTrue(root is ConstValueNode);
            var value = (ConstValueNode)root;
            Assert.AreEqual(2, value.Value);
            Assert.AreEqual(2, expr.Compute());

            expr = parser.Parse("2.2");
            root = expr.Tree.Root;
            Assert.IsTrue(root is ConstValueNode);
            value = (ConstValueNode)root;
            Assert.AreEqual(2.2, value.Value);
            Assert.AreEqual(2.2, expr.Compute());
        }

        /// <summary>Тестирование префиксных унарных операторов</summary>
        [TestMethod, Description("Тестирование префиксных унарных операторов")]
        public void ExpressionTree_PrefixUnaryOperator_Test()
        {
            var parser = new ExpressionParser();
            var expr = parser.Parse("-2");
            var root = expr.Tree.Root;
            Assert.IsTrue(root is subtractionOperatorNode);
            Assert.AreEqual(-2, expr.Compute());

            expr = parser.Parse("-2+3");
            root = expr.Tree.Root;
            Assert.IsTrue(root is AdditionOperatorNode);
            Assert.IsTrue(root.Left is subtractionOperatorNode);
            Assert.IsTrue(root["l"].Left == null);
            Assert.IsTrue(root["l"].Right is ConstValueNode);
            Assert.IsTrue(root.Right is ConstValueNode);
            Assert.AreEqual(1, expr.Compute());

            expr = parser.Parse("2*-1");
            root = expr.Tree.Root;
            Assert.IsTrue(root is MultiplicationOperatorNode);
            Assert.IsTrue(root.Left is ConstValueNode);
            Assert.IsTrue(root.Right is subtractionOperatorNode);
            Assert.IsTrue(root["r"].Left == null);
            Assert.IsTrue(root["r"].Right is ConstValueNode);
            Assert.AreEqual(-2, expr.Compute());

            Assert.AreEqual(0.5, parser.Parse("4^0.5^-1").Compute());

        }

        /// <summary>Тестирование оператора равенства</summary>
        [TestMethod, Description("Тестирование оператора равенства")]
        public void MathExpression_OperatorOfEquality_SimpleTest()
        {
            var parser = new ExpressionParser();
            var expr = parser.Parse("4 = 2");
            Assert.IsInstanceOfType(expr.Tree.Root, typeof(EqualityOperatorNode));
            Assert.AreEqual(0, expr.Compute());
            expr = parser.Parse("x = y");
            Assert.AreEqual(1, expr.Compute());
            Assert.AreEqual(1, expr.Compute(4, 4));
            Assert.AreEqual(0, expr.Compute(3, 7));

            var func = expr.Compile<Func<double, double, double>>();
            Assert.AreEqual(1, func(5, 5));
            Assert.AreEqual(0, func(5, 7));

            Assert.AreEqual(0, parser.Parse("2+2*2 = 7*8").Compute());
            Assert.AreEqual(1, parser.Parse("2+2*2 = 10-4").Compute());
            Assert.AreEqual(10, parser.Parse("(7-2 = 10/2) + 9").Compute());
        }

        /// <summary>Тестирование оператора неравенства</summary>
        [TestMethod, Description("Тестирование оператора неравенства")]
        public void MathExpression_OperatorOfNotEquality_SimpleTest()
        {
            var parser = new ExpressionParser();
            var expr = parser.Parse("4 ≠ 2");
            Assert.IsInstanceOfType(expr.Tree.Root, typeof(NotOperatorNode));
            Assert.AreEqual(1, expr.Compute());
            Assert.IsInstanceOfType(parser.Parse("4!2").Tree.Root, typeof(NotOperatorNode));
            expr = parser.Parse("x ≠ y");
            Assert.AreEqual(0, expr.Compute());
            Assert.AreEqual(0, expr.Compute(4, 4));
            Assert.AreEqual(1, expr.Compute(3, 7));
            Assert.AreEqual(1, expr.Compute(7, 3));

            var func = expr.Compile<Func<double, double, double>>();
            Assert.AreEqual(0, func(5, 5));
            Assert.AreEqual(1, func(5, 7));

            Assert.AreEqual(1, parser.Parse("2+2*2 ≠ 7*8").Compute());
            Assert.AreEqual(0, parser.Parse("2+2*2 ≠ 10-4").Compute());
            Assert.AreEqual(9, parser.Parse("(7-2 ≠ 10/2) + 9").Compute());
        }

        /// <summary>Тестирование оператора больше</summary>
        [TestMethod, Description("Тестирование оператора больше")]
        public void MathExpression_OperatorGtraterThen_SimpleTest()
        {
            var parser = new ExpressionParser();
            var expr = parser.Parse("4 > 2");
            Assert.IsInstanceOfType(expr.Tree.Root, typeof(GreaterThenOperatorNode));
            Assert.AreEqual(1, expr.Compute());
            expr = parser.Parse("x > y");

            var func = expr.Compile<Func<double, double, double>>();
            Assert.AreEqual(0, func(4, 4));
            Assert.AreEqual(0, func(3, 7));
            Assert.AreEqual(1, func(7, 3));

            Assert.AreEqual(0, expr.Compute());
            Assert.AreEqual(0, expr.Compute(4, 4));
            Assert.AreEqual(0, expr.Compute(3, 7));
            Assert.AreEqual(1, expr.Compute(7, 3));
        }

        /// <summary>Тестирование оператора меньше</summary>
        [TestMethod, Description("Тестирование оператора меньше")]
        public void MathExpression_OperatorLessThen_SimpleTest()
        {
            var parser = new ExpressionParser();
            var expr = parser.Parse("4 < 2");
            Assert.IsInstanceOfType(expr.Tree.Root, typeof(LessThenOperatorNode));
            Assert.AreEqual(0, expr.Compute());
            expr = parser.Parse("x < y");

            var func = expr.Compile<Func<double, double, double>>();
            Assert.AreEqual(0, func(4, 4));
            Assert.AreEqual(1, func(3, 7));
            Assert.AreEqual(0, func(7, 3));

            Assert.AreEqual(0, expr.Compute());
            Assert.AreEqual(0, expr.Compute(4, 4));
            Assert.AreEqual(1, expr.Compute(3, 7));
            Assert.AreEqual(0, expr.Compute(7, 3));
        }


        /// <summary>Тестирование оператора И</summary>
        [TestMethod, Description("Тестирование оператора И")]
        public void MathExpression_OperatorAnd_SimpleTest()
        {
            var parser = new ExpressionParser();
            var expr = parser.Parse("4 & 2");
            Assert.IsInstanceOfType(expr.Tree.Root, typeof(AndOperatorNode));
            Assert.AreEqual(1, expr.Compute());
            expr = parser.Parse("x & y");

            var func = expr.Compile<Func<double, double, double>>();
            Assert.AreEqual(0, func(0, 0));
            Assert.AreEqual(0, func(0, 4));
            Assert.AreEqual(0, func(3, 0));
            Assert.AreEqual(1, func(7, 3));

            Assert.AreEqual(0, expr.Compute());
            Assert.AreEqual(0, expr.Compute(0, 0));
            Assert.AreEqual(0, expr.Compute(0, 4));
            Assert.AreEqual(0, expr.Compute(3, 0));
            Assert.AreEqual(1, expr.Compute(7, 3));
        }

        /// <summary>Тестирование оператора ИЛИ</summary>
        [TestMethod, Description("Тестирование оператора ИЛИ")]
        public void MathExpression_OperatorOr_SimpleTest()
        {
            var parser = new ExpressionParser();
            var expr = parser.Parse("4 | 2");
            Assert.IsInstanceOfType(expr.Tree.Root, typeof(OrOperatorNode));
            Assert.AreEqual(1, expr.Compute());
            expr = parser.Parse("x | y");

            var func = expr.Compile<Func<double, double, double>>();
            Assert.AreEqual(0, func(0, 0));
            Assert.AreEqual(1, func(0, 4));
            Assert.AreEqual(1, func(3, 0));
            Assert.AreEqual(1, func(7, 3));

            Assert.AreEqual(0, expr.Compute());
            Assert.AreEqual(0, expr.Compute(0, 0));
            Assert.AreEqual(1, expr.Compute(0, 4));
            Assert.AreEqual(1, expr.Compute(3, 0));
            Assert.AreEqual(1, expr.Compute(7, 3));
        }

        /// <summary>Тестирование оператора НЕ</summary>
        [TestMethod, Description("Тестирование оператора НЕ")]
        public void MathExpression_OperatorNot_SimpleTest()
        {
            var parser = new ExpressionParser();
            var expr = parser.Parse("!2");
            Assert.IsInstanceOfType(expr.Tree.Root, typeof(NotOperatorNode));
            Assert.AreEqual(0, expr.Compute());
            expr = parser.Parse("!x");

            var func = expr.Compile<Func<double, double>>();
            Assert.AreEqual(1, func(0));
            Assert.AreEqual(0, func(3));
            Assert.AreEqual(0, func(-7));

            Assert.AreEqual(1, expr.Compute());
            Assert.AreEqual(1, expr.Compute(0));
            Assert.AreEqual(0, expr.Compute(3));
            Assert.AreEqual(0, expr.Compute(-7));
        }

        /// <summary>Тестирование логических операторов</summary>
        [TestMethod, Description("Тестирование логических операторов")]
        public void ExpressionTree_LogicOperator_ComplexTest()
        {
            var parser = new ExpressionParser();
            var expr = parser.Parse("(3&0)|(4<0)");
            Assert.IsInstanceOfType(expr.Tree.Root, typeof(OrOperatorNode));
            Assert.IsInstanceOfType(expr.Tree.Root.Left?.Left, typeof(AndOperatorNode));
            Assert.IsInstanceOfType(expr.Tree.Root.Right?.Left, typeof(LessThenOperatorNode));
            Assert.AreEqual(0, expr.Compute());
            var _0 = expr.Compile();
            Assert.AreEqual(0, _0());
        }

        /// <summary>Тестирование оператора выбора ЕСЛИ ? ТО : ИНАЧЕ</summary>
        [TestMethod, Description("Тестирование оператора выбора ЕСЛИ ? ТО : ИНАЧЕ")]
        public void ExpressionTree_SelectorOperator_Test()
        {
            var parser = new ExpressionParser();
            var expr = parser.Parse("(x>3)?7:-8");
            var root = expr.Tree.Root;
            Assert.IsInstanceOfType(root, typeof(SelectorOperatorNode));
            Assert.IsInstanceOfType(root["l/l"], typeof(GreaterThenOperatorNode));
            Assert.IsInstanceOfType(root.Right, typeof(VariantOperatorNode));
            Assert.IsInstanceOfType(root["r/l"], typeof(ConstValueNode));
            Assert.IsInstanceOfType(root["r/r"], typeof(subtractionOperatorNode));
            Assert.IsInstanceOfType(root["r/r/r"], typeof(ConstValueNode));
            Assert.AreEqual(7, expr.Compute(4));
            Assert.AreEqual(-8, expr.Compute(2));

            var abs_expr = parser.Parse("x>0?x:-x");
            var abs_f = abs_expr.Compile<Func<double, double>>();
            for (var i = -5; i <= 5; i++)
                Assert.AreEqual(Math.Abs(i), abs_f(i));

            var interval_expr = parser.Parse("-1<x&x<1?5:0");
            var interval_f = interval_expr.Compile<Func<double, double>>();
            Assert.AreEqual(0, interval_f(-5));
            Assert.AreEqual(0, interval_f(-1));
            Assert.AreEqual(5, interval_f(0));
            Assert.AreEqual(5, interval_f(0.5));
            Assert.AreEqual(0, interval_f(1));
            Assert.AreEqual(0, interval_f(5));
        }

        [TestMethod, Timeout(300)]
        public void MathExpression_AbsExpression_StressTest()
        {
            var parser = new ExpressionParser();
            var abs_expr = parser.Parse("x>0?x:-x");
            var abs_f = abs_expr.Compile<Func<double, double>>();
            for (var i = 0; i < 2500000; i++)
            {
                abs_f(-10);
                abs_f(10);
            }
        }

        [TestMethod, Timeout(200)]
        public void MathExpression_AbsExpression_Original_StressTest()
        {
            var parser = new ExpressionParser();
            var abs_expr = parser.Parse("x>0?x:-x");
            // ReSharper disable once UnusedVariable
            var abs_f = abs_expr.Compile<Func<double, double>>();
            for (var i = 0; i < 2500000; i++)
            {
                // ReSharper disable ReturnValueOfPureMethodIsNotUsed
                abs_f(-10);
                abs_f(10);
                // ReSharper restore ReturnValueOfPureMethodIsNotUsed
            }
        }

        /// <summary>Тестирование переменных</summary>
        [TestMethod, Description("Тестирование переменных")]
        public void ExpressionTree_VariablesTest()
        {
            var parser = new ExpressionParser();

            Assert.AreEqual(0, parser.Parse("3+5").Variable.Count); //В выражении нет переменных

            var expr1v = parser.Parse("x");

            Assert.AreEqual(1, expr1v.Variable.Count); //В выражении одна переменная

            var root = expr1v.Tree.Root;
            Assert.IsTrue(root is VariableValueNode);
            var var_node = root as VariableValueNode;
            Assert.AreEqual("x", var_node.Name);
            var @var = var_node.Variable;
            Assert.AreEqual("x", @var.Name);
            Assert.AreEqual(0, @var.Value);

            @var.Value = 7;
            Assert.AreEqual(7, @var.Value);

            expr1v.Variable["x"] = 5;
            Assert.AreEqual(5, @var.Value);

            Assert.AreEqual(5, expr1v.Compute());
            Assert.AreEqual(1, expr1v.Compute(1));
            Assert.AreEqual(10, expr1v.Compute(10));


            var expr2v = parser.Parse("x+y");

            Assert.AreEqual(2, expr2v.Variable.Count); //В выражении две переменных

            root = expr2v.Tree.Root;
            Assert.IsTrue(root is AdditionOperatorNode);
            Assert.IsTrue(root.Left is VariableValueNode);
            Assert.IsTrue(root.Right is VariableValueNode);

            parser.VariableProcessing += (s, e) => { if (e.Argument.Name == "x") e.Argument.Value = 7; };
            parser.VariableProcessing += (s, e) => { if (e.Argument.Name == "y") e.Argument.Value = 5; };

            var expr2v2 = parser.Parse("x+y");
            Assert.AreEqual(7 + 5, expr2v2.Compute());
            Assert.AreEqual(1 + 5, expr2v2.Compute(1));
            Assert.AreEqual(1 + 2, expr2v2.Compute(1, 2));

            var ConstAndValueExpr = (parser = new ExpressionParser()).Parse("5x");
            Assert.AreEqual(10, ConstAndValueExpr.Compute(2));
            Assert.AreEqual(3, parser.Parse("2x-7").Compute(5));
            Assert.AreEqual(3, parser.Parse("-7+2x").Compute(5));
            Assert.AreEqual(3, parser.Parse("-7+(2x)").Compute(5));
        }

        /// <summary>Тестирование коллекции переменных</summary>
        [TestMethod, Description("Тестирование коллекции переменных")]
        public void ExpressionVariableCollection_Test()
        {
            var parser = new ExpressionParser();
            var expr = parser.Parse("2+2*2");
            var var_collection = expr.Variable;
            Assert.IsNotNull(var_collection);
            Assert.AreEqual(0, var_collection.Count);

            var var_x = var_collection["x"];
            Assert.AreEqual(1, var_collection.Count);
            var var_x1 = var_collection["x"];
            Assert.IsTrue(ReferenceEquals(var_x, var_x1));

            Assert.AreEqual(0, var_x.Value);
            var_x.Value = 5;
            Assert.AreEqual(5, var_x.Value);
            Assert.AreEqual(5, var_x1.Value);


            var var_y = var_collection["y"];
            var_x.Value = 7;
            Assert.AreEqual(2, var_collection.Count);
            Assert.IsFalse(ReferenceEquals(var_x, var_y));
            Assert.AreEqual(7, var_x.Value);
            Assert.AreEqual(0, var_y.Value);

            expr = parser.Parse("2x^4-7x^3+3x^2-2x+3-7y");
            var_collection = expr.Variable;
            Assert.AreEqual(2, var_collection.Count);
            var_x = var_collection[0];
            var_y = var_collection[1];
            Assert.AreEqual("x", var_x.Name);
            Assert.AreEqual("y", var_y.Name);
            var l_var = new LambdaExpressionVariable(() => 10);
            var_collection["x"] = l_var;
            var vars = expr.Tree
                        .Where(node => node is VariableValueNode)
                        .Cast<VariableValueNode>()
                        .Select(node => node.Variable)
                        .Where(v => v.Name == "x")
                        .ToArray();
            Assert.IsTrue(vars.All(v => ReferenceEquals(v, l_var)));
            vars.Foreach(v => Assert.AreEqual(10, v.GetValue()));


            expr = parser.Parse("2x-y");
            expr.Variable["x"] = new LambdaExpressionVariable(() => 20);
            Assert.IsInstanceOfType(expr.Variable["x"], typeof(LambdaExpressionVariable));
            Assert.AreEqual(0, expr.Variable["x"].Value);
            Assert.AreEqual(20, expr.Variable["x"].GetValue());
            Assert.AreEqual(20, expr.Variable["x"].Value);
            Assert.AreEqual(40, expr.Compute());

            expr = parser.Parse("(2x-3.27y)^(sin(x)/x)-2z+3q/2x+y");
            var_collection = expr.Variable;
            var x_var_call_counter = 0;
            var_collection["x"] = new LambdaExpressionVariable(() =>
            {
                // ReSharper disable once AccessToModifiedClosure
                x_var_call_counter++;
                return 1;
            });
            expr.Compute();
            Assert.AreEqual(4, x_var_call_counter);
            x_var_call_counter = 0;
            expr.Compile()();
            Assert.AreEqual(4, x_var_call_counter);

            Assert.IsTrue(var_collection.Add(new ExpressionVariable("test1")));
            var var_test2 = new ExpressionVariable("test2");
            Assert.IsTrue(var_collection.Add(var_test2));
            Assert.IsTrue(var_collection.Add(new ExpressionVariable("test3")));
            Assert.IsFalse(var_collection.Add(new ExpressionVariable("test1")));
            Assert.IsFalse(var_collection.Add(new ExpressionVariable("x")));
            Assert.IsTrue(var_collection.Remove(var_test2));
            Assert.IsFalse(var_collection.Remove(var_test2));
            Assert.IsFalse(var_collection.Remove(var_collection["x"]));
            Assert.IsTrue(var_collection.Remove(var_collection["test3"]));

            var var_names = var_collection.Names.ToArray();
            Assert.AreEqual(5, var_collection.Count);
            Assert.AreEqual(5, var_names.Length);
            CollectionAssert.AreEqual(new[] { "y", "z", "q", "x", "test1" }, var_names);
            Assert.IsTrue(var_collection.Exist("x"));
            Assert.IsTrue(var_collection.Exist("test1"));
            Assert.IsFalse(var_collection.Exist("test2"));
            Assert.IsTrue(var_collection.Exist(v => v is LambdaExpressionVariable));
            Assert.IsFalse(var_collection.Exist(v => v is EventExpressionVariable));

            Assert.IsTrue(var_collection.ExistInTree("x"));
            Assert.IsTrue(var_collection.ExistInTree("y"));
            Assert.IsTrue(var_collection.ExistInTree("z"));
            Assert.IsTrue(var_collection.ExistInTree("q"));
            Assert.IsFalse(var_collection.ExistInTree("test1"));
#pragma warning disable 183
            // ReSharper disable IsExpressionAlwaysTrue
            Assert.IsTrue(var_collection.ExistInTree(v => v.Variable is ExpressionVariable));
            // ReSharper restore IsExpressionAlwaysTrue
#pragma warning restore 183
            Assert.IsTrue(var_collection.ExistInTree(v => v.Variable is LambdaExpressionVariable));
            Assert.IsFalse(var_collection.ExistInTree(v => v.Variable is EventExpressionVariable));

            Assert.AreEqual(4, var_collection.GetTreeNodes("x").Count());
            Assert.AreEqual(2, var_collection.GetTreeNodes("y").Count());
            Assert.AreEqual(1, var_collection.GetTreeNodes("z").Count());
            Assert.AreEqual(1, var_collection.GetTreeNodes("q").Count());
            Assert.AreEqual(0, var_collection.GetTreeNodes("test1").Count());

            Assert.AreEqual(4, var_collection.GetTreeNodes(vn => vn.Variable is LambdaExpressionVariable).Count());
            Assert.AreEqual(0, var_collection.GetTreeNodes(vn => vn.Variable is EventExpressionVariable).Count());

            Assert.AreEqual(8, var_collection.GetTreeNodesOf<ExpressionVariable>().Count());
            Assert.AreEqual(4, var_collection.GetTreeNodesOf<LambdaExpressionVariable>().Count());
            Assert.AreEqual(0, var_collection.GetTreeNodesOf<EventExpressionVariable>().Count());

            Assert.AreEqual(4, var_collection.GetTreeNodesVOf<ExpressionVariable>(v => v.Name == "x").Count());
            Assert.AreEqual(4, var_collection.GetTreeNodesVOf<LambdaExpressionVariable>(v => v.Name == "x").Count());
            Assert.AreEqual(2, var_collection.GetTreeNodesVOf<ExpressionVariable>(v => v.Name == "y").Count());
            Assert.AreEqual(3, var_collection.GetTreeNodesOf<ExpressionVariable>(vn => vn.Parent?.Parent is subtractionOperatorNode).Count());
            Assert.AreEqual(1, var_collection.GetTreeNodesOf<LambdaExpressionVariable>(vn => vn.Parent?.Parent is subtractionOperatorNode).Count());
        }

        /// <summary>Тестирование коллекции констант</summary>
        [TestMethod, Description("Тестирование коллекции констант")]
        public void ExpressionConstantCollection_Test()
        {
            var parser = new ExpressionParser();
            var expr = parser.Parse("2e^5sin(2pi*x)*3-pi");

            var const_collection = expr.Constants;
            Assert.IsNotNull(const_collection);
            var constants = const_collection.ToArray();
            Assert.AreEqual("e", constants[0].Name);
            Assert.AreEqual(Math.E, constants[0].Value);
            Assert.AreEqual("pi", constants[1].Name);
            Assert.AreEqual(Math.PI, constants[1].Value);

            parser.Constants.Add("c1", -13);
            expr = parser.Parse("13+c1");
            const_collection = expr.Constants;
            constants = const_collection.ToArray();
            Assert.AreEqual(1, constants.Length);
            Assert.AreEqual("c1", constants[0].Name);
            Assert.AreEqual(constants[0], expr.Constants["c1"]);
            Assert.AreEqual("c1", expr.Constants["c1"].Name);
            Assert.AreEqual(-13, constants[0].Value);
            Assert.AreEqual(-13, expr.Constants["c1"].Value);
            Assert.AreEqual(0, expr.Compute());
            Assert.AreEqual(0, expr.Compute(5));

            expr = parser.Parse("3+c2");
            Assert.AreEqual(0, expr.Constants.Count);
            Assert.AreEqual(1, expr.Variable.Count);
            expr.Variable.MoveToConstCollection("c2");
            Assert.AreEqual(1, expr.Constants.Count);
            Assert.AreEqual(0, expr.Variable.Count);
        }

        /// <summary>Тестирование коллекции функций</summary>
        [TestMethod, Description("Тестирование коллекции функций")]
        public void ExpressionFunctionCollection_Test()
        {
            var parser = new ExpressionParser();
            var expr = parser.Parse("10^5");
            var func_collection = expr.Functions;
            Assert.IsNotNull(func_collection);
            Assert.AreEqual(0, func_collection.Count);

            parser.FindFunction += (s, e) =>
            {
                if (e.Name == "sinc" && e.ArgumentCount == 1)
                    e.Function = new Func<double, double>(x => Math.Sin(x) / x);
            };
            expr = parser.Parse("2sin(2pi*x-3)/sinc(y)");
            func_collection = expr.Functions;
            Assert.AreEqual(2, func_collection.Count);
            var sin_func = func_collection["sin", 1];
            Assert.IsNotNull(sin_func);
            Assert.AreEqual("sin", sin_func.Name);
            var sinс_func = func_collection["sinc", 1];
            Assert.IsNotNull(sinс_func);
            Assert.AreEqual("sinc", sinс_func.Name);
        }

        /// <summary>Тестирование процесса распознавания функций</summary>
        [TestMethod, Description("Тестирование процесса распознавания функций")]
        public void ExpressionParser_FunctionParseTest()
        {
            var parser = new ExpressionParser();

            var expr = parser.Parse("sin(x)");
            var root = expr.Tree.Root;
            Assert.IsTrue(root is FunctionNode);
            var function_node = (FunctionNode)root;
            Assert.AreEqual("sin", function_node.Name);
            Assert.AreEqual(Math.Sin(0.7 * Math.PI), expr.Compute(0.7 * Math.PI));

            expr = parser.Parse("3cos(2pi*t-2)+1");
            root = expr.Tree.Root;
            var node = root["l/r"] as FunctionNode;
            Assert.IsNotNull(node);
            Assert.AreEqual("cos", node.Name);
            Assert.AreEqual(1, node.ArgumentsNames.Length);
            root = node.Arguments.First().Value;
            Assert.IsTrue(root is subtractionOperatorNode);

            expr = parser.Parse("2log(3,e)");
            root = expr.Tree.Root;
            node = root.Right as FunctionNode;
            Assert.IsNotNull(node);
            Assert.AreEqual("log", node.Name);
            Assert.AreEqual(2, node.ArgumentsNames.Length);
            var args = node.Arguments.Select(a => a.Value).ToArray();
            Assert.IsTrue(args[0] is ConstValueNode);
            Assert.IsTrue(args[1] is VariableValueNode);
            Assert.IsTrue(((VariableValueNode)args[1]).Variable.IsConstant);

            parser.FindFunction += (s, e) =>
            {
                switch (e.Name)
                {
                    case "test":
                        switch (e.ArgumentCount)
                        {
                            case 3:
                                e.Function = (Func<double, double, double, double>)((a, b, c) => a + b * c);
                                break;
                        }
                        break;
                    case "average":
                        switch (e.ArgumentCount)
                        {
                            case 2:
                                e.Function = (Func<double, double, double>)((a, b) => (a + b) / 2);
                                break;
                            case 3:
                                e.Function = (Func<double, double, double, double>)((a, b, c) => (a + b + c) / 3);
                                break;
                        }

                        break;
                }
            };

            var test_expr = parser.Parse("test(2,3,4)");
            Assert.AreEqual(14, test_expr.Compute());
            var average2_expr = parser.Parse("average(2,4)");
            Assert.AreEqual(3, average2_expr.Compute());
            var average3_expr = parser.Parse("average(2,4,6)");
            Assert.AreEqual(4, average3_expr.Compute());
        }

        /// <summary>Тестирование математических операторов для выражений +,-,*,/,^</summary>
        [TestMethod, Description("Тестирование математических операторов для выражений +,-,*,/,^")]
        public void MathExpression_OperatorsTest()
        {
            var parser = new ExpressionParser();

            var exprA = parser.Parse("3+7"); //10
            var exprB = parser.Parse("2+3"); //5

            var result = exprA * exprB;

            var root = result.Tree.Root;
            Assert.IsTrue(root is MultiplicationOperatorNode);
            Assert.IsTrue(root.Left is ComputedBracketNode);
            Assert.IsTrue(root["l/l"] is AdditionOperatorNode);
            Assert.IsTrue(root["l/l/l"] is ConstValueNode);
            Assert.IsTrue(root["l/l/r"] is ConstValueNode);
            Assert.IsTrue(root.Right is ComputedBracketNode);
            Assert.IsTrue(root["r/l"] is AdditionOperatorNode);
            Assert.IsTrue(root["r/l/l"] is ConstValueNode);
            Assert.IsTrue(root["r/l/r"] is ConstValueNode);

            Assert.AreEqual(50, result.Compute());
            Assert.AreEqual(15, (exprA + exprB).Compute());
            Assert.AreEqual(5, (exprA - exprB).Compute());
            Assert.AreEqual(2, (exprA / exprB).Compute());
            Assert.AreEqual(1e5, (exprA ^ exprB).Compute());
        }

        /// <summary>Тестирование процесса компиляции математического выражения</summary>
        [TestMethod, Description("Тестирование процесса компиляции математического выражения")]
        public void MathExpression_CompileTest()
        {
            var parser = new ExpressionParser();

            var expr = parser.Parse("5");
            var comp = expr.Compile();
            Assert.AreEqual(5, comp());
            comp = parser.Parse("7+3").Compile();
            Assert.AreEqual(10, comp());
            Assert.AreEqual(6, parser.Parse("2+2*2").Compile()());
            Assert.AreEqual(8, parser.Parse("(2+2)*2").Compile()());
            expr = parser.Parse("2x");
            comp = expr;
            Assert.AreEqual(0, comp());
            expr.Variable["x"] = 5;
            Assert.AreEqual(10, comp());
            var variable = expr.Variable["x"];
            variable.Value = 7;
            Assert.AreEqual(14, comp());
        }

        /// <summary>Строка математического выражения для нагрузочного тестирования</summary>
        private const string __StressTestExpressionStr = "(5 * x * x * x - 3 * x * x + 2 * x - 1) / (2 * x)";
        /// <summary>Таймаут на тестирование вычислений после процесса компиляции</summary>
        private const int __StressTest_CompileTimeout = 150;
        /// <summary>Таймаут на тестирование вычислений после процесса разбора мат.выражения</summary>
        private const int __StressTest_ComputeTimeout = 3900;

        /// <summary>Нагрузочное тестирование скомпилированного выражения</summary>
        [TestMethod, Timeout(__StressTest_CompileTimeout), Description("Нагрузочное тестирование скомпилированного выражения")]
        public void MathExpression_CompileStressTest()
        {
            var parser = new ExpressionParser();

            var expr = parser.Parse(__StressTestExpressionStr);
            const double x = 0.7;
            var func = expr.Compile<Func<double, double>>();
            Assert.AreEqual((5 * x * x * x - 3 * x * x + 2 * x - 1) / (2 * x), func(x));

            for (var i = 0; i < 250000; i++)
                func(x);
        }

        /// <summary>Нагрузочное тестирование вычисления выражения</summary>
        [TestMethod, Timeout(__StressTest_ComputeTimeout), Description("Нагрузочное тестирование вычисления выражения")]
        public void MathExpression_ComputeStressTest()
        {
            var parser = new ExpressionParser();

            var expr = parser.Parse(__StressTestExpressionStr);
            const double x = 0.7;
            Assert.AreEqual((5 * x * x * x - 3 * x * x + 2 * x - 1) / (2 * x), expr.Compute(x));

            for (var i = 0; i < 250000; i++)
                expr.Compute(x);
        }

        /// <summary>Тестирование оператора сложения двух выражений</summary>
        [TestMethod, Description("Тестирование оператора сложения двух выражений")]
        public void MathExpression_Operator_Addition_SimpleTest()
        {
            var parser = new ExpressionParser();
            var A = parser.Parse("5");
            var B = parser.Parse("7");
            var C = A + B;
            var tree = C.Tree;
            Assert.IsInstanceOfType(tree.Root, typeof(AdditionOperatorNode));
            Assert.IsInstanceOfType(tree.Root.Left, typeof(ConstValueNode));
            Assert.IsInstanceOfType(tree.Root.Right, typeof(ConstValueNode));
            Assert.AreEqual(5, ((ConstValueNode)tree.Root.Left).Value);
            Assert.AreEqual(7, ((ConstValueNode)tree.Root.Right).Value);
            Assert.AreEqual(5 + 7, C.Compute());
        }

        /// <summary>Тестирование оператора вычитания двух выражений</summary>
        [TestMethod, Description("Тестирование оператора вычитания двух выражений")]
        public void MathExpression_Operator_subtraction_SimpleTest()
        {
            var parser = new ExpressionParser();
            var A = parser.Parse("5");
            var B = parser.Parse("7");
            var C = A - B;
            var tree = C.Tree;
            Assert.IsInstanceOfType(tree.Root, typeof(subtractionOperatorNode));
            Assert.IsInstanceOfType(tree.Root.Left, typeof(ConstValueNode));
            Assert.IsInstanceOfType(tree.Root.Right, typeof(ConstValueNode));
            Assert.AreEqual(5, ((ConstValueNode)tree.Root.Left).Value);
            Assert.AreEqual(7, ((ConstValueNode)tree.Root.Right).Value);
            Assert.AreEqual(5 - 7, C.Compute());
        }

        /// <summary>Тестирование оператора умножения двух выражений</summary>
        [TestMethod, Description("Тестирование оператора умножения двух выражений")]
        public void MathExpression_Operator_Multiplycation_SimpleTest()
        {
            var parser = new ExpressionParser();
            var A = parser.Parse("5");
            var B = parser.Parse("7");
            var C = A * B;
            var tree = C.Tree;
            Assert.IsInstanceOfType(tree.Root, typeof(MultiplicationOperatorNode));
            Assert.IsInstanceOfType(tree.Root.Left, typeof(ConstValueNode));
            Assert.IsInstanceOfType(tree.Root.Right, typeof(ConstValueNode));
            Assert.AreEqual(5, ((ConstValueNode)tree.Root.Left).Value);
            Assert.AreEqual(7, ((ConstValueNode)tree.Root.Right).Value);
            Assert.AreEqual(5 * 7, C.Compute());
        }

        /// <summary>Тестирование оператора деления двух выражений</summary>
        [TestMethod, Description("Тестирование оператора деления двух выражений")]
        public void MathExpression_Operator_Division_SimpleTest()
        {
            var parser = new ExpressionParser();
            var A = parser.Parse("4");
            var B = parser.Parse("2");
            var C = A / B;
            var tree = C.Tree;
            Assert.IsInstanceOfType(tree.Root, typeof(DivisionOperatorNode));
            Assert.IsInstanceOfType(tree.Root.Left, typeof(ConstValueNode));
            Assert.IsInstanceOfType(tree.Root.Right, typeof(ConstValueNode));
            Assert.AreEqual(4, ((ConstValueNode)tree.Root.Left).Value);
            Assert.AreEqual(2, ((ConstValueNode)tree.Root.Right).Value);
            Assert.AreEqual(4 / 2, C.Compute());
        }

        /// <summary>Тестирование оператора сложения двух выражений</summary>
        [TestMethod, Description("Тестирование операторов")]
        public void MathExpression_Operator_ComplexTest()
        {
            var parser = new ExpressionParser();
            var A = parser.Parse("2");
            var B = parser.Parse("3");
            var C = parser.Parse("4");
            var D = A + B * C;
            Assert.AreEqual(2 + 3 * 4, D.Compute());

            D = A + B;
            D *= C;

            Assert.AreEqual((2 + 3) * 4, D.Compute());
        }

        //[TestMethod]
        //public void ExpressionParser_Mathematic_Test()
        //{
        //    var parser = new ExpressionParser();
        //    var expression_str = "sqrt(sin(3x))*cos(2x)^(3x)+tan(cos(sin(x)))";
        //    var expr = parser.Parse(expression_str);
        //    var result = expr.Compute(Math.PI / 4);
        //}

        [TestMethod]
        public void MathExpression_ComplexOperator_Summ_Test()
        {
            var parser = new ExpressionParser();

            var polynom = new Polynom(1, 3, 5, 7, 9, 11);

            var CoreFunctionFinded = false;
            var CoreFunctionExecuted = false;
            var LimitFunctionFinded = false;
            var LimitFunctionExecuted = false;

            var a_list = new List<(double index, double value)>(6);
            parser.FindFunction += (s, e) =>
            {
                if (e.SignatureEqual("a", 1))
                {
                    e.Function = new Func<double, double>(i =>
                    {
                        CoreFunctionExecuted = true;
                        var a = polynom[(int)i];
                        a_list.Add((i, a));
                        return a;
                    });
                    CoreFunctionFinded = true;
                }
                else if (e.SignatureEqual("Length", 1))
                {
                    e.Function = new Func<double, double>(y => { LimitFunctionExecuted = true; return polynom.Length; });
                    LimitFunctionFinded = true;
                }
            };

            var expr = parser.Parse("Sum[i=0..Length(a)]{a(i)*x^i}");
            Assert.IsTrue(LimitFunctionFinded);
            Assert.IsTrue(CoreFunctionFinded);

            var root = Assert.That.Value(expr.Tree.Root)
               .As<FunctionalNode>()
               .Where(node => node.Operator).Check(@operator => @operator.Is<SummOperator>())
               .ActualValue;

            {
                var parameters_expr = root.Parameters;
                var params_root = parameters_expr.Tree.Root as EqualityOperatorNode;
                Assert.IsNotNull(params_root);
                Assert.IsInstanceOfType(params_root.Left, typeof(VariableValueNode));
                Assert.IsNotNull(params_root.Left);
                Assert.AreEqual("i", ((VariableValueNode)params_root.Left).Variable.Name);
                Assert.IsInstanceOfType(params_root.Right, typeof(IntervalNode));

                Assert.That.Value(params_root.Right).As<IntervalNode>()
                   .Where(interval_node => interval_node.Left).Check(left => left.As<ConstValueNode>().Where(c => c.Value).IsEqual(0))
                   .Where(interval_node => interval_node.Right).Check(right => right.As<FunctionNode>()
                       .Where(function_node => function_node.ArgumentsNames.Length).Check(length => length.IsEqual(1))
                       .Where(function_node => function_node.Function).Check(function => function
                           .Where(func => func.Name).Check(name => name.IsEqual("Length"))
                           .Where(func => func.Delegate).Check(func => func.As<Func<double, double>>().Where(f => f.Invoke(0)))));

                 var interval = Assert.That.Value(params_root.Right).As<IntervalNode>().ActualValue;

                var length_function = Assert.That.Value(interval.Right).As<FunctionNode>().ActualValue;
                Assert.That.Value(((Func<double, double>)length_function.Function.Delegate).Invoke(0)).IsEqual(6);
                Assert.IsTrue(LimitFunctionExecuted);
                LimitFunctionExecuted = false;
                Assert.That.Value(length_function.Function.GetValue(new[] { 0.0 })).IsEqual(6);
                Assert.IsTrue(LimitFunctionExecuted);

                Assert.IsTrue(parameters_expr.Variable.Exist("a"));
            }

            {
                var core_expr = root.Core;
                var core_root = core_expr.Tree.Root as MultiplicationOperatorNode;
                Assert.IsNotNull(core_root);
            }

            const double x = 2;
            var p = polynom.Value(x);
            LimitFunctionExecuted = false;
            CoreFunctionExecuted = false;
            var result = expr.Compute(x);
            Assert.IsTrue(LimitFunctionExecuted);
            Assert.IsTrue(CoreFunctionExecuted);
            Assert.AreEqual(p, result,
                "Значение, полученное от полинома и мат.выражения не совпадают.\r\n" +
                $"Ошибка {Math.Abs(p - result)}({Math.Abs(p - result) / p:p})");
        }
    }
}
