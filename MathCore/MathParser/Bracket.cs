using System;
using System.Diagnostics;
using MathCore.Annotations;
// ReSharper disable UnusedMember.Global

namespace MathCore.MathParser
{
    /// <summary>Скобки</summary>
    public class Bracket : IEquatable<Bracket>, ICloneable<Bracket>
    {
        /// <summary>Круглые скобки</summary>
        [NotNull]
        public static Bracket NewRound => new Bracket("(", ")");

        /// <summary>Квадратные скобки</summary>
        [NotNull]
        public static Bracket NewRect => new Bracket("[", "]");

        /// <summary>Фигурные скобки</summary>
        [NotNull]
        public static Bracket NewFigure => new Bracket("{", "}");

        /// <summary>Открывающая скобка</summary>
        [NotNull]
        public string Start { get; }

        /// <summary>Закрывающая скобка</summary>
        [NotNull]
        public string Stop { get; }

        /// <summary>Скобки</summary>
        /// <param name="Start">Строка открывающей скобки</param>
        /// <param name="Stop">Строка закрывающей скобки</param>
        public Bracket([NotNull] string Start, [NotNull] string Stop)
        {
            this.Start = Start;
            this.Stop = Stop;
        }

        /// <summary>Проверка на эквивалентность другим скобкам</summary>
        /// <param name="other">Проверяемые на эквивалентность скобки</param>
        /// <returns>Истина, если проверяемые скобки эквивалентны данным</returns>
        public bool Equals(Bracket other) => other != null && (ReferenceEquals(this, other) || string.Equals(Start, other.Start) && string.Equals(Stop, other.Stop));

        /// <summary>Проверка на эквивалентность</summary>
        /// <param name="obj">Проверяемый объект</param>
        /// <returns>Истина, если объект - скобки и вид скобок совпадает</returns>
        public override bool Equals(object obj) => obj != null && (ReferenceEquals(this, obj) || obj.GetType() == GetType() && Equals((Bracket)obj));

        /// <summary>Получить хэш-код</summary>
        /// <returns>Хэш-код</returns>
        public override int GetHashCode() { unchecked { return ((Start?.GetHashCode() ?? 0) * 397) ^ (Stop?.GetHashCode() ?? 0); } }

        object ICloneable.Clone() => Clone();

        /// <summary>Клонирование скобок</summary>
        /// <returns>Клон скобок</returns>
        [NotNull]
        public virtual Bracket Clone() => new Bracket(Start, Stop);

        /// <summary>Строковое представление скобок</summary>
        /// <returns>Строковое представление</returns>
        public override string ToString() => Surround("...");

        /// <summary>Разместить текст в скобках</summary>
        /// <param name="str">Размещаемый текст</param>
        /// <returns>Текст в скобках</returns>
        [NotNull]
        public string Surround([CanBeNull] string str)
        {
            Trace.TraceWarning("В обёртку блока скобок передана пустая строка");
            return $"{Start}{str}{Stop}";
        }
    }
}