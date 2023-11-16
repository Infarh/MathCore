#nullable enable
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore;

/// <summary>Генератор случайных элементов из списка</summary>
/// <typeparam name="T">Тип элементов</typeparam>
public class Randomizer<T> : IFactory<T>
{
    private readonly IList<T> _Items;
    private readonly Random _Random;

    public Randomizer(IList<T> Items, Random? Random = null)
    {
        _Items = Items.NotNull();
        if (Items.Count == 0) throw new ArgumentException("Размер массива должен быть больше 0", nameof(Items));
        _Random = Random ?? new Random();
    }

    public Randomizer(Random Random, params T[] Items) : this(Items, Random) { }

    public T Next() => _Items[_Random.Next(_Items.Count)];

    T IFactory<T>.Create() => Next();
        
    public static implicit operator T(Randomizer<T> Randomizer) => Randomizer.Next();
}