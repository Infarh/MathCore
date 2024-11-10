#nullable enable
namespace MathCore.Values;

public class LambdaSetOf<T>(IEnumerable<T> enumerable) : AbstractSetOf<T>
{
    public override int Power => enumerable.Count();

    /// <summary>Добавить элемент в множество</summary>
    /// <param name="Value">Добавляемый элемент</param>
    /// <returns>True, если элемент успешно добавлен, false - если элемент уже есть в множестве</returns>
    /// <exception cref="NotSupportedException">Если добавление элементов не поддерживается</exception>
    public override bool Add(T Value)
    {
        var collection = enumerable as ICollection<T> ?? throw new NotSupportedException("Добавление элементов не поддерживается");
        if (collection.Contains(Value)) return false;
        collection.Add(Value);
        return true;
    }

    public override IEnumerator<T> GetEnumerator() => enumerable.GetEnumerator();
}