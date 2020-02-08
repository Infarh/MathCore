using System;
using System.Runtime.InteropServices;

namespace MathCore
{
    /// <summary>Интервальный предикат</summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    //[System.ComponentModel.TypeConverter(typeof(IntervalConverter))]
    public readonly struct TimeInterval
    {
        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Включена ли нижняя граница интервала?</summary>
        private readonly bool _MinInclude;
        /// <summary>Включена ли верхняя граница интервала?</summary>
        private readonly bool _MaxInclude;
        /// <summary>Нижняя граница интервала</summary>
        private readonly TimeSpan _Min;
        /// <summary>Верхняя граница интервала</summary>
        private readonly TimeSpan _Max;

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Включена ли нижняя граница интервала?</summary>
        public bool MinInclude => _MinInclude;

        /// <summary>Включена ли верхняя граница интервала?</summary>
        public bool MaxInclude => _MaxInclude;

        /// <summary>Нижняя граница интервала</summary>
        public TimeSpan Min => _Min;

        /// <summary>Верхняя граница интервала</summary>
        public TimeSpan Max => _Max;
        /// <summary>Протяжённость интервала</summary>
        public TimeSpan Length => _Max - _Min;

        public TimeSpan Middle => TimeSpan.FromSeconds((_Min + _Max).TotalSeconds / 2);

        /// <summary>Интервал</summary>
        /// <param name="Max">Верхняя граница интервала</param>
        public TimeInterval(TimeSpan Max) : this(default, true, Max, true) { }

        /// <summary>Интервал</summary>
        /// <param name="Min">Нижняя граница интервала</param>
        /// <param name="Max">Верхняя граница интервала</param>
        public TimeInterval(TimeSpan Min, TimeSpan Max) : this(Min, true, Max, true) { }

        /// <summary>Интервал</summary>
        /// <param name="Min">Нижняя граница интервала</param>
        /// <param name="Max">Верхняя граница интервала</param>
        /// <param name="IncludeLimits">Включать пределы?</param>
        public TimeInterval(TimeSpan Min, TimeSpan Max, bool IncludeLimits) : this(Min, IncludeLimits, Max, IncludeLimits) { }

        /// <summary>Интервал</summary>
        /// <param name="Min">Нижняя граница интервала</param>
        /// <param name="MinInclude">Включена ли нижняя граница интервала?</param>
        /// <param name="Max">Верхняя граница интервала</param>
        /// <param name="MaxInclude">Включена ли верхняя граница интервала</param>
        public TimeInterval(TimeSpan Min, bool MinInclude, TimeSpan Max, bool MaxInclude)
        {
            _Min = Min;
            _Max = Max;
            _MinInclude = MinInclude;
            _MaxInclude = MaxInclude;
        }

        /// <summary>Проверка на вхождение в интервал</summary>
        /// <param name="X">Проверяемая величина</param>
        /// <returns>Истина, если величина входит в интервал</returns>
        public bool Check(TimeSpan X) => _MinInclude && X == _Min || _MaxInclude && X == _Max || X > _Min && X < _Max;

        public bool Check(TimeSpan X, TimeSpan MinOffset, TimeSpan MaxOffset)
        {
            var min = _Min + MinOffset;
            var max = _Max + MaxOffset;

            return _MinInclude && X == min || _MaxInclude && X == max || X > min && X < max;
        }

        public bool Check(TimeSpan X, TimeSpan Offset) => Check(X, Offset, -Offset);

        public TimeSpan Normalize(TimeSpan X) => X > _Max ? _Max : X < _Min ? _Min : X;

        public bool IsExclude(TimeInterval I) => !IsInclude(I);

        /// <summary>
        /// Проверка вхождения интервала в интервал
        /// </summary>
        /// <param name="I">Проверяемый интервал</param>
        /// <returns>Истина, если проверяемый интервал входит</returns>
        public bool IsInclude(TimeInterval I) => Check(I._MinInclude ? I._Min : I._Min + TimeSpan.FromTicks(1)) && Check(I._MaxInclude ? I._Max : I._Max - TimeSpan.FromTicks(1));

        public bool IsIntersect(TimeInterval I)
        {
            if (_Min == I._Min && _Max == I._Max) return true;

            var min_include = Check(I._Min);
            if (!I._MinInclude && I._Min == _Max) min_include = false;

            var max_include = Check(I._Max);
            if (!I._MaxInclude && I._Max == _Min) max_include = false;

            return min_include || max_include;
        }

        public override string ToString() => $"{(MinInclude ? "[" : "(")}{Min}; {Max}{(MaxInclude ? "]" : ")")}";

        /// <summary>Оператор неявного приведения типа к предикату</summary>
        /// <param name="I">Интервал</param>
        /// <returns>Предикат от вещественного типа двойной точности</returns>
        public static implicit operator Predicate<TimeSpan>(TimeInterval I) => I.Check;

        public static implicit operator TimeSpan(TimeInterval I) => I.Length;
        public static explicit operator TimeInterval(TimeSpan V) => new TimeInterval(TimeSpan.Zero, true, V, true);

        /// <summary>Оператор проверки на вхождение величины в интервал</summary>
        /// <param name="x">Проверяемая величина</param>
        /// <param name="I">Интервал</param>
        /// <returns>Истина, если величина внутри интервала</returns>
        public static bool operator ^(TimeSpan x, TimeInterval I) => I.Check(x);

        /// <summary>Оператор проверки на вхождение величины в интервал</summary>
        /// <param name="X">Проверяемая величина</param>
        /// <param name="I">Интервал</param>
        /// <returns>Истина, если величина внутри интервала</returns>
        public static bool operator ^(TimeInterval I, TimeSpan X) => X ^ I;
    }

