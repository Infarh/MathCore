using System.Diagnostics;
using System.Linq.Expressions;

using MathCore.Annotations;

namespace MathCore.Expressions.Complex;

/// <summary>Абстрактное выражение комплексного числа, построенное на деревьях выражений</summary>
/// <remarks>Предоставляет операции над комплексными выражениями, формируя выражения из действительной и мнимой частей</remarks>
[PublicAPI]
public abstract class ComplexExpression
{
    /* -------------------------------------------------------------------------------------------------------------------- */
    #region Static service methods

    /// <summary>Формирует константное выражение из числа</summary>
    /// <param name="value">Значение константы</param>
    /// <returns>Константное выражение</returns>
    [NotNull]
    protected static ConstantExpression Constant([NotNull] object value) => Expression.Constant((double)value);
    /// <summary>Формирует выражение отрицания (смены знака)</summary>
    /// <param name="value">Операнд</param>
    /// <returns>Выражение отрицания</returns>
    [NotNull]
    protected static UnaryExpression Negate([NotNull] Expression value) => Expression.Negate(value);
    /// <summary>Формирует выражение суммы двух операндов</summary>
    /// <param name="Left">Левый операнд</param>
    /// <param name="Right">Правый операнд</param>
    /// <returns>Выражение суммы</returns>
    [NotNull]
    protected static BinaryExpression Add([NotNull] Expression Left, [NotNull] Expression Right) => Expression.Add(Left, Right);
    /// <summary>Формирует выражение разности двух операндов</summary>
    /// <param name="Left">Левый операнд</param>
    /// <param name="Right">Правый операнд</param>
    /// <returns>Выражение разности</returns>
    [NotNull]
    protected static BinaryExpression Subtract([NotNull] Expression Left, [NotNull] Expression Right) => Expression.Subtract(Left, Right);
    /// <summary>Формирует выражение произведения двух операндов</summary>
    /// <param name="Left">Левый операнд</param>
    /// <param name="Right">Правый операнд</param>
    /// <returns>Выражение произведения</returns>
    [NotNull]
    protected static BinaryExpression Multiply([NotNull] Expression Left, [NotNull] Expression Right) => Expression.Multiply(Left, Right);
    /// <summary>Формирует выражение частного двух операндов</summary>
    /// <param name="Left">Левый операнд (делимое)</param>
    /// <param name="Right">Правый операнд (делитель)</param>
    /// <returns>Выражение частного</returns>
    [NotNull]
    protected static BinaryExpression Divide([NotNull] Expression Left, [NotNull] Expression Right) => Expression.Divide(Left, Right);
    /// <summary>Формирует выражение частного выражения и числа</summary>
    /// <param name="Left">Левый операнд (делимое)</param>
    /// <param name="Right">Числовой делитель</param>
    /// <returns>Выражение частного</returns>
    [NotNull]
    protected static BinaryExpression Divide([NotNull] Expression Left, double Right) => Expression.Divide(Left, Constant(Right));
    /// <summary>Формирует выражение возведения в степень</summary>
    /// <param name="Left">Основание степени</param>
    /// <param name="Right">Показатель степени</param>
    /// <returns>Выражение степени</returns>
    [NotNull]
    protected static BinaryExpression GetPower([NotNull] Expression Left, [NotNull] Expression Right) => Expression.Power(Left, Right);
    /// <summary>Формирует выражение возведения числа в степень выражения</summary>
    /// <param name="Left">Числовое основание степени</param>
    /// <param name="Right">Показатель степени</param>
    /// <returns>Выражение степени</returns>
    [NotNull]
    protected static BinaryExpression GetPower(double Left, [NotNull] Expression Right) => Expression.Power(Constant(Left), Right);
    /// <summary>Формирует выражение возведения выражения в числовую степень</summary>
    /// <param name="Left">Основание степени</param>
    /// <param name="Right">Числовой показатель степени</param>
    /// <returns>Выражение степени</returns>
    [NotNull]
    protected static BinaryExpression GetPower([NotNull] Expression Left, double Right) => Expression.Power(Left, Constant(Right));
    /// <summary>Формирует выражение, обратное заданному</summary>
    /// <param name="value">Исходное выражение</param>
    /// <returns>Выражение обратной величины</returns>
    [NotNull, PublicAPI]
    protected static Expression Inverse([NotNull] Expression value) => Divide(Constant(1d), value);

