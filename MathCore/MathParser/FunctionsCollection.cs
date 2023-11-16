#nullable enable
using System.Collections;
using System.Diagnostics;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.MathParser;

/// <summary>Коллекция функций</summary>
[DebuggerDisplay("Количество зарегистрированных функций = {" + nameof(Count) + "}")]
public class FunctionsCollection : IEnumerable<ExpressionFunction>
{
    /// <summary>Список функций математического выражения</summary>
    private readonly List<ExpressionFunction> _Functions = new();

    /// <summary>Имена функций</summary>
    public IEnumerable<string> Names => _Functions.Select(v => v.Name);

    /// <summary>Количество используемых функций</summary>
    public int Count => _Functions.Count;

    /// <summary>Индексатор функций по имени и списку параметров</summary>
    /// <param name="Name">Имя функции</param>
    /// <param name="ArgumentsCount">Количество аргументов</param>
    /// <returns>Функция, удовлетворяющая загаданной сигнатуре</returns>
    public ExpressionFunction this[string Name, int ArgumentsCount]
    {
        get
        {
            var function = _Functions.FirstOrDefault(f => f.IsEqualSignature(Name, ArgumentsCount));
            if(function != null) return function;
            function = new ExpressionFunction(Name, new string[ArgumentsCount]);
            _Functions.Add(function);
            return function;
        }
    }

    /// <summary>Индексатор функций по имени и списку параметров</summary>
    /// <param name="Name">Имя функции</param>
    /// <param name="Arguments">Список имён аргументов</param>
    /// <returns>Функция, удовлетворяющая загаданной сигнатуре</returns>
    public ExpressionFunction this[string Name, params string[] Arguments]
    {
        get
        {
            var function = _Functions.FirstOrDefault(f => f.IsEqualSignature(Name, Arguments));
            if(function != null) return function;
            function = new ExpressionFunction(Name, Arguments);
            _Functions.Add(function);
            return function;
        }
    }

    /// <summary>Добавить функцию в коллекцию</summary>
    /// <param name="function">Функция</param>
    /// <returns>Истина, если функция была добавлена</returns>
    public bool Add(ExpressionFunction function)
    {
        if(_Functions.FirstOrDefault(F => F.IsEqualSignature(function.Name, function.Arguments)) != null) 
            return false;

        _Functions.Add(function);
        return true;
    }

    /// <inheritdoc />
    IEnumerator<ExpressionFunction> IEnumerable<ExpressionFunction>.GetEnumerator() => _Functions.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Functions).GetEnumerator();
}