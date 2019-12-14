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

namespace MathCore.MathParser
{
    /// <summary>Математическое выражение</summary>
    public class MathExpression : IDisposable, ICloneable<MathExpression>
    {
        /// <summary>Дерево математического выражения</summary>
        private ExpressionTree _ExpressionTree;

        /// <summary>Коллекция переменных математического выражения</summary>
        [NotNull]
        private readonly VariabelsCollection _Variables;

        /// <summary>Коллекция констант математического выражения</summary>
        [NotNull]
        private readonly ConstantsCollection _Constants;

        /// <summary>Коллекция функций, участвующих в выражении</summary>
        [NotNull]
        private readonly FunctionsCollection _Functions;

        /// <summary>Коллекция функционалов</summary>
        [NotNull]
        private readonly FunctionalsCollection _Functionals;

        /// <summary>Имя выражения</summary>
        [NotNull]
        private string _Name;

        /// <exception cref="ArgumentNullException" accessor="set"><paramref name="value"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException" accessor="set">Указано пустое имя функции</exception>
        [NotNull]
        public string Name
        {
            get => _Name;
            set
            {
                if(value is null)
                    throw new ArgumentNullException(nameof(value), @"Не указано имя функции");
                if(string.IsNullOrEmpty(value))
                    throw new ArgumentException(@"Указано пустое имя функции", nameof(value));
                _Name = value;
            }
        }

        /// <summary>Является ли выражение предвычислимым?</summary>
        public bool IsPrecomputable => _ExpressionTree.Root.IsPrecomputable;

        /// <summary>Дерево математического выражения</summary>
        [NotNull]
        public ExpressionTree Tree { [DST] get => _ExpressionTree; [DST] set => _ExpressionTree = value; }

        /// <summary>Переменные, входящие в математическое выражение</summary>
        [NotNull]
        public VariabelsCollection Variable => _Variables;

        /// <summary>Константы, входящие в математическое выражение</summary>
        [NotNull]
        public ConstantsCollection Constants => _Constants;

        /// <summary>Коллекция функций, участвующих в выражении</summary>
        [NotNull]
        public FunctionsCollection Functions => _Functions;

        /// <summary>Коллекция функционалов</summary>
        [NotNull]
        public FunctionalsCollection Functionals => _Functionals;

        /// <summary>Инициализация пустого математического выражения</summary>
        /// <param name="Name">Имя функции</param>
        public MathExpression([NotNull] string Name = "f")
        {
            _Name = Name;
            _Variables = new VariabelsCollection(this);     // Коллекция переменных
            _Constants = new ConstantsCollection(this);     // Коллекция констант
            _Functions = new FunctionsCollection(this);     // Коллекция функций
            _Functionals = new FunctionalsCollection(this); // Коллекция функционалов
        }

        /// <summary>Инициализация нового математического выражения</summary>
        /// <param name="Tree">Дерево математического выражения</param>
        /// <param name="Name">Имя функции</param>
        private MathExpression([NotNull] ExpressionTree Tree, [NotNull] string Name = "f")
            : this(Name)
        {
            _ExpressionTree = Tree; //Сохраняем ссылку на корень дерева

            foreach(var tree_node in Tree) // обходим все элементы дерева
            {
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
        }

        /// <summary>Инициализация нового математического выражения</summary>
        /// <param name="StrExpression">Строковое представление выражения</param>
        /// <param name="Parser">Ссылка на парсер</param>
        internal MathExpression([NotNull] string StrExpression, [NotNull] ExpressionParser Parser)
            : this()
        {
            var terms = new BlockTerm(StrExpression);    // разбить строку на элементы
            var root = terms.GetSubTree(Parser, this);   // выделить корень дерева из первого элемента
            _ExpressionTree = new ExpressionTree(root); // Создать дерево выражения из корня
        }

        /// <summary>Уничтожить математическое выражение</summary>
        void IDisposable.Dispose() => _ExpressionTree.Dispose();

        /// <summary>Вычисление математического выражения</summary>
        /// <returns>Значение выражения</returns>
        public double Compute() => ((ComputedNode)_ExpressionTree.Root).Compute();

        /// <summary>Вычисление математического выражения</summary>
        /// <returns>Значение выражения</returns>
        public double Compute([NotNull] params double[] arg)
        {
            for(int i = 0, arg_count = arg.Length, var_count = Variable.Count; i < arg_count && i < var_count; i++)
                Variable[i].Value = arg[i];
            return ((ComputedNode)_ExpressionTree.Root).Compute();
        }

        /// <summary>Компиляция математического выражения в функцию без параметров</summary>
        /// <returns>Функция типа double func(void) без параметров</returns>
        [NotNull]
        public Func<double> Compile() => Compile<Func<double>>();

        /// <summary>Компиляция функции одной переменной</summary>
        /// <returns>Делегат функции одной переменной</returns>
        [NotNull]
        public Func<double, double> Compile1() => Compile<Func<double, double>>();

        /// <summary>Компиляция функции двух переменных</summary>
        /// <returns>Делегат функции двух переменных</returns>
        [NotNull]
        public Func<double, double, double> Compile2() => Compile<Func<double, double, double>>();

        /// <summary>Компиляция функции трёх переменных</summary>
        /// <returns>Делегат функции трёх переменных</returns>
        [NotNull]
        public Func<double, double, double, double> Compile3() => Compile<Func<double, double, double, double>>();

        /// <summary>Компиляция математического выражения в функцию указанного типа</summary>
        /// <param name="ArgumentName">Список имён параметров</param>
        /// <returns>Делегат скомпилированного выражения</returns>
        [NotNull]
        public Delegate Compile([NotNull] params string[] ArgumentName) => 
            Expression.Lambda(GetExpression(out var vars, ArgumentName), vars).Compile();

        /// <summary>Многопараметрическая компиляция мат.выражения</summary>
        /// <param name="ArgumentName">Массив имён компилируемых параметров</param>
        /// <returns>Делегат функции, принимающий на вход массив значений параметров</returns>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        [NotNull]
        public Func<double[], double> CompileMultyParameters([NotNull] params string[] ArgumentName)
        {
            // Словарь индексов переменных
            var var_dictionary = new Dictionary<string, int>();

            // Выходной массив параметров выражения, получаемый при сборке дерева
            // Сборка дерева
            var compilation = GetExpression(out _, ArgumentName);

            // Если массив имён компилируемых входных переменных не пуст
            if((ArgumentName.Length) > 0)
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
                if(!(call.Object is ConstantExpression constant && constant.Value is ExpressionVariabel))
                    return call; // пропускаем узел
                //Извлекаем из узла переменную дерева
                var v = (ExpressionVariabel)((ConstantExpression)call.Object).Value;
                //Если переменная дерева - константа, либо если её имя отсутствует в словаре компилируемых переменных
                if(v.IsConstant || !var_dictionary.ContainsKey(v.Name)) return call; // то пропускаем узел
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
        public TDelegate Compile<TDelegate>([NotNull] params string[] ArgumentName)
        {
            var compilation = GetExpression<TDelegate>(out var vars, ArgumentName);
            return Expression.Lambda<TDelegate>(compilation, vars).Compile();
        }

        /// <summary>Получить Linq.Expression выражение, построенное на основе дерева выражений</summary>
        /// <param name="vars">Список входных переменных</param>
        /// <param name="ArgumentName">Список имён аргументов</param>
        /// <returns>Выражение типа Linq.Expression</returns>
        [NotNull]
        public Expression GetExpression([NotNull] out ParameterExpression[] vars, [NotNull] params string[] ArgumentName)
        {
            vars = ArgumentName.Select(name => Expression.Parameter(typeof(double), name)).ToArray();
            return ((ComputedNode)_ExpressionTree.Root).Compile(vars);
        }

        /// <summary>Получить Linq.Expression выражение, построенное на основе дерева выражений</summary>
        /// <typeparam name="TDelegate">Тип делегата выражения</typeparam>
        /// <param name="vars">Список входных переменных</param>
        /// <param name="ArgumentName">Список имён аргументов</param>
        /// <returns>Выражение типа Linq.Expression</returns>
        [NotNull]
        public Expression GetExpression<TDelegate>(
            [CanBeNull] out ParameterExpression[] vars,
            [NotNull] params string[] ArgumentName)
        {
            var t = typeof(TDelegate);
            vars = null;
            if(ArgumentName.Length == 0)
            {
                var args = t.GetGenericArguments();
                if(args.Length > 1)
                {
                    vars = new ParameterExpression[Math.Min(args.Length - 1, Variable.Count)];
                    for(var i = 0; i < vars.Length; i++)
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
        [NotNull] public override string ToString() => $"{_Name}({_Variables.Select(v => v.Name).ToSeparatedStr(", ")})={_ExpressionTree.Root}";

        /// <summary>Перенос констант из выражения источника в выражение приёмник</summary>
        /// <param name="Source">Выражение источник</param>
        /// <param name="Result">Выражение приёмник</param>
        private static void CheckConstatnsCollection([NotNull] MathExpression Source, [NotNull] MathExpression Result) =>
            Source.Constants
               .Select(constant => Result.Variable[constant.Name])
               .Where(c => Result.Variable.Remove(c))
               .Foreach(Result.Constants, (c, constants) => constants.Add(c));

        /// <summary>Клонирование выражения</summary>
        /// <returns>Копия объектной модели выражения</returns>
        [NotNull]
        public MathExpression Clone()
        {
            var result = new MathExpression(_ExpressionTree.Clone());
            CheckConstatnsCollection(this, result);
            return result;
        }

        object ICloneable.Clone() => Clone();

        /// <summary>Комбинация двух выражений с использованием узла-оператора</summary>
        /// <param name="x">Первое выражение</param>
        /// <param name="y">Второе выражение</param>
        /// <param name="node">Узел операции</param>
        /// <returns>Математическое выражение, в корне дерева которого лежит узел оператора. Поддеревья - корни первого и второго выражений</returns>
        [NotNull]
        protected static MathExpression CombineExpressions([NotNull] MathExpression x, [NotNull] MathExpression y, [NotNull] OperatorNode node)
        {
            var x_tree = x.Tree.Clone();
            var y_tree = y.Tree.Clone();

            if(x_tree.Root is OperatorNode x_operator_node && x_operator_node.Priority < node.Priority)
                x_tree.Root = new ComputedBracketNode(Bracket.NewRound, x_operator_node);
            if(y_tree.Root is OperatorNode y_operator_node_root && y_operator_node_root.Priority < node.Priority)
                y_tree.Root = new ComputedBracketNode(Bracket.NewRound, y_operator_node_root);

            node.Left = x_tree.Root;
            node.Right = y_tree.Root;

            var z = new MathExpression(new ExpressionTree(node));
            CheckConstatnsCollection(x, z);
            CheckConstatnsCollection(y, z);
            return z;
        }

        /// <summary>Оператор сложения двух выражений</summary>
        /// <param name="x">Первое слагаемое</param>
        /// <param name="y">Второе слагаемое</param>
        /// <returns>Выражение-сумма, корень которого - узел суммы. Поддеревья - корни выражений слагаемых</returns>
        [NotNull]
        public static MathExpression operator +([NotNull] MathExpression x, [NotNull] MathExpression y) => CombineExpressions(x, y, new AdditionOperatorNode());

        /// <summary>Оператор вычитания двух выражений</summary>
        /// <param name="x">Уменьшаемое</param>
        /// <param name="y">Вычитаемое</param>
        /// <returns>Выражение-разность, корень которого - узел разности. Поддеревья - корни выражений вычитаемого и уменьшаемого</returns>
        [NotNull]
        public static MathExpression operator -([NotNull] MathExpression x, [NotNull] MathExpression y) => CombineExpressions(x, y, new SubstractionOperatorNode());

        /// <summary>Оператор умножения двух выражений</summary>
        /// <param name="x">Первый сомножитель</param>
        /// <param name="y">Второй сомножитель</param>
        /// <returns>Выражение-произведения, корень которого - узел произведения. Поддеревья - корни выражений сомножителей</returns>
        [NotNull]
        public static MathExpression operator *([NotNull] MathExpression x, [NotNull] MathExpression y) => CombineExpressions(x, y, new MultiplicationOperatorNode());

        /// <summary>Оператор деления двух выражений</summary>
        /// <param name="x">Делимое</param>
        /// <param name="y">Делитель</param>
        /// <returns>Выражение-частное, корень которого - узел деления. Поддеревья - корни выражений делимого и делителя</returns>
        [NotNull]
        public static MathExpression operator /([NotNull] MathExpression x, [NotNull] MathExpression y) => CombineExpressions(x, y, new DivisionOperatorNode());

        /// <summary>Оператор возведения в степень</summary>
        /// <param name="x">Основание</param>
        /// <param name="y">Показатель степени</param>
        /// <returns>Выражение-степень, корень которого - узел степени. Поддеревья - корни выражений Основания и показателя степени</returns>
        [NotNull]
        public static MathExpression operator ^([NotNull] MathExpression x, [NotNull] MathExpression y) => CombineExpressions(x, y, new PowerOperatorNode());

        /// <summary>Оператор неявного приведения типов математического выражения к типу дерева выражения</summary>
        /// <param name="Expression">Математическое выражение</param>
        /// <returns>Дерево математического выражения</returns>
        [NotNull]
        public static implicit operator ExpressionTree([NotNull] MathExpression Expression) => Expression.Tree;

        /// <summary>Оператор неявного приведения типов дерева выражения к типу математического выражения</summary>
        /// <param name="Tree">Дерево математического выражения</param>
        /// <returns>Математическое выражение, содержащее указанное дерево</returns>
        [NotNull]
        public static implicit operator MathExpression([NotNull] ExpressionTree Tree) => new MathExpression(Tree);

        /// <summary>Оператор неявного приведения типов математического выражения к типу делегата функции double Func(void)</summary>
        /// <param name="expr">Математическое выражения</param>
        /// <returns>Результат компиляции математического выражения</returns>
        [NotNull]
        public static implicit operator Func<double>([NotNull] MathExpression expr) => expr.Compile();
    }
}