    /// <summary>Формирует выражение модуля (абсолютной величины)</summary>
    /// <param name="value">Исходное выражение</param>
    /// <returns>Выражение модуля</returns>
    [NotNull, PublicAPI]
    protected static Expression GetAbs([NotNull] Expression value) => Expression.Call(((Func<double, double>)Math.Abs).Method, value);
    /// <summary>Формирует выражение квадратного корня</summary>
    /// <param name="value">Подкоренное выражение</param>
    /// <returns>Выражение квадратного корня</returns>
    [NotNull]
    protected static Expression GetSqrt([NotNull] Expression value) => Expression.Call(((Func<double, double>)Math.Sqrt).Method, value);
    /// <summary>Формирует выражение арктангенса двух аргументов</summary>
    /// <param name="y">Ордината точки</param>
    /// <param name="x">Абсцисса точки</param>
    /// <returns>Выражение арктангенса двух аргументов</returns>
    [NotNull]
    protected static Expression GetAtan2([NotNull] Expression y, [NotNull] Expression x) => Expression.Call(((Func<double, double, double>)Math.Atan2).Method, y, x);
    /// <summary>Формирует выражение синуса</summary>
    /// <param name="value">Аргумент синуса</param>
    /// <returns>Выражение синуса</returns>
    [NotNull]
    protected static Expression GetSin([NotNull] Expression value) => Expression.Call(((Func<double, double>)Math.Sin).Method, value);
    /// <summary>Формирует выражение косинуса</summary>
    /// <param name="value">Аргумент косинуса</param>
    /// <returns>Выражение косинуса</returns>
    [NotNull]
    protected static Expression GetCos([NotNull] Expression value) => Expression.Call(((Func<double, double>)Math.Cos).Method, value);
    /// <summary>Формирует выражение натурального логарифма</summary>
    /// <param name="value">Аргумент логарифма</param>
    /// <returns>Выражение натурального логарифма</returns>
    [NotNull]
    protected static Expression GetLog([NotNull] Expression value) => Expression.Call(((Func<double, double>)Math.Log).Method, value);

    #endregion

    /* -------------------------------------------------------------------------------------------------------------------- */

    #region Static public methods

    /// <summary>Формирует комплексное выражение из комплексного числа</summary>
    /// <param name="Z">Комплексное число</param>
    /// <returns>Комплексное константное выражение</returns>
    [NotNull, PublicAPI]
    public static ComplexConstantExpression Mod(MathCore.Complex Z) => Mod(Z.Re, Z.Im);
    /// <summary>Формирует комплексное выражение из действительной и мнимой частей</summary>
    /// <param name="Re">Действительная часть</param>
    /// <param name="Im">Мнимая часть (по умолчанию 0)</param>
    /// <returns>Комплексное константное выражение</returns>
    [NotNull]
    public static ComplexConstantExpression Mod(double Re, double Im = 0) => new(Re, Im);
    /// <summary>Формирует комплексное выражение с заданной действительной частью</summary>
    /// <param name="Re">Выражение действительной части</param>
    /// <returns>Комплексное константное выражение</returns>
    [NotNull]
    public static ComplexConstantExpression Mod(Expression Re) => new(Re, Constant(0d));
    /// <summary>Формирует комплексное выражение из выражений действительной и мнимой частей</summary>
    /// <param name="Re">Выражение действительной части</param>
    /// <param name="Im">Выражение мнимой части</param>
    /// <returns>Комплексное константное выражение</returns>
    [NotNull, PublicAPI]
    public static ComplexConstantExpression Mod(Expression Re, Expression Im) => new(Re, Im);

    /// <summary>Формирует комплексное выражение с заданными модулем и аргументом</summary>
    /// <param name="Abs">Модуль комплексного числа</param>
    /// <param name="Arg">Аргумент комплексного числа</param>
    /// <returns>Комплексное константное выражение</returns>
    [NotNull, PublicAPI]
    public static ComplexConstantExpression Exp(double Abs, double Arg) => Exp(Constant(Abs), Constant(Arg));

