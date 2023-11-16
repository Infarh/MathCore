#nullable enable
namespace MathCore.Values;

/// <summary>Множество</summary>
/// <typeparam name="T">Тип элементов множества</typeparam>
public partial class SetOf<T>
{
    public static explicit operator SetOf<T>(T[] array) => new(array);
    public static explicit operator SetOf<T>(List<T> list) => new(list);

    public static SetOf<T> operator +(SetOf<T> set, IEnumerable<T> enumerable) => new(set.Concat(enumerable));

    public static SetOf<T> operator +(IEnumerable<T> enumerable, SetOf<T> set) => new(set.ConcatInverted(enumerable));

    public static SetOf<T> operator -(SetOf<T> set, IEnumerable<T> enumerable)
    {
        var result = set.Clone();
        if(result.Power == 0) return result;
        enumerable.Foreach(result, (t, r) => { while(r.Remove(t)) { } });
        return result;
    }

    public static IEnumerable<T> operator -(IEnumerable<T> collection, SetOf<T> set) => collection.Where(set.NotContains);

    public static SetOf<T> operator &(SetOf<T> A, SetOf<T> B) => new(A.Where(B.Contains));
}