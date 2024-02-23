#nullable enable
using MathCore.Vectors;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace MathCore.Interpolation;

// ReSharper disable once CommentTypo
/// <summary>Интерполирование функций естественными кубическими сплайнами</summary>
/// <remarks>Разработчик: Назар Андриенко Email: nuzikprogrammer@gmail.com</remarks>
public class CubicSpline : Interpolator, IInterpolator
{
    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Структура, описывающая сплайн на каждом сегменте сетки</summary>
    private struct SplineState(double a, double b, double c, double d, double x)
    {
        public readonly double a = a;
        public double b = b;
        public double c = c;
        public double d = d;
        public readonly double x = x;

        public SplineState(double a, double x) : this(a, 0, 0, 0, x)
        {
        }
    }

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Сплайн</summary>
    private SplineState[] _SplineStates;

    /* -------------------------------------------------------------------------------------------- */

    public double this[double x] => Value(x);

    /* -------------------------------------------------------------------------------------------- */

    public CubicSpline(double[] X, double[] Y) => Initialize(X, Y);

    public CubicSpline(IList<Complex> Points)
    {
        var count = Points.Count;
        var x     = new double[count];
        var y     = new double[count];
        for(var i = 0; i < count; i++)
        {
            x[i] = Points[i].Re;
            y[i] = Points[i].Im;
        }
        Initialize(x, y);
    }

    public CubicSpline(IList<Vector2D> Points)
    {
        var count = Points.Count;
        var x     = new double[count];
        var y     = new double[count];
        for(var i = 0; i < count; i++)
        {
            x[i] = Points[i].X;
            y[i] = Points[i].Y;
        }
        Initialize(x, y);
    }

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Инициализация сплайна</summary>
    /// <param name="X">Массив аргументов</param><param name="Y">Массив значений</param>
    /// <exception cref="ArgumentException">Возникает в случае, если размерности массивов не равны</exception>
    public void Initialize(double[] X, double[] Y)
    {
        if(X.Length != Y.Length) throw new ArgumentException("Размеры массивов должны совпадать");

        var count = X.Length;
        _SplineStates = new SplineState[count];

        for(var i = 0; i < count; i++)
            _SplineStates[i] = new(Y[i], X[i]);

        _SplineStates[0].c = _SplineStates[count - 1].c = 0;

        // Решение СЛАУ относительно коэффициентов сплайнов c[i] методом прогонки для трехдиагональных матриц
        // Вычисление прогоночных коэффициентов - прямой ход метода прогонки
        var alpha          = new double[count - 1];
        var beta           = new double[count - 1];
        alpha[0] = beta[0] = 0;
        for(var i = 1; i < count - 1; i++)
        {
            var h_i  = X[i] - X[i - 1];
            var h_i1 = X[i + 1] - X[i];
            var a    = h_i;
            var c    = 2 * (h_i + h_i1);
            var b    = h_i1;
            var f    = 6 * ((Y[i + 1] - Y[i]) / h_i1 - (Y[i] - Y[i - 1]) / h_i);
            var z    = a * alpha[i - 1] + c;
            alpha[i] = -b / z;
            beta[i]  = (f - a * beta[i - 1]) / z;
        }

        // Нахождение решения - обратный ход метода прогонки
        for(var i = count - 2; i > 0; i--)
            _SplineStates[i].c = alpha[i] * _SplineStates[i + 1].c + beta[i];

        // По известным коэффициентам c[i] находим значения b[i] и d[i]
        for(var i = count - 1; i > 0; i--)
        {
            var h_i = X[i] - X[i - 1];
            _SplineStates[i].d = (_SplineStates[i].c - _SplineStates[i - 1].c) / h_i;
            _SplineStates[i].b = h_i * (2 * _SplineStates[i].c + _SplineStates[i - 1].c) / 6 + (Y[i] - Y[i - 1]) / h_i;
        }
    }

    /* -------------------------------------------------------------------------------------------- */

    public double Value(double x)
    {
        var         count = _SplineStates.Length;
        SplineState state;
        if(x <= _SplineStates[0].x) // Если x меньше точки сетки x[0] - пользуемся первым эл-тов массива
            state = _SplineStates[0];
        else if(x >= _SplineStates[count - 1].x) // Если x больше точки сетки x[n - 1] - пользуемся последним эл-том массива
            state = _SplineStates[count - 1];
        else // Иначе x лежит между граничными точками сетки - производим бинарный поиск нужного эл-та массива
        {
            int i = 0, j = count - 1;
            while(i + 1 < j)
            {
                var k                         = i + (j - i) / 2;
                if(x <= _SplineStates[k].x) j = k; else i = k;
            }
            state = _SplineStates[j];
        }

        var dx = x - state.x;
        // Вычисляем значение сплайна в заданной точке по схеме Горнера 
        return state.a + (state.b + (state.c / 2 + state.d * dx / 6) * dx) * dx;
    }

    public Func<double, double> GetFunction() => Value;
}