    /// <summary>Формирует комплексное выражение с заданными выражениями модуля и аргумента</summary>
    /// <param name="Abs">Выражение модуля</param>
    /// <param name="Arg">Выражение аргумента</param>
    /// <returns>Комплексное константное выражение</returns>
    [NotNull]
    public static ComplexConstantExpression Exp([NotNull] Expression Abs, [NotNull] Expression Arg) => new(Multiply(Abs, GetCos(Arg)), Multiply(Abs, GetSin(Arg)));

    /// <summary>Формирует комплексное выражение единичного модуля с заданным аргументом</summary>
    /// <param name="Arg">Аргумент комплексного числа</param>
    /// <returns>Комплексное константное выражение</returns>
    [NotNull, PublicAPI]
    public static ComplexConstantExpression Exp(double Arg) => Exp(Constant(Arg));

    /// <summary>Формирует комплексное выражение единичного модуля с заданным выражением аргумента</summary>
    /// <param name="Arg">Выражение аргумента</param>
    /// <returns>Комплексное константное выражение</returns>
    [NotNull]
    public static ComplexConstantExpression Exp([NotNull] Expression Arg) => new(GetCos(Arg), GetSin(Arg));

    #endregion

    /* -------------------------------------------------------------------------------------------------------------------- */

    #region Fields

    private Expression _Re = null!;
    private Expression _Im = null!;

    #endregion

    /* -------------------------------------------------------------------------------------------------------------------- */

    #region Properties

    /// <summary>Действительная часть комплексного выражения</summary>
    public Expression Re => _Re ??= GetRe();

    /// <summary>Мнимая часть комплексного выражения</summary>
    public Expression Im => _Im ??= GetIm();

    /// <summary>Модуль комплексного выражения</summary>
    [NotNull] public Expression Abs => GetSqrt(Power);

    /// <summary>Квадрат модуля комплексного выражения</summary>
    [NotNull] public Expression Power => Add(GetPower(Re, 2), GetPower(Im, 2));

    /// <summary>Аргумент комплексного выражения</summary>
    [NotNull] public Expression Arg => GetAtan2(Im, Re);

    /// <summary>Комплексно-сопряжённое выражение</summary>
    [PublicAPI]
    [NotNull] public ComplexExpression ComplexConjugate => new ComplexConjugateExpression(this);

    #endregion

    /* -------------------------------------------------------------------------------------------------------------------- */


    /* -------------------------------------------------------------------------------------------------------------------- */

    /// <summary>Возвращает выражение действительной части комплексного выражения</summary>
    /// <returns>Выражение действительной части</returns>
    [NotNull] protected abstract Expression GetRe();
    /// <summary>Возвращает выражение мнимой части комплексного выражения</summary>
    /// <returns>Выражение мнимой части</returns>
    [NotNull] protected abstract Expression GetIm();

    /// <summary>Формирует лямбда-выражение заданного делегата из комплексного выражения</summary>
    /// <typeparam name="TDelegate">Тип делегата лямбда-выражения</typeparam>
    /// <param name="Parameters">Параметры лямбда-выражения</param>
    /// <returns>Лямбда-выражение заданного типа делегата</returns>
    [NotNull, PublicAPI]
    public Expression<TDelegate> Lambda<TDelegate>(params IEnumerable<ParameterExpression> Parameters)
    {
        var t_complex = typeof(MathCore.Complex);
        var constructor = t_complex.GetConstructor([typeof(double), typeof(double)]);
        Debug.Assert(constructor != null, "MathCore.Complex.ctor info != null");
        var expression = Expression.New(constructor, Re, Im);
        return Expression.Lambda<TDelegate>(expression, Parameters);
    }

    /* -------------------------------------------------------------------------------------------------------------------- */

    /// <summary>Неявное преобразование комплексного числа в комплексное выражение</summary>
    /// <param name="Z">Комплексное число</param>
    /// <returns>Комплексное константное выражение</returns>
    public static implicit operator ComplexExpression(MathCore.Complex Z) => Mod(Z);

