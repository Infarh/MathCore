// ReSharper disable once CheckNamespace
namespace System.Xml.XPath;

internal sealed class NumberFunctions : Query
{
    #region Fields

    private readonly Function.FunctionType _FuncType;
    private readonly Query _Qy;

    #endregion

    #region Constructors

    public NumberFunctions(Query qy, Function.FunctionType FType)
    {
        _Qy       = qy;
        _FuncType = FType;
    }

    public NumberFunctions() { }

    public NumberFunctions(Query qy)
    {
        _Qy       = qy;
        _FuncType = Function.FunctionType.FuncNumber;
    }

    #endregion

    #region Methods

    //
    //
    internal override object GetValue(XPathReader reader) =>
        _FuncType switch
        {
            Function.FunctionType.FuncNumber  => Number(reader),
            Function.FunctionType.FuncFloor   => Floor(reader),
            Function.FunctionType.FuncCeiling => Ceiling(reader),
            Function.FunctionType.FuncRound   => Round(reader),
            _                                 => new object()
        };

    //
    //
    internal override XPathResultType ReturnType() => XPathResultType.Number;

    //
    //
    internal static double Number(bool Qy) => Convert.ToInt32(Qy);

    //
    //
    internal static double Number(string Qy)
    {
        try
        {
            return Convert.ToDouble(Qy);
        } catch(Exception)
        {
            return double.NaN;
        }
    }

    //
    //
    //internal static double sum(XPathReader reader) {
    //    return 0;

    //}

    //
    //
    internal static double Number(double num) => num;

    //
    // number number(object?)
    // string: IEEE 754, NaN
    // boolean: true 1, false 0
    // node-set: number(string(node-set))
    //
    // <Root><e a='1'/></Root>
    // /Root/e[@a=number('1')]
    // /Root/e[number(@a)=1]

    private double Number(XPathReader reader)
    {
        if(_Qy is null) return double.NaN;
        var obj = _Qy.GetValue(reader);

        return obj is null ? double.NaN : Convert.ToDouble(obj);
    }


    private double Floor(XPathReader reader) => Math.Floor(Convert.ToDouble(_Qy.GetValue(reader)));

    private double Ceiling(XPathReader reader) => Math.Ceiling(Convert.ToDouble(_Qy.GetValue(reader)));

    private double Round(XPathReader reader)
    {
        var n = Convert.ToDouble(_Qy.GetValue(reader));
        // Math.Round does bankers rounding and Round(1.5) == Round(2.5) == 2
        // This is incorrect in XPath and to fix this we are useing Math.Floor(n + 0.5) instead
        // To deal with -0.0 we have to use Math.Round in [0.5, 0.0]
        return n is >= -0.5 and <= 0.0 ? Math.Round(n) : Math.Floor(n + 0.5);
    }

    #endregion
}