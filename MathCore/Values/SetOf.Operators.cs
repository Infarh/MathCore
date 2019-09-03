using System.Collections.Generic;
using System.Linq;
using MathCore.Annotations;

namespace MathCore.Values
{
    /// <summary>Множество</summary>
    /// <typeparam name="T">Тип элементов множества</typeparam>
    public partial class SetOf<T>
    {
        [NotNull]
        public static explicit operator SetOf<T>(T[] array) => new SetOf<T>(array);
        [NotNull]
        public static explicit operator SetOf<T>(List<T> list) => new SetOf<T>(list);

        [NotNull]
        public static SetOf<T> operator +([NotNull] SetOf<T> set, [NotNull] IEnumerable<T> enumerable) => new SetOf<T>(set.Concat(enumerable));

        [NotNull]
        public static SetOf<T> operator +([NotNull] IEnumerable<T> enumerable, [NotNull] SetOf<T> set) => new SetOf<T>(set.ConcatInverted(enumerable));

        [NotNull]
        public static SetOf<T> operator -([NotNull] SetOf<T> set, IEnumerable<T> enumerable)
        {
            var result = set.Clone();
            if(result.Power == 0) return result;
            enumerable.Foreach(result, (t, r) => { while(r.Remove(t)) { } });
            return result;
        }

        [NotNull]
        public static IEnumerable<T> operator -([NotNull] IEnumerable<T> collection, [NotNull] SetOf<T> set) => collection.Where(set.NotContains);

        [NotNull]
        public static SetOf<T> operator &([NotNull] SetOf<T> A, [NotNull] SetOf<T> B) => new SetOf<T>(A.Where(B.Contains));
    }
}