    /// <summary>Сложение целого числа и комплексного выражения</summary>
    [NotNull] public static ComplexBinaryExpression operator +(int x, ComplexExpression y) => new ComplexAddExpression(Mod(x), y);
    /// <summary>Сложение числа с плавающей запятой и комплексного выражения</summary>
    [NotNull] public static ComplexBinaryExpression operator +(float x, ComplexExpression y) => new ComplexAddExpression(Mod(x), y);
    /// <summary>Сложение действительного числа и комплексного выражения</summary>
    [NotNull] public static ComplexBinaryExpression operator +(double x, ComplexExpression y) => new ComplexAddExpression(Mod(x), y);
    /// <summary>Сложение комплексного числа и комплексного выражения</summary>
    [NotNull] public static ComplexBinaryExpression operator +(MathCore.Complex x, ComplexExpression y) => new ComplexAddExpression(x.Expression, y);
    /// <summary>Сложение выражения и комплексного выражения</summary>
    [NotNull] public static ComplexBinaryExpression operator +(Expression x, ComplexExpression y) => new ComplexAddExpression(Mod(x), y);
    /// <summary>Сложение комплексного выражения и целого числа</summary>
    [NotNull] public static ComplexBinaryExpression operator +(ComplexExpression x, int y) => new ComplexAddExpression(x, Mod(y));
    /// <summary>Сложение комплексного выражения и числа с плавающей запятой</summary>
    [NotNull] public static ComplexBinaryExpression operator +(ComplexExpression x, float y) => new ComplexAddExpression(x, Mod(y));
    /// <summary>Сложение комплексного выражения и действительного числа</summary>
    [NotNull] public static ComplexBinaryExpression operator +(ComplexExpression x, double y) => new ComplexAddExpression(x, Mod(y));
    /// <summary>Сложение комплексного выражения и комплексного числа</summary>
    [NotNull] public static ComplexBinaryExpression operator +(ComplexExpression x, MathCore.Complex y) => new ComplexAddExpression(x, y.Expression);
    /// <summary>Сложение комплексного выражения и выражения</summary>
    [NotNull] public static ComplexBinaryExpression operator +(ComplexExpression x, Expression y) => new ComplexAddExpression(x, Mod(y));
    /// <summary>Сложение двух комплексных выражений</summary>
    [NotNull] public static ComplexBinaryExpression operator +(ComplexExpression x, ComplexExpression y) => new ComplexAddExpression(x, y);


    /// <summary>Вычитание комплексного выражения из целого числа</summary>
    [NotNull] public static ComplexBinaryExpression operator -(int x, ComplexExpression y) => new ComplexSubtractExpression(Mod(x), y);
    /// <summary>Вычитание комплексного выражения из числа с плавающей запятой</summary>
    [NotNull] public static ComplexBinaryExpression operator -(float x, ComplexExpression y) => new ComplexSubtractExpression(Mod(x), y);
    /// <summary>Вычитание комплексного выражения из действительного числа</summary>
    [NotNull] public static ComplexBinaryExpression operator -(double x, ComplexExpression y) => new ComplexSubtractExpression(Mod(x), y);
    /// <summary>Вычитание комплексного выражения из комплексного числа</summary>
    [NotNull] public static ComplexBinaryExpression operator -(MathCore.Complex x, ComplexExpression y) => new ComplexSubtractExpression(x.Expression, y);
    /// <summary>Вычитание комплексного выражения из выражения</summary>
    [NotNull] public static ComplexBinaryExpression operator -(Expression x, ComplexExpression y) => new ComplexSubtractExpression(Mod(x), y);
    /// <summary>Вычитание целого числа из комплексного выражения</summary>
    [NotNull] public static ComplexBinaryExpression operator -(ComplexExpression x, int y) => new ComplexSubtractExpression(x, Mod(y));
    /// <summary>Вычитание числа с плавающей запятой из комплексного выражения</summary>
    [NotNull] public static ComplexBinaryExpression operator -(ComplexExpression x, float y) => new ComplexSubtractExpression(x, Mod(y));
    /// <summary>Вычитание действительного числа из комплексного выражения</summary>
    [NotNull] public static ComplexBinaryExpression operator -(ComplexExpression x, double y) => new ComplexSubtractExpression(x, Mod(y));
    /// <summary>Вычитание комплексного числа из комплексного выражения</summary>
    [NotNull] public static ComplexBinaryExpression operator -(ComplexExpression x, MathCore.Complex y) => new ComplexSubtractExpression(x, y.Expression);
    /// <summary>Вычитание выражения из комплексного выражения</summary>
    [NotNull] public static ComplexBinaryExpression operator -(ComplexExpression x, Expression y) => new ComplexSubtractExpression(x, Mod(y));
    /// <summary>Вычитание двух комплексных выражений</summary>
    [NotNull] public static ComplexBinaryExpression operator -(ComplexExpression x, ComplexExpression y) => new ComplexSubtractExpression(x, y);


