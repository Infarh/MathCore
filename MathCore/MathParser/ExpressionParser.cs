#nullable enable
using MathCore.MathParser.ExpressionTrees.Nodes;
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable AssignmentIsFullyDiscarded

namespace MathCore.MathParser;

/// <summary>Парсер математических выражений</summary>
public class ExpressionParser
{
    /// <summary>Событие возникает при добавлении нового узла в дерево выражения</summary>
    public event EventHandler<EventArgs<ExpressionTreeNode>>? NewNodeAdded;

    /// <summary>При добавлении нового узла в дерево выражения</summary>
    /// <param name="e">Аргумент события, содержащий добавляемый узел</param>
    protected virtual void OnNewNodeAdded(EventArgs<ExpressionTreeNode> e) => NewNodeAdded?.Invoke(this, e);

    /// <summary>Обработка очередного добавляемого в дерево узла</summary>
    /// <param name="NewNode">Новый добавляемый узел дерева выражения</param>
    protected virtual void OnNewNodeAdded(ref ExpressionTreeNode NewNode)
    {
        ProcessNewNode(ref NewNode);

        var e = new EventArgs<ExpressionTreeNode>(NewNode);
        NewNodeAdded?.Invoke(this, e);
        if (!ReferenceEquals(e.Argument, NewNode))
            NewNode = e.Argument;
    }

    /// <summary>Обработка нового узла дерева выражения</summary>
    /// <param name="NewNode">Новый добавляемый узел</param>
    protected virtual void ProcessNewNode(ref ExpressionTreeNode NewNode)
    {
        switch (NewNode)
        {
            case CharNode char_node:
                {
                    switch (char_node.Value)
                    {
                        case '.' when NewNode.Parent is CharNode { Value: '.' }:
                            var value_node = NewNode[n => n.Parent].Last(n => n is not OperatorNode || n.Left is null);
                            (NewNode["./."] ?? throw new InvalidOperationException("NewNode/. is null")).Right = null;
                            var parent                       = value_node.Parent;
                            var interval_node                = new IntervalNode(value_node);
                            if (parent != null) parent.Right = interval_node;
                            NewNode = interval_node;
                            break;
                    }
                    break;
                }
            case OperatorNode:
                break;
        }
    }

    /// <summary>Событие предобработки входящей строки</summary>
    public event EventHandler<EventArgs<string>>? StringPreprocessing;

    /// <summary>Генерация события предобработки входящей строки</summary>
    /// <param name="args">Аргумент события, содержащий обрабатываемую строку</param>
    protected virtual void OnStringPreprocessing(EventArgs<string> args) => StringPreprocessing?.Invoke(this, args);

    /// <summary>Генерация события предобработки входящей строки</summary>
    /// <param name="StrExpression">Обрабатываемая строка</param>
    private void OnStringPreprocessing(ref string StrExpression)
    {
        var args = new EventArgs<string>(StrExpression);
        OnStringPreprocessing(args);
        StrExpression = args.Argument;
    }

    /// <summary>Аргумент события обнаружения функции</summary>
    public sealed class FindFunctionEventArgs : EventArgs
    {
        /// <summary>Имя обнаруженной функции</summary>
        public string Name { get; }
        /// <summary>Массив имён аргументов функции</summary>
        public IReadOnlyList<string> Arguments { get; }

        /// <summary>Количество аргументов функции</summary>
        public int ArgumentCount => Arguments.Count;

        /// <summary>Делегат функции, который надо использовать при её вычислении</summary>
        public Delegate? Function { get; set; }

        /// <summary>Инициализация аргумента события обнаружения функции</summary>
        /// <param name="Name">Имя функции</param>
        /// <param name="Arguments">Массив имён аргументов функции</param>
        public FindFunctionEventArgs(string Name, IReadOnlyList<string> Arguments)
        {
            this.Name      = Name;
            this.Arguments = Arguments;
        }

