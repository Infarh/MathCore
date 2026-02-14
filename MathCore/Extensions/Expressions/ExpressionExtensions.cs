using System.Linq.Expressions;
using System.Reflection;

using static System.Linq.Expressions.Expression;

using bEx = System.Linq.Expressions.BinaryExpression;
using Ex = System.Linq.Expressions.Expression;
using iEx = System.Linq.Expressions.IndexExpression;
using lEx = System.Linq.Expressions.LambdaExpression;
using mcEx = System.Linq.Expressions.MethodCallExpression;
using mEx = System.Linq.Expressions.MemberExpression;
using pEx = System.Linq.Expressions.ParameterExpression;
using uEx = System.Linq.Expressions.UnaryExpression;
// ReSharper disable MergeCastWithTypeCheck
// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable ConvertIfStatementToSwitchStatement
// ReSharper disable InvertIf

// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace MathCore.Extensions.Expressions;

/// <summary>Методы расширения для работы с деревьями выражений Expression</summary>
/// <remarks>
/// Предоставляет удобные методы для построения, модификации и анализа деревьев выражений.
/// Включает операции арифметики, сравнения, логики, преобразования типов и создания лямбда-выражений.
/// </remarks>
/// <example>
/// <code>
/// var parameter = "x".ParameterOf&lt;double&gt;();
/// var expr = parameter.Multiply(2).Add(5); // x * 2 + 5
/// var lambda = expr.CreateLambda&lt;Func&lt;double, double&gt;&gt;(parameter);
/// var func = lambda.Compile();
/// var result = func(10); // 25
/// </code>
/// </example>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Стиль", "IDE0046:Преобразовать в условное выражение", Justification = "<Ожидание>")]
public static partial class ExpressionExtensions
{

    #region Types

    #endregion

    #region Methods

    /// <summary>Выполняет подстановку одного лямбда-выражения в другое</summary>
    /// <param name="Expr">Основное выражение, в которое выполняется подстановка</param>
    /// <param name="Substitution">Выражение для подстановки</param>
    /// <returns>Новое лямбда-выражение с выполненной подстановкой</returns>
    /// <exception cref="FormatException">Количество аргументов подстановки не равно 1, или во входном выражении отсутствует подставляемый параметр</exception>
    /// <example>
    /// <code>
    /// Expression&lt;Func&lt;int, int&gt;&gt; main_expr = x => x * 2;
    /// Expression&lt;Func&lt;int, int&gt;&gt; subst_expr = x => x + 5;
    /// var result = main_expr.Substitute(subst_expr); // (x + 5) * 2
    /// </code>
    /// </example>
    public static lEx? Substitute(this lEx Expr, lEx Substitution)
    {
        var parameters = Expr.Parameters;
        var substitute_parameters = Substitution.Parameters;
        if (substitute_parameters.Count != 1)
            throw new FormatException("Количество аргументов подстановки не равно 1");
        var substitute_parameter = substitute_parameters[0];
        if (!parameters.Contains(p => p.Name == substitute_parameter.Name && p.Type == substitute_parameter.Type))
            throw new FormatException("Во входном выражении отсутствует подставляемый параметр");

        var visitor = new SubstitutionVisitor(Substitution);
        var result = visitor.Visit(Expr);

        return result as lEx;
    }

    /// <summary>Выполняет подстановку выражения вместо указанного параметра</summary>
    /// <typeparam name="TDelegate">Тип делегата выражения подстановки</typeparam>
    /// <param name="MainEx">Основное выражение</param>
    /// <param name="ParameterName">Имя параметра для подстановки</param>
    /// <param name="SubstExpression">Выражение для подстановки</param>
    /// <returns>Новое лямбда-выражение с выполненной подстановкой</returns>
    public static lEx? Substitute<TDelegate>
    (
        this lEx MainEx,
        string ParameterName,
        Expression<TDelegate> SubstExpression
    )
    {
        #region Проверка параметров

        var main_parameter = MainEx.Parameters.FirstOrDefault(p => p.Name == ParameterName) ?? throw new($"Could not find input parameter \"{ParameterName}\" in Expression \"{MainEx}\"");
        var substitution_parameter = SubstExpression.Parameters.FirstOrDefault(p => p.Name == ParameterName) ?? throw new($"Could not find substitution parameter \"{ParameterName}\" in Expression \"{SubstExpression}\"");
        if (substitution_parameter.Type != main_parameter.Type)
            throw new(
                $"The substitute Expression return type \"{SubstExpression.Type}\" does not match the type of the substituted variable \"{ParameterName}:{main_parameter.Type}\""
            );

        #endregion

        var pars = MainEx.Parameters.ToList();

        var idx_to_subst = pars.IndexOf(main_parameter);

        pars.RemoveAt(idx_to_subst);

        foreach (var subst_pe in SubstExpression.Parameters)
        {
            if (pars.Any(pe => pe.Name == subst_pe.Name))
                throw new($"Input parameter of name \"{subst_pe.Name}\" already exists in the main Expression");
            pars.Insert(idx_to_subst, subst_pe);
            idx_to_subst++;
        }

        var visitor =
            new SubstExpressionVisitor
            {
                SubstExpression = SubstExpression,
                ParamExpressionToSubstitute = main_parameter
            };

        return (lEx?)visitor.Visit(Lambda(MainEx.Body, pars));
    }

    /// <summary>Создаёт выражение создания объекта через конструктор</summary>
    /// <param name="constructor">Информация о конструкторе</param>
    /// <returns>Выражение создания объекта</returns>
    public static NewExpression NewExpression(this ConstructorInfo constructor) => New(constructor);

    /// <summary>Создаёт выражение создания объекта через конструктор с аргументами</summary>
    /// <param name="constructor">Информация о конструкторе</param>
    /// <param name="arguments">Аргументы конструктора</param>
    /// <returns>Выражение создания объекта с аргументами</returns>
    public static NewExpression NewExpression(this ConstructorInfo constructor, IEnumerable<Ex> arguments) => New(constructor, arguments);

    /// <summary>Получает выражение доступа к свойству</summary>
    /// <param name="obj">Объект, содержащий свойство</param>
    /// <param name="Info">Информация о свойстве</param>
    /// <returns>Выражение доступа к свойству</returns>
    public static mEx GetProperty(this Ex obj, PropertyInfo Info) => Property(obj, Info);

    /// <summary>Получает выражение доступа к свойству по имени</summary>
    /// <param name="obj">Объект, содержащий свойство</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Выражение доступа к свойству</returns>
    public static mEx GetProperty(this Ex obj, string PropertyName) => Property(obj, PropertyName);

    /// <summary>Получает выражение доступа к полю</summary>
    /// <param name="obj">Объект, содержащий поле</param>
    /// <param name="Info">Информация о поле</param>
    /// <returns>Выражение доступа к полю</returns>
    public static mEx GetField(this Ex obj, FieldInfo Info) => Field(obj, Info);

    /// <summary>Получает выражение доступа к полю по имени</summary>
    /// <param name="obj">Объект, содержащий поле</param>
    /// <param name="FieldName">Имя поля</param>
    /// <returns>Выражение доступа к полю</returns>
    public static mEx GetField(this Ex obj, string FieldName) => Field(obj, FieldName);

    /// <summary>Создаёт выражение присваивания</summary>
    /// <param name="dest">Целевое выражение</param>
    /// <param name="source">Исходное выражение</param>
    /// <returns>Выражение присваивания</returns>
    public static bEx Assign(this Ex dest, Ex source) => Ex.Assign(dest, source);

    /// <summary>Создаёт выражение присваивания с автоматическим преобразованием значения</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="dest">Целевое выражение</param>
    /// <param name="source">Исходное значение</param>
    /// <returns>Выражение присваивания</returns>
    public static bEx Assign<T>(this Ex dest, T source) => Ex.Assign(dest, source as Ex ?? source.ToExpression());

    /// <summary>Создаёт выражение присваивания с обратным порядком аргументов</summary>
    /// <param name="source">Исходное выражение</param>
    /// <param name="dest">Целевое выражение</param>
    /// <returns>Выражение присваивания</returns>
    public static bEx AssignTo(this Ex source, Ex dest) => Ex.Assign(dest, source);

