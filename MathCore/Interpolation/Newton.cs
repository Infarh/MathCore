#nullable enable

using MathCore.Vectors;

namespace MathCore.Interpolation;

/// <summary>Полином Ньютона</summary>
public class Newton : Interpolator, IInterpolator
{
    /// <summary>Рассчитать коэффициенты полинома Ньютона для заданного набора точек</summary>
    /// <param name="X">Массив значений на оси X</param>
    /// <param name="Y">Массив значений на оси Y</param>
    /// <returns>Массив коэффициентов полинома</returns>
    /// <exception cref="ArgumentNullException">Если отсутствует ссылка на массив <paramref name="X"/>, или <paramref name="Y"/></exception>
    /// <exception cref="InvalidOperationException">Если длина <paramref name="X"/> не равна <paramref name="Y"/></exception>
    public static double[] GetPolynomCoefficients(double[] X, double[] Y) => GetPolynomCoefficients(X, Y, P: new double[Y.Length]);

    /// <summary>Рассчитать коэффициенты полинома Ньютона для заданного набора точек</summary>
    /// <param name="x0">Начальное смещение аргумента</param>
    /// <param name="dx">Шаг сетки аргумента</param>
    /// <param name="Y">Массив значений на оси Y</param>
    /// <returns>Массив коэффициентов полинома</returns>
    /// <exception cref="ArgumentNullException">Если отсутствует ссылка на массив <paramref name="Y"/></exception>
    public static double[] GetPolynomCoefficients(double x0, double dx, double[] Y) => GetPolynomCoefficients(x0, dx, Y, P: new double[Y.Length]);

    /// <summary>Рассчитать коэффициенты полинома Ньютона для заданного набора точек</summary>
    /// <param name="X">Массив значений на оси X</param>
    /// <param name="Y">Массив значений на оси Y</param>
    /// <param name="P">Массив, в который будут вычислены коэффициенты полинома</param>
    /// <returns>Массив коэффициентов полинома</returns>
    /// <exception cref="ArgumentNullException">Если отсутствует ссылка на массив <paramref name="X"/>, или <paramref name="Y"/></exception>
    /// <exception cref="InvalidOperationException">Если длина <paramref name="X"/> не равна <paramref name="Y"/></exception>
    /// <exception cref="ArgumentException">Если размер массива <paramref name="P"/> меньше размера массива <paramref name="X"/></exception>
    public static double[] GetPolynomCoefficients(double[] X, double[] Y, double[] P)
    {
        if (X is null) throw new ArgumentNullException(nameof(X));
        if (Y is null) throw new ArgumentNullException(nameof(Y));
        if (P is null) throw new ArgumentNullException(nameof(P));

        const string x_length = nameof(X) + "." + nameof(Array.Length);
        const string y_length = nameof(X) + "." + nameof(Array.Length);
        if (X.Length != Y.Length)
            throw new InvalidOperationException("Размеры массивов не совпадают")
            {
                Data =
                {
                    { x_length, X.Length },
                    { y_length, Y.Length },
                }
            };
        
        var length = X.Length;

        const string p_length = nameof(P) + "." + nameof(Array.Length);
        if (P.Length < length)
            throw new ArgumentException("Размер массива результата недостаточен для записи в него коэффициентов полинома", nameof(P))
            {
                Data =
                {
                    { x_length, X.Length },
                    { y_length, Y.Length },
                    { p_length , P.Length },
                }
            };

        if(P.Length > length)
            Array.Clear(P, length, P.Length - length);

        var len = length - 1;

        // Готовимся к расчёту дельт - заполняем массив дельт исходными значениями функции
        // Массив дельт будет в обратном порядке: последняя дельта идёт первой в массиве
        // Это требуется для того, что бы этот же массив использовать для расчёта результата
        // где для расчёта первого коэффициента полинома нужна последняя дельта и коэффициент занимает её место
        //var delta = new double[length];
        for (var i = 0; i < length; i++)    // O(N)
            P[len - i] = Y[i];

        // Итерационно рассчитываем массив дельт
        for (var i = 1; i < length; i++)    // O(N^2/2)
            for (var j = len; j >= i; j--)
                P[len - j] = (P[len - j] - P[length - j]) / (X[j] - X[j - i]);

        // По схеме Горнера рассчитываем коэффициенты результирующего полинома
        for (var i = length - 2; i >= 0; i--)   // O(N^2)
        {
            // Запоминаем текущее значение дельты в самом начале затем,
            // что коэффициент полинома займёт её место в массиве
            var d = P[len - i];
            var x0 = -X[i]; // Также захватываем значение текущего корня полинома-множителя

            // p(x) <- p(x)*(x-x0[i]) + d[i] - итерационная формула вычисления следующего значения коэффициентов полинома

            var j = len - i;
            P[j] = P[--j];

            while (j > 0)
                P[j] = P[j] * x0 + P[--j];

            P[0] = P[0] * x0 + d;
        }

        // O(N) + O(N^2/2) + O(N^2)
        // O(N + N^2/2 + N^2)
        // O(3/2*N^2 + N)

        return P;
    }

    /// <summary>Рассчитать коэффициенты полинома Ньютона для заданного набора точек</summary>
    /// <param name="x0">Начальное смещение аргумента</param>
    /// <param name="dx">Шаг сетки аргумента</param>
    /// <param name="Y">Массив значений на оси Y</param>
    /// <param name="P">Массив, в который будут вычислены коэффициенты полинома</param>
    /// <returns>Массив коэффициентов полинома</returns>
    /// <exception cref="ArgumentNullException">Если отсутствует ссылка на массив <paramref name="Y"/></exception>
    /// <exception cref="ArgumentNullException">Если отсутствует ссылка на массив <paramref name="P"/></exception>
    public static double[] GetPolynomCoefficients(double x0, double dx, double[] Y, double[] P)
    {
        if (Y is null) throw new ArgumentNullException(nameof(Y));
        if (P is null) throw new ArgumentNullException(nameof(P));

        var length = Y.Length;

        const string p_length = nameof(P) + "." + nameof(Array.Length);
        if (P.Length < length)
            throw new ArgumentException("Размер массива результата недостаточен для записи в него коэффициентов полинома", nameof(P))
            {
                Data =
                {
                    { nameof(x0), x0 },
                    { nameof(dx), dx },
                    { p_length , P.Length },
                }
            };

        if (P.Length > length)
            Array.Clear(P, length, P.Length - length);

        var len = length - 1;

        // Готовимся к расчёту дельт - заполняем массив дельт исходными значениями функции
        // Массив дельт будет в обратном порядке: последняя дельта идёт первой в массиве
        // Это требуется для того, что бы этот же массив использовать для расчёта результата
        // где для расчёта первого коэффициента полинома нужна последняя дельта и коэффициент занимает её место
        //var delta = new double[length];
        for (var i = 0; i < length; i++)    // O(N)
            P[len - i] = Y[i];

        // Итерационно рассчитываем массив дельт
        for (var i = 1; i < length; i++)    // O(N^2/2)
            for (var j = len; j >= i; j--)
                P[len - j] = (P[len - j] - P[length - j]) / dx;

        // По схеме Горнера рассчитываем коэффициенты результирующего полинома
        for (var i = length - 2; i >= 0; i--)   // O(N^2)
        {
            // Запоминаем текущее значение дельты в самом начале затем,
            // что коэффициент полинома займёт её место в массиве
            var d = P[len - i];
            //var x0 = -X[i]; // Также захватываем значение текущего корня полинома-множителя

            // p(x) <- p(x)*(x-x0[i]) + d[i] - итерационная формула вычисления следующего значения коэффициентов полинома

            var j = len - i;
            P[j] = P[--j];

            while (j > 0)
                P[j] = P[j] * x0 + P[--j];

            P[0] = P[0] * x0 + d;
        }

        // O(N) + O(N^2/2) + O(N^2)
        // O(N + N^2/2 + N^2)
        // O(3/2*N^2 + N)

        return P;
    }

    /// <summary>Сформировать полином Ньютона по массиву точек</summary>
    /// <param name="X">Массив значений на оси X</param>
    /// <param name="Y">Массив значений на оси Y</param>
    /// <returns>Полином Ньютона</returns>
    /// <exception cref="ArgumentNullException">Если отсутствует ссылка на массив <paramref name="X"/>, или <paramref name="Y"/></exception>
    /// <exception cref="InvalidOperationException">Если длина <paramref name="X"/> не равна <paramref name="Y"/></exception>
    public static Polynom GetPolynom(double[] X, double[] Y) => new (GetPolynomCoefficients(X, Y));

    public static Polynom GetPolynom(double x0, double dx, double[] Y) => new (GetPolynomCoefficients(x0, dx, Y));

    private readonly Polynom _Polynom;

    public double this[double x] => Value(x);

    public Newton(double x0, double dx, double[] Y) : this(GetPolynomCoefficients(x0, dx, Y)) { }
    public Newton(double[] X, double[] Y) : this(GetPolynomCoefficients(X, Y)) { }

    private Newton(double[] PolynomCoefficients) => _Polynom = new(PolynomCoefficients);

    public Newton(params IReadOnlyList<Vector2D> P)
    {
        var length = P.Count;
        var x = new double[length];
        var y = new double[length];

        for (var i = 0; i < length; i++)
            P[i].Deconstruct(out x[i], out y[i]);

        _Polynom = GetPolynom(x, y);
    }

    public double Value(double x) => _Polynom.Value(x);

    public Func<double, double> GetFunction() => _Polynom.Value;
}