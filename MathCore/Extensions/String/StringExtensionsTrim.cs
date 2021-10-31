﻿using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>Ограничение длины строки с удалением центральной её части</summary>
    public static class StringExtensionsTrim
    {
        /// <summary>Ограничение длины строки с удалением центральной её части</summary>
        /// <param name="Str">Обрезаемая строка</param>
        /// <param name="Length">Требуемая длина</param>
        /// <param name="ReplacementPattern">Шаблон замены</param>
        /// <returns>Строка с удалённой внутренней частью</returns>
        [NotNull]
        public static string TrimByLength([NotNull] this string Str, int Length, [NotNull] string ReplacementPattern = "..")
        {
            if (Str is null) throw new ArgumentNullException(nameof(Str));

            if(Str.Length <= Length) return Str;
            if(Length == 0) return string.Empty;

            var dl1 = Str.Length - Length + ReplacementPattern.Length;
            var dl2 = dl1 / 2;
            dl1 -= dl2;

            var s1 = Str[..(Str.Length / 2 - dl1)];
            var start = Str.Length/2 + dl2;
            var len = Str.Length - Str.Length / 2 - dl2;
            var s2 = Str.Substring(start, len);

            return string.Format("{0}{2}{1}", s1, s2, ReplacementPattern);
        }
    }
}