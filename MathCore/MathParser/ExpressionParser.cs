﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MathCore.Annotations;
using MathCore.MathParser.ExpressionTrees.Nodes;
using MathCore.Values;
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.MathParser
{
    /// <summary>Парсер математических выражений</summary>
    public class ExpressionParser
    {
        /// <summary>Событие возникает при добавлении нового узла в дерево выражения</summary>
        public event EventHandler<EventArgs<ExpressionTreeNode>> NewNodeAdded;

        /// <summary>При добавлении нового узла в дерево выражения</summary>
        /// <param name="e">Аргумент события, содержащий добавляемй узел</param>
        protected virtual void OnNewNodeAdded([NotNull] EventArgs<ExpressionTreeNode> e)
        {
            NewNodeAdded?.Invoke(this, e);
        }

        /// <summary>Обработка очередного добавляемого в дерево узла</summary>
        /// <param name="NewNode">Новый добавляемый узел дерева выражения</param>
        protected virtual void OnNewNodeAdded([NotNull] ref ExpressionTreeNode NewNode)
        {
            ProcessNewNode(ref NewNode);

            var e = new EventArgs<ExpressionTreeNode>(NewNode);
            NewNodeAdded?.Invoke(this, e);
            if (!ReferenceEquals(e.Argument, NewNode))
                NewNode = e.Argument;
        }

        /// <summary>Обработка нового узла дерева выражения</summary>
        /// <param name="NewNode">Новый добавляемый узел</param>
        protected virtual void ProcessNewNode([NotNull] ref ExpressionTreeNode NewNode)
        {
            switch (NewNode)
            {
                case CharNode char_node:
                    {
                        switch (char_node.Value)
                        {
                            case '.':
                                if (NewNode.Parent is CharNode c && c.Value == '.')
                                {
                                    var value_node = NewNode[n => n.Parent].Last(n => !(n is OperatorNode) || n.Left is null);
                                    NewNode["./."].Right = null;
                                    var parent = value_node.Parent;
                                    var interval_node = new IntervalNode(value_node);
                                    if (parent != null) parent.Right = interval_node;
                                    NewNode = interval_node;
                                }
                                break;
                        }
                        break;
                    }
                case OperatorNode _:
                    break;
            }
        }

        /// <summary>Событие предобработки входящей строки</summary>
        public event EventHandler<EventArgs<string>> StringPreprocessing;

        /// <summary>Генерация события предобработки входящей строки</summary>
        /// <param name="args">Аргумент события, содержащий обрабатываемую строку</param>
        protected virtual void OnStringPreprocessing([NotNull] EventArgs<string> args)
        {
            StringPreprocessing?.Invoke(this, args);
        }

        /// <summary>Генерация события предобработки входящей строки</summary>
        /// <param name="StrExpression">Обрабатываемая строка</param>
        private void OnStringPreprocessing([NotNull] ref string StrExpression)
        {
            var args = new EventArgs<string>(StrExpression);
            OnStringPreprocessing(args);
            StrExpression = args.Argument;
        }

        /// <summary>Аргумент события обнаружения функции</summary>
        public sealed class FindFunctionEventArgs : EventArgs
        {
            /// <summary>Имя обнаруженой функции</summary>
            public string Name { get; }
            /// <summary>Массив имён аргументов функции</summary>
            public string[] Arguments { get; }

            /// <summary>Количество аргументов функции</summary>
            public int ArgumentCount => Arguments.Length;

            /// <summary>Делегат функции, который надо использовать при её вычислении</summary>
            public Delegate Function { get; set; }
            /// <summary>Инициализация аргумента события обнаружения функции</summary>
            /// <param name="Name">Имя функции</param>
            /// <param name="Arguments">Массив имён аргументов функции</param>
            public FindFunctionEventArgs([NotNull] string Name, [NotNull] string[] Arguments)
            {
                this.Name = Name;
                this.Arguments = Arguments;
            }

            /// <summary>Проверка на совпадение сигнатуры функции по имени и числу переменных</summary>
            /// <param name="name">Имя проверяемой функции</param>
            /// <param name="ArgumentsCount">Число переменных</param>
            /// <returns></returns>
            public bool SignatureEqual([NotNull] string name, int ArgumentsCount)
            {
                return Name == name && ArgumentsCount == ArgumentCount;
            }
        }

        /// <summary>Событие, возникающее в процессе разбора математического выражения при обнаружении функции</summary>
        public event EventHandler<FindFunctionEventArgs> FindFunction;

        /// <summary>Обработчик события обнаружения функции в процессе разбора выражения</summary>
        /// <param name="Args">Аргументы события, содержащие имя функции, имена аргументов и делегат метода функции</param>
        protected virtual void OnFindFunction([NotNull] FindFunctionEventArgs Args)
        {
            FindFunction?.Invoke(this, Args);
        }

        /// <summary>Обработчик события обнаружения функции в процессе разбора выражения</summary>
        /// <param name="Name">Имя функции</param>
        /// <param name="Arguments">Аргументы функции</param>
        /// <returns>Делегат функции</returns>
        private Delegate OnFunctionFind([NotNull] string Name, [NotNull] string[] Arguments)
        {
            var args = new FindFunctionEventArgs(Name, Arguments);
            OnFindFunction(args);
            return args.Function;
        }

        /// <summary>Событие обработки переменных при разборе мат.выражений</summary>
        public event EventHandler<EventArgs<ExpressionVariabel>> VariableProcessing;

        /// <summary> Обработка обнаруженной переменной</summary>
        /// <param name="e">Обнаруженная переменная</param>
        protected virtual void OnVariableProcessing([NotNull] EventArgs<ExpressionVariabel> e)
        {
            VariableProcessing?.Invoke(this, e);
        }

        /// <summary> Обработка обнаруженной переменной</summary>
        /// <param name="Variable">Обнаруженная переменная</param>
        private void OnVariableProcessing([NotNull] ExpressionVariabel Variable)
        {
            OnVariableProcessing(new EventArgs<ExpressionVariabel>(Variable));
        }

        /// <summary>Множество запрещённых символов</summary>
        [NotNull]
        private readonly SetOf<char> _ExcludeCharsSet = new SetOf<char>(" \r\n");

        /// <summary>Словарь констант</summary>
        [NotNull]
        private readonly Dictionary<string, double> _Constans = new Dictionary<string, double>();

        /// <summary>Множество запрещённых симовлов</summary>
        [NotNull]
        public SetOf<char> ExcludeCharsSet => _ExcludeCharsSet;

        /// <summary>Разделитель выражений (по умолчанию ';')</summary>
        public char ExpressionSeparator { get; set; }

        /// <summary>Разделитель целой части и мантисы десятичного числа</summary>
        public char DecimalSeparator { get; set; }

        /// <summary>Константы</summary>
        [NotNull]
        public Dictionary<string, double> Constants => _Constans;

        /// <summary>Парсер математических выражений</summary>
        public ExpressionParser()
        {
            ExpressionSeparator = ',';
            DecimalSeparator = '.';

            #region Добавление стандартных констант

            _Constans.Add("pi", Math.PI);
            _Constans.Add("pi2", Consts.pi2);
            _Constans.Add("pi05", Consts.pi05);
            _Constans.Add("e", Math.E);
            _Constans.Add("sqrt2", Consts.sqrt_2);
            _Constans.Add("sqrt2inv", Consts.sqrt_2_inv);
            _Constans.Add("sqrt3", Consts.sqrt_3);
            _Constans.Add("sqrt5", Consts.sqrt_5);
            _Constans.Add("ToDeg", Consts.Geometry.ToDeg);
            _Constans.Add("ToRad", Consts.Geometry.ToRad);

            #endregion
        }

        /// <summary>Предварительная обработка входного строкового выражения</summary>
        /// <param name="Str">Обрабатываемая строка</param>
        // Удаление из строки всех символов, из множества запрещённых симоволов
        protected virtual void StrPreprocessing([NotNull] ref string Str)
        {
            Str = new string(Str.Where(_ExcludeCharsSet.NotContains).ToArray());
        }

        /// <summary>Разобрать строку математического выражения</summary>
        /// <param name="StrExpression">Строковое представление математического выражения</param>
        /// <returns>Математическое выражение</returns>
        [NotNull]
        public MathExpression Parse([NotNull] string StrExpression)
        {
            StrPreprocessing(ref StrExpression);
            OnStringPreprocessing(ref StrExpression);

            var expression = new MathExpression(StrExpression, this);

            ProcessVariables(expression);
            ProcessFunctions(expression);

            return expression;
        }

        /// <summary>Обработка переменных</summary>
        /// <param name="Expression">Обрабатываемое математическое выражение</param>
        internal void ProcessVariables([NotNull] MathExpression Expression)
        {
            var tree_vars = Expression.Tree.Root.GetVariables().ToArray();
            Expression.Variable
                .Where(v => !tree_vars.Contains(v))
                .ToArray()
                .Foreach(v => Expression.Variable.Remove(v));
            foreach (var variable in Expression.Variable.ToArray())
            {
                if (_Constans.ContainsKey(variable.Name))
                {
                    Expression.Variable.MoveToConstCollection(variable);
                    variable.Value = _Constans[variable.Name];
                }
                OnVariableProcessing(variable);
            }
        }

        /// <summary>Обработка функций</summary>
        /// <param name="Expression">Обрабатываемое математическое выражение</param>
        [SuppressMessage("ReSharper", "CyclomaticComplexity")]
        internal void ProcessFunctions([NotNull] MathExpression Expression)
        {
            foreach (var function in Expression.Functions)
                function.Delegate = function.Arguments.Length switch
                {
                    1 => (function.Name switch
                    {
                        "Sin" => new Func<double, double>(Math.Sin),
                        "SIN" => new Func<double, double>(Math.Sin),
                        "sin" => new Func<double, double>(Math.Sin),
                        "COS" => new Func<double, double>(Math.Cos),
                        "Cos" => new Func<double, double>(Math.Cos),
                        "cos" => new Func<double, double>(Math.Cos),
                        "TAN" => new Func<double, double>(Math.Tan),
                        "Tan" => new Func<double, double>(Math.Tan),
                        "tan" => new Func<double, double>(Math.Tan),
                        "tn" => new Func<double, double>(Math.Tan),
                        "ATAN" => new Func<double, double>(Math.Atan),
                        "ATan" => new Func<double, double>(Math.Atan),
                        "Atan" => new Func<double, double>(Math.Atan),
                        "atan" => new Func<double, double>(Math.Atan),
                        "atn" => new Func<double, double>(Math.Atan),
                        "Atn" => new Func<double, double>(Math.Atan),
                        "CTG" => new Func<double, double>(x => 1 / Math.Tan(x)),
                        "Ctg" => new Func<double, double>(x => 1 / Math.Tan(x)),
                        "ctg" => new Func<double, double>(x => 1 / Math.Tan(x)),
                        "Sign" => new Func<double, double>(x => Math.Sign(x)),
                        "sign" => new Func<double, double>(x => Math.Sign(x)),
                        "Abs" => new Func<double, double>(Math.Abs),
                        "abs" => new Func<double, double>(Math.Abs),
                        "Exp" => new Func<double, double>(Math.Exp),
                        "EXP" => new Func<double, double>(Math.Exp),
                        "exp" => new Func<double, double>(Math.Exp),
                        "Sqrt" => new Func<double, double>(Math.Sqrt),
                        "SQRT" => new Func<double, double>(Math.Sqrt),
                        "√" => new Func<double, double>(Math.Sqrt),
                        "sqrt" => new Func<double, double>(Math.Sqrt),
                        "log10" => new Func<double, double>(Math.Log10),
                        "Log10" => new Func<double, double>(Math.Log10),
                        "LOG10" => new Func<double, double>(Math.Log10),
                        "lg" => new Func<double, double>(Math.Log10),
                        "Lg" => new Func<double, double>(Math.Log10),
                        "LG" => new Func<double, double>(Math.Log10),
                        "loge" => new Func<double, double>(Math.Log),
                        "Loge" => new Func<double, double>(Math.Log),
                        "LOGe" => new Func<double, double>(Math.Log),
                        "ln" => new Func<double, double>(Math.Log),
                        "Ln" => new Func<double, double>(Math.Log),
                        "LN" => new Func<double, double>(Math.Log),
                        _ => (OnFunctionFind(function.Name, function.Arguments) ??
                              throw new NotSupportedException($"Обработка функции {function.Name} не поддерживается"))
                    }),
                    2 => (function.Name switch
                    {
                        "ATAN" => new Func<double, double, double>(Math.Atan2),
                        "ATan" => new Func<double, double, double>(Math.Atan2),
                        "Atan" => new Func<double, double, double>(Math.Atan2),
                        "atan" => new Func<double, double, double>(Math.Atan2),
                        "atn" => new Func<double, double, double>(Math.Atan2),
                        "Atn" => new Func<double, double, double>(Math.Atan2),
                        "Atan2" => new Func<double, double, double>(Math.Atan2),
                        "atan2" => new Func<double, double, double>(Math.Atan2),
                        "log" => new Func<double, double, double>(Math.Log),
                        "Log" => new Func<double, double, double>(Math.Log),
                        "LOG" => new Func<double, double, double>(Math.Log),
                        _ => function.Delegate
                    }),
                    _ => (OnFunctionFind(function.Name, function.Arguments) ??
                          throw new NotSupportedException($"Обработка функции {function.Name} не поддерживается"))
                };
        }

        /// <summary>Метод определения узла дерева, реализующего оператор</summary>
        /// <param name="Name">Имя оператора</param>
        /// <returns>Узел дерева оператора</returns>
        [NotNull]
        public virtual ExpressionTreeNode GetOperatorNode(char Name) =>
            Name switch
            {
                '+' => (ExpressionTreeNode)new AdditionOperatorNode(),
                '-' => new SubstractionOperatorNode(),
                '*' => new MultiplicationOperatorNode(),
                '×' => new MultiplicationOperatorNode(),
                '·' => new MultiplicationOperatorNode(),
                '/' => new DivisionOperatorNode(),
                '^' => new PowerOperatorNode(),
                '=' => new EqualityOperatorNode(),
                '>' => new GreaterThenOperatorNode(),
                '<' => new LessThenOperatorNode(),
                '!' => new NotOperatorNode(),
                '≠' => new NotOperatorNode(),
                ':' => new VariantOperatorNode(),
                '?' => new SelectorOperatorNode(),
                '&' => new AndOperatorNode(),
                '|' => new OrOperatorNode(),
                _ => new CharNode(Name)
            };

        /// <summary>Метод определения функционала по имени</summary>
        /// <param name="Name">Имя функционала</param>
        /// <returns>Функционал</returns>
        /// <exception cref="NotSupportedException">Возникает для неопределённых имён функционалов</exception>
        [NotNull] public static Functional GetFunctional([NotNull] string Name) =>
            Name switch
            {
                "summ" => (Functional)new SummOperator(Name),
                "Sum" => new SummOperator(Name),
                "Σ" => new SummOperator(Name),
                "int" => new IntegralOperator(Name),
                "integral" => new IntegralOperator(Name),
                "Int" => new IntegralOperator(Name),
                "Integral" => new IntegralOperator(Name),
                "∫" => new IntegralOperator(Name),
                _ => throw new NotSupportedException($"Функционал {Name} не поддерживается")
            };

        /// <summary>Метод излвечения корня дерева из последовательности элементов математического выражения</summary>
        /// <param name="Group">группа элементов математического выражения</param>
        /// <param name="MathExpression">Ссылка на математическое выражение</param>
        /// <returns>Корень дерева мат.выражения</returns>
        [NotNull]
        internal ExpressionTreeNode GetRoot([NotNull] Term[] Group, [NotNull] MathExpression MathExpression)
        {
            // Ссылка на последний обработанный узел дерева
            ExpressionTreeNode last = null;
            for (var i = 0; i < Group.Length; i++) // в цикле по всем элементам группы
            {
                var node = Group[i].GetSubTree(this, MathExpression); // извлеч поддерево для текущего элемента группы
                // Если очередной элемент группы...

                switch (Group[i])
                {
                    case NumberTerm _: // ...очередной элемент число, то
                        //...если есть впереди ещё два элемента и прошла удачная попытка считать разделитель дробного числи и мантиссу  
                        if (i + 2 < Group.Length && NumberTerm.TryAddFractionPart(ref node, Group[i + 1], DecimalSeparator, Group[i + 2]))
                            i += 2; //...то увеличить индекс текущей группы на два.
                        break;
                    case BlockTerm block: //...очередной элемент блок (со скобками)
                        node = new ComputedBracketNode( // очередной узел дерева - это вычислимый блок
                            new Bracket( //вид скобок:
                                block.OpenBracket, // копируем вид открывающей скобки
                                block.CloseBracket),  // копируем вид закрывающей скобки
                            node); //Внутри узел дерева
                        break;
                }

                //Проводим комбинацию текущего узла предыдущим узлом дерева
                Combine(last, last = node); // и назначаем текущий узел дерева предыдущим

                if (last.IsRoot && last is VariantOperatorNode && last.Left is VariableValueNode variable)
                    last = new FunctionArgumentNameNode(variable.Name);

                OnNewNodeAdded(ref last);
            }

            // Если ссылка на предыдущий узел отсутствует, то это ошибка формата
            if (last is null) throw new FormatException();
            return last.Root; // вернуть корень дерева текущего элемента
        }

        /// <summary>Комбинация предыдущего и текущего узлов дерева</summary>
        /// <param name="Last">Предыдущий узел дерева (уже интегрированный в дерево)</param>
        /// <param name="Node">Текущий узел, который надо вставить в дерево</param>
        // ReSharper disable once CyclomaticComplexity
        public virtual void Combine([CanBeNull] ExpressionTreeNode Last, [NotNull] ExpressionTreeNode Node)
        {
            if (Last is null) return; // Если предыдущий узел дерева не указан, возврат

            if (Node is CharNode) // Если текущий узел - символьный узел, то
            {
                Last.LastRightChild = Node; // просто назначить его самым правым дочерним
                return;
            }

            // представляем текущий узел в виде узла-оператора
            if (Node is OperatorNode operator_node)  // если текущий узел является оператором...
            {
                //Пытаемся получить оператор предыдущего узла:
                // пытаемся привести предыдущий узел к типу узла оператора
                // либо пытаемся привести родительский узел предыдущего узла к типу узла оператора
                var parent_operator = Last as OperatorNode ?? Last.Parent as OperatorNode;

                if (parent_operator != null) // Если получена ссылка не предыдущий узел-оператор (и текущий является оператором)... то
                {
                    // Если левое поддерево предыдущего операторо пусто и родитель предыдущего оператора - тоже оператор
                    //      op <- устанавливаем предыдущим оператором родителя
                    //      |
                    //      op 
                    //     /  \
                    //  null   ?
                    if (parent_operator.Left is null && parent_operator.Parent is OperatorNode @operator)
                        parent_operator = @operator;


                    if (parent_operator.Left is null)          // Если левое поддерево предыдущего оператора пусто...
                        operator_node.Left = parent_operator; //  устанавливаем предыдущий оператор в качестве левого поддерева текущего
                    else if (parent_operator.Right is null)    // Иначе если правое поддерево пусто
                        parent_operator.Right = Node;         //  установить текущий оператор правым поддеревом предыдущего
                    else                                      // Иначе если конфликт приоритетов
                    {
                        var priority = operator_node.Priority;  // извлекаем приоритет текущего узла
                        // Если приоритет текущего оператора меньше, либо равен приоритету предыдущего
                        if (priority <= parent_operator.Priority)
                        {
                            // то надо подниматься вверх под дереву до тех пор
                            parent_operator = (OperatorNode)parent_operator.Parents
                                        // пока встречаемые на пути операторы имеют приоритет выше приоритета текущего оператора
                                        .TakeWhile(n => n is OperatorNode node && priority <= node.Priority)
                                        // взять последний из последовательности
                                        .LastOrDefault() ?? parent_operator; // если вернулась пустая ссылка, то взять предыдущий оператор

                            // На текущий момент предыдущий оператор имеет приоритет выше приоритета текущего оператора

                            if (parent_operator.IsRoot) // Если предыдущий оператор - корень дерева
                                // Если приоритет оператора в корне дерева больше, либо равен приоритету текущего оператора
                                if (priority <= parent_operator.Priority) // todo: проверить необходимость условия
                                    // Присвоить левому поддереву текущего оператора предыдущий
                                    operator_node.Left = parent_operator;
                                // todo: проверить достижимость следующего блока
                                else // если оператор в корне дерева имеет меньший приоритет, чем приоритет текущего оператора
                                {    //  то вставить текущий оператор в правое поддерево родителя предыдущего оператора
                                    throw new NotImplementedException("!!!");
                                    //var right = parent_operator.Right; // сохранить правое поддерево предыдущего оператора
                                    //parent_operator.Right = Node; // У предыдущего оператора в правое поддерево внести текущий оператор
                                    //operator_node.Left = right;   // В левое поддерево текущего оператора внести сохранённое правое поддерево
                                }
                            else // Иначе если предыдущий оператор не корень
                            {
                                var parent = parent_operator.Parent; // сохранить ссылку на родителя предыдущего оператора
                                parent.Right = Node;                 // записать текущий оператор в качестве правого поддерева
                                operator_node.Left = parent_operator;// записать предыдущий оператора левым поддеревом текущего
                            }
                        }
                        else //если приоритет текущего оператора больше приоритета предыдущего
                        {
                            // то надо спускаться в правое поддерево до тех пор
                            parent_operator = (OperatorNode)parent_operator.RightNodes
                                        // пока встречаемые на пути операторы имеют левые поддеревья и приоритет операторов меньше текущего
                                        .TakeWhile(n => n is OperatorNode node && node.Left != null && node.Priority < priority)
                                        // взять последний из последовательности
                                        .LastOrDefault() ?? parent_operator;  // если вернулась пустая ссылка, то взять предыдущий оператор

                            // На текущий момент предыдущий оператор имеет приоритет ниже приоритета текущего оператора

                            var right = parent_operator.Right; // сохранить правое поддерево предыдущего оператора
                            parent_operator.Right = Node;      // в правое поддерево предыдущего оператора попадает текущий
                            operator_node.Left = right;        // в левое поддерево текущего оператора записывается сохранённое правое 
                        }
                    }
                }
                else // Если предыдущий узел не является оператором
                {
                    var parent = Last.Parent;
                    var is_left = Last.IsLeftSubtree;
                    var is_right = Last.IsRightSubtree;
                    operator_node.Left = Last; // записать предыдущий узел левым поддеревом текущего
                    if (is_left)
                        parent.Left = operator_node;
                    else if (is_right)
                        parent.Right = operator_node;
                }
                return; // возврат
            }
            // Если текущий узел оператором не является

            if (Last is OperatorNode) // если предыдущий узел является оператором
            {
                Last.Right = Node; // добавыить текуий в правое поддерево предыдущего
                return;            // возврат
            }
            // Если ни текущий не придыдущий узлы не являются операторами

            //Если предыдущий узел был числом, или предыдущий узел был скобками и текущий - скобки
            if (Last is ConstValueNode || (Last is ComputedBracketNode && Node is ComputedBracketNode))
            {
                //Сохряняем ссылку на родителя предыдущего узла
                var parent = Last.Parent;
                if (parent != null) // если родитель есть
                    // в правое поддерево родителя записываем оператор перемножения предыдущего узла и текущего
                    parent.Right = new MultiplicationOperatorNode(Last, Node);
                else // если предыдущий узел был узлом дерева
                    // создаём новый узел-оператор умножения предыдущего узла и текущего, который становится новым корнем дерева
                    new MultiplicationOperatorNode(Last, Node);
                return; // возврат.
            }

            Last.Right = Node;
            //throw new FormatException(); // Если не сработало ни одно условие, то это ошибка формата
        }
    }
}