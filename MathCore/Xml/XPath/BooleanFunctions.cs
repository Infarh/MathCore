// ReSharper disable once CheckNamespace
namespace System.Xml.XPath;

internal sealed class BooleanFunctions : Query
{
    #region Fields

    private readonly Function.FunctionType _FuncType;

    private readonly Query _Qy;

    #endregion

    #region Constructors

    internal BooleanFunctions(Query qy, Function.FunctionType FType)
    {
        _Qy       = qy;
        _FuncType = FType;
    }

    internal BooleanFunctions(Query qy)
    {
        _Qy       = qy;
        _FuncType = Function.FunctionType.FuncBoolean;
    }

    #endregion

    #region Methods

    internal override object GetValue(XPathReader reader)
    {
        var obj = _FuncType switch
        {
            Function.FunctionType.FuncBoolean => ToBoolean(reader),
            Function.FunctionType.FuncNot     => Not(reader),
            Function.FunctionType.FuncTrue    => true,
            Function.FunctionType.FuncFalse   => false,
            Function.FunctionType.FuncLang    => Lang(reader),
            _                                 => new object()
        };
        return obj;
    }

    //
    //
    //
    internal override XPathResultType ReturnType() => XPathResultType.Boolean;


    //
    //
    //
    internal static bool ToBoolean(double number) => !number.Equals(0d) && !double.IsNaN(number);

    //
    // boolean boolean(object)
    //
    //
    internal static bool ToBoolean(string str) => str.Length > 0;

    //
    // boolean boolean(object)
    // Number: NaN, 0 -> false
    // String: Length = 0 -> false
    // Node-set: empty -> false
    //
    // <Root><e a='1'>text</e></Root>
    // 1). /Root/e[boolean(2)]
    // 2). /Root/e[boolean[string(child::text)]
    // 3). /Root/e[boolean(child::text)]
    internal bool ToBoolean(XPathReader reader)
    {
        var ret = true;

        var obj = _Qy.GetValue(reader);

        if(obj is double)
        {
            var number                                        = Convert.ToDouble(obj);
            if(number.Equals(0d) || double.IsNaN(number)) ret = false;
        }
        else if(obj is string)
        {
            if(obj.ToString().Length == 0) ret = false;
        }
        else if(obj is bool)
            ret = Convert.ToBoolean(obj);
        else if(obj is null && reader.NodeType != XmlNodeType.EndElement)
            ret = false;

        return ret;
    } // toBoolean

    //
    // boolean not(boolean)
    //
    private bool Not(XPathReader reader) => !ToBoolean(reader);

    //
    // boolean lang(string)
    //
    private bool Lang(XPathReader reader)
    {
        var str  = _Qy.GetValue(reader).ToString();
        var lang = reader.XmlLang.ToLower();
        return lang.Equals(str) || str.Equals(lang.Split('-')[0]);
    }

    #endregion
}