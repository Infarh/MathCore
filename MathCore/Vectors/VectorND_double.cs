namespace MathCore.Vectors;

/// <summary>Вектор с размерностью и элементами типа double</summary>
public class VectorND_double : Vector<double>
{
    /// <summary>Длина вектора</summary>
    public double Length => Math.Sqrt(this.Sum());

    /// <summary>Конструктор</summary>
    /// <param name="Dimension">Размерность</param>
    public VectorND_double(int Dimension) : base(Dimension) { }

    /// <summary>Конструктор</summary>
    /// <param name="Elements">Массив элементов</param>
    public VectorND_double(double[] Elements) : base(Elements) { }

    /// <summary>Конструктор</summary>
    /// <param name="Elements">Коллекция элементов</param>
    public VectorND_double(IEnumerable<double> Elements) : base(Elements.ToArray()) { }

    /// <summary>Попозиционное произведение векторов</summary>
    /// <param name="b">Второй вектор</param>
    /// <returns>Вектор попозиционного произведения</returns>
    public VectorND_double GetProduction(VectorND_double b)
    {
        if (Dimension != b.Dimension) throw new ArgumentException("Размерности векторов не совпадают");
        return new(new double[Dimension].Initialize(this, b, (i, v, bb) => v![i] * bb![i]));
    }

    /// <summary>Скалярное произведение векторов</summary>
    /// <param name="b">Второй вектор</param>
    /// <returns>Скалярное произведение</returns>
    public double GetScalarProduction(VectorND_double b)
    {
        if (Dimension != b.Dimension) throw new ArgumentException("Размерности векторов не совпадают");
        return GetProduction(b).Sum();
    }

    /// <summary>Векторное произведение множества векторов</summary>
    /// <param name="Vectors">Массив векторов</param>
    /// <returns>Векторное произведение</returns>
    public static VectorND_double GetVectorProduction(VectorND_double[] Vectors)
    {
        if (Vectors is null || Vectors.Length == 0)
            throw new ArgumentOutOfRangeException(nameof(Vectors),
                "Векторное произведение не может быть рассчитано для пустого множества векторов");

        var N = Vectors[0].Dimension;
        for (var i = 1; i < Vectors.Length; i++)
            if (Vectors[i].Dimension != N)
                throw new ArgumentOutOfRangeException(nameof(Vectors),
                    $"Размерность вектора {i}:{Vectors[i].Dimension} не соответствует размерности первого вектора {N}");

        if (N - 1 != Vectors.Length)
            throw new ArgumentOutOfRangeException(nameof(Vectors),
                "Количество векторов для векторного произведения не соответствует размерности векторного пространства");


        var M = new Matrix(N, (i, j) => i == 0 ? 0 : Vectors[i - 1][j]);
        var k = -1;
        // ReSharper disable once HeapView.CanAvoidClosure
        return new(new double[N].Initialize(i => M.GetMinor(i, 0).GetDeterminant() * (k *= -1)));
    }

    /// <summary>Обратный вектор (по элементным инверсиям)</summary>
    /// <returns>Обратный вектор</returns>
    public VectorND_double GetInversed() => new(new double[Dimension].Initialize(this, (i, v) => 1 / v![i]));

    /// <summary>Сложение двух векторов</summary>
    /// <param name="a">Первый вектор</param>
    /// <param name="b">Второй вектор</param>
    /// <returns>Сумма векторов</returns>
    public static VectorND_double operator +(VectorND_double a, VectorND_double b)
    {
        if (a.Dimension != b.Dimension) throw new ArgumentException("Размерности векторов не совпадают");
        return new(new double[a.Dimension].Initialize(a, b, (i, aa, bb) => aa![i] + bb![i]));
    }

    /// <summary>Сложение вектора и скаляра</summary>
    /// <param name="a">Вектор</param>
    /// <param name="b">Скаляр</param>
    /// <returns>Вектор, каждая компонента которого увеличена на скаляр</returns>
    public static VectorND_double operator +(VectorND_double a, double b) => new(new double[a.Dimension].Initialize(a, b, (i, aa, bb) => aa![i] + bb));

