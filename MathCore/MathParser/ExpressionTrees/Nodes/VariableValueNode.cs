using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using MathCore.Annotations;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    /// <summary>Узел дерева, хранящий переменную</summary>
    public class VariableValueNode : ValueNode
    {
        private ExpressionVariabel _Variable;

        /// <summary>Признак возможности получения тревиального значения</summary>
        public override bool IsPrecomputable => _Variable.IsPrecomputable;

        /// <summary>Ссылка на переменную</summary>
        [NotNull]
        public ExpressionVariabel Variable { get => _Variable; set => _Variable = value; }

        /// <summary>Значение узла</summary>
        public override double Value { get => _Variable.Value; set => _Variable.Value = value; }

        /// <summary>Имя переменной</summary> 
        public string Name { [DST] get => _Variable.Name; }

        /// <summary>Новый узел переменной</summary>
        /// <param name="Variable">Переменная</param>
        public VariableValueNode([NotNull] ExpressionVariabel Variable)
        {
            Contract.Requires(Variable != null);
            _Variable = Variable;
        }

        /// <summary>Преобразование в строковую форму</summary>
        /// <returns>Строковое представление</returns>
        public override string ToString() => $"{Left?.ToString() ?? ""}{Name}{Right?.ToString() ?? ""}";

        /// <summary>Вычислить значение поддерева</summary>
        /// <returns>Численное значение поддерева</returns>
        [DST]
        public override double Compute() => _Variable.GetValue();

        /// <summary>Скомпилировать в выражение</summary>
        /// <returns>Скомпилированное выражение System.Linq.Expressions</returns>
        public override Expression Compile() => Expression.Call
        (
            Expression.Constant(_Variable),
            Variable.GetType().GetMethod("GetValue", new Type[0]) ?? throw new InvalidOperationException("Метод GetValue не найден")
        );

        /// <summary>Скомпилировать в выражение</summary>
        /// <param name="Parameters">Массив параметров</param>
        /// <returns>Скомпилированное выражение System.Linq.Expressions</returns>
        public override Expression Compile(ParameterExpression[] Parameters) => Parameters.Find(p => p.Name == Name) ?? Compile();

        public override IEnumerable<ExpressionVariabel> GetVariables() => base.GetVariables().AppendFirst(_Variable);

        /// <summary>Клонирование узла</summary>
        /// <returns>Клон узла</returns>
        public override ExpressionTreeNode Clone() => new VariableValueNode(_Variable.Clone())
        {
            Left = Left?.Clone(),
            Right = Right?.Clone()
        };
    }
}