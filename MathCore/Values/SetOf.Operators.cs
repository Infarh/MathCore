using System.Collections.Generic;
using System.Linq;

namespace MathCore.Values
{
    public partial class SetOf<T>
    {
        public static explicit operator SetOf<T>(T[] array) => new SetOf<T>(array);
        public static explicit operator SetOf<T>(List<T> list) => new SetOf<T>(list);

        public static SetOf<T> operator +(SetOf<T> set, IEnumerable<T> enumerable) => new SetOf<T>(set.Concat(enumerable));

        public static SetOf<T> operator +(IEnumerable<T> enumerable, SetOf<T> set) => new SetOf<T>(set.ConcatInverted(enumerable));

        public static SetOf<T> operator -(SetOf<T> set, IEnumerable<T> enumerable)
        {
            var result = set.Clone();
            if(result.Power == 0) return result;
            enumerable.Foreach(t => { while(result.Remove(t)) { } });
            return result;
        }

        public static IEnumerable<T> operator -(IEnumerable<T> collection, SetOf<T> set) => collection.Where(set.NotContains);

        public static SetOf<T> operator &(SetOf<T> A, SetOf<T> B) => new SetOf<T>(A.Where(B.Contains));
    }
}