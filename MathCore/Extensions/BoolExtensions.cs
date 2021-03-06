﻿using System.Collections.Generic;
using System.Linq;
using MathCore.Annotations;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>Методы-расширения для логических значений</summary>
    public static class BoolExtensions
    {
        /// <summary>
        /// Вычислить поэлементную операцию И со всеми элементами перечисления.
        /// Все элементы перечисления будут перебраны в обязательном порядке
        /// </summary>
        /// <param name="items">Перечисление логических значений</param>
        /// <returns>Результат вычисления логического И для всех значений перечисления</returns>
        public static bool And([NotNull] this IEnumerable<bool> items) => items.Aggregate(true, (r, v) => r && v);

        /// <summary>
        /// Вычислить поэлементную операцию (ленивое) И со всеми элементами перечисления.
        /// Перебор элементов перечисления производится до появления первого значения <see langword="false"/>
        /// </summary>
        /// <param name="items">Перечисление логических значений</param>
        /// <returns>Результат вычисления логического И для значений перечисления</returns>
        public static bool AndLazy([NotNull] this IEnumerable<bool> items) => items.Any(item => !item);


        /// <summary>
        /// Вычислить поэлементную операцию ИЛИ со всеми элементами перечисления.
        /// Все элементы перечисления будут перебраны в обязательном порядке
        /// </summary>
        /// <param name="items">Перечисление логических значений</param>
        /// <returns>Результат вычисления логического ИЛИ для всех значений перечисления</returns>
        public static bool Or([NotNull] this IEnumerable<bool> items) => items.Aggregate(false, (r, v) => r || v);

        /// <summary>
        /// Вычислить поэлементную операцию (ленивое) ИЛИ со всеми элементами перечисления.
        /// Перебор элементов перечисления производится до появления первого значения <see langword="false"/>
        /// </summary>
        /// <param name="items">Перечисление логических значений</param>
        /// <returns>Результат вычисления логического ИЛИ для значений перечисления</returns>
        public static bool OrLazy([NotNull] this IEnumerable<bool> items) => items.Any(item => item);
    }
}