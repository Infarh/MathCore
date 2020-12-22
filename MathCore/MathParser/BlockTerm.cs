﻿using System;
using System.Collections.Generic;
using System.Linq;
using MathCore.Annotations;
using MathCore.MathParser.ExpressionTrees.Nodes;
// ReSharper disable ConvertToAutoProperty
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.MathParser
{
    /// <summary>Элемент мат.выражения - блок со скобками</summary>
    internal sealed class BlockTerm : Term
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
        public BlockTerm([NotNull] string Str) : this(string.Empty, Str, string.Empty) { }

        /// <summary>Новый блок выражения</summary>
        /// <param name="OpenBracket">Открывающаяся скобка</param>
        /// <param name="Str">Строковое значение блока</param>
        /// <param name="CloseBracket">Закрывающаяся скобка</param>
        public BlockTerm([NotNull] string OpenBracket, [NotNull] string Str, [NotNull] string CloseBracket)
            : base(string.Format("{0}{2}{1}", OpenBracket, CloseBracket, Str))
        {
            _OpenBracket = OpenBracket;
            _CloseBracket = CloseBracket;
            _Terms = GetTerms(Str) ?? throw new InvalidOperationException();
        }

        public BlockTerm(string OpenBracket, [NotNull] Term[] terms, string CloseBracket)
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
        [CanBeNull]
        private static string GetNumberString([NotNull] string Str, ref int pos)
        {
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
            var result = string.Empty;
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
            if(Str is null) return null;
            if(Str.Length == 0) return Array.Empty<Term>();
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
                                    var block_str = Str.GetBracketText(ref pos) ?? throw new InvalidOperationException("Получена пустая ссылка на блок выражения");
                                    var block = new BlockTerm("(", block_str, ")");
                                    value = new FunctionTerm((StringTerm)value, block);
                                }
                                break;
                            case '[':
                                {
                                    var block_str = Str.GetBracketText(ref pos, "[", "]") ?? throw new InvalidOperationException("Получена пустая ссылка на блок выражения");
                                    var block = new BlockTerm("[", block_str, "]");
                                    value = new FunctionTerm((StringTerm)value, block);
                                }
                                break;
                            case '{':
                                {
                                    var block_str = Str.GetBracketText(ref pos, "{", "}") ?? throw new InvalidOperationException("Получена пустая ссылка на блок выражения");
                                    var block = new BlockTerm("{", block_str, "}");
                                    value = new FunctionTerm((StringTerm)value, block);
                                }
                                break;
                        }
                    if(pos < len && Str[pos] == '{')
                        value = new FunctionalTerm
                        (
                            (FunctionTerm)value,
                            new BlockTerm("{", Str.GetBracketText(ref pos, "{", "}") ?? throw new InvalidOperationException("Получена пустая ссылка на блок выражения"), "}")
                        );
                    result.Add(value);
                }
                else if(char.IsDigit(c))
                    result.Add(new NumberTerm(GetNumberString(Str, ref pos) ?? throw new InvalidOperationException("Получена пустая ссылка на строку числового значения")));
                else
                    switch(c)
                    {
                        case '(':
                            {
                                var block_str = Str.GetBracketText(ref pos) ?? throw new InvalidOperationException("Получена пустая ссылка на блок выражения");
                                var block = new BlockTerm("(", block_str, ")");
                                result.Add(block);
                            }
                            break;
                        case '[':
                            {
                                var block_str = Str.GetBracketText(ref pos, "[", "]") ?? throw new InvalidOperationException("Получена пустая ссылка на блок выражения");
                                var block = new BlockTerm("[", block_str, "]");
                                result.Add(block);
                            }
                            break;
                        case '{':
                            {
                                var block_str = Str.GetBracketText(ref pos, "{", "}") ?? throw new InvalidOperationException("Получена пустая ссылка на блок выражения");
                                var block = new BlockTerm("{", block_str, "}");
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

        /// <summary>Преобразование в строковое представление</summary>
        /// <returns>Строковое представление</returns>
        [NotNull]
        public override string ToString() => $"{OpenBracket ?? string.Empty}{Terms.ToSeparatedStr()}{CloseBracket ?? string.Empty}";

        /// <summary>Получить корень поддерева выражений</summary>
        /// <param name="Parser">Парсер выражения</param>
        /// <param name="Expression">Математическое выражение</param>
        /// <returns>Корень поддерева</returns>
        public override ExpressionTreeNode GetSubTree(ExpressionParser Parser, MathExpression Expression)
        {
            var separator = Parser.ExpressionSeparator; // фиксируем символ-разделитель выражений
            // Разбиваем последовательность элементов выражения на группы, разделённые символом-разделителем
            // Извлекаем из каждой группы корень дерева выражений и складываем их в массив
            var roots = Terms
                .Split(t => t is CharTerm term && term.Value == separator)
                .Select(g => Parser.GetRoot(g, Expression)).ToArray();


            if(roots.Length == 1) return roots[0]; // Если найден только один корень, то возвращаем его
            // Иначе корней найдено много
            ExpressionTreeNode argument = null; // объявляем ссылку на аргумент
            // проходим по всем найденным корням
            foreach (var root in roots)
            {
                var arg = root switch
                {
                    FunctionArgumentNode => root,
                    FunctionArgumentNameNode name_node => new FunctionArgumentNode(name_node),
                    VariantOperatorNode when root.Left is VariableValueNode value_node => new FunctionArgumentNode(value_node.Name, root.Right),
                    _ => new FunctionArgumentNode(string.Empty, root)
                };

                argument = argument is null 
                    ? arg                   // Если аргумент не был указан, то сохраняем полученный узел, как аргумент
                    : argument.Right = arg; //  сохраняем полученный узел в правое поддерево аргумента
            }
            // Если аргумент не был выделен, то что-то пошло не так - ошибка формата
            if(argument is null) throw new FormatException("Не определён аргумент функции");
            return argument.Root; // Вернуть корень аргумента
        }
    }
}