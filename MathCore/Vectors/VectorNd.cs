#nullable enable
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.Vectors;

public class VectorND<T>
{
    private readonly VectorND<T>[]? _Dimensions;

    public ref readonly VectorND<T> this[int i] => ref _Dimensions[i];

    public T[] Values { get; }

    public bool IsFinite => _Dimensions is null;

    public VectorND(int[] Dimensions) : this(Dimensions, 0) { }

    private VectorND(int[] Dimensions, int i)
    {
        if(Dimensions is null) throw new ArgumentNullException(nameof(Dimensions));
        if(i + 1 > Dimensions.Length) 
            throw new ArgumentOutOfRangeException(nameof(i), "Запрашиваемый индекс оси превышает указанную размерность");

        if(i + 1 == Dimensions.Length)
        {
            _Dimensions = null;
            Values      = new T[Dimensions[i]];
            return;
        }

        var length = Dimensions[i];
        if(length < 1)
            throw new ArgumentOutOfRangeException(nameof(Dimensions), $"Размер по оси {i} не может быть меньше 1"); 

        _Dimensions = new VectorND<T>[length];
        for(var j = 0; j < length; j++)
            _Dimensions[j] = new VectorND<T>(Dimensions, i);
    }
}