using System.Text;

namespace System.Linq.Expressions
{
    /// <summary>
    /// TeX supports several styles for multiplication sign
    /// </summary>
    public enum MultiplicationSign
    {
        /// <summary>
        /// Without any sign
        /// </summary>
        None,
        /// <summary>
        /// * sign
        /// </summary>
        Asterisk,
        /// <summary>
        /// x sign
        /// </summary>
        Times
    }

    /// <summary>
    ///  ласс "посетилител€", который "изучает" дерево выражени€ путем переопределени€ соответствующих
    /// виртуальных методов базового класса System.Linq.Expressions.ExpressionVisitor
    /// </summary>
    public class TeXExpressionVisitor : ExpressionVisitor
    {
        //----------------------------------------------------------------------------------------//
        // «акрытые пол€
        //----------------------------------------------------------------------------------------//
        // —юда мы будет сохран€ть "пройденные" части выражени€
        private readonly StringBuilder _Result = new StringBuilder();

        //----------------------------------------------------------------------------------------//
        //  онструкторы
        //----------------------------------------------------------------------------------------//

        //  онструктор принимает выражение, которое будет преобразовано в формат TeX-а
        public TeXExpressionVisitor(Expression expression) => Visit(expression);

        // Ћ€мбда-выражение анализируетс€ несколько по-иному, поскольку нам нужно только тело
        // выражени€, без первого параметра
        public TeXExpressionVisitor(LambdaExpression expression) => Visit(expression.Body);


        //----------------------------------------------------------------------------------------//
        // ќткрытые интерфейс
        //----------------------------------------------------------------------------------------//
        public string GenerateTeXExpression(string expressionName, MultiplicationSign multiplicationSign =
            MultiplicationSign.Asterisk)
        {
            // »змен€ем сгенерированную строку в зависимости от типа знака "умножени€"
            return GenerateTeXExpressionImpl(expressionName, multiplicationSign);
        }
        public string GenerateTeXExpression(MultiplicationSign multiplicationSign =
            MultiplicationSign.Asterisk) => GenerateTeXExpressionImpl(null, multiplicationSign);


        //----------------------------------------------------------------------------------------//
        // ѕереопределенные виртуальные методы базового класса ExpressionVisitor
        //----------------------------------------------------------------------------------------//
        // ћетоды, переопределенные от класса ExpressionVisitor
        protected override Expression VisitUnary(UnaryExpression node)
        {
            if(node.NodeType == ExpressionType.Negate)
                _Result.Append("-");
            return base.VisitUnary(node);
        }

        protected override Expression VisitBinary(BinaryExpression node) => IsInfix(node.NodeType) ? VisitInfixBinary(node) : VisitPrefixBinary(node);

        protected override Expression VisitMember(MemberExpression node)
        {
            var name = node.Member.Name.Split('.').Last();
            _Result.Append(name);
            return node;
        }

        public override Expression Visit(Expression node)
        {
            if(node is ConstantExpression constant)
            {
                _Result.Append(constant.Value);
                return Expression.Constant(constant.Value, constant.Type);
            }
            return base.Visit(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            _Result.Append(node.Name.Split('.').Last());
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            // Ёто очень проста€ реализаци€ поиска определенных методов.
            // ƒл€ промышленного кода мы можем добавить кэширование или что-то
            // подобное, здесь же этого соверешнно достаточно
            var pow_method = typeof(Math).GetMethod("Pow");

            if(node.Method == pow_method)
            {
                Visit(node.Arguments[0]);
                _Result.AppendFormat("^{{{0}}}", node.Arguments[1]);
                return node;
            }
            return base.Visit(node);
        }

        //----------------------------------------------------------------------------------------//
        // ¬спомогательные закрытые методы
        //----------------------------------------------------------------------------------------//

        // ћетодв возвращает true, если выражение нужно оборачивать в скобки: ()
        private static bool RequiresPrecedence(ExpressionType nodeType) =>
            nodeType switch
            {
                ExpressionType.Add => true,
                ExpressionType.Subtract => true,
                _ => false
            };

        // ќператор делени€ несколько отличаетс€ от всех остальных операторов с двум€ аргументами,
        // поскольку он требует другого пор€дка аргументов
        private static bool IsInfix(ExpressionType nodeType) => nodeType != ExpressionType.Divide;

        // Ѕольшинство операторов требуют аргументы в следующем пор€дке:
        // {arg1} op {arg2}
        private Expression VisitInfixBinary(BinaryExpression node)
        {
            var requires_precedence = RequiresPrecedence(node.NodeType);
            if(requires_precedence) _Result.Append("(");

            Visit(node.Left);

            switch(node.NodeType)
            {
                case ExpressionType.Multiply:
                    _Result.Append("*");
                    break;
                case ExpressionType.Add:
                    _Result.Append("+");
                    break;
                case ExpressionType.Subtract:
                    _Result.Append("-");
                    break;
                default:
                    throw new NotSupportedException($"The binary operator '{node.NodeType}' is not supported");
            }

            Visit(node.Right);

            if(requires_precedence) _Result.Append(")");
            return node;
        }


        /// <summary>
        /// ќператор делени€ \fract требует иного пор€дка аргументов:
        /// \frac{arg1}{arg2} 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private Expression VisitPrefixBinary(BinaryExpression node)
        {
            // ƒл€ делени€ (x + 2) на 3, мы должны получить следующее выражение
            // \frac{x + 2}{3}
            switch(node.NodeType)
            {
                case ExpressionType.Divide:
                    _Result.Append(@"\frac");
                    break;
                default:
                    throw new InvalidOperationException($"Unknown prefix BinaryExpression {node.Type}");
            }

            _Result.Append("{");
            Visit(node.Left);
            _Result.Append("}");

            _Result.Append("{");
            Visit(node.Right);
            _Result.Append("}");
            return node;
        }

        // ћетод, реализующий получение строкового представлени€ полученного выражени€
        private string GenerateTeXExpressionImpl(string expressionName, MultiplicationSign multiplicationSign)
        {
            switch(multiplicationSign)
            {
                case MultiplicationSign.Times:
                    _Result.Replace("*", @" \times ");
                    break;
                case MultiplicationSign.None:
                    _Result.Replace("*", "");
                    break;
            }

            var texExpression = _Result.ToString();

            // ѕолученное выражение содержит избыточные круглые скобки, которые следует убрать
            if(texExpression.Length > 0)
                if(texExpression[0] == '(' && texExpression[texExpression.Length - 1] == ')')
                    texExpression = texExpression.Substring(1, texExpression.Length - 2);

            // ƒобавл€ем "им€ выражени€" если оно указано
            return !string.IsNullOrEmpty(expressionName)
                ? $"{{{expressionName}}}{{=}}{texExpression}"
                : texExpression;
        }
    }
}