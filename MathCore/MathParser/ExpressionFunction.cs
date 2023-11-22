#nullable enable
namespace MathCore.MathParser;

/// <summary>функция в структуре математического выражения</summary>
public class ExpressionFunction : ExpressionItem, ICloneable<ExpressionFunction>
{
    private Delegate? _Delegate;

    /// <summary>Делегат функции</summary>
    public Delegate? Delegate { get => _Delegate; set => Set(ref _Delegate, value); }

    /// <summary>Массив имён аргументов</summary>
    public IReadOnlyList<string> Arguments { get; }

    /// <summary>Инициализация новой функции структуры математического выражения по сигнатуре</summary>
    /// <param name="Name">Имя функции</param>
    /// <param name="Arguments">Список имён аргументов</param>
    public ExpressionFunction(string Name, IReadOnlyList<string> Arguments) : base(Name) => this.Arguments = Arguments;

    /// <summary>Метод получения значения функции по массиву значений её аргументов</summary>
    /// <param name="arguments">Массив аргументов функции</param>
    /// <returns>Значение функции</returns>
    public double GetValue(double[] arguments) => (double)Delegate.DynamicInvoke(arguments.Cast<object>().ToArray())!;

    /// <summary>Проверка на эквивалентность сигнатуре</summary>
    /// <param name="sName">Имя функции</param>
    /// <param name="ArgumentsCount">Количество аргументов</param>
    /// <returns>Истина, если сигнатура соответствует функции</returns>
    public bool IsEqualSignature(string sName, int ArgumentsCount) => Name == sName && Arguments.Count == ArgumentsCount;

    /// <summary>Проверка на эквивалентность сигнатуре</summary>
    /// <param name="SigName">Имя функции</param>
    /// <param name="arg">Массив имён аргументов</param>
    /// <returns>Истина, если сигнатура соответствует функции</returns>
    public bool IsEqualSignature(string SigName, IReadOnlyList<string?> arg)
    {
        if(!string.Equals(Name, SigName, StringComparison.CurrentCulture)) return false;
        var args = Arguments;
        if(args.Count != arg.Count) return false;
        for(int i = 0, N = args.Count; i < N; i++)
        {
            var arg_null = args[i] is null;
            var arg2_null = arg[i] is null;
            if(arg_null != arg2_null) return false;
            if(!arg_null && args[i] != arg[i]) return false;
            if(!arg2_null && arg[i] != args[i]) return false;
        }

        return true;
    }

    /// <summary>Метод получения значения функции. В общем виде не поддерживается.</summary>
    /// <returns>Значение функции</returns>
    public override double GetValue() => throw new NotSupportedException();

    /// <summary>Клонирование функции</summary>
    /// <returns>Клон функции</returns>
    public ExpressionFunction Clone() => new(Name, Arguments) { Delegate = (Delegate)Delegate.Clone() };

    object ICloneable.Clone() => Clone();
}