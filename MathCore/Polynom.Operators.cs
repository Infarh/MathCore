#nullable enable
namespace MathCore;

public partial class Polynom
{
    /// <summary>Явное приведение типов полинома к делегату функции преобразования double->double</summary>
    /// <param name="P">Полином</param>
    /// <returns>Делегат функции преобразования</returns>
    [DST]
    public static implicit operator Func<double, double>(Polynom P) => P.GetFunction();

    /// <summary>Явное приведение типов полинома к делегату комплексной функции преобразования Complex->Complex</summary>
    /// <param name="P">Полином</param>
    /// <returns>Делегат комплексной функции преобразования</returns>
    [DST]
    public static implicit operator Func<Complex, Complex>(Polynom P) => P.GetComplexFunction();

    /// <summary>Оператор сложения двух полиномов</summary>
    /// <param name="P">Первое слагаемое</param>
    /// <param name="Q">Второе слагаемое</param>
    /// <returns>Сумма полиномов</returns>
    [DST]
    public static Polynom operator +(Polynom P, Polynom Q) => new(Array.Sum(P._a, Q._a));

    /// <summary>
    /// Оператор отрицания полинома (изменяет знак всех коэффициентов на обратной). Эквивалентно умножению полинома на -1
    /// </summary>
    /// <param name="P">Отрицаемый полином</param>
    /// <returns>Полином Q = -P</returns>
    public static Polynom operator -(Polynom P) => new(Array.Negate(P._a));

    /// <summary>Оператор вычитания полинома Q из полинома P</summary>
    /// <param name="P">Уменьшаемое</param>
    /// <param name="Q">Вычитаемое</param>
    /// <returns>Разность</returns>
    public static Polynom operator -(Polynom P, Polynom Q) => new(Array.Subtract(P._a, Q._a));

    /// <summary>Оператор произведения полинома Q и полинома P</summary>
    /// <param name="P">Первый сомножитель</param>
    /// <param name="Q">Второй сомножитель</param>
    /// <returns>Произведение полиномов</returns>
    public static Polynom operator *(Polynom P, Polynom Q) => new(Array.Multiply(P._a, Q._a));
    
    /// <summary>Оператор деления двух полиномов</summary>
    /// <param name="p">Полином делимого</param>
    /// <param name="q">Полином делителя</param>
    /// <returns>Результат деления полиномов, включающий частное и остаток от деления</returns>
    public static PolynomDivisionResult operator /(Polynom p, Polynom q) =>
        new(p.DivideTo(q, out var remainder), remainder, q);

    /// <summary>Умножение полинома на вещественное число</summary>
    /// <param name="P">Полином</param>
    /// <param name="q">Вещественное число</param>
    /// <returns>Полином - результат умножения исходного полинома на вещественное число</returns>
    public static Polynom operator *(Polynom P, double q) => new(Array.Multiply(P._a, q));

    /// <summary>Умножение полинома на вещественное число</summary>
    /// <param name="p">Вещественное число</param>
    /// <param name="Q">Полином</param>
    /// <returns>Полином - результат умножения исходного полинома на вещественное число</returns>
    public static Polynom operator *(double p, Polynom Q) => new(Array.Multiply(Q._a, p));

    /// <summary>Деление полинома на вещественное число</summary>
    /// <param name="P">Полином</param>
    /// <param name="q">Вещественное число</param>
    /// <returns>Полином - результат деления исходного полинома на вещественное число</returns>
    public static Polynom operator /(Polynom P, double q) => new(Array.Divide(P._a, q));

    /// <summary>Оператор дифференцирования</summary>
    /// <param name="p">Дифференцируемый полином</param>
    /// <param name="Order">Порядок дифференцирования</param>
    /// <returns>Полином - результат дифференцирования</returns>
    public static Polynom operator >>(Polynom p, int Order) => p.GetDifferential(Order);

    /// <summary>Оператор интегрирования</summary>
    /// <param name="p">Дифференцируемый полином</param>
    /// <param name="Order">Кратность интегрирования</param>
    /// <returns>Полином - результат интегрирования</returns>
    public static Polynom operator <<(Polynom p, int Order) => p.GetIntegral(Order: Order);

    /// <summary>Оператор неявного приведения типа полинома в массив вещественных значений коэффициентов</summary>
    /// <param name="p">Полином</param>
    /// <returns>Массив значений коэффициентов</returns>
    [DST]
    public static explicit operator double[](Polynom p) => p.Coefficients;

    /// <summary>Оператор приведения типа вещественного числа к типу полинома</summary>
    /// <param name="a">Вещественное число</param>
    [DST]
    public static implicit operator Polynom(double a) => new(a);

    /// <summary>Оператор приведения типа вещественного числа к типу полинома</summary>
    /// <param name="a">Вещественное число</param>
    [DST]
    public static implicit operator Polynom(float a) => new(a);

    /// <summary>Оператор приведения типа целого числа к типу полинома</summary>
    /// <param name="a">Целое число</param>
    [DST]
    public static implicit operator Polynom(int a) => new(a);

    /// <summary>Оператор приведения типа целого числа к типу полинома</summary>
    /// <param name="a">Целое число</param>
    [DST]
    public static implicit operator Polynom(short a) => new(a);
}