        /// <summary>Проверка на совпадение сигнатуры функции по имени и числу переменных</summary>
        /// <param name="name">Имя проверяемой функции</param>
        /// <param name="ArgumentsCount">Число переменных</param>
        /// <returns></returns>
        public bool SignatureEqual(string name, int ArgumentsCount) => Name == name && ArgumentsCount == ArgumentCount;
    }

    /// <summary>Событие, возникающее в процессе разбора математического выражения при обнаружении функции</summary>
    public event EventHandler<FindFunctionEventArgs>? FindFunction;

    /// <summary>Обработчик события обнаружения функции в процессе разбора выражения</summary>
    /// <param name="Args">Аргументы события, содержащие имя функции, имена аргументов и делегат метода функции</param>
    protected virtual void OnFindFunction(FindFunctionEventArgs Args) => FindFunction?.Invoke(this, Args);

    /// <summary>Обработчик события обнаружения функции в процессе разбора выражения</summary>
    /// <param name="Name">Имя функции</param>
    /// <param name="Arguments">Аргументы функции</param>
    /// <returns>Делегат функции</returns>
    private Delegate? OnFunctionFind(string Name, IReadOnlyList<string> Arguments)
    {
        var args = new FindFunctionEventArgs(Name, Arguments);
        OnFindFunction(args);
        return args.Function;
    }

    /// <summary>Событие обработки переменных при разборе мат.выражений</summary>
    public event EventHandler<EventArgs<ExpressionVariable>>? VariableProcessing;

    /// <summary> Обработка обнаруженной переменной</summary>
    /// <param name="e">Обнаруженная переменная</param>
    protected virtual void OnVariableProcessing(EventArgs<ExpressionVariable> e) => VariableProcessing?.Invoke(this, e);

    /// <summary> Обработка обнаруженной переменной</summary>
    /// <param name="Variable">Обнаруженная переменная</param>
    private void OnVariableProcessing(ExpressionVariable Variable) => OnVariableProcessing(new EventArgs<ExpressionVariable>(Variable));

    /// <summary>Множество запрещённых символов</summary>
    private readonly HashSet<char> _ExcludeCharsSet = new(" \r\n");

    /// <summary>Словарь констант</summary>
    private readonly Dictionary<string, double> _Constants = [];

    /// <summary>Множество запрещённых символов</summary>
    public HashSet<char> ExcludeCharsSet => _ExcludeCharsSet;

    /// <summary>Разделитель выражений (по умолчанию ';')</summary>
    public char ExpressionSeparator { get; set; }

    /// <summary>Разделитель целой части и мантисcы десятичного числа</summary>
    public char DecimalSeparator { get; set; }

    /// <summary>Константы</summary>
    public Dictionary<string, double> Constants => _Constants;

    /// <summary>Парсер математических выражений</summary>
    public ExpressionParser()
    {
        ExpressionSeparator = ',';
        DecimalSeparator    = '.';

        #region Добавление стандартных констант

        _Constants.Add("pi", Math.PI);
        _Constants.Add("pi2", Consts.pi2);
        _Constants.Add("pi05", Consts.pi05);
        _Constants.Add("e", Math.E);
        _Constants.Add("sqrt2", Consts.sqrt_2);
        _Constants.Add("sqrt2inv", Consts.sqrt_2_inv);
        _Constants.Add("sqrt3", Consts.sqrt_3);
        _Constants.Add("sqrt5", Consts.sqrt_5);
        _Constants.Add("ToDeg", Consts.Geometry.ToDeg);
        _Constants.Add("ToRad", Consts.Geometry.ToRad);

        #endregion
    }

    /// <summary>Предварительная обработка входного строкового выражения</summary>
    /// <param name="Str">Обрабатываемая строка</param>
    // Удаление из строки всех символов, из множества запрещённых символов
    protected virtual void StrPreprocessing(ref string Str) => Str = new string(Str.WhereNot(_ExcludeCharsSet.Contains).ToArray());