    /// <summary>Создаёт выражение унарного минуса</summary>
    /// <param name="obj">Выражение для инверсии знака</param>
    /// <returns>Выражение отрицания</returns>
    public static uEx Negate(this Ex obj) => Ex.Negate(obj);

    /// <summary>Создаёт выражение присваивания с добавлением</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение присваивания с добавлением</returns>
    public static bEx AddAssign(this Ex left, Ex right) => Ex.AddAssign(left, right);

    /// <summary>Создаёт выражение присваивания с добавлением значения</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правое значение</param>
    /// <returns>Выражение присваивания с добавлением</returns>
    public static bEx AddAssign<T>(this Ex left, T right) => Ex.AddAssign(left, right as Ex ?? right.ToExpression());

    private static bool IsNumeric(this Ex ex) => ex.Type.IsNumeric();

    private static bool IsNumeric(this Type type) =>
        type == typeof(double)
        || type == typeof(float)
        || type == typeof(byte)
        || type == typeof(sbyte)
        || type == typeof(short)
        || type == typeof(ushort)
        || type == typeof(int)
        || type == typeof(uint)
        || type == typeof(long)
        || type == typeof(ulong);

    private static Ex TryConvert(this Ex a, ref Ex b)
    {
        var ta = a.Type;
        var tb = b.Type;

        if (ta == typeof(double)) b = b.ConvertTo(ta);
        else if (tb == typeof(double)) a = a.ConvertTo(tb);
        else if (ta == typeof(float)) b = b.ConvertTo(ta);
        else if (tb == typeof(float)) a = a.ConvertTo(tb);
        else if (ta == typeof(ulong)) b = b.ConvertTo(ta);
        else if (tb == typeof(ulong)) a = a.ConvertTo(tb);
        else if (ta == typeof(long)) b = b.ConvertTo(ta);
        else if (tb == typeof(long)) a = a.ConvertTo(tb);
        else if (ta == typeof(uint)) b = b.ConvertTo(ta);
        else if (tb == typeof(uint)) a = a.ConvertTo(tb);
        else if (ta == typeof(int)) b = b.ConvertTo(ta);
        else if (tb == typeof(int)) a = a.ConvertTo(tb);
        else if (ta == typeof(sbyte)) b = b.ConvertTo(ta);
        else if (tb == typeof(sbyte)) a = a.ConvertTo(tb);
        else if (ta == typeof(byte)) b = b.ConvertTo(ta);
        else if (tb == typeof(byte)) a = a.ConvertTo(tb);
        return a;
    }

    /// <summary>Создаёт выражение сложения с автоматическим приведением типов</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение сложения</returns>
    public static bEx AddWithConversion(this Ex left, Ex right) =>
        !left.IsNumeric() || !right.IsNumeric() || left.Type == right.Type
            ? left.Add(right)
            : left.TryConvert(ref right).Add(right);

    /// <summary>Создаёт выражение сложения</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение сложения</returns>
    /// <example>
    /// <code>
    /// var x = "x".ParameterOf&lt;int&gt;();
    /// var expr = x.Add(5); // x + 5
    /// </code>
    /// </example>
    public static bEx Add(this Ex left, Ex right) => Ex.Add(left, right);

    /// <summary>Создаёт выражение сложения с опциональным приведением типов</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <param name="conversion">Использовать автоматическое приведение типов</param>
    /// <returns>Выражение сложения</returns>
    public static bEx Add(this Ex left, Ex right, bool conversion) => conversion ? left.AddWithConversion(right) : Ex.Add(left, right);

    /// <summary>Создаёт выражение сложения с целым числом</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Целое число</param>
    /// <returns>Выражение сложения</returns>
    public static bEx Add(this Ex left, int right) => left.AddWithConversion(right.ToExpression());

    /// <summary>Создаёт выражение сложения с вещественным числом</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Вещественное число</param>
    /// <returns>Выражение сложения</returns>
    public static bEx Add(this Ex left, double right) => left.AddWithConversion(right.ToExpression());

    /// <summary>Создаёт выражение сложения с десятичным числом</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Десятичное число</param>
    /// <returns>Выражение сложения</returns>
    public static bEx Add(this Ex left, decimal right) => left.AddWithConversion(right.ToExpression());

    /// <summary>Создаёт выражение конкатенации строк</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Строка</param>
    /// <returns>Выражение конкатенации</returns>
    public static bEx Add(this Ex left, string right) => left.Add(right.ToExpression());