    /// <summary>Сложение скаляра и вектора</summary>
    /// <param name="a">Скаляр</param>
    /// <param name="b">Вектор</param>
    /// <returns>Вектор, каждая компонента которого увеличена на скаляр</returns>
    public static VectorND_double operator +(double a, VectorND_double b) => new(new double[b.Dimension].Initialize(a, b, (i, aa, bb) => aa + bb![i]));

    /// <summary>Вычитание двух векторов</summary>
    /// <param name="a">Первый вектор</param>
    /// <param name="b">Второй вектор</param>
    /// <returns>Разность векторов</returns>
    public static VectorND_double operator -(VectorND_double a, VectorND_double b)
    {
        if (a.Dimension != b.Dimension) throw new ArgumentException("Размерности векторов не совпадают");
        return new(new double[a.Dimension].Initialize(a, b, (i, aa, bb) => aa![i] - bb![i]));
    }

    /// <summary>Вычитание вектора и скаляра</summary>
    /// <param name="a">Вектор</param>
    /// <param name="b">Скаляр</param>
    /// <returns>Вектор, каждая компонента которого уменьшена на скаляр</returns>
    public static VectorND_double operator -(VectorND_double a, double b) => new(new double[a.Dimension].Initialize(a, b, (i, aa, bb) => aa![i] - bb));

    /// <summary>Вычитание вектора из скаляра</summary>
    /// <param name="a">Скаляр</param>
    /// <param name="b">Вектор</param>
    /// <returns>Вектор, каждая компонента которого вычтена из скаляра</returns>
    public static VectorND_double operator -(double a, VectorND_double b) => new(new double[b.Dimension].Initialize(a, b, (i, aa, bb) => aa - bb![i]));

    /// <summary>Скалярное произведение векторов</summary>
    /// <param name="a">Первый вектор</param>
    /// <param name="b">Второй вектор</param>
    /// <returns>Скалярное произведение</returns>
    public static double operator *(VectorND_double a, VectorND_double b) => a.GetScalarProduction(b);

    /// <summary>Умножение вектора на скаляр</summary>
    /// <param name="a">Вектор</param>
    /// <param name="b">Скаляр</param>
    /// <returns>Вектор, каждая компонента которого умножена на скаляр</returns>
    public static VectorND_double operator *(VectorND_double a, double b) => new(new double[a.Dimension].Initialize(a, b, (i, aa, bb) => aa![i] * bb));

    /// <summary>Умножение вектора на скаляр (коммутативное)</summary>
    /// <param name="a">Скаляр</param>
    /// <param name="b">Вектор</param>
    /// <returns>Вектор, каждая компонента которого умножена на скаляр</returns>
    public static VectorND_double operator *(double a, VectorND_double b) => new(new double[b.Dimension].Initialize(a, b, (i, aa, bb) => aa * bb![i]));

    /// <summary>Деление скаляра на скалярное произведение векторов</summary>
    /// <param name="a">Первый вектор</param>
    /// <param name="b">Второй вектор</param>
    /// <returns>Результат деления</returns>
    public static double operator /(VectorND_double a, VectorND_double b) => a * b.GetInversed();

    /// <summary>Деление вектора на скаляр</summary>
    /// <param name="a">Вектор</param>
    /// <param name="b">Скаляр</param>
    /// <returns>Вектор, каждая компонента которого поделена на скаляр</returns>
    public static VectorND_double operator /(VectorND_double a, double b) => new(new double[a.Dimension].Initialize(a, b, (i, aa, bb) => aa![i] / bb));

    /// <summary>Деление скаляра на вектор</summary>
    /// <param name="a">Скаляр</param>
    /// <param name="b">Вектор</param>
    /// <returns>Вектор, каждая компонента которого равна скаляр, делённый на соответствующую компоненту вектора</returns>
    public static VectorND_double operator /(double a, VectorND_double b) => new(new double[b.Dimension].Initialize(a, b, (i, aa, bb) => aa / bb![i]));
}