namespace MathCore;

public readonly ref partial struct StringPtr
{
    /// <summary>Преобразователь подстроки в требуемый тип</summary>
    /// <typeparam name="T">Целевой тип</typeparam>
    /// <param name="p">Исходная подстрока</param>
    /// <returns>Результат преобразования</returns>
    /// <example>
    /// <![CDATA[
    /// StringPtr.Selector<int> selector = p => p.ParseInt32();
    /// ]]>
    /// </example>
    public delegate T Selector<out T>(StringPtr p);
}