    /// <summary>Создаёт выражение сложения с произвольным значением</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Значение</param>
    /// <returns>Выражение сложения</returns>
    public static bEx Add<T>(this Ex left, T right) => Ex.Add(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение сложения последовательности значений</summary>
    /// <typeparam name="T">Тип значений</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Список значений для сложения</param>
    /// <returns>Выражение сложения всех значений</returns>
    public static bEx? Add<T>(this Ex? left, params IReadOnlyList<T>? right)
    {
        var i = 0;
        Ex l;
        if (left != null) l = left;
        else if (right is null || right.Count == i) return null;
        else l = right[i++].ToExpression();
        while (i < right?.Count)
            l = l.AddWithConversion(right[i++].ToExpression());
        return (bEx)l;
    }

    /// <summary>Создаёт выражение присваивания с вычитанием</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение присваивания с вычитанием</returns>
    public static bEx SubtractAssign(this Ex left, Ex right) => Ex.SubtractAssign(left, right);

    /// <summary>Создаёт выражение присваивания с вычитанием значения</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Значение</param>
    /// <returns>Выражение присваивания с вычитанием</returns>
    public static bEx SubtractAssign<T>(this Ex left, T right) => Ex.SubtractAssign(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение вычитания с автоматическим приведением типов</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение вычитания</returns>
    public static bEx SubtractWithConversion(this Ex left, Ex right) =>
        !left.IsNumeric() || !right.IsNumeric() || left.Type == right.Type
            ? left.Subtract(right)
            : left.TryConvert(ref right).Subtract(right);

    /// <summary>Создаёт выражение вычитания</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение вычитания</returns>
    public static bEx Subtract(this Ex left, Ex right) => Ex.Subtract(left, right);

    /// <summary>Создаёт выражение вычитания с опциональным приведением типов</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <param name="conversion">Использовать автоматическое приведение типов</param>
    /// <returns>Выражение вычитания</returns>
    public static bEx Subtract(this Ex left, Ex right, bool conversion) => conversion ? left.SubtractWithConversion(right) : Ex.Subtract(left, right);

    /// <summary>Создаёт выражение вычитания целого числа</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Целое число</param>
    /// <returns>Выражение вычитания</returns>
    public static bEx Subtract(this Ex left, int right) => left.SubtractWithConversion(right.ToExpression());

    /// <summary>Создаёт выражение вычитания вещественного числа</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Вещественное число</param>
    /// <returns>Выражение вычитания</returns>
    public static bEx Subtract(this Ex left, double right) => left.SubtractWithConversion(right.ToExpression());

    /// <summary>Создаёт выражение вычитания десятичного числа</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Десятичное число</param>
    /// <returns>Выражение вычитания</returns>
    public static bEx Subtract(this Ex left, decimal right) => left.SubtractWithConversion(right.ToExpression());

    /// <summary>Создаёт выражение вычитания произвольного значения</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Значение</param>
    /// <returns>Выражение вычитания</returns>
    public static bEx Subtract<T>(this Ex left, T right) => Ex.Subtract(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение присваивания с умножением</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение присваивания с умножением</returns>
    public static bEx MultiplyAssign(this Ex left, Ex right) => Ex.MultiplyAssign(left, right);

    /// <summary>Создаёт выражение присваивания с умножением на значение</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Значение</param>
    /// <returns>Выражение присваивания с умножением</returns>
    public static bEx MultiplyAssign<T>(this Ex left, T right) => Ex.MultiplyAssign(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение умножения с автоматическим приведением типов</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение умножения</returns>
    public static bEx MultiplyWithConversion(this Ex left, Ex right) =>
        !left.IsNumeric() || !right.IsNumeric() || left.Type == right.Type
            ? left.Mult(right)
            : left.TryConvert(ref right).Mult(right);

    /// <summary>Создаёт выражение умножения</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <param name="conversion">Использовать автоматическое приведение типов</param>
    /// <returns>Выражение умножения</returns>
    /// <example>
    /// <code>
    /// var x = "x".ParameterOf&lt;double&gt;();
    /// var expr = x.Multiply(2.5); // x * 2.5
    /// </code>
    /// </example>
    public static bEx Multiply(this Ex left, Ex right, bool conversion = false) => conversion ? left.MultiplyWithConversion(right) : Ex.Multiply(left, right);

    /// <summary>Создаёт выражение умножения на целое число</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Целое число</param>
    /// <returns>Выражение умножения</returns>
    public static bEx Multiply(this Ex left, int right) => left.MultiplyWithConversion(right.ToExpression());

    /// <summary>Создаёт выражение умножения на вещественное число</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Вещественное число</param>
    /// <returns>Выражение умножения</returns>
    public static bEx Multiply(this Ex left, double right) => left.MultiplyWithConversion(right.ToExpression());

    /// <summary>Создаёт выражение последовательного умножения списка выражений</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Список выражений для умножения</param>
    /// <returns>Выражение умножения всех значений</returns>
    public static bEx? Multiply(this Ex? left, params IReadOnlyList<Ex>? right)
    {
        Ex l;
        if (left != null) l = left;
        else if (right is not { Count: > 0 }) return null;
        else l = right[1];

        var i = 1;
        while (i < right?.Count)
            l = l.MultiplyWithConversion(right[i++]);
        return (bEx)l;
    }

    /// <summary>Создаёт выражение умножения</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Значение</param>
    /// <returns>Выражение умножения</returns>
    public static bEx Mult<T>(this Ex left, T right) => Ex.Multiply(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение умножения двух выражений</summary>
    /// <typeparam name="T">Тип для обобщённого метода</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение умножения</returns>
    public static bEx Mult<T>(this Ex left, Ex right) => Ex.Multiply(left, right);

    /// <summary>Создаёт выражение последовательного умножения списка значений</summary>
    /// <typeparam name="T">Тип значений</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Список значений для умножения</param>
    /// <returns>Выражение умножения всех значений</returns>
    public static bEx? Multiply<T>(this Ex? left, params IReadOnlyList<T>? right)
    {
        Ex l;
        if (left != null) l = left;
        else if (right is not { Count: > 0 }) return null;
        else l = right[1] as Ex ?? right[1].ToExpression();

        var i = 1;
        while (i < right?.Count)
        {
            var v = right[i++];
            l = l.MultiplyWithConversion(v as Ex ?? v.ToExpression());
        }
        return (bEx)l;
    }

    /// <summary>Создаёт выражение присваивания с делением</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение присваивания с делением</returns>
    public static bEx DivideAssign(this Ex left, Ex right) => Ex.DivideAssign(left, right);

    /// <summary>Создаёт выражение присваивания с делением на значение</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Значение</param>
    /// <returns>Выражение присваивания с делением</returns>
    public static bEx DivideAssign<T>(this Ex left, T right) => Ex.DivideAssign(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение деления с автоматическим приведением типов</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение деления</returns>
    public static bEx DivideWithConversion(this Ex left, Ex right)
    {
        if (!left.IsNumeric() || !right.IsNumeric() || left.Type == right.Type) return left.Divide(right);
        return left.TryConvert(ref right).Divide(right);
    }

    /// <summary>Создаёт выражение деления</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение деления</returns>
    public static bEx Divide(this Ex left, Ex right) => Ex.Divide(left, right);

    /// <summary>Создаёт выражение деления на произвольное значение</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Значение</param>
    /// <returns>Выражение деления</returns>
    public static bEx Divide<T>(this Ex left, T right) => Ex.Divide(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение деления с опциональным приведением типов</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <param name="conversion">Использовать автоматическое приведение типов</param>
    /// <returns>Выражение деления</returns>
    public static bEx Divide(this Ex left, Ex right, bool conversion) => conversion ? left.DivideWithConversion(right) : Ex.Divide(left, right);

    /// <summary>Создаёт выражение деления на целое число</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Целое число</param>
    /// <returns>Выражение деления</returns>
    public static bEx Divide(this Ex left, int right) => left.DivideWithConversion(right.ToExpression());

    /// <summary>Создаёт выражение деления на вещественное число</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Вещественное число</param>
    /// <returns>Выражение деления</returns>
    public static bEx Divide(this Ex left, double right) => left.DivideWithConversion(right.ToExpression());

    /// <summary>Создаёт выражение присваивания с возведением в степень</summary>
    /// <param name="left">Основание</param>
    /// <param name="right">Показатель степени</param>
    /// <returns>Выражение присваивания с возведением в степень</returns>
    public static bEx PowerAssign(this Ex left, Ex right) => Ex.PowerAssign(left, right);

    /// <summary>Создаёт выражение присваивания с возведением в степень</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Основание</param>
    /// <param name="right">Показатель степени</param>
    /// <returns>Выражение присваивания с возведением в степень</returns>
    public static bEx PowerAssign<T>(this Ex left, T right) => Ex.PowerAssign(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение возведения в степень с автоматическим приведением типов</summary>
    /// <param name="left">Основание</param>
    /// <param name="right">Показатель степени</param>
    /// <returns>Выражение возведения в степень</returns>
    public static bEx PowerWithConversion(this Ex left, Ex right)
    {
        if (!left.IsNumeric() || !right.IsNumeric() || left.Type == right.Type) return left.Power(right);
        return left.TryConvert(ref right).Power(right);
    }

    /// <summary>Создаёт выражение возведения в степень с автоматическим приведением типов (обратный порядок)</summary>
    /// <param name="left">Показатель степени</param>
    /// <param name="right">Основание</param>
    /// <returns>Выражение возведения в степень</returns>
    public static bEx PowerOfWithConversion(this Ex left, Ex right)
    {
        if (!left.IsNumeric() || !right.IsNumeric() || left.Type == right.Type) return right.Power(left);
        return right.TryConvert(ref left).Power(left);
    }

    /// <summary>Создаёт выражение возведения в степень</summary>
    /// <param name="left">Основание</param>
    /// <param name="right">Показатель степени</param>
    /// <returns>Выражение возведения в степень</returns>
    /// <example>
    /// <code>
    /// var x = "x".ParameterOf&lt;double&gt;();
    /// var expr = x.Power(2); // x^2
    /// </code>
    /// </example>
    public static bEx Power(this Ex left, Ex right) => Ex.Power(left, right);

    /// <summary>Создаёт выражение возведения в степень с произвольным значением показателя</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Основание</param>
    /// <param name="right">Показатель степени</param>
    /// <returns>Выражение возведения в степень</returns>
    public static bEx Power<T>(this Ex left, T right) => Ex.Power(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение возведения в степень с опциональным приведением типов</summary>
    /// <param name="left">Основание</param>
    /// <param name="right">Показатель степени</param>
    /// <param name="conversion">Использовать автоматическое приведение типов</param>
    /// <returns>Выражение возведения в степень</returns>
    public static bEx Power(this Ex left, Ex right, bool conversion) => conversion ? left.PowerWithConversion(right) : Ex.Power(left, right);

    /// <summary>Создаёт выражение возведения правого операнда в степень левого</summary>
    /// <param name="left">Показатель степени</param>
    /// <param name="right">Основание</param>
    /// <returns>Выражение возведения в степень</returns>
    public static bEx PowerOf(this Ex left, Ex right) => Ex.Power(right, left);

    /// <summary>Создаёт выражение возведения значения в степень левого операнда</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Показатель степени</param>
    /// <param name="right">Основание</param>
    /// <returns>Выражение возведения в степень</returns>
    public static bEx PowerOf<T>(this Ex left, T right) => Ex.Power(right as Ex ?? right.ToExpression(), left);

    /// <summary>Создаёт выражение возведения в степень с обратным порядком операндов</summary>
    /// <param name="left">Показатель степени</param>
    /// <param name="right">Основание</param>
    /// <param name="conversion">Использовать автоматическое приведение типов</param>
    /// <returns>Выражение возведения в степень</returns>
    public static bEx PowerOf(this Ex left, Ex right, bool conversion) => conversion ? left.PowerOfWithConversion(right) : Ex.Power(right, left);

    /// <summary>Создаёт выражение возведения в степень целого числа</summary>
    /// <param name="left">Основание</param>
    /// <param name="right">Показатель степени</param>
    /// <returns>Выражение возведения в степень</returns>
    public static bEx Power(this Ex left, int right) => left.PowerWithConversion(right.ToExpression());

    /// <summary>Создаёт выражение возведения целого числа в степень</summary>
    /// <param name="left">Показатель степени</param>
    /// <param name="right">Основание</param>
    /// <returns>Выражение возведения в степень</returns>
    public static bEx PowerOf(this Ex left, int right) => right.ToExpression().PowerWithConversion(left);

    /// <summary>Создаёт выражение возведения в степень вещественного числа</summary>
    /// <param name="left">Основание</param>
    /// <param name="right">Показатель степени</param>
    /// <returns>Выражение возведения в степень</returns>
    public static bEx Power(this Ex left, double right) => left.PowerWithConversion(right.ToExpression());

    /// <summary>Создаёт выражение возведения вещественного числа в степень</summary>
    /// <param name="left">Показатель степени</param>
    /// <param name="right">Основание</param>
    /// <returns>Выражение возведения в степень</returns>
    public static bEx PowerOf(this Ex left, double right) => right.ToExpression().PowerWithConversion(left);

    /// <summary>Создаёт выражение вычисления квадратного корня</summary>
    /// <param name="expr">Выражение</param>
    /// <returns>Выражение вычисления квадратного корня</returns>
    public static mcEx Sqrt(this Ex expr) => MathExpression.Sqrt(expr);

    /// <summary>Создаёт выражение возведения в степень 0.5 (квадратный корень)</summary>
    /// <param name="expr">Выражение</param>
    /// <returns>Выражение возведения в степень 0.5</returns>
    public static bEx SqrtPower(this Ex expr) => MathExpression.SqrtPower(expr);

    /// <summary>Создаёт выражение корня заданной степени</summary>
    /// <param name="expr">Выражение</param>
    /// <param name="power">Степень корня</param>
    /// <returns>Выражение вычисления корня</returns>
    public static bEx SqrtPower(this Ex expr, Ex power) => MathExpression.SqrtPower(expr, power);

    /// <summary>Создаёт выражение корня заданной степени</summary>
    /// <typeparam name="T">Тип степени</typeparam>
    /// <param name="expr">Выражение</param>
    /// <param name="power">Степень корня</param>
    /// <returns>Выражение вычисления корня</returns>
    public static bEx SqrtPower<T>(this Ex expr, T power) => MathExpression.SqrtPower(expr, power as Ex ?? power.ToExpression());

    /// <summary>Создаёт выражение проверки равенства</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение проверки равенства</returns>
    public static bEx IsEqual(this Ex left, Ex right) => Equal(left, right);

    /// <summary>Создаёт выражение проверки равенства со значением</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Значение</param>
    /// <returns>Выражение проверки равенства</returns>
    public static bEx IsEqual<T>(this Ex left, T right) => Equal(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение проверки неравенства</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение проверки неравенства</returns>
    public static bEx IsNotEqual(this Ex left, Ex right) => NotEqual(left, right);

    /// <summary>Создаёт выражение проверки неравенства со значением</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Значение</param>
    /// <returns>Выражение проверки неравенства</returns>
    public static bEx IsNotEqual<T>(this Ex left, T right) => NotEqual(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение проверки "больше"</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение проверки "больше"</returns>
    public static bEx IsGreaterThan(this Ex left, Ex right) => GreaterThan(left, right);

    /// <summary>Создаёт выражение проверки "больше" со значением</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Значение</param>
    /// <returns>Выражение проверки "больше"</returns>
    public static bEx IsGreaterThan<T>(this Ex left, T right) => GreaterThan(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение проверки "больше или равно"</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение проверки "больше или равно"</returns>
    public static bEx IsGreaterThanOrEqual(this Ex left, Ex right) => GreaterThanOrEqual(left, right);

    /// <summary>Создаёт выражение проверки "больше или равно" со значением</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Значение</param>
    /// <returns>Выражение проверки "больше или равно"</returns>
    public static bEx IsGreaterThanOrEqual<T>(this Ex left, T right) => GreaterThanOrEqual(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение проверки "меньше"</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение проверки "меньше"</returns>
    public static bEx IsLessThan(this Ex left, Ex right) => LessThan(left, right);

    /// <summary>Создаёт выражение проверки "меньше" со значением</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Значение</param>
    /// <returns>Выражение проверки "меньше"</returns>
    public static bEx IsLessThan<T>(this Ex left, T right) => LessThan(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение проверки "меньше или равно"</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение проверки "меньше или равно"</returns>
    public static bEx IsLessThanOrEqual(this Ex left, Ex right) => LessThanOrEqual(left, right);

    /// <summary>Создаёт выражение проверки "меньше или равно" со значением</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Значение</param>
    /// <returns>Выражение проверки "меньше или равно"</returns>
    public static bEx IsLessThanOrEqual<T>(this Ex left, T right) => LessThanOrEqual(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение побитового И</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение побитового И</returns>
    public static bEx And(this Ex left, Ex right) => Ex.And(left, right);

    /// <summary>Создаёт выражение побитового И со значением</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Значение</param>
    /// <returns>Выражение побитового И</returns>
    public static bEx And<T>(this Ex left, T right) => Ex.And(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение присваивания с побитовым И</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение присваивания с побитовым И</returns>
    public static bEx AndAssign(this Ex left, Ex right) => Ex.AndAssign(left, right);

    /// <summary>Создаёт выражение присваивания с побитовым И со значением</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Значение</param>
    /// <returns>Выражение присваивания с побитовым И</returns>
    public static bEx AndAssign<T>(this Ex left, T right) => Ex.AndAssign(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение присваивания с побитовым ИЛИ</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение присваивания с побитовым ИЛИ</returns>
    public static bEx OrAssign(this Ex left, Ex right) => Ex.OrAssign(left, right);

    /// <summary>Создаёт выражение присваивания с побитовым ИЛИ со значением</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Значение</param>
    /// <returns>Выражение присваивания с побитовым ИЛИ</returns>
    public static bEx OrAssign<T>(this Ex left, T right) => Ex.OrAssign(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение побитового ИЛИ</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение побитового ИЛИ</returns>
    public static bEx Or(this Ex left, Ex right) => Ex.Or(left, right);

    /// <summary>Создаёт выражение побитового ИЛИ со значением</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Значение</param>
    /// <returns>Выражение побитового ИЛИ</returns>
    public static bEx Or<T>(this Ex left, T right) => Ex.Or(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение логического отрицания</summary>
    /// <param name="d">Выражение для отрицания</param>
    /// <returns>Выражение логического отрицания</returns>
    public static uEx Not(this Ex d) => Ex.Not(d);

    /// <summary>Создаёт выражение логического И с сокращённой оценкой (AndAlso)</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение логического И с сокращённой оценкой</returns>
    public static bEx AndLazy(this Ex left, Ex right) => AndAlso(left, right);

    /// <summary>Создаёт выражение логического И с сокращённой оценкой со значением</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Значение</param>
    /// <returns>Выражение логического И с сокращённой оценкой</returns>
    public static bEx AndLazy<T>(this Ex left, T right) => AndAlso(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение логического ИЛИ с сокращённой оценкой (OrElse)</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение логического ИЛИ с сокращённой оценкой</returns>
    public static bEx OrLazy(this Ex left, Ex right) => OrElse(left, right);

    /// <summary>Создаёт выражение логического ИЛИ с сокращённой оценкой со значением</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Значение</param>
    /// <returns>Выражение логического ИЛИ с сокращённой оценкой</returns>
    public static bEx OrLazy<T>(this Ex left, T right) => OrElse(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение объединения с null (оператор ??)</summary>
    /// <param name="first">Первое выражение</param>
    /// <param name="second">Второе выражение</param>
    /// <returns>Выражение объединения с null</returns>
    public static bEx Coalesce(this Ex first, Ex second) => Ex.Coalesce(first, second);

    /// <summary>Создаёт выражение объединения с null со значением</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="first">Первое выражение</param>
    /// <param name="second">Значение</param>
    /// <returns>Выражение объединения с null</returns>
    public static bEx Coalesce<T>(this Ex first, T second) => Ex.Coalesce(first, second as Ex ?? second.ToExpression());

    /// <summary>Создаёт цепочку выражений объединения с null</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Список выражений для объединения</param>
    /// <returns>Цепочка выражений объединения с null</returns>
    public static bEx? Coalesce(this Ex? left, params IReadOnlyList<Ex>? right)
    {
        var i = 0;
        Ex l;
        if (left != null) l = left;
        else if (right is null || right.Count == i) return null;
        else l = right[i++];
        while (i < right?.Count)
            l = l.Coalesce(right[i++]);
        return (bEx)l;
    }

    /// <summary>Создаёт цепочку выражений объединения с null для значений</summary>
    /// <typeparam name="T">Тип значений</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Список значений для объединения</param>
    /// <returns>Цепочка выражений объединения с null</returns>
    public static bEx? Coalesce<T>(this Ex? left, params IReadOnlyList<T>? right)
    {
        var i = 0;
        Ex l;
        if (left != null) l = left;
        else if (right is null || right.Count == i) return null;
        else
        {
            var v = right[i++];
            l = v as Ex ?? v.ToExpression();
        }

        while (i < right?.Count) l = l.Coalesce(right[i++]);

        return (bEx)l;
    }

    /// <summary>Создаёт выражение присваивания с исключающим ИЛИ</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение присваивания с исключающим ИЛИ</returns>
    public static bEx XORAssign(this Ex left, Ex right) => ExclusiveOrAssign(left, right);

    /// <summary>Создаёт выражение присваивания с исключающим ИЛИ со значением</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Значение</param>
    /// <returns>Выражение присваивания с исключающим ИЛИ</returns>
    public static bEx XORAssign<T>(this Ex left, T right) => ExclusiveOrAssign(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение исключающего ИЛИ (XOR)</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение исключающего ИЛИ</returns>
    public static bEx XOR(this Ex left, Ex right) => ExclusiveOr(left, right);

    /// <summary>Создаёт выражение исключающего ИЛИ со значением</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Значение</param>
    /// <returns>Выражение исключающего ИЛИ</returns>
    public static bEx XOR<T>(this Ex left, T right) => ExclusiveOr(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт цепочку выражений исключающего ИЛИ</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Список выражений</param>
    /// <returns>Цепочка выражений исключающего ИЛИ</returns>
    public static bEx? XOR(this Ex? left, params IReadOnlyList<Ex>? right)
    {
        if (left is null) return null;
        if (right is not { Count: > 0 }) return null;

        var i = 0;
        var l = left;

        while (i < right.Count)
            l = l.XOR(right[i++]);

        return (bEx)l;
    }

    /// <summary>Создаёт цепочку выражений исключающего ИЛИ для значений</summary>
    /// <typeparam name="T">Тип значений</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Список значений</param>
    /// <returns>Цепочка выражений исключающего ИЛИ</returns>
    public static bEx? XOR<T>(this Ex? left, params IReadOnlyList<T>? right)
    {
        if (left is null) return null;
        if (right is not { Count: > 0 }) return null;

        var i = 0;
        var l = left;
        while (i < right.Count)
            l = l.XOR(right[i++]);

        return (bEx)l;
    }

    /// <summary>Создаёт выражение присваивания с остатком от деления</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение присваивания с остатком от деления</returns>
    public static bEx ModuloAssign(this Ex left, Ex right) => Ex.ModuloAssign(left, right);

    /// <summary>Создаёт выражение присваивания с остатком от деления на значение</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Значение</param>
    /// <returns>Выражение присваивания с остатком от деления</returns>
    public static bEx ModuloAssign<T>(this Ex left, T right) => Ex.ModuloAssign(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение остатка от деления (оператор %)</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение остатка от деления</returns>
    public static bEx Modulo(this Ex left, Ex right) => Ex.Modulo(left, right);

    /// <summary>Создаёт выражение остатка от деления на значение</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Значение</param>
    /// <returns>Выражение остатка от деления</returns>
    public static bEx Modulo<T>(this Ex left, T right) => Ex.Modulo(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение присваивания с левым побитовым сдвигом</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Количество бит для сдвига</param>
    /// <returns>Выражение присваивания с левым сдвигом</returns>
    public static bEx LeftShiftAssign(this Ex left, Ex right) => Ex.LeftShiftAssign(left, right);

    /// <summary>Создаёт выражение присваивания с левым побитовым сдвигом на значение</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Количество бит для сдвига</param>
    /// <returns>Выражение присваивания с левым сдвигом</returns>
    public static bEx LeftShiftAssign<T>(this Ex left, T right) => Ex.LeftShiftAssign(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение левого побитового сдвига</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Количество бит для сдвига</param>
    /// <returns>Выражение левого побитового сдвига</returns>
    public static bEx LeftShift(this Ex left, Ex right) => Ex.LeftShift(left, right);

    /// <summary>Создаёт выражение левого побитового сдвига на значение</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Количество бит для сдвига</param>
    /// <returns>Выражение левого побитового сдвига</returns>
    public static bEx LeftShift<T>(this Ex left, T right) => Ex.LeftShift(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение присваивания с правым побитовым сдвигом</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Количество бит для сдвига</param>
    /// <returns>Выражение присваивания с правым сдвигом</returns>
    public static bEx RightShiftAssign(this Ex left, Ex right) => Ex.RightShiftAssign(left, right);

    /// <summary>Создаёт выражение присваивания с правым побитовым сдвигом на значение</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Количество бит для сдвига</param>
    /// <returns>Выражение присваивания с правым сдвигом</returns>
    public static bEx RightShiftAssign<T>(this Ex left, T right) => Ex.RightShiftAssign(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение правого побитового сдвига</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Количество бит для сдвига</param>
    /// <returns>Выражение правого побитового сдвига</returns>
    public static bEx RightShift(this Ex left, Ex right) => Ex.RightShift(left, right);

    /// <summary>Создаёт выражение правого побитового сдвига на значение</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Количество бит для сдвига</param>
    /// <returns>Выражение правого побитового сдвига</returns>
    public static bEx RightShift<T>(this Ex left, T right) => Ex.RightShift(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение проверки ссылочного равенства</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение проверки ссылочного равенства</returns>
    public static bEx IsRefEqual(this Ex left, Ex right) => ReferenceEqual(left, right);

    /// <summary>Создаёт выражение проверки ссылочного равенства со значением</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Значение</param>
    /// <returns>Выражение проверки ссылочного равенства</returns>
    public static bEx IsRefEqual<T>(this Ex left, T right) => ReferenceEqual(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт выражение проверки ссылочного неравенства</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <returns>Выражение проверки ссылочного неравенства</returns>
    public static bEx IsIsRefEqual(this Ex left, Ex right) => ReferenceNotEqual(left, right);

    /// <summary>Создаёт выражение проверки ссылочного неравенства со значением</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Значение</param>
    /// <returns>Выражение проверки ссылочного неравенства</returns>
    public static bEx IsIsRefEqual<T>(this Ex left, T right) => ReferenceNotEqual(left, right as Ex ?? right.ToExpression());

    /// <summary>Создаёт условное выражение (тернарный оператор ?:)</summary>
    /// <param name="Condition">Условие</param>
    /// <param name="Then">Выражение при истинности условия</param>
    /// <param name="Else">Выражение при ложности условия</param>
    /// <returns>Условное выражение</returns>
    public static Ex Condition(this Ex Condition, Ex Then, Ex Else) => Ex.Condition(Condition, Then, Else);

    /// <summary>Создаёт условное выражение с результатом заданного типа</summary>
    /// <typeparam name="T">Тип результата</typeparam>
    /// <param name="Condition">Условие</param>
    /// <param name="Then">Значение при истинности условия</param>
    /// <param name="Else">Значение при ложности условия</param>
    /// <returns>Условное выражение</returns>
    public static Ex ConditionWithResult<T>(this Ex Condition, T Then, T Else) => Ex.Condition(Condition, Then as Ex ?? Then.ToExpression(), Else as Ex ?? Else.ToExpression());

    /// <summary>Создаёт выражение создания объекта через конструктор по умолчанию</summary>
    /// <param name="type">Тип объекта</param>
    /// <returns>Выражение создания объекта</returns>
    public static Ex ToNewExpression(this Type type) => New(type.GetConstructor(Type.EmptyTypes) ?? throw new InvalidOperationException());

    /// <summary>Создаёт выражение создания объекта через конструктор с параметрами</summary>
    /// <param name="type">Тип объекта</param>
    /// <param name="p">Выражения параметров конструктора</param>
    /// <returns>Выражение создания объекта</returns>
    public static Ex ToNewExpression(this Type type, params IEnumerable<Ex> p) =>
        New(type.GetConstructor([.. p.Select(pp => pp.Type)]) ?? throw new InvalidOperationException("Конструктор не найден"));

    /// <summary>Создаёт выражение создания объекта через конструктор с параметрами</summary>
    /// <typeparam name="T">Тип параметров</typeparam>
    /// <param name="type">Тип объекта</param>
    /// <param name="p">Значения параметров конструктора</param>
    /// <returns>Выражение создания объекта</returns>
    public static Ex ToNewExpression<T>(this Type type, params IEnumerable<T> p) =>
        New(type.GetConstructor([.. p.Select(pp => pp!.GetType())])
            ?? throw new InvalidOperationException("Конструктор не найден"));

    /// <summary>Создаёт параметр выражения с заданным именем и типом</summary>
    /// <param name="ParameterName">Имя параметра</param>
    /// <param name="type">Тип параметра</param>
    /// <returns>Выражение параметра</returns>
    public static pEx ParameterOf(this string ParameterName, Type type) => Parameter(type, ParameterName);

    /// <summary>Создаёт параметр выражения с заданным именем и типом</summary>
    /// <typeparam name="T">Тип параметра</typeparam>
    /// <param name="ParameterName">Имя параметра</param>
    /// <returns>Выражение параметра</returns>
    /// <example>
    /// <code>
    /// var x_param = "x".ParameterOf&lt;double&gt;();
    /// var y_param = "y".ParameterOf&lt;double&gt;();
    /// </code>
    /// </example>
    public static pEx ParameterOf<T>(this string ParameterName) => Parameter(typeof(T), ParameterName);

    /// <summary>Создаёт выражение вызова метода</summary>
    /// <param name="obj">Объект, на котором вызывается метод</param>
    /// <param name="method">Имя метода</param>
    /// <param name="arg">Аргументы метода</param>
    /// <returns>Выражение вызова метода</returns>
    public static mcEx GetCall(this Ex obj, string method, IEnumerable<Ex> arg)
        => Call(obj, method, [.. (arg = [.. arg]).Select(a => a.Type)], (Ex[])arg);

    /// <summary>Создаёт выражение вызова метода с аргументами</summary>
    /// <param name="obj">Объект, на котором вызывается метод</param>
    /// <param name="method">Имя метода</param>
    /// <param name="arg">Аргументы метода</param>
    /// <returns>Выражение вызова метода</returns>
    public static mcEx GetCall(this Ex obj, string method, params Ex[] arg)
        => Call(obj, method, [.. arg.Select(a => a.Type)], arg);

    /// <summary>Создаёт выражение вызова метода по информации о методе</summary>
    /// <param name="obj">Объект, на котором вызывается метод</param>
    /// <param name="method">Информация о методе</param>
    /// <param name="arg">Аргументы метода</param>
    /// <returns>Выражение вызова метода</returns>
    public static mcEx GetCall(this Ex obj, MethodInfo method, params IEnumerable<Ex> arg) => Call(obj, method, arg);

    /// <summary>Создаёт выражение вызова метода без аргументов</summary>
    /// <param name="obj">Объект, на котором вызывается метод</param>
    /// <param name="method">Информация о методе</param>
    /// <returns>Выражение вызова метода</returns>
    public static mcEx GetCall(this Ex obj, MethodInfo method) => Call(obj, method);

    /// <summary>Создаёт выражение вызова метода делегата с аргументами</summary>
    /// <param name="obj">Объект, на котором вызывается метод</param>
    /// <param name="d">Делегат</param>
    /// <param name="arg">Аргументы метода</param>
    /// <returns>Выражение вызова метода</returns>
    public static mcEx GetCall(this Ex obj, Delegate d, IEnumerable<Ex> arg) => obj.GetCall(d.Method, arg);

    /// <summary>Создаёт выражение вызова метода делегата с аргументами</summary>
    /// <param name="obj">Объект, на котором вызывается метод</param>
    /// <param name="d">Делегат</param>
    /// <param name="arg">Аргументы метода</param>
    /// <returns>Выражение вызова метода</returns>
    public static mcEx GetCall(this Ex obj, Delegate d, params Ex[] arg) => obj.GetCall(d.Method, arg);

    /// <summary>Создаёт выражение вызова метода делегата без аргументов</summary>
    /// <param name="obj">Объект, на котором вызывается метод</param>
    /// <param name="d">Делегат</param>
    /// <returns>Выражение вызова метода</returns>
    public static mcEx GetCall(this Ex obj, Delegate d) => obj.GetCall(d.Method);

    /// <summary>Создаёт выражение вызова делегата</summary>
    /// <param name="d">Делегат для вызова</param>
    /// <param name="arg">Аргументы делегата</param>
    /// <returns>Выражение вызова делегата</returns>
    public static InvocationExpression GetInvoke(this Ex d, params IEnumerable<Ex> arg) => Invoke(d, arg);

    /// <summary>Создаёт выражение доступа к элементу массива</summary>
    /// <param name="d">Массив</param>
    /// <param name="arg">Индексы</param>
    /// <returns>Выражение доступа к элементу массива</returns>
    public static iEx ArrayAccess(this Ex d, params IEnumerable<Ex> arg) => Ex.ArrayAccess(d, arg);

    /// <summary>Создаёт выражение индексирования массива</summary>
    /// <param name="d">Массив</param>
    /// <param name="arg">Индексы</param>
    /// <returns>Выражение индексирования массива</returns>
    public static mcEx ArrayIndex(this Ex d, params IEnumerable<Ex> arg) => Ex.ArrayIndex(d, arg);

    /// <summary>Создаёт выражение получения длины массива</summary>
    /// <param name="d">Массив</param>
    /// <returns>Выражение получения длины массива</returns>
    public static uEx ArrayLength(this Ex d) => Ex.ArrayLength(d);

    /// <summary>Создаёт выражение преобразования типа</summary>
    /// <param name="d">Выражение для преобразования</param>
    /// <param name="type">Целевой тип</param>
    /// <returns>Выражение преобразования типа</returns>
    public static uEx ConvertTo(this Ex d, Type type) => Convert(d, type);

    /// <summary>Создаёт выражение преобразования типа</summary>
    /// <typeparam name="T">Целевой тип</typeparam>
    /// <param name="d">Выражение для преобразования</param>
    /// <returns>Выражение преобразования типа</returns>
    public static uEx ConvertTo<T>(this Ex d) => Convert(d, typeof(T));

    /// <summary>Создаёт выражение инкремента</summary>
    /// <param name="d">Выражение для инкремента</param>
    /// <returns>Выражение инкремента</returns>
    public static uEx Increment(this Ex d) => Ex.Increment(d);

    /// <summary>Создаёт выражение обратной величины (1/x)</summary>
    /// <param name="expr">Выражение</param>
    /// <returns>Выражение обратной величины</returns>
    public static bEx Inverse(this Ex expr) => 1.ToExpression().Divide(expr);

    /// <summary>Создаёт выражение декремента</summary>
    /// <param name="d">Выражение для декремента</param>
    /// <returns>Выражение декремента</returns>
    public static uEx Decrement(this Ex d) => Ex.Decrement(d);

    /// <summary>Создаёт выражение проверки истинности</summary>
    /// <param name="d">Выражение для проверки</param>
    /// <returns>Выражение проверки истинности</returns>
    public static uEx IsTrue(this Ex d) => Ex.IsTrue(d);

    /// <summary>Создаёт выражение проверки ложности</summary>
    /// <param name="d">Выражение для проверки</param>
    /// <returns>Выражение проверки ложности</returns>
    public static uEx IsFalse(this Ex d) => Ex.IsFalse(d);

    /// <summary>Создаёт выражение цитирования (Quote)</summary>
    /// <param name="d">Выражение для цитирования</param>
    /// <returns>Выражение цитирования</returns>
    public static uEx Quote(this Ex d) => Ex.Quote(d);

    /// <summary>Создаёт выражение побитового дополнения (оператор ~)</summary>
    /// <param name="d">Выражение</param>
    /// <returns>Выражение побитового дополнения</returns>
    public static uEx OnesComplement(this Ex d) => Ex.OnesComplement(d);

    /// <summary>Создаёт выражение значения по умолчанию для типа</summary>
    /// <param name="d">Тип</param>
    /// <returns>Выражение значения по умолчанию</returns>
    public static DefaultExpression Default(this Type d) => Ex.Default(d);

    /// <summary>Создаёт выражение постфиксного инкремента с присваиванием</summary>
    /// <param name="d">Выражение</param>
    /// <returns>Выражение постфиксного инкремента</returns>
    public static uEx PostIncrementAssign(this Ex d) => Ex.PostIncrementAssign(d);

    /// <summary>Создаёт выражение префиксного инкремента с присваиванием</summary>
    /// <param name="d">Выражение</param>
    /// <returns>Выражение префиксного инкремента</returns>
    public static uEx PreIncrementAssign(this Ex d) => Ex.PreIncrementAssign(d);

    /// <summary>Создаёт выражение постфиксного декремента с присваиванием</summary>
    /// <param name="d">Выражение</param>
    /// <returns>Выражение постфиксного декремента</returns>
    public static uEx PostDecrementAssign(this Ex d) => Ex.PostDecrementAssign(d);

    /// <summary>Создаёт выражение префиксного декремента с присваиванием</summary>
    /// <param name="d">Выражение</param>
    /// <returns>Выражение префиксного декремента</returns>
    public static uEx PreDecrementAssign(this Ex d) => Ex.PreDecrementAssign(d);

    /// <summary>Создаёт выражение выброса исключения</summary>
    /// <param name="d">Выражение исключения</param>
    /// <returns>Выражение выброса исключения</returns>
    public static uEx Throw(this Ex d) => Ex.Throw(d);

    /// <summary>Создаёт выражение выброса исключения с указанием типа</summary>
    /// <param name="d">Выражение исключения</param>
    /// <param name="type">Тип</param>
    /// <returns>Выражение выброса исключения</returns>
    public static uEx Throw(this Ex d, Type type) => Ex.Throw(d, type);

    /// <summary>Создаёт выражение безопасного приведения типа (оператор as)</summary>
    /// <param name="d">Выражение для приведения</param>
    /// <param name="type">Целевой тип</param>
    /// <returns>Выражение безопасного приведения типа</returns>
    public static uEx TypeAs(this Ex d, Type type) => Ex.TypeAs(d, type);

    /// <summary>Создаёт выражение проверки типа (оператор is)</summary>
    /// <param name="d">Выражение для проверки</param>
    /// <param name="type">Тип для проверки</param>
    /// <returns>Выражение проверки типа</returns>
    public static TypeBinaryExpression TypeIs(this Ex d, Type type) => Ex.TypeIs(d, type);

    /// <summary>Создаёт выражение извлечения значения из упакованного типа (unbox)</summary>
    /// <param name="d">Выражение упакованного значения</param>
    /// <param name="type">Тип значения</param>
    /// <returns>Выражение извлечения значения</returns>
    public static uEx Unbox(this Ex d, Type type) => Ex.Unbox(d, type);

    /// <summary>Создаёт выражение унарного плюса</summary>
    /// <param name="d">Выражение</param>
    /// <returns>Выражение унарного плюса</returns>
    public static uEx UnaryPlus(this Ex d) => Ex.UnaryPlus(d);

    /// <summary>Создаёт унарное выражение заданного типа</summary>
    /// <param name="d">Выражение</param>
    /// <param name="UType">Тип унарного выражения</param>
    /// <param name="type">Тип результата</param>
    /// <returns>Унарное выражение</returns>
    public static uEx MakeUnary(this Ex d, ExpressionType UType, Type type) => Ex.MakeUnary(UType, d, type);

    /// <summary>Создаёт бинарное выражение заданного типа</summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    /// <param name="UType">Тип бинарного выражения</param>
    /// <returns>Бинарное выражение</returns>
    public static bEx MakeUnary(this Ex left, Ex right, ExpressionType UType) => MakeBinary(UType, left, right);

    /// <summary>Создаёт лямбда-выражение с типизированным делегатом</summary>
    /// <typeparam name="TDelegate">Тип делегата</typeparam>
    /// <param name="body">Тело выражения</param>
    /// <param name="p">Параметры</param>
    /// <returns>Лямбда-выражение</returns>
    /// <example>
    /// <code>
    /// var x = "x".ParameterOf&lt;double&gt;();
    /// var body = x.Multiply(2).Add(1);
    /// var lambda = body.CreateLambda&lt;Func&lt;double, double&gt;&gt;(x);
    /// </code>
    /// </example>
    public static Expression<TDelegate> CreateLambda<TDelegate>(this Ex body, params IEnumerable<pEx> p) => Lambda<TDelegate>(body, p);

    /// <summary>Создаёт лямбда-выражение</summary>
    /// <param name="body">Тело выражения</param>
    /// <param name="p">Параметры</param>
    /// <returns>Лямбда-выражение</returns>
    public static lEx CreateLambda(this Ex body, params pEx[] p) => Lambda(body, p);

    /// <summary>Создаёт лямбда-выражение с указанием типа делегата</summary>
    /// <param name="body">Тело выражения</param>
    /// <param name="DelegateType">Тип делегата</param>
    /// <param name="p">Параметры</param>
    /// <returns>Лямбда-выражение</returns>
    public static lEx CreateLambda(this Ex body, Type DelegateType, params IEnumerable<pEx> p) => Lambda(DelegateType, body, p);

    /// <summary>Компилирует выражение в делегат</summary>
    /// <typeparam name="TDelegate">Тип делегата</typeparam>
    /// <param name="body">Тело выражения</param>
    /// <param name="p">Параметры</param>
    /// <returns>Скомпилированный делегат</returns>
    /// <example>
    /// <code>
    /// var x = "x".ParameterOf&lt;int&gt;();
    /// var func = x.Add(5).CompileTo&lt;Func&lt;int, int&gt;&gt;(x);
    /// var result = func(10); // 15
    /// </code>
    /// </example>
    public static TDelegate CompileTo<TDelegate>(this Ex body, params IEnumerable<pEx> p) => body.CreateLambda<TDelegate>(p).Compile();

    /// <summary>Создаёт полную копию дерева выражения</summary>
    /// <param name="expr">Выражение для клонирования</param>
    /// <returns>Копия выражения</returns>
    public static Ex CloneExpression(this Ex expr)
    {
        var visitor = new CloningVisitor();
        return visitor.Visit(expr).NotNull();
    }

    /// <summary>Создаёт копию массива выражений</summary>
    /// <param name="expr">Массив выражений</param>
    /// <returns>Копия массива выражений</returns>
    public static Ex[]? CloneArray(this Ex[]? expr)
    {
        if (expr is null) return null;
        var visitor = new CloningVisitor();
        var result = new Ex[expr.Length];
        for (var i = 0; i < result.Length; i++)
            result[i] = visitor.Visit(expr[i]).NotNull();
        return result;
    }

    /// <summary>Создаёт копию двумерного массива выражений</summary>
    /// <param name="expr">Двумерный массив выражений</param>
    /// <returns>Копия двумерного массива выражений</returns>
    public static Ex[,]? CloneArray(this Ex[,]? expr)
    {
        if (expr is null) return null;
        var visitor = new CloningVisitor();

        var n = expr.GetLength(0);
        var m = expr.GetLength(1);
        var result = new Ex[n, m];
        for (var i = 0; i < n; i++)
            for (var j = 0; j < m; j++)
                result[i, j] = visitor.Visit(expr[i, j]).NotNull();
        return result;
    }

    #endregion

    /// <summary>Инвертирует логическое выражение</summary>
    /// <typeparam name="T">Тип параметра</typeparam>
    /// <param name="Expr">Логическое выражение</param>
    /// <returns>Инвертированное логическое выражение</returns>
    public static Expression<Func<T, bool>> IsEqual<T>(this Expression<Func<T, bool>> Expr) =>
        Lambda<Func<T, bool>>(Expr.Body.Not(), Expr.Parameters);

    /// <summary>Создаёт выражение проверки равенства результата выражения значению</summary>
    /// <typeparam name="T">Тип параметра</typeparam>
    /// <typeparam name="TValue">Тип значения</typeparam>
    /// <param name="Expr">Выражение</param>
    /// <param name="Value">Значение для сравнения</param>
    /// <returns>Логическое выражение проверки равенства</returns>
    public static Expression<Func<T, bool>> IsEqual<T, TValue>(this Expression<Func<T, TValue>> Expr, TValue Value) =>
        Lambda<Func<T, bool>>(Expr.Body.IsEqual(Value), Expr.Parameters);

    /// <summary>Создаёт выражение проверки неравенства результата выражения значению</summary>
    /// <typeparam name="T">Тип параметра</typeparam>
    /// <typeparam name="TValue">Тип значения</typeparam>
    /// <param name="Expr">Выражение</param>
    /// <param name="Value">Значение для сравнения</param>
    /// <returns>Логическое выражение проверки неравенства</returns>
    public static Expression<Func<T, bool>> IsNotEqual<T, TValue>(this Expression<Func<T, TValue>> Expr, TValue Value) =>
        Lambda<Func<T, bool>>(Expr.Body.IsNotEqual(Value), Expr.Parameters);

    /// <summary>Создаёт выражение проверки "больше" для результата выражения</summary>
    /// <typeparam name="T">Тип параметра</typeparam>
    /// <typeparam name="TValue">Тип значения</typeparam>
    /// <param name="Expr">Выражение</param>
    /// <param name="Value">Значение для сравнения</param>
    /// <returns>Логическое выражение проверки "больше"</returns>
    public static Expression<Func<T, bool>> IsGreaterThan<T, TValue>(this Expression<Func<T, TValue>> Expr, TValue Value) =>
        Lambda<Func<T, bool>>(Expr.Body.IsGreaterThan(Value), Expr.Parameters);

    /// <summary>Создаёт выражение проверки "меньше" для результата выражения</summary>
    /// <typeparam name="T">Тип параметра</typeparam>
    /// <typeparam name="TValue">Тип значения</typeparam>
    /// <param name="Expr">Выражение</param>
    /// <param name="Value">Значение для сравнения</param>
    /// <returns>Логическое выражение проверки "меньше"</returns>
    public static Expression<Func<T, bool>> IsLessThan<T, TValue>(this Expression<Func<T, TValue>> Expr, TValue Value) =>
        Lambda<Func<T, bool>>(Expr.Body.IsLessThan(Value), Expr.Parameters);

    /// <summary>Создаёт выражение проверки "больше или равно" для результата выражения</summary>
    /// <typeparam name="T">Тип параметра</typeparam>
    /// <typeparam name="TValue">Тип значения</typeparam>
    /// <param name="Expr">Выражение</param>
    /// <param name="Value">Значение для сравнения</param>
    /// <returns>Логическое выражение проверки "больше или равно"</returns>
    public static Expression<Func<T, bool>> IsGreaterThanOrEqual<T, TValue>(this Expression<Func<T, TValue>> Expr, TValue Value) =>
        Lambda<Func<T, bool>>(Expr.Body.IsGreaterThanOrEqual(Value), Expr.Parameters);

    /// <summary>Создаёт выражение проверки "меньше или равно" для результата выражения</summary>
    /// <typeparam name="T">Тип параметра</typeparam>
    /// <typeparam name="TValue">Тип значения</typeparam>
    /// <param name="Expr">Выражение</param>
    /// <param name="Value">Значение для сравнения</param>
    /// <returns>Логическое выражение проверки "меньше или равно"</returns>
    public static Expression<Func<T, bool>> IsLessThanOrEqual<T, TValue>(this Expression<Func<T, TValue>> Expr, TValue Value) =>
        Lambda<Func<T, bool>>(Expr.Body.IsLessThanOrEqual(Value), Expr.Parameters);

    /// <summary>Создаёт выражение добавления значения к результату выражения</summary>
    /// <typeparam name="T">Тип параметра</typeparam>
    /// <typeparam name="TValue">Тип значения</typeparam>
    /// <param name="Expr">Выражение</param>
    /// <param name="Value">Значение для добавления</param>
    /// <returns>Выражение с добавлением</returns>
    public static Expression<Func<T, TValue>> Add<T, TValue>(this Expression<Func<T, TValue>> Expr, TValue Value) =>
        Lambda<Func<T, TValue>>(Expr.Body.Add(Value), Expr.Parameters);

    /// <summary>Создаёт выражение вычитания значения из результата выражения</summary>
    /// <typeparam name="T">Тип параметра</typeparam>
    /// <typeparam name="TValue">Тип значения</typeparam>
    /// <param name="Expr">Выражение</param>
    /// <param name="Value">Значение для вычитания</param>
    /// <returns>Выражение с вычитанием</returns>
    public static Expression<Func<T, TValue>> Subtract<T, TValue>(this Expression<Func<T, TValue>> Expr, TValue Value) =>
        Lambda<Func<T, TValue>>(Expr.Body.Subtract(Value), Expr.Parameters);

    /// <summary>Создаёт выражение умножения результата выражения на значение</summary>
    /// <typeparam name="T">Тип параметра</typeparam>
    /// <typeparam name="TValue">Тип значения</typeparam>
    /// <param name="Expr">Выражение</param>
    /// <param name="Value">Значение для умножения</param>
    /// <returns>Выражение с умножением</returns>
    public static Expression<Func<T, TValue>> Multiply<T, TValue>(this Expression<Func<T, TValue>> Expr, TValue Value) =>
        Lambda<Func<T, TValue>>(Expr.Body.Mult(Value), Expr.Parameters);

    /// <summary>Создаёт выражение деления результата выражения на значение</summary>
    /// <typeparam name="T">Тип параметра</typeparam>
    /// <typeparam name="TValue">Тип значения</typeparam>
    /// <param name="Expr">Выражение</param>
    /// <param name="Value">Значение для деления</param>
    /// <returns>Выражение с делением</returns>
    public static Expression<Func<T, TValue>> Divide<T, TValue>(this Expression<Func<T, TValue>> Expr, TValue Value) =>
        Lambda<Func<T, TValue>>(Expr.Body.Divide(Value), Expr.Parameters);

    /// <summary>Создаёт выражение возведения результата выражения в степень</summary>
    /// <typeparam name="T">Тип параметра</typeparam>
    /// <typeparam name="TValue">Тип значения</typeparam>
    /// <param name="Expr">Выражение</param>
    /// <param name="Value">Показатель степени</param>
    /// <returns>Выражение с возведением в степень</returns>
    public static Expression<Func<T, TValue>> Power<T, TValue>(this Expression<Func<T, TValue>> Expr, TValue Value) =>
        Lambda<Func<T, TValue>>(Expr.Body.Power(Value), Expr.Parameters);

    /// <summary>Упрощает дерево выражения, применяя правила оптимизации</summary>
    /// <param name="expr">Выражение для упрощения</param>
    /// <returns>Упрощённое выражение</returns>
    /// <remarks>Применяет правила упрощения к бинарным операциям в дереве выражения</remarks>
    public static Ex? Simplify(this Ex expr)
    {
        var visitor = new ExpressionRebuilder();
        visitor.BinaryVisited += ExpressionSimplifierRules.Binary;

        return visitor.Visit(expr);
    }

    /// <summary>Создаёт выражение вычисления модуля (абсолютного значения)</summary>
    /// <param name="x">Выражение</param>
    /// <returns>Выражение вычисления модуля</returns>
    public static MethodCallExpression GetAbs(this Ex x) => Call(((Func<double, double>)Math.Abs).Method, x);
}