    /// <summary>Умножение целого числа и комплексного выражения</summary>
    [NotNull] public static ComplexBinaryExpression operator *(int x, ComplexExpression y) => new ComplexMultiplyExpression(Mod(x), y);
    /// <summary>Умножение числа с плавающей запятой и комплексного выражения</summary>
    [NotNull] public static ComplexBinaryExpression operator *(float x, ComplexExpression y) => new ComplexMultiplyExpression(Mod(x), y);
    /// <summary>Умножение действительного числа и комплексного выражения</summary>
    [NotNull] public static ComplexBinaryExpression operator *(double x, ComplexExpression y) => new ComplexMultiplyExpression(Mod(x), y);
    /// <summary>Умножение комплексного числа и комплексного выражения</summary>
    [NotNull] public static ComplexBinaryExpression operator *(MathCore.Complex x, ComplexExpression y) => new ComplexMultiplyExpression(x.Expression, y);
    /// <summary>Умножение выражения и комплексного выражения</summary>
    [NotNull] public static ComplexBinaryExpression operator *(Expression x, ComplexExpression y) => new ComplexMultiplyExpression(Mod(x), y);
    /// <summary>Умножение комплексного выражения и целого числа</summary>
    [NotNull] public static ComplexBinaryExpression operator *(ComplexExpression x, int y) => new ComplexMultiplyExpression(x, Mod(y));
    /// <summary>Умножение комплексного выражения и числа с плавающей запятой</summary>
    [NotNull] public static ComplexBinaryExpression operator *(ComplexExpression x, float y) => new ComplexMultiplyExpression(x, Mod(y));
    /// <summary>Умножение комплексного выражения и действительного числа</summary>
    [NotNull] public static ComplexBinaryExpression operator *(ComplexExpression x, double y) => new ComplexMultiplyExpression(x, Mod(y));
    /// <summary>Умножение комплексного выражения и комплексного числа</summary>
    [NotNull] public static ComplexBinaryExpression operator *(ComplexExpression x, MathCore.Complex y) => new ComplexMultiplyExpression(x, y.Expression);
    /// <summary>Умножение комплексного выражения и выражения</summary>
    [NotNull] public static ComplexBinaryExpression operator *(ComplexExpression x, Expression y) => new ComplexMultiplyExpression(x, Mod(y));
    /// <summary>Умножение двух комплексных выражений</summary>
    [NotNull] public static ComplexBinaryExpression operator *(ComplexExpression x, ComplexExpression y) => new ComplexMultiplyExpression(x, y);