    /// <summary>Интервальный предикат</summary>
    [Serializable]
    //[System.ComponentModel.TypeConverter(typeof(IntervalConverter))]
    public class DateTimeInterval
    {
        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Включена ли нижняя граница интервала?</summary>
        private bool _MinInclude = true;
        /// <summary>Включена ли верхняя граница интервала?</summary>
        private bool _MaxInclude = true;
        /// <summary>Нижняя граница интервала</summary>
        private DateTime _Min;
        /// <summary>Верхняя граница интервала</summary>
        private DateTime _Max;

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Включена ли нижняя граница интервала?</summary>
        public bool MinInclude { get => _MinInclude; set => _MinInclude = value; }

        /// <summary>Включена ли верхняя граница интервала?</summary>
        public bool MaxInclude { get => _MaxInclude; set => _MaxInclude = value; }

        /// <summary>Нижняя граница интервала</summary>
        public DateTime Min { get => _Min; set => _Min = value; }

        /// <summary>Верхняя граница интервала</summary>
        public DateTime Max { get => _Max; set => _Max = value; }

        /// <summary>Протяжённость интервала</summary>
        public DateTime Length
        {
            get => new DateTime(_Max.Ticks - _Min.Ticks);
            set
            {
                var middle = Middle;
                var tics = value.Ticks / 2;
                _Min = new DateTime(middle.Ticks - tics);
                _Max = new DateTime(middle.Ticks + tics);
            }
        }

        public DateTime Middle
        {
            get => new DateTime((_Min.Ticks + _Max.Ticks) / 2);
            set
            {
                var half_length = Length.Ticks / 2;
                _Min = new DateTime(value.Ticks - half_length);
                _Max = new DateTime(value.Ticks + half_length);
            }
        }

        /// <summary>Интервал</summary>
        /// <param name="Max">Верхняя граница интервала</param>
        public DateTimeInterval(DateTime Max) => _Max = Max;

        /// <summary>Интервал</summary>
        /// <param name="Min">Нижняя граница интервала</param>
        /// <param name="Max">Верхняя граница интервала</param>
        public DateTimeInterval(DateTime Min, DateTime Max) : this(Max) => _Min = Min;