    /// <summary>Разобрать строку математического выражения</summary>
    /// <param name="StrExpression">Строковое представление математического выражения</param>
    /// <returns>Математическое выражение</returns>
    public MathExpression Parse(string StrExpression)
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
    internal void ProcessVariables(MathExpression Expression)
    {
        var tree_vars = Expression.Tree.Root.GetVariables().ToArray();
        Expression.Variable
           .Where(v => !tree_vars.Contains(v))
           .ToArray()
           .Foreach(v => Expression.Variable.Remove(v));
        foreach (var variable in Expression.Variable.ToArray())
        {
            if (_Constants.ContainsKey(variable.Name))
            {
                Expression.Variable.MoveToConstCollection(variable);
                variable.Value = _Constants[variable.Name];
            }
            OnVariableProcessing(variable);
        }
    }

    /// <summary>Обработка функций</summary>
    /// <param name="Expression">Обрабатываемое математическое выражение</param>
    internal void ProcessFunctions(MathExpression Expression)
    {
        foreach (var function in Expression.Functions)
        {
            if (string.IsNullOrEmpty(function.Name))
                throw new InvalidOperationException("Пустая строка с именем функции");

            switch (function.Arguments.Count)
            {
                case 1:
                    switch (function.Name)
                    {
                        case "Sin":
                        case "SIN":
                        case "sin":
                            function.Delegate = new Func<double, double>(Math.Sin);
                            break;
                        case "COS":
                        case "Cos":
                        case "cos":
                            function.Delegate = new Func<double, double>(Math.Cos);
                            break;
                        case "TAN":
                        case "Tan":
                        case "tan":
                        case "tn":
                            function.Delegate = new Func<double, double>(Math.Tan);
                            break;
                        case "ATAN":
                        case "ATan":
                        case "Atan":
                        case "atan":
                        case "atn":
                        case "Atn":
                            function.Delegate = new Func<double, double>(Math.Atan);
                            break;
                        case "CTG":
                            function.Delegate = new Func<double, double>(x => 1 / Math.Tan(x));
                            break;
                        case "Ctg":
                            function.Delegate = new Func<double, double>(x => 1 / Math.Tan(x));
                            break;
                        case "ctg":
                            function.Delegate = new Func<double, double>(x => 1 / Math.Tan(x));
                            break;
                        case "Sign":
                            function.Delegate = new Func<double, double>(x => Math.Sign(x));
                            break;
                        case "sign":
                            function.Delegate = new Func<double, double>(x => Math.Sign(x));
                            break;
                        case "Abs":
                        case "abs":
                            function.Delegate = new Func<double, double>(Math.Abs);
                            break;
                        case "Exp":
                        case "EXP":
                        case "exp":
                            function.Delegate = new Func<double, double>(Math.Exp);
                            break;
                        case "Sqrt":
                        case "SQRT":
                        case "√":
                        case "sqrt":
                            function.Delegate = new Func<double, double>(Math.Sqrt);
                            break;
                        case "log10":
                        case "Log10":
                        case "LOG10":
                        case "lg":
                        case "Lg":
                        case "LG":
                            function.Delegate = new Func<double, double>(Math.Log10);
                            break;
                        case "loge":
                        case "Loge":
                        case "LOGe":
                        case "ln":
                        case "Ln":
                        case "LN":
                            function.Delegate = new Func<double, double>(Math.Log);
                            break;
                    }

                    break;
                case 2:
                    switch (function.Name)
                    {
                        case "ATAN":
                        case "ATan":
                        case "Atan":
                        case "atan":
                        case "atn":
                        case "Atn":
                        case "Atan2":
                        case "atan2":
                            function.Delegate = new Func<double, double, double>(Math.Atan2);
                            break;
                        case "log":
                        case "Log":
                        case "LOG":
                            function.Delegate = new Func<double, double, double>(Math.Log);
                            break;
                    }
                    break;
            }

            function.Delegate ??= OnFunctionFind(function.Name, function.Arguments)
                ?? throw new InvalidOperationException($"Не удалось определить делегат для функции {function.Name}, либо функция не поддерживается");
        }
    }

    /// <summary>Метод определения узла дерева, реализующего оператор</summary>
    /// <param name="Name">Имя оператора</param>
    /// <returns>Узел дерева оператора</returns>
    public virtual ExpressionTreeNode GetOperatorNode(char Name) => Name switch
    {
        '+' => new AdditionOperatorNode(),
        '-' => new subtractionOperatorNode(),
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
        _   => new CharNode(Name)
    };

    /// <summary>Метод определения функционала по имени</summary>
    /// <param name="Name">Имя функционала</param>
    /// <returns>Функционал</returns>
    /// <exception cref="NotSupportedException">Возникает для неопределённых имён функционалов</exception>
    public static Functional GetFunctional(string Name) => Name switch
    {
        "sum"      => new SumOperator(Name),
        "Sum"      => new SumOperator(Name),
        "Σ"        => new SumOperator(Name),
        "int"      => new IntegralOperator(Name),
        "integral" => new IntegralOperator(Name),
        "Int"      => new IntegralOperator(Name),
        "Integral" => new IntegralOperator(Name),
        "∫"        => new IntegralOperator(Name),
        _          => throw new NotSupportedException($"Функционал {Name} не поддерживается")
    };

    /// <summary>Метод извлечения корня дерева из последовательности элементов математического выражения</summary>
    /// <param name="Group">группа элементов математического выражения</param>
    /// <param name="MathExpression">Ссылка на математическое выражение</param>
    /// <returns>Корень дерева мат.выражения</returns>
    internal ExpressionTreeNode GetRoot(Term[] Group, MathExpression MathExpression)
    {
        // Ссылка на последний обработанный узел дерева
        ExpressionTreeNode? last = null;
        for (var i = 0; i < Group.Length; i++) // в цикле по всем элементам группы
        {
            var node = Group[i].GetSubTree(this, MathExpression); // извлечь поддерево для текущего элемента группы
            // Если очередной элемент группы...

            switch (Group[i])
            {
                case NumberTerm _: // ...очередной элемент число, то
                    //...если есть впереди ещё два элемента и прошла удачная попытка считать разделитель дробного числи и мантиссу  
                    if (i + 2 < Group.Length && NumberTerm.TryAddFractionPart(ref node, Group[i + 1], DecimalSeparator, Group[i + 2]))
                        i += 2; //...то увеличить индекс текущей группы на два.
                    break;
                case BlockTerm block:               //...очередной элемент блок (со скобками)
                    node = new ComputedBracketNode( // очередной узел дерева - это вычислимый блок
                        new Bracket(                //вид скобок:
                            block.OpenBracket,      // копируем вид открывающей скобки
                            block.CloseBracket),    // копируем вид закрывающей скобки
                        node);                      //Внутри узел дерева
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
    public virtual void Combine(ExpressionTreeNode? Last, ExpressionTreeNode Node)
    {
        if (Last is null) return; // Если предыдущий узел дерева не указан, возврат

        if (Node is CharNode) // Если текущий узел - символьный узел, то
        {
            Last.LastRightChild = Node; // просто назначить его самым правым дочерним
            return;
        }

        // представляем текущий узел в виде узла-оператора
        if (Node is OperatorNode operator_node) // если текущий узел является оператором...
        {
            //Пытаемся получить оператор предыдущего узла:
            // пытаемся привести предыдущий узел к типу узла оператора
            // либо пытаемся привести родительский узел предыдущего узла к типу узла оператора
            var parent_operator = Last as OperatorNode ?? Last.Parent as OperatorNode;

            if (parent_operator != null) // Если получена ссылка не предыдущий узел-оператор (и текущий является оператором)... то
            {
                // Если левое поддерево предыдущего оператор пусто и родитель предыдущего оператора - тоже оператор
                //      op <- устанавливаем предыдущим оператором родителя
                //      |
                //      op 
                //     /  \
                //  null   ?
                if (parent_operator.Left is null && parent_operator.Parent is OperatorNode @operator)
                    parent_operator = @operator;


                if (parent_operator.Left is null)         // Если левое поддерево предыдущего оператора пусто...
                    operator_node.Left = parent_operator; //  устанавливаем предыдущий оператор в качестве левого поддерева текущего
                else if (parent_operator.Right is null)   // Иначе если правое поддерево пусто
                    parent_operator.Right = Node;         //  установить текущий оператор правым поддеревом предыдущего
                else                                      // Иначе если конфликт приоритетов
                {
                    var priority = operator_node.Priority; // извлекаем приоритет текущего узла
                    // Если приоритет текущего оператора меньше, либо равен приоритету предыдущего
                    if (priority <= parent_operator.Priority)
                    {
                        // то надо подниматься вверх под дереву до тех пор
                        parent_operator = (OperatorNode?)parent_operator.Parents
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
                                //  то вставить текущий оператор в правое поддерево родителя предыдущего оператора
                                throw new NotImplementedException("!!!");
                        //var right = parent_operator.Right; // сохранить правое поддерево предыдущего оператора
                        //parent_operator.Right = Node; // У предыдущего оператора в правое поддерево внести текущий оператор
                        //operator_node.Left = right;   // В левое поддерево текущего оператора внести сохранённое правое поддерево
                        else // Иначе если предыдущий оператор не корень
                        {
                            var parent = parent_operator.Parent;  // сохранить ссылку на родителя предыдущего оператора
                            parent.Right       = Node;            // записать текущий оператор в качестве правого поддерева
                            operator_node.Left = parent_operator; // записать предыдущий оператора левым поддеревом текущего
                        }
                    }
                    else //если приоритет текущего оператора больше приоритета предыдущего
                    {
                        // то надо спускаться в правое поддерево до тех пор
                        parent_operator = (OperatorNode?)parent_operator.RightNodes
                            // пока встречаемые на пути операторы имеют левые поддеревья и приоритет операторов меньше текущего
                           .TakeWhile(n => n is OperatorNode { Left: { } } node && node.Priority < priority)
                            // взять последний из последовательности
                           .LastOrDefault() ?? parent_operator; // если вернулась пустая ссылка, то взять предыдущий оператор

                        // На текущий момент предыдущий оператор имеет приоритет ниже приоритета текущего оператора

                        var right = parent_operator.Right; // сохранить правое поддерево предыдущего оператора
                        parent_operator.Right = Node;      // в правое поддерево предыдущего оператора попадает текущий
                        operator_node.Left    = right;     // в левое поддерево текущего оператора записывается сохранённое правое 
                    }
                }
            }
            else // Если предыдущий узел не является оператором
            {
                var parent   = Last.Parent;
                var is_left  = Last.IsLeftSubtree;
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
            Last.Right = Node; // добавить текущий в правое поддерево предыдущего
            return;            // возврат
        }
        // Если ни текущий не предыдущий узлы не являются операторами

        //Если предыдущий узел был числом, или предыдущий узел был скобками и текущий - скобки
        if (Last is ConstValueNode || (Last is ComputedBracketNode && Node is ComputedBracketNode))
        {
            //Сохраняем ссылку на родителя предыдущего узла
            var parent = Last.Parent;
            if (parent != null) // если родитель есть
                // в правое поддерево родителя записываем оператор перемножения предыдущего узла и текущего
                parent.Right = new MultiplicationOperatorNode(Last, Node);
            else // если предыдущий узел был узлом дерева
                // создаём новый узел-оператор умножения предыдущего узла и текущего, который становится новым корнем дерева
                _ = new MultiplicationOperatorNode(Last, Node);
            return; // возврат.
        }

        Last.Right = Node;
        //throw new FormatException(); // Если не сработало ни одно условие, то это ошибка формата
    }
}