using System;
using System.Diagnostics.Contracts;
using System.Linq;
using MathCore.Annotations;
using MathCore.MathParser.ExpressionTrees.Nodes;

namespace MathCore.MathParser
{
    /// <summary>Числовой элемент математического выражения</summary>
    sealed class NumberTerm : Term
    {
        /// <summary>Численное значение элемента</summary>
        private int _IntValue;

        /// <summary>Численное значение элемента</summary>
        public int Value
        {
            get => _IntValue;
            set
            {
                _IntValue = value;
                _Value = value.ToString();
            }
        }

        /// <summary>Новый численный элемент мат.выражения</summary>
        /// <param name="Str">Строковое значение элемента</param>
        public NumberTerm([NotNull] string Str) : base(Str)
        {
            Contract.Requires(!string.IsNullOrEmpty(Str));
            Contract.Requires(Str.All(char.IsDigit));
            _IntValue = int.Parse(Str);
        }

        public NumberTerm(int Value) : base(Value.ToString()) => _IntValue = Value;

        /// <summary>Извлеч поддерево</summary>
        /// <param name="Parser">Парсер</param>
        /// <param name="Expression">Математическое выражение</param>
        /// <returns>Узел константного значения</returns>
        public override ExpressionTreeNode GetSubTree(ExpressionParser Parser, MathExpression Expression)
            => new ConstValueNode(_IntValue);

        /// <summary>Попытаться добавить дробное значение числа</summary>
        /// <param name="node">Узел выражения</param>
        /// <param name="SeparatorTerm">Блок разделитель</param>
        /// <param name="DecimalSeparator">Блок с целой частью числа</param>
        /// <param name="FrationPartTerm">Блок с дробной частью числа</param>
        /// <returns>Истина, если действие совершено успешно. Ложь, если в последующих блоках не содержится нужной информации</returns>
        public static bool TryAddFractionPart(ref ExpressionTreeNode node, Term SeparatorTerm, char DecimalSeparator, Term FrationPartTerm)
        {
            var value = node as ConstValueNode;
            if(value == null) throw new ArgumentException("Неверный тип узла дерева");
            var separator = SeparatorTerm as CharTerm;
            if(separator == null || separator.Value != DecimalSeparator) return false;
            var fraction = FrationPartTerm as NumberTerm;
            if(fraction == null) return false;

            var v_value = fraction.Value;
            if(v_value == 0) return true;
            node = new ConstValueNode(value.Value + v_value / Math.Pow(10, Math.Truncate(Math.Log10(v_value)) + 1));
            return true;
        }
    }
}