        /// <summary>Интервал</summary>
        /// <param name="Min">Нижняя граница интервала</param>
        /// <param name="Max">Верхняя граница интервала</param>
        /// <param name="IncludeLimits">Включать пределы?</param>
        public DateTimeInterval(DateTime Min, DateTime Max, bool IncludeLimits) : this(Min, IncludeLimits, Max, IncludeLimits) { }

        /// <summary>Интервал</summary>
        /// <param name="Min">Нижняя граница интервала</param>
        /// <param name="MinInclude">Включена ли нижняя граница интервала?</param>
        /// <param name="Max">Верхняя граница интервала</param>
        public DateTimeInterval(DateTime Min, bool MinInclude, DateTime Max) : this(Min, Max) => _MinInclude = MinInclude;

        /// <summary>Интервал</summary>
        /// <param name="Min">Нижняя граница интервала</param>
        /// <param name="MinInclude">Включена ли нижняя граница интервала?</param>
        /// <param name="Max">Верхняя граница интервала</param>
        /// <param name="MaxInclude">Включена ли верхняя граница интервала</param>
        public DateTimeInterval(DateTime Min, bool MinInclude, DateTime Max, bool MaxInclude) : this(Min, MinInclude, Max) => _MaxInclude = MaxInclude;

        /// <summary>Проверка на вхождение в интервал</summary>
        /// <param name="X">Проверяемая величина</param>
        /// <returns></returns>
        public bool Check(DateTime X) => _MinInclude && X == _Min || _MaxInclude && X == _Max || X > _Min && X < _Max;

        public bool Check(DateTime X, TimeSpan MinOffset, TimeSpan MaxOffset)
        {
            var min = _Min + MinOffset;
            var max = _Max + MaxOffset;

            return _MinInclude && X == min || _MaxInclude && X == max || X > min && X < max;
        }

        public bool Check(DateTime X, TimeSpan Offset) => Check(X, Offset, -Offset);

        public DateTime Normalize(DateTime X) => X > _Max ? _Max : X < _Min ? _Min : X;

        public bool IsExclude(DateTimeInterval I) => !IsInclude(I);

        public bool IsInclude(DateTimeInterval I) => Check(I._MinInclude ? I._Min : I._Min + TimeSpan.FromTicks(1)) && Check(I._MaxInclude ? I._Max : I._Max - TimeSpan.FromTicks(1));

        public bool IsIntersect(DateTimeInterval I)
        {
            if (I.Min == Min && I.Max == Max) return true;

            var min_include = Check(I.Min);
            if (!I.MinInclude && I.Min == Max) min_include = false;

            var max_include = Check(I.Max);
            if (!I.MaxInclude && I.Max == Min) max_include = false;

            return min_include || max_include;
        }

        public override string ToString() => $"{(MinInclude ? "[" : "(")}{Min}; {Max}{(MaxInclude ? "]" : ")")}";

        /// <summary>Оператор неявного приведения типа к предикату</summary>
        /// <param name="I">Интервал</param>
        /// <returns>Предикат от вещественного типа двойной точности</returns>
        public static implicit operator Predicate<DateTime>(DateTimeInterval I) => I.Check;

        public static implicit operator DateTime(DateTimeInterval I) => I.Length;
        public static explicit operator DateTimeInterval(DateTime V) => new DateTimeInterval(DateTime.MinValue, true, V, true);

        /// <summary>Оператор проверки на вхождение величины в интервал</summary>
        /// <param name="x">Проверяемая величина</param>
        /// <param name="I">Интервал</param>
        /// <returns>Истина, если величина внутри интервала</returns>
        public static bool operator ^(DateTime x, DateTimeInterval I) => I.Check(x);

        /// <summary>Оператор проверки на вхождение величины в интервал</summary>
        /// <param name="X">Проверяемая величина</param>
        /// <param name="I">Интервал</param>
        /// <returns>Истина, если величина внутри интервала</returns>
        public static bool operator ^(DateTimeInterval I, DateTime X) => X ^ I;
    }

}