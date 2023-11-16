#nullable enable
namespace MathCore.Values;

public class LambdaSetOf<T>(IEnumerable<T> enumerable) : AbstractSetOf<T>
{
    public override int Power => enumerable.Count();

    public override bool Add(T Value)
    {
        var collection = enumerable as ICollection<T> ?? throw new NotSupportedException("Добавление элементов не поддерживается");
        if (collection.Contains(Value)) return false;
        collection.Add(Value);
        return true;
    }

    public override IEnumerator<T> GetEnumerator() => enumerable.GetEnumerator();
}