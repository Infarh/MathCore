
// ReSharper disable MemberCanBeProtected.Global

namespace MathCore.Functions.Differentiable;

[Copyright("http://habrahabr.ru/post/149630/")]
/// <summary>Дифференцируемая функция</summary>
public abstract class Function
{
    /// <summary>Значение функции в точке</summary>
    /// <param name="x">Аргумент</param>
    /// <returns>Значение функции</returns>
    public abstract double Value(double x);
    /// <summary>Производная функции</summary>
    /// <returns>Производная функция</returns>
    public abstract Function Derivative();
    /// <summary>Оператор сложения функций</summary>
    public static Function operator +(Function f1, Function f2) => new Addition(f1, f2);
    /// <summary>Оператор вычитания функций</summary>
    public static Function operator -(Function f1, Function f2) => new Subtraction(f1, f2);
    /// <summary>Оператор умножения функций</summary>
    public static Function operator *(Function f1, Function f2) => new Multiplication(f1, f2);
    /// <summary>Оператор деления функций</summary>
    public static Function operator /(Function f1, Function f2) => new Division(f1, f2);
    /// <summary>Оператор возведения функции в степень константы</summary>
    public static Function operator ^(Function f1, Constant c) => new OperatorPowerOf(f1, c);
}

/// <summary>Константа</summary>
/// <param name="c">Значение константы</param>
public class Constant(double c) : Function
{
    private readonly double _C = c;
    // ReSharper disable once UnusedMember.Global
    /// <summary>Значение константы</summary>
    public double C => _C;
    /// <inheritdoc />
    public override double Value(double x) => _C;
    /// <inheritdoc />
    public override Function Derivative() => new Zero();
    /// <summary>Неявное приведение числа к константе</summary>
    public static implicit operator Constant(double c) => new(c);
    /// <summary>Неявное приведение константы к числу</summary>
    public static implicit operator double(Constant c) => c._C;
}

/// <summary>Нулевая константа</summary>
public class Zero() : Constant(0);
/// <summary>Единичная константа</summary>
public class One() : Constant(1);

/// <summary>Тождественная функция (возвращает аргумент)</summary>
public class Identity : Function
{
    /// <inheritdoc />
    public override double Value(double x) => x;
    /// <inheritdoc />
    public override Function Derivative() => new One();
}