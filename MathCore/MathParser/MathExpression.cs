using System;
using System.Collections.Generic;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
using System.Diagnostics.Contracts;
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

        /// <summary>Коллекция функций, учавствующих в выражении</summary>
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
                if(value == null)
                    throw new ArgumentNullException(nameof(value), @"Не указано имя функции");
                if(string.IsNullOrEmpty(value))
                    throw new ArgumentException(@"Указано пустое имя функции", nameof(value));
                Contract.EndContractBlock();
                _Name = value;
            }
        }

        /// <summary>Является ли выражение предвычислимым?</summary>
        public bool IsPrecomputable => _ExpressionTree.Root.IsPrecomputable;

        /// <summary>Дерево математического выражения</summary>
        [NotNull]
        public ExpressionTree Tree
        {
            [DST]
            get
            {
                Contract.Ensures(Contract.Result<ExpressionTree>() != null);
                return _ExpressionTree;
            }
            [DST]
            set
            {
                Contract.Requires(value != null);
                _ExpressionTree = value;
            }
        }

        /// <summary>Переменные, входящие в математическое выражение</summary>
        [NotNull]
        public VariabelsCollection Variable => _Variables;

        /// <summary>Константы, входящие в математическое выражение</summary>
        [NotNull]
        public ConstantsCollection Constants => _Constants;

        /// <summary>Коллекция функций, учавствующих в выражении</summary>
        [NotNull]
        public FunctionsCollection Functions => _Functions;

        /// <summary>Коллекция функционалов</summary>
        [NotNull]
        public FunctionalsCollection Functionals => _Functionals;

        /// <summary>Инициализация пустого математического выражения</summary>
        /// <param name="Name">Имя функции</param>
        public MathExpression([NotNull] string Name = "f")
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(Name));
            _Name = Name;
            _Variables = new VariabelsCollection(this); // Коллекция переменных
            _Constants = new ConstantsCollection(this); // Коллекция констант
            _Functions = new FunctionsCollection(this);     // Коллекция функций
            _Functionals = new FunctionalsCollection(this); // Коллекция функционалов
        }

        /// <summary>Инициализация нового математического выражения</summary>
        /// <param name="Tree">Дерево математического выражения</param>
        /// <param name="Name">Имя функции</param>
        private MathExpression([NotNull] ExpressionTree Tree, [NotNull] string Name = "f")
            : this(Name)
        {
            Contract.Requires(Tree != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(Name));
            _ExpressionTree = Tree; //Сохраняем ссылку на корень дерева

            foreach(var tree_node in Tree) // обходим все элементы дерева
            {
                switch (tree_node)
                {
                    case VariableValueNode value_node:
                        var variabel = value_node.Variable;      // Извлечь переменную
                        if(variabel.IsConstant)                  // Если переменная - константа
                            _Constants.Add(variabel);           //   сохранить в коллекции констант
                        else                                     //  иначе...
                            _Variables.Add(variabel);           //   сохранить в коллекции переменных
                        break;
                    case FunctionNode function_node:
                        _Functions.Add(function_node.Function); // то сохранить функцию в коллекции
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
            Contract.Requires(!string.IsNullOrEmpty(StrExpression));
            Contract.Requires(Parser != null);
            Contract.Ensures(Tree != null);
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
            Contract.Requires(arg != null);
            for(int i = 0, arg_count = arg.Length, var_count = Variable.Count; i < arg_count && i < var_count; i++)
                Variable[i].Value = arg[i];
            return ((ComputedNode)_ExpressionTree.Root).Compute();
        }

        /// <summary>Компиляция математического выражения в функцию без параметров</summary>
        /// <returns>Функция типа double func(void) без параметров</returns>
        [NotNull]
        public Func<double> Compile()
        {
            Contract.Ensures(Contract.Result<Func<double>>() != null);
            return Compile<Func<double>>();
        }

        /// <summary>Компиляция функции одной переменной</summary>
        /// <returns>Делегат функции одной переменной</returns>
        [NotNull]
        public Func<double, double> Compile1()
        {
            Contract.Ensures(Contract.Result<Func<double, double>>() != null);
            return Compile<Func<double, double>>();
        }

        /// <summary>Компиляция функции двух переменных</summary>
        /// <returns>Делегат функции двух переменных</returns>
        [NotNull]
        public Func<double, double, double> Compile2()
        {
            Contract.Ensures(Contract.Result<Func<double, double, double>>() != null);
            return Compile<Func<double, double, double>>();
        }

        /// <summary>Компиляция функции трёх переменных</summary>
        /// <returns>Делегат функции трёх переменных</returns>
        [NotNull]
        public Func<double, double, double, double> Compile3()
        {
            Contract.Ensures(Contract.Result<Func<double, double, double, double>>() != null);
            return Compile<Func<double, double, double, double>>();
        }

        /// <summary>Компиляция математического выражения в функцию указанного типа</summary>
        /// <param name="ArgumentName">Список имён параметров</param>
        /// <returns>Делегат скомпелированного выражения</returns>
        [NotNull]
        public Delegate Compile([NotNull] params string[] ArgumentName)
        {
            Contract.Requires(ArgumentName != null);
            Contract.Ensures(Contract.Result<Delegate>() != null);
            var compilation = GetExpression(out var vars, ArgumentName);
            return Expression.Lambda(compilation, vars).Compile();
        }

        /// <summary>Многопараметрическая компиляия мат.выражения</summary>
        /// <param name="ArgumentName">Массив имён компилируемых параметров</param>
        /// <returns>Делегат функции, принимающий на вход массив значений параметров</returns>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        [NotNull]
        public Func<double[], double> CompileMultyParameters([NotNull] params string[] ArgumentName)
        {
            Contract.Requires(ArgumentName != null);
            Contract.Ensures(Contract.Result<Func<double[], double>>() != null);
            // Словарь индексов переменных
            var var_dictionary = new Dictionary<string, int>();

            // Выходной массив параметров выражения, получаемый при сборке дерева
            // Сборка дерева
            var compilation = GetExpression(out var vars, ArgumentName);

            // Если массив имён компилируемых входных переменных не пуст
            if((ArgumentName?.Length ?? 0) > 0)
                ArgumentName.Foreach(var_dictionary.Add); // то заполняем словарь индексов
            else // Если массив переменых не указан, то сиздаём функцию по всем переменным
                Variable.Select(v => v.Name).Foreach(var_dictionary.Add);

            // Создаём входную переменную выражения с типом "вещественный массив"
            var array_parameter = Expression.Parameter(typeof(double[]), "args_array");
            // Создаём пул выражений - индексаторов входного массива
            var array_indexers = new Dictionary<int, Expression>();
            // Объявляем метод получения выражения индексатора входного массива по указанному значению индекса
            Func<int, Expression> GetIndexedParameter = index =>
            {
                // Если пул уже содержит указанный индекс, то выдать соответствующий индексатор
                if(array_indexers.ContainsKey(index))
                    return array_indexers[index];
                // иначе создаём новый индексатор
                var indexer = Expression.ArrayIndex(array_parameter, Expression.Constant(index));
                array_indexers.Add(index, indexer); // и сохраняем его в пуле
                return indexer; // возвращаем новый индексатор
            };

            // Создаём пересборщик дерева выражения Linq.Expression
            var rebuilder = new ExpressionRebuilder();
            // Добавляем обработчик узлов вызова методов
            rebuilder.MethodCallVisited += (s, e) => // Если очередной узел дерева - вызов метода, то...
            {
                var call = e.Argument; // Извлекаем ссылку на узел
                //Если целевой объект вызова - не(!) константное значение и оно не соответствует типу переменной дерева MathExpressionTree 
                if(!(call.Object is ConstantExpression && ((ConstantExpression)call.Object).Value is ExpressionVariabel))
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
            // Собираем лямда-выражение
            var lamda = Expression.Lambda<Func<double[], double>>(compilation, array_parameter);
            return lamda.Compile(); // Компилируем лямда-выражение и возвращаем делегат
        }

        /// <summary>Компиляция математического выражения в функцию указанного типа</summary>
        /// <typeparam name="TDelegate">Тип делегата функции</typeparam>
        /// <param name="ArgumentName">Список имён параметров</param>
        /// <returns>Делегат скомпелированного выражения</returns>
        [NotNull]
        public TDelegate Compile<TDelegate>([NotNull] params string[] ArgumentName)
        {
            Contract.Requires(ArgumentName != null);
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
            Contract.Requires(ArgumentName != null);
            Contract.Ensures(Contract.ValueAtReturn(out vars) != null);
            vars = ArgumentName.Select(name => Expression.Parameter(typeof(double), name)).ToArray();
            return ((ComputedNode)_ExpressionTree.Root).Compile(vars);
        }

        /// <summary>Получить Linq.Expression выражение, построенное на основе дерева выражений</summary>
        /// <typeparam name="TDelegate">Тип делегата выражения</typeparam>
        /// <param name="vars">Список входных переменных</param>
        /// <param name="ArgumentName">Список имён аргументов</param>
        /// <returns>Выражение типа Linq.Expression</returns>
        [NotNull]
        public Expression GetExpression<TDelegate>([CanBeNull] out ParameterExpression[] vars,
            [NotNull] params string[] ArgumentName)
        {
            Contract.Requires(ArgumentName != null);
            Contract.Requires(typeof(TDelegate).IsSubclassOf(typeof(Delegate)));
            Contract.Ensures(Contract.Result<Expression>() != null);

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
            var compilation = vars == null
                ? ((ComputedNode)_ExpressionTree.Root).Compile()
                : ((ComputedNode)_ExpressionTree.Root).Compile(vars);
            return compilation;
        }

        /// <summary>Преобразование в строку</summary>
        /// <returns>Строковое представление</returns>
        public override string ToString()
        {
            Contract.Ensures(Contract.Result<string>() != null);
            return $"{_Name}({_Variables.Select(v => v.Name).ToSeparatedStr(", ")})={_ExpressionTree.Root}";
        }

        /// <summary>Перенос констант из выражения источника в выражение приёмник</summary>
        /// <param name="Source">Выражение источник</param>
        /// <param name="Result">Выражение приёмник</param>
        private static void CheckConstatnsCollection([NotNull] MathExpression Source, [NotNull] MathExpression Result)
        {
            Contract.Requires(Source != null);
            Contract.Requires(Result != null);
            Source.Constants
                .Select(constant => Result.Variable[constant.Name])
                .Where(c => Result.Variable.Remove(c))
                .Foreach(c => Result.Constants.Add(c));
        }

        /// <summary>Клонирование выражения</summary>
        /// <returns>Копия объектной модели выражения</returns>
        [NotNull]
        public MathExpression Clone()
        {
            Contract.Ensures(Contract.Result<MathExpression>() != null);
            var result = new MathExpression(_ExpressionTree.Clone());
            CheckConstatnsCollection(this, result);
            return result;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        [NotNull]
        object ICloneable.Clone() => Clone();

        /// <summary>Комбинация двух выражений с использованием узла-оператора</summary>
        /// <param name="x">Первое выражение</param>
        /// <param name="y">Второе выражение</param>
        /// <param name="node">Узел операции</param>
        /// <returns>Математическое выражение, в корне дерева которого лежит узел оператора. Поддеревья - корни первого и второго выражений</returns>
        [NotNull]
        protected static MathExpression CombineExpressions([NotNull] MathExpression x, [NotNull] MathExpression y, [NotNull] OperatorNode node)
        {
            Contract.Requires(x != null);
            Contract.Requires(y != null);
            Contract.Requires(node != null);
            Contract.Ensures(Contract.Result<MathExpression>() != null);

            var xT = x.Tree.Clone();
            var yT = y.Tree.Clone();

            if(xT.Root is OperatorNode && ((OperatorNode)xT.Root).Priority < node.Priority)
                xT.Root = new ComputedBracketNode(Bracket.NewRound, xT.Root);
            if(yT.Root is OperatorNode && ((OperatorNode)yT.Root).Priority < node.Priority)
                yT.Root = new ComputedBracketNode(Bracket.NewRound, yT.Root);

            node.Left = xT.Root;
            node.Right = yT.Root;

            var z = new MathExpression(new ExpressionTree(node));
            CheckConstatnsCollection(x, z);
            CheckConstatnsCollection(y, z);
            return z;
        }

        /// <summary>Оператор сложения двух выражений</summary>
        /// <param name="x">Первое слогаемое</param>
        /// <param name="y">Второе слогаемое</param>
        /// <returns>Выражение-сумма, корень которого - узел суммы. Поддеревья - корни выражений слогаемых</returns>
        [NotNull]
        public static MathExpression operator +(MathExpression x, MathExpression y) => CombineExpressions(x, y, new AdditionOperatorNode());

        /// <summary>Оператор вычитания двух выражений</summary>
        /// <param name="x">Уменьшаемое</param>
        /// <param name="y">Вычитаемое</param>
        /// <returns>Выражение-разность, корень которого - узел разности. Поддеревья - корни выражений вычитаемого и уменьшаемого</returns>
        [NotNull]
        public static MathExpression operator -(MathExpression x, MathExpression y) => CombineExpressions(x, y, new SubstractionOperatorNode());

        /// <summary>Оператор умножения двух выражений</summary>
        /// <param name="x">Первый сомножитель</param>
        /// <param name="y">Второй сомножитель</param>
        /// <returns>Выражение-произведения, корень которого - узел произведения. Поддеревья - корни выражений сомножителей</returns>
        [NotNull]
        public static MathExpression operator *(MathExpression x, MathExpression y) => CombineExpressions(x, y, new MultiplicationOperatorNode());

        /// <summary>Оператор деления двух выражений</summary>
        /// <param name="x">Делимое</param>
        /// <param name="y">Делитель</param>
        /// <returns>Выражение-частное, корень которого - узел деления. Поддеревья - корни выражений делимого и делителя</returns>
        [NotNull]
        public static MathExpression operator /(MathExpression x, MathExpression y) => CombineExpressions(x, y, new DivisionOperatorNode());

        /// <summary>Оператор возведения в степень</summary>
        /// <param name="x">Основание</param>
        /// <param name="y">Показатель степени</param>
        /// <returns>Выражение-степень, корень которого - узел степени. Поддеревья - корни выражений Основания и показателя степени</returns>
        [NotNull]
        public static MathExpression operator ^(MathExpression x, MathExpression y) => CombineExpressions(x, y, new PowerOperatorNode());

        /// <summary>Оператор неявного приведения типов математического выражения к типу дерева выражения</summary>
        /// <param name="Expression">Математическое выражение</param>
        /// <returns>Дерево математического выражения</returns>
        public static implicit operator ExpressionTree(MathExpression Expression) => Expression.Tree;

        /// <summary>Оператор неявного приведения типов дерева выражения к типу математического выражения</summary>
        /// <param name="Tree">Дерево математического выражения</param>
        /// <returns>Математическое выражение, содержащее указанное дерево</returns>
        public static implicit operator MathExpression(ExpressionTree Tree) => new MathExpression(Tree);

        /// <summary>Оператор неявного приведения типов математического выражения к типу делегата функции double Func(void)</summary>
        /// <param name="expr">Математическое выражения</param>
        /// <returns>Результат компиляции математического выражения</returns>
        [NotNull]
        public static implicit operator Func<double>(MathExpression expr)
        {
            Contract.Requires(expr != null);
            return expr.Compile();
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(_Variables != null);
            Contract.Invariant(_Constants != null);
            Contract.Invariant(_Functions != null);
            Contract.Invariant(_Functionals != null);
            Contract.Invariant(_Name != null);
        }
    }
}