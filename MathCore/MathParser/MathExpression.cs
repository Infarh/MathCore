#nullable enable
using System;
using System.Collections.Generic;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
using System.Linq;
using System.Linq.Expressions;
using MathCore.Annotations;
using MathCore.MathParser.ExpressionTrees;
using MathCore.MathParser.ExpressionTrees.Nodes;
// ReSharper disable ClassCanBeSealed.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable ConvertToAutoProperty
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.MathParser
{
    /// <summary>Математическое выражение</summary>
    public class MathExpression : IDisposable, ICloneable<MathExpression>
    {
        /// <summary>Дерево математического выражения</summary>
        private ExpressionTree _ExpressionTree = null!;

        /// <summary>Коллекция переменных математического выражения</summary>
        private readonly VariablesCollection _Variables;

        /// <summary>Коллекция констант математического выражения</summary>
        private readonly ConstantsCollection _Constants;

        /// <summary>Коллекция функций, участвующих в выражении</summary>
        private readonly FunctionsCollection _Functions;

        /// <summary>Коллекция функционалов</summary>
        private readonly FunctionalsCollection _Functionals;

        /// <summary>Имя выражения</summary>
        private string _Name;

        /// <exception cref="ArgumentNullException" accessor="set"><paramref name="value"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException" accessor="set">Указано пустое имя функции</exception>
        public string Name
        {
            get => _Name;
            set
            {
                if (value is null)
                    throw new ArgumentNullException(nameof(value), @"Не указано имя функции");
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException(@"Указано пустое имя функции", nameof(value));
                _Name = value;
            }
        }

        /// <summary>Является ли выражение предвычислимым?</summary>
        public bool IsPrecomputable => _ExpressionTree.Root.IsPrecomputable;

        /// <summary>Дерево математического выражения</summary>
        public ExpressionTree Tree { [DST] get => _ExpressionTree; [DST] set => _ExpressionTree = value; }

        /// <summary>Перечисление узлов дерева, содержащих переменные</summary>
        public IEnumerable<VariableValueNode> VariableNodes => _ExpressionTree.OfType<VariableValueNode>();

        /// <summary>Переменные, входящие в математическое выражение</summary>
        public VariablesCollection Variable => _Variables;

        /// <summary>Константы, входящие в математическое выражение</summary>
        public ConstantsCollection Constants => _Constants;

        /// <summary>Коллекция функций, участвующих в выражении</summary>
        public FunctionsCollection Functions => _Functions;

        /// <summary>Коллекция функционалов</summary>
        public FunctionalsCollection Functionals => _Functionals;

        /// <summary>Инициализация пустого математического выражения</summary>
        /// <param name="Name">Имя функции</param>
        public MathExpression(string Name = "f")
        {
            _Name = Name;
            _Variables = new VariablesCollection(this);     // Коллекция переменных
            _Constants = new ConstantsCollection();         // Коллекция констант
            _Functions = new FunctionsCollection();         // Коллекция функций
            _Functionals = new FunctionalsCollection();     // Коллекция функционалов
        }

        /// <summary>Инициализация нового математического выражения</summary>
        /// <param name="Tree">Дерево математического выражения</param>
        /// <param name="Name">Имя функции</param>
        private MathExpression(ExpressionTree Tree, string Name = "f")
            : this(Name)
        {
            _ExpressionTree = Tree; //Сохраняем ссылку на корень дерева

            foreach (var tree_node in Tree) // обходим все элементы дерева
                switch (tree_node)
                {
                    case VariableValueNode value_node:
                        var variable = value_node.Variable;      // Извлечь переменную
                        if (variable.IsConstant)                 // Если переменная - константа
                            _Constants.Add(variable);            //   сохранить в коллекции констант
                        else                                     //  иначе...
                            _Variables.Add(variable);            //   сохранить в коллекции переменных
                        break;
                    case FunctionNode function_node:
                        _Functions.Add(function_node.Function);  // то сохранить функцию в коллекции
                        break;
                }
        }

        /// <summary>Инициализация нового математического выражения</summary>
        /// <param name="StrExpression">Строковое представление выражения</param>
        /// <param name="Parser">Ссылка на парсер</param>
        internal MathExpression(string StrExpression, ExpressionParser Parser)
            : this()
        {
            var terms = new BlockTerm(StrExpression);    // разбить строку на элементы
            var root = terms.GetSubTree(Parser, this);   // выделить корень дерева из первого элемента
            _ExpressionTree = new ExpressionTree(root); // Создать дерево выражения из корня
        }

        #region IDisposable

        /// <summary>Уничтожить математическое выражение</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _Disposed;

        /// <summary>Освобождение ресурсов выражения</summary>
        /// <param name="disposing">Требуется ли выполнить освобождение управляемых ресурсов?</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_Disposed || !disposing) return;
            _Disposed = true;
            _ExpressionTree.Dispose();
        }

        #endregion

        /// <summary>Вычисление математического выражения</summary>
        /// <returns>Значение выражения</returns>
        public double Compute() => ((ComputedNode)_ExpressionTree.Root).Compute();

        /// <summary>Вычисление математического выражения</summary>
        /// <returns>Значение выражения</returns>
        public double Compute(params double[] arg)
        {
            for (int i = 0, arg_count = arg.Length, var_count = Variable.Count; i < arg_count && i < var_count; i++)
                Variable[i].Value = arg[i];
            return ((ComputedNode)_ExpressionTree.Root).Compute();
        }

        /// <summary>Компиляция математического выражения в функцию без параметров</summary>
        /// <returns>Функция типа double func(void) без параметров</returns>
        public Func<double> Compile() => Compile<Func<double>>();

        /// <summary>Компиляция функции одной переменной</summary>
        /// <returns>Делегат функции одной переменной</returns>
        public Func<double, double> Compile1() => Compile<Func<double, double>>();

        /// <summary>Компиляция функции двух переменных</summary>
        /// <returns>Делегат функции двух переменных</returns>
        public Func<double, double, double> Compile2() => Compile<Func<double, double, double>>();

        /// <summary>Компиляция функции трёх переменных</summary>
        /// <returns>Делегат функции трёх переменных</returns>
        public Func<double, double, double, double> Compile3() => Compile<Func<double, double, double, double>>();

        /// <summary>Компиляция математического выражения в функцию указанного типа</summary>
        /// <param name="ArgumentName">Список имён параметров</param>
        /// <returns>Делегат скомпилированного выражения</returns>
        public Delegate Compile(params string[] ArgumentName) =>
            Expression.Lambda(GetExpression(out var vars, ArgumentName), vars).Compile();

        /// <summary>Многопараметрическая компиляция мат.выражения</summary>
        /// <param name="ArgumentName">Массив имён компилируемых параметров</param>
        /// <returns>Делегат функции, принимающий на вход массив значений параметров</returns>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public Func<double[], double> CompileMultiParameters(params string[] ArgumentName)
        {
            // Словарь индексов переменных
            var var_dictionary = new Dictionary<string, int>();

            // Выходной массив параметров выражения, получаемый при сборке дерева
            // Сборка дерева
            var compilation = GetExpression(out _, ArgumentName);

            // Если массив имён компилируемых входных переменных не пуст
            if (ArgumentName.Length > 0)
                ArgumentName.Foreach(var_dictionary.Add); // то заполняем словарь индексов
            else // Если массив переменных не указан, то создаём функцию по всем переменным
                Variable.Select(v => v.Name).Foreach(var_dictionary.Add);

            // Создаём входную переменную выражения с типом "вещественный массив"
            var array_parameter = Expression.Parameter(typeof(double[]), "args_array");
            // Создаём пул выражений - индексаторов входного массива
            var array_indexers = new Dictionary<int, Expression>();
            // Объявляем метод получения выражения индексатора входного массива по указанному значению индекса
            Expression GetIndexedParameter(int index)
            {
                // Если пул уже содержит указанный индекс, то выдать соответствующий индексатор
                if (array_indexers.ContainsKey(index)) return array_indexers[index];
                // иначе создаём новый индексатор
                var indexer = Expression.ArrayIndex(array_parameter, Expression.Constant(index));
                array_indexers.Add(index, indexer); // и сохраняем его в пуле
                return indexer; // возвращаем новый индексатор
            }

            // Создаём пересборщик дерева выражения Linq.Expression 
            var rebuilder = new ExpressionRebuilder();
            // Добавляем обработчик узлов вызова методов
            rebuilder.MethodCallVisited += (s, e) => // Если очередной узел дерева - вызов метода, то...
            {
                var call = e.Argument; // Извлекаем ссылку на узел
                //Если целевой объект вызова - не(!) константное значение и оно не соответствует типу переменной дерева MathExpressionTree 
                if (!(call.Object is ConstantExpression { Value: ExpressionVariable } constant))
                    return call; // пропускаем узел
                //Извлекаем из узла переменную дерева
                var v = (ExpressionVariable)((ConstantExpression)call.Object).Value;
                //Если переменная дерева - константа, либо если её имя отсутствует в словаре компилируемых переменных
                if (v.IsConstant || !var_dictionary.ContainsKey(v.Name)) return call; // то пропускаем узел
                var index = var_dictionary[v.Name]; // Запрашиваем индекс переменной
                var indexer = GetIndexedParameter(index); // Извлекаем индексатор из пула по указанному индексу
                return indexer; // заменяем текущий узел индексатором
            };

            compilation = rebuilder.Visit(compilation); // Пересобираем дерево
            // Собираем лямбда-выражение
            var lambda = Expression.Lambda<Func<double[], double>>(compilation, array_parameter);
            return lambda.Compile(); // Компилируем лямбда-выражение и возвращаем делегат
        }

        /// <summary>Компиляция математического выражения в функцию указанного типа</summary>
        /// <typeparam name="TDelegate">Тип делегата функции</typeparam>
        /// <param name="ArgumentName">Список имён параметров</param>
        /// <returns>Делегат скомпилированного выражения</returns>
        [NotNull]
        public TDelegate Compile<TDelegate>(params string[] ArgumentName)
        {
            var compilation = GetExpression<TDelegate>(out var vars, ArgumentName);
            return Expression.Lambda<TDelegate>(compilation, vars).Compile();
        }

        /// <summary>Получить Linq.Expression выражение, построенное на основе дерева выражений</summary>
        /// <param name="vars">Список входных переменных</param>
        /// <param name="ArgumentName">Список имён аргументов</param>
        /// <returns>Выражение типа Linq.Expression</returns>
        public Expression GetExpression(out ParameterExpression[] vars, params string[] ArgumentName)
        {
            vars = ArgumentName.Select(name => Expression.Parameter(typeof(double), name)).ToArray();
            return ((ComputedNode)_ExpressionTree.Root).Compile(vars);
        }

        /// <summary>Получить Linq.Expression выражение, построенное на основе дерева выражений</summary>
        /// <typeparam name="TDelegate">Тип делегата выражения</typeparam>
        /// <param name="vars">Список входных переменных</param>
        /// <param name="ArgumentName">Список имён аргументов</param>
        /// <returns>Выражение типа Linq.Expression</returns>
        public Expression GetExpression<TDelegate>(
            [CanBeNull] out ParameterExpression[] vars,
            params string[] ArgumentName)
        {
            var t = typeof(TDelegate);
            vars = null;
            if (ArgumentName.Length == 0)
            {
                var args = t.GetGenericArguments();
                if (args.Length > 1)
                {
                    vars = new ParameterExpression[Math.Min(args.Length - 1, Variable.Count)];
                    for (var i = 0; i < vars.Length; i++)
                        vars[i] = Expression.Parameter(typeof(double), Variable[i].Name);
                }
            }
            else
                vars = ArgumentName.Select(name => Expression.Parameter(typeof(double), name)).ToArray();
            var compilation = vars is null
                ? ((ComputedNode)_ExpressionTree.Root).Compile()
                : ((ComputedNode)_ExpressionTree.Root).Compile(vars);
            return compilation;
        }

        /// <summary>Преобразование в строку</summary>
        /// <returns>Строковое представление</returns>
        public override string ToString() => $"{_Name}({_Variables.Select(v => v.Name).ToSeparatedStr(", ")})={_ExpressionTree.Root}";

        /// <summary>Перенос констант из выражения источника в выражение приёмник</summary>
        /// <param name="Source">Выражение источник</param>
        /// <param name="Result">Выражение приёмник</param>
        private static void CheckConstantsCollection(MathExpression Source, MathExpression Result) =>
            Source.Constants
               .Select(constant => Result.Variable[constant.Name])
               .Where(c => Result.Variable.Remove(c))
               .Foreach(Result.Constants, (c, constants) => constants.Add(c));

        /// <summary>Клонирование выражения</summary>
        /// <returns>Копия объектной модели выражения</returns>
        public MathExpression Clone()
        {
            var result = new MathExpression(_ExpressionTree.Clone());
            CheckConstantsCollection(this, result);
            return result;
        }

        object ICloneable.Clone() => Clone();

        /// <summary>Комбинация двух выражений с использованием узла-оператора</summary>
        /// <param name="x">Первое выражение</param>
        /// <param name="y">Второе выражение</param>
        /// <param name="node">Узел операции</param>
        /// <returns>Математическое выражение, в корне дерева которого лежит узел оператора. Поддеревья - корни первого и второго выражений</returns>
        protected static MathExpression CombineExpressions(MathExpression x, MathExpression y, OperatorNode node)
        {
            var x_tree = x.Tree.Clone();
            var y_tree = y.Tree.Clone();

            if (x_tree.Root is OperatorNode x_operator_node && x_operator_node.Priority < node.Priority)
                x_tree.Root = new ComputedBracketNode(Bracket.NewRound, x_operator_node);
            if (y_tree.Root is OperatorNode y_operator_node_root && y_operator_node_root.Priority < node.Priority)
                y_tree.Root = new ComputedBracketNode(Bracket.NewRound, y_operator_node_root);

            node.Left = x_tree.Root;
            node.Right = y_tree.Root;

            var z = new MathExpression(new ExpressionTree(node));
            CheckConstantsCollection(x, z);
            CheckConstantsCollection(y, z);
            return z;
        }

        /// <summary>Оператор сложения двух выражений</summary>
        /// <param name="x">Первое слагаемое</param>
        /// <param name="y">Второе слагаемое</param>
        /// <returns>Выражение-сумма, корень которого - узел суммы. Поддеревья - корни выражений слагаемых</returns>
        public static MathExpression operator +(MathExpression x, MathExpression y) => CombineExpressions(x, y, new AdditionOperatorNode());

        /// <summary>Оператор вычитания двух выражений</summary>
        /// <param name="x">Уменьшаемое</param>
        /// <param name="y">Вычитаемое</param>
        /// <returns>Выражение-разность, корень которого - узел разности. Поддеревья - корни выражений вычитаемого и уменьшаемого</returns>
        public static MathExpression operator -(MathExpression x, MathExpression y) => CombineExpressions(x, y, new subtractionOperatorNode());

        /// <summary>Оператор умножения двух выражений</summary>
        /// <param name="x">Первый сомножитель</param>
        /// <param name="y">Второй сомножитель</param>
        /// <returns>Выражение-произведения, корень которого - узел произведения. Поддеревья - корни выражений сомножителей</returns>
        public static MathExpression operator *(MathExpression x, MathExpression y) => CombineExpressions(x, y, new MultiplicationOperatorNode());

        /// <summary>Оператор деления двух выражений</summary>
        /// <param name="x">Делимое</param>
        /// <param name="y">Делитель</param>
        /// <returns>Выражение-частное, корень которого - узел деления. Поддеревья - корни выражений делимого и делителя</returns>
        public static MathExpression operator /(MathExpression x, MathExpression y) => CombineExpressions(x, y, new DivisionOperatorNode());

        /// <summary>Оператор возведения в степень</summary>
        /// <param name="x">Основание</param>
        /// <param name="y">Показатель степени</param>
        /// <returns>Выражение-степень, корень которого - узел степени. Поддеревья - корни выражений Основания и показателя степени</returns>
        public static MathExpression operator ^(MathExpression x, MathExpression y) => CombineExpressions(x, y, new PowerOperatorNode());

        /// <summary>Оператор неявного приведения типов математического выражения к типу дерева выражения</summary>
        /// <param name="Expression">Математическое выражение</param>
        /// <returns>Дерево математического выражения</returns>
        public static implicit operator ExpressionTree(MathExpression Expression) => Expression.Tree;

        /// <summary>Оператор неявного приведения типов дерева выражения к типу математического выражения</summary>
        /// <param name="Tree">Дерево математического выражения</param>
        /// <returns>Математическое выражение, содержащее указанное дерево</returns>
        public static implicit operator MathExpression(ExpressionTree Tree) => new(Tree);

        /// <summary>Оператор неявного приведения типов математического выражения к типу делегата функции double Func(void)</summary>
        /// <param name="expr">Математическое выражения</param>
        /// <returns>Результат компиляции математического выражения</returns>
        public static implicit operator Func<double>(MathExpression expr) => expr.Compile();
    }
}