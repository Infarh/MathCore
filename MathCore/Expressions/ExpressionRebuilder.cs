using System.Collections.Generic;
using System.Collections.ObjectModel;

// ReSharper disable once CheckNamespace
namespace System.Linq.Expressions
{
    /// <summary>Пересборщик дерева выражения Linq.Expression</summary>
    //[Diagnostics.DST]
    public class ExpressionRebuilder : ExpressionVisitorEx
    {
        /// <summary>Метод генерации события посещения узла типа Expression</summary>
        /// <typeparam name="TExpressionNode">Тип узла дерева</typeparam>
        /// <param name="Handlers">Событие</param>
        /// <param name="Node">Узел дерева</param>
        /// <param name="Base">Базовый метод обработки узла</param>
        /// <returns>Узел, которым надо заместить посещённый узел дерева</returns>
        private Expression InvokeEvent<TExpressionNode>(EventHandlerReturn<EventArgs<TExpressionNode>, Expression> Handlers, TExpressionNode Node, Func<TExpressionNode, Expression> Base)
            where TExpressionNode : Expression
        {
            // Если обработчиков события нет, то вызываем базовый метод и возвращаем результат
            var element = Base(Node); // Вызываем базовый метод для получения замены
            if(Handlers is null) return element;
            var node = element as TExpressionNode;
            return node != null
                ? Handlers(this, new EventArgs<TExpressionNode>(node))
                : element; // иначе возвращаем элемент, от базового метода
        }

        /// <summary>Метод генерации события посещения узла произвольного типа</summary>
        /// <typeparam name="TElement">Тип узла дерева</typeparam><typeparam name="TOut">Тип выходного узла</typeparam>
        /// <param name="Handlers">Событие</param>
        /// <param name="Node">Посещённый узел дерева</param>
        /// <param name="Base">Базовый метод обработки узла</param>
        /// <returns>Узел, которым надо заместить посещённый узел дерева</returns>
        private TOut InvokeEvent<TElement, TOut>(EventHandlerReturn<EventArgs<TOut>, TOut> Handlers, TElement Node, Func<TElement, TOut> Base)
        {
            // Если обработчиков события нет, то вызываем базовый метод и возвращаем результат
            var element = Base(Node); // Вызываем базовый метод для получения замены
            if(Handlers is null) return element;
            // Генерируем событие с передачей в него узла, полученного от базового дерева
            return Handlers(this, new EventArgs<TOut>(element));
        }

        /// <summary>Событие возникает при посещении любого узла дерева</summary>
        public event EventHandlerReturn<EventArgs<Expression>, Expression> Visited;

        /// <summary>Посетить узел дерева</summary><param name="Node">Узел дерева</param><returns>Новый узел дерева</returns>
        public override Expression Visit(Expression Node) => InvokeEvent(Visited, Node, base.Visit);

        /// <summary>Событие возникает при посещении узла дерева бинарного выражения</summary>
        public event EventHandlerReturn<EventArgs<BinaryExpression>, Expression> BinaryVisited;
        protected override Expression VisitBinary(BinaryExpression b) => InvokeEvent(BinaryVisited, b, base.VisitBinary);

        /// <summary>Событие возникает при посещении узла привязки</summary>
        public event EventHandlerReturn<EventArgs<MemberBinding>, MemberBinding> BindingVisited;
        protected override MemberBinding VisitBinding(MemberBinding binding) => InvokeEvent(BindingVisited, binding, base.VisitBinding);

        /// <summary>Событие возникает при посещении коллекции привязки</summary>
        public event EventHandlerReturn<EventArgs<IEnumerable<MemberBinding>>, IEnumerable<MemberBinding>> BindingListVisited;
        protected override IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original) => InvokeEvent(BindingListVisited, original, base.VisitBindingList);

        /// <summary>Событие возникает при посещении узла условного оператора</summary>
        public event EventHandlerReturn<EventArgs<ConditionalExpression>, Expression> ConditionalVisited;
        protected override Expression VisitConditional(ConditionalExpression c) => InvokeEvent(ConditionalVisited, c, base.VisitConditional);

        /// <summary>Событие возникает при посещении узла константы</summary>
        public event EventHandlerReturn<EventArgs<ConstantExpression>, Expression> ConstantlVisited;
        protected override Expression VisitConstant(ConstantExpression c) => InvokeEvent(ConstantlVisited, c, base.VisitConstant);

        /// <summary>Событие возникает при посещении узла инициализатора объекта</summary>
        public event EventHandlerReturn<EventArgs<ElementInit>, ElementInit> ElementInitializerVisited;
        protected override ElementInit VisitElementInitializer(ElementInit initializer) => InvokeEvent(ElementInitializerVisited, initializer, base.VisitElementInitializer);

        /// <summary>Событие возникает при посещении коллекции инициализаторов объекта</summary>
        public event EventHandlerReturn<EventArgs<IEnumerable<ElementInit>>, IEnumerable<ElementInit>> ElementInitializerListVisited;
        protected override IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original) => InvokeEvent(ElementInitializerListVisited, original, base.VisitElementInitializerList);

        /// <summary>Событие возникает при посещении коллекции выражений</summary>
        public event EventHandlerReturn<EventArgs<ReadOnlyCollection<Expression>>, ReadOnlyCollection<Expression>> ExpressionListVisited;
        protected override ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original) => InvokeEvent(ExpressionListVisited, original, base.VisitExpressionList);

        /// <summary>Событие возникает при посещении лямбда-выражения</summary>
        public event EventHandlerReturn<EventArgs<LambdaExpression>, Expression> LambdaVisited;
        protected override Expression VisitLambda(LambdaExpression lambda) => InvokeEvent(LambdaVisited, lambda, base.VisitLambda);

        /// <summary>Событие возникает при посещении узла инициализатора коллекции</summary>
        public event EventHandlerReturn<EventArgs<ListInitExpression>, Expression> ListInitVisited;
        protected override Expression VisitListInit(ListInitExpression init) => InvokeEvent(ListInitVisited, init, base.VisitListInit);

        /// <summary>Событие возникает при посещении узла вызова функции</summary>
        public event EventHandlerReturn<EventArgs<InvocationExpression>, Expression> InvocationVisited;
        protected override Expression VisitInvocation(InvocationExpression iv) => InvokeEvent(InvocationVisited, iv, base.VisitInvocation);

        /// <summary>Событие возникает при посещении узла доступа к члену объекта</summary>
        public event EventHandlerReturn<EventArgs<MemberExpression>, Expression> MemberAccessVisited;
        protected override Expression VisitMemberAccess(MemberExpression m) => InvokeEvent(MemberAccessVisited, m, base.VisitMemberAccess);

        /// <summary>Событие возникает при посещении узла присвоения члену объекта значения</summary>
        public event EventHandlerReturn<EventArgs<MemberAssignment>, MemberAssignment> MemberAssignmentVisited;
        protected override MemberAssignment VisitMemberAssignment(MemberAssignment assignment) => InvokeEvent(MemberAssignmentVisited, assignment, base.VisitMemberAssignment);

        /// <summary>Событие возникает при посещении узла инициализатора свойства объекта</summary>
        public event EventHandlerReturn<EventArgs<MemberInitExpression>, Expression> MemberInitVisited;
        protected override Expression VisitMemberInit(MemberInitExpression init) => InvokeEvent(MemberInitVisited, init, base.VisitMemberInit);

        /// <summary>Событие возникает при посещении узла инициализатора коллекции объектов</summary>
        public event EventHandlerReturn<EventArgs<MemberListBinding>, MemberListBinding> MemberListBindingVisited;
        protected override MemberListBinding VisitMemberListBinding(MemberListBinding binding) => InvokeEvent(MemberListBindingVisited, binding, base.VisitMemberListBinding);

        /// <summary>Событие возникает при посещении узла инициализатора элементов элемента</summary>
        public event EventHandlerReturn<EventArgs<MemberMemberBinding>, MemberMemberBinding> MemberMemberBindingVisited;
        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding) => InvokeEvent(MemberMemberBindingVisited, binding, base.VisitMemberMemberBinding);

        /// <summary>Событие возникает при посещении узла вызова метода</summary>
        public event EventHandlerReturn<EventArgs<MethodCallExpression>, Expression> MethodCallVisited;
        protected override Expression VisitMethodCall(MethodCallExpression m) => InvokeEvent(MethodCallVisited, m, base.VisitMethodCall);

        /// <summary>Событие возникает при посещении узла конструктора</summary>
        public event EventHandlerReturn<EventArgs<NewExpression>, NewExpression> NewVisited;
        protected override NewExpression VisitNew(NewExpression nex) => InvokeEvent(NewVisited, nex, base.VisitNew);

        /// <summary>Событие возникает при посещении узла конструктора массива</summary>
        public event EventHandlerReturn<EventArgs<NewArrayExpression>, Expression> NewArrayVisited;
        protected override Expression VisitNewArray(NewArrayExpression na) => InvokeEvent(NewArrayVisited, na, base.VisitNewArray);

        /// <summary>Событие возникает при посещении узла параметра выражения</summary>
        public event EventHandlerReturn<EventArgs<ParameterExpression>, Expression> ParameterVisited;
        protected override Expression VisitParameter(ParameterExpression p) => InvokeEvent(ParameterVisited, p, base.VisitParameter);

        /// <summary>Событие возникает при посещении узла определения типа выражения</summary>
        public event EventHandlerReturn<EventArgs<TypeBinaryExpression>, Expression> TypeIsVisited;
        protected override Expression VisitTypeIs(TypeBinaryExpression b) => InvokeEvent(TypeIsVisited, b, base.VisitTypeIs);

        /// <summary>Событие возникает при посещении узла унарного выражения</summary>
        public event EventHandlerReturn<EventArgs<UnaryExpression>, Expression> UnaryVisited;
        protected override Expression VisitUnary(UnaryExpression u) => InvokeEvent(UnaryVisited, u, base.VisitUnary);
    }
}