    /// <summary>Деление целого числа на комплексное выражение</summary>
    [NotNull] public static ComplexBinaryExpression operator /(int x, ComplexExpression y) => new ComplexDivideExpression(Mod(x), y);
    /// <summary>Деление числа с плавающей запятой на комплексное выражение</summary>
    [NotNull] public static ComplexBinaryExpression operator /(float x, ComplexExpression y) => new ComplexDivideExpression(Mod(x), y);
    /// <summary>Деление действительного числа на комплексное выражение</summary>
    [NotNull] public static ComplexBinaryExpression operator /(double x, ComplexExpression y) => new ComplexDivideExpression(Mod(x), y);
    /// <summary>Деление комплексного числа на комплексное выражение</summary>
    [NotNull] public static ComplexBinaryExpression operator /(MathCore.Complex x, ComplexExpression y) => new ComplexDivideExpression(x.Expression, y);
    /// <summary>Деление выражения на комплексное выражение</summary>
    [NotNull] public static ComplexBinaryExpression operator /(Expression x, ComplexExpression y) => new ComplexDivideExpression(Mod(x), y);
    /// <summary>Деление комплексного выражения на целое число</summary>
    [NotNull] public static ComplexBinaryExpression operator /(ComplexExpression x, int y) => new ComplexDivideExpression(x, Mod(y));
    /// <summary>Деление комплексного выражения на число с плавающей запятой</summary>
    [NotNull] public static ComplexBinaryExpression operator /(ComplexExpression x, float y) => new ComplexDivideExpression(x, Mod(y));
    /// <summary>Деление комплексного выражения на действительное число</summary>
    [NotNull] public static ComplexBinaryExpression operator /(ComplexExpression x, double y) => new ComplexDivideExpression(x, Mod(y));
    /// <summary>Деление комплексного выражения на комплексное число</summary>
    [NotNull] public static ComplexBinaryExpression operator /(ComplexExpression x, MathCore.Complex y) => new ComplexDivideExpression(x, y.Expression);
    /// <summary>Деление комплексного выражения на выражение</summary>
    [NotNull] public static ComplexBinaryExpression operator /(ComplexExpression x, Expression y) => new ComplexDivideExpression(x, Mod(y));
    /// <summary>Деление двух комплексных выражений</summary>
    [NotNull] public static ComplexBinaryExpression operator /(ComplexExpression x, ComplexExpression y) => new ComplexDivideExpression(x, y);

    /// <summary>Возведение целого числа в степень комплексного выражения</summary>
    [NotNull] public static ComplexBinaryExpression operator ^(int x, ComplexExpression y) => new ComplexPowerExpression(Mod(x), y);
    /// <summary>Возведение числа с плавающей запятой в степень комплексного выражения</summary>
    [NotNull] public static ComplexBinaryExpression operator ^(float x, ComplexExpression y) => new ComplexPowerExpression(Mod(x), y);
    /// <summary>Возведение действительного числа в степень комплексного выражения</summary>
    [NotNull] public static ComplexBinaryExpression operator ^(double x, ComplexExpression y) => new ComplexPowerExpression(Mod(x), y);
    /// <summary>Возведение комплексного числа в степень комплексного выражения</summary>
    [NotNull] public static ComplexBinaryExpression operator ^(MathCore.Complex x, ComplexExpression y) => new ComplexPowerExpression(x.Expression, y);
    /// <summary>Возведение выражения в степень комплексного выражения</summary>
    [NotNull] public static ComplexBinaryExpression operator ^(Expression x, ComplexExpression y) => new ComplexPowerExpression(Mod(x), y);
    /// <summary>Возведение комплексного выражения в целочисленную степень</summary>
    [NotNull] public static ComplexBinaryExpression operator ^(ComplexExpression x, int y) => new ComplexPowerExpression(x, Mod(y));
    /// <summary>Возведение комплексного выражения в степень числа с плавающей запятой</summary>
    [NotNull] public static ComplexBinaryExpression operator ^(ComplexExpression x, float y) => new ComplexPowerExpression(x, Mod(y));
    /// <summary>Возведение комплексного выражения в действительную степень</summary>
    [NotNull] public static ComplexBinaryExpression operator ^(ComplexExpression x, double y) => new ComplexPowerExpression(x, Mod(y));
    /// <summary>Возведение комплексного выражения в степень комплексного числа</summary>
    [NotNull] public static ComplexBinaryExpression operator ^(ComplexExpression x, MathCore.Complex y) => new ComplexPowerExpression(x, y.Expression);
    /// <summary>Возведение комплексного выражения в степень выражения</summary>
    [NotNull] public static ComplexBinaryExpression operator ^(ComplexExpression x, Expression y) => new ComplexPowerExpression(x, Mod(y));
    /// <summary>Возведение комплексного выражения в степень комплексного выражения</summary>
    [NotNull] public static ComplexBinaryExpression operator ^(ComplexExpression x, ComplexExpression y) => new ComplexPowerExpression(x, y);


    /* -------------------------------------------------------------------------------------------------------------------- */
}