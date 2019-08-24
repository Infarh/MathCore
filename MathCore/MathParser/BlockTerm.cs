using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using MathCore.Annotations;
using MathCore.MathParser.ExpressionTrees.Nodes;

namespace MathCore.MathParser
{
    /// <summary>Элемент мат.выражения - блок со скобками</summary>
    sealed class BlockTerm : Term
    {
        /// <summary>Строковое значение открывающейся скобки</summary>
        private readonly string _OpenBracket;
        /// <summary>Строковое значение закрывающейся скобки</summary>
        private readonly string _CloseBracket;

        /// <summary>Массив элементов подвыражения</summary>
        [NotNull]
        private readonly Term[] _Terms;

        /// <summary>Строковое значение открывающейся скобки</summary>
        public string OpenBracket => _OpenBracket;

        /// <summary>Строковое значение закрывающейся скобки</summary>
        public string CloseBracket => _CloseBracket;

        /// <summary>Массив элементов подвыражения</summary>
        [NotNull]
        public Term[] Terms => _Terms;

        /// <summary>Новый блок математического выражения</summary>
        /// <param name="Str">Строковое значение блока</param>
        public BlockTerm(string Str) : this("", Str, "") { }

        /// <summary>Новый блок выражения</summary>
        /// <param name="OpenBracket">Открывающаяся скобка</param>
        /// <param name="Str">Строковое значение блока</param>
        /// <param name="CloseBracket">Закрывающаяся скобка</param>
        public BlockTerm([NotNull] string OpenBracket, [NotNull] string Str, [NotNull] string CloseBracket)
            : base(string.Format("{0}{2}{1}", OpenBracket ?? "", CloseBracket ?? "", Str))
        {
            Contract.Requires(!string.IsNullOrEmpty(Str));
            _OpenBracket = OpenBracket;
            _CloseBracket = CloseBracket;
            _Terms = GetTerms(Str);
        }

        public BlockTerm(string OpenBracket, Term[] terms, string CloseBracket)
            : base($"{OpenBracket}{terms.ToSeparatedStr()}{CloseBracket}")
        {
            _Terms = terms;
            _OpenBracket = OpenBracket;
            _CloseBracket = CloseBracket;
        }

        /// <summary>Получить цифровую строку</summary>
        /// <param name="Str">Исследуемая строка</param>
        /// <param name="pos">Исходная позиция в строке</param>
        /// <returns>Строка цифрового значения</returns>
        private static string GetNumberString([NotNull] string Str, ref int pos)
        {
            Contract.Requires(!string.IsNullOrEmpty(Str));
            Contract.Ensures(Contract.ValueAtReturn(out pos) >= 0);
            Contract.Ensures(Contract.ValueAtReturn(out pos) < Str.Length);

            var p = pos;
            var l = Str.Length;
            while(p < l && !char.IsDigit(Str, p)) p++;
            if(p >= l) return null;
            var start = p;
            while(p < l && char.IsDigit(Str, p)) p++;
            pos = p;
            return Str.Substring(start, p - start);
        }

        /// <summary>Получить имя из строки</summary>
        /// <param name="Str">Исходная строка</param>
        /// <param name="pos">Положение в строке</param>
        /// <returns>Строка имени</returns>
        private static string GetNameString([NotNull] string Str, ref int pos)
        {
            Contract.Requires(!string.IsNullOrEmpty(Str));
            Contract.Ensures(Contract.ValueAtReturn(out pos) >= 0);
            Contract.Ensures(Contract.ValueAtReturn(out pos) < Str.Length);

            var result = "";
            var L = Str.Length;
            var i = pos;
            while(i < L && (char.IsLetter(Str[i]) || Str[i] == '∫'))
                result += Str[i++];
            if(i == L || !char.IsDigit(Str[i]))
            {
                pos = i;
                return result;
            }
            while(i < L && char.IsDigit(Str[i]))
                result += Str[i++];
            pos += result.Length;
            return result;
        }

        /// <summary>Получить список элементов математического выражения из строки</summary>
        /// <param name="Str">Строковое представление математического выражения</param>
        /// <returns>Массив элементов математического выражения</returns>
        [CanBeNull]
        private static Term[] GetTerms([CanBeNull] string Str)
        {
            if(Str == null) return null;
            if(Str.Length == 0) return new Term[0];
            var pos = 0;
            var len = Str.Length;
            var result = new List<Term>();
            while(pos < len)
            {
                var c = Str[pos];
                if(char.IsLetter(c) || c == '∫')
                {
                    Term value = new StringTerm(GetNameString(Str, ref pos));
                    if(pos < len)
                        switch(Str[pos])
                        {
                            case '(':
                                {
                                    var blokStr = Str.GetBracketText(ref pos);
                                    var block = new BlockTerm("(", blokStr, ")");
                                    value = new FunctionTerm((StringTerm)value, block);
                                }
                                break;
                            case '[':
                                {
                                    var blokStr = Str.GetBracketText(ref pos, "[", "]");
                                    var block = new BlockTerm("[", blokStr, "]");
                                    value = new FunctionTerm((StringTerm)value, block);
                                }
                                break;
                            case '{':
                                {
                                    var blokStr = Str.GetBracketText(ref pos, "{", "}");
                                    var block = new BlockTerm("{", blokStr, "}");
                                    value = new FunctionTerm((StringTerm)value, block);
                                }
                                break;
                        }
                    if(pos < len && Str[pos] == '{')
                        value = new FunctionalTerm
                        (
                            (FunctionTerm)value,
                            new BlockTerm("{", Str.GetBracketText(ref pos, "{", "}"), "}")
                        );
                    result.Add(value);
                }
                else if(char.IsDigit(c))
                    result.Add(new NumberTerm(GetNumberString(Str, ref pos)));
                else
                    switch(c)
                    {
                        case '(':
                            {
                                var blokStr = Str.GetBracketText(ref pos);
                                var block = new BlockTerm("(", blokStr, ")");
                                result.Add(block);
                            }
                            break;
                        case '[':
                            {
                                var blokStr = Str.GetBracketText(ref pos, "[", "]");
                                var block = new BlockTerm("[", blokStr, "]");
                                result.Add(block);
                            }
                            break;
                        case '{':
                            {
                                var blokStr = Str.GetBracketText(ref pos, "{", "}");
                                var block = new BlockTerm("{", blokStr, "}");
                                result.Add(block);
                            }
                            break;
                        default:
                            result.Add(new CharTerm(Str[pos++]));
                            break;
                    }
            }
            return result.ToArray();
        }

        /// <summary>Преборазование в строковое представление</summary>
        /// <returns>Строковое представление</returns>
        public override string ToString() => $"{OpenBracket ?? ""}{Terms.ToSeparatedStr()}{CloseBracket ?? ""}";

        /// <summary>Получить корень поддерева выражений</summary>
        /// <param name="Parser">Парсер выражения</param>
        /// <param name="Expression">Математическое выражение</param>
        /// <returns>Корень поддерева</returns>
        public override ExpressionTreeNode GetSubTree(ExpressionParser Parser, MathExpression Expression)
        {
            Contract.Requires(Parser != null);
            Contract.Requires(Expression != null);
            Contract.Ensures(Contract.Result<ExpressionTreeNode>() != null);
            var separator = Parser.ExpressionSeparator; // фиксируем символ-разделитель выражений
            // Разбиваем последовательность элементов выражения на группы, разделённые симовлом-разделителем
            // Излвекаем из каждой группы корень дерева выражений и складываем их в массив
            var roots = Terms
                .Split(t => t is CharTerm && ((CharTerm)t).Value == separator)
                .Select(g => Parser.GetRoot(g, Expression)).ToArray();


            if(roots.Length == 1) return roots[0]; // Если найден только один корень, то возвращаем его
            // Иначе корней найдено много
            ExpressionTreeNode argument = null; // объявляем ссылку на аргумент
            for(var i = 0; i < roots.Length; i++) // проходим по всем найденным корням
            {
                var root = roots[i];
                ExpressionTreeNode arg;          // Если очередной корень дерева
                if(root is FunctionArgumentNode) // - аргумент функции
                    arg = root;                  // -- оставляем его без изменений
                else if(root is FunctionArgumentNameNode)  // - узел имени аргумента
                    // -- создаём новый именованный аргумент функции
                    arg = new FunctionArgumentNode(root as FunctionArgumentNameNode);
                else if(root is VariantOperatorNode && root.Left is VariableValueNode)
                    arg = new FunctionArgumentNode(((VariableValueNode)root.Left).Name, root.Right);
                else // - во всех остальных случаях
                    arg = new FunctionArgumentNode("", root); // -- создаём аргумент функции без имени
                if(argument == null) argument = arg; // Если аргумент не был указан, то сохраняем полученный узел, как аргумент
                else                                 // иначе
                    argument = argument.Right = arg; //  сохраняем полученный узел в правое поддерево аргумента
            }
            // Если аргумент не был выделен, то что-то пошло не так - ошибка формата
            if(argument == null) throw new FormatException("Не определён аргумент функции");
            return argument.Root; // Вернуть корень аргумента
        }
    }
}