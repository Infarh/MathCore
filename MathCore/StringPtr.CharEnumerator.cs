namespace MathCore;

public readonly ref partial struct StringPtr
{
    /// <summary>Перечислитель символов подстроки для использования в <c>foreach</c></summary>
    /// <param name="Str">Исходная строка</param>
    /// <param name="Pos">Начальное положение подстроки</param>
    /// <param name="Length">Длина подстроки</param>
    /// <example>
    /// <![CDATA[
    /// var ptr = new StringPtr("abc");
    /// foreach (var c in ptr)
    /// {
    ///     Console.WriteLine(c);
    /// }
    /// ]]>
    /// </example>
    public ref struct CharEnumerator(string Str, int Pos, int Length)
    {

        /// <summary>Начальное положение подстроки</summary>
        private readonly int _Pos = Pos;

        /// <summary>Длина подстроки</summary>
        private readonly int _Length = Length;

        /// <summary>Текущий индекс в подстроке</summary>
        private int _Index = 0;

        /// <summary>Текущий символ подстроки</summary>
        public char Current { get; private set; } = '0';

        /// <summary>Смещение перечислителя к следующему символу</summary>
        /// <returns>Истина, если текущее положение символа находится в пределах подстроки</returns>
        /// <example>
        /// <![CDATA[
        /// var ptr = new StringPtr("ab");
        /// var e = ptr.GetEnumerator();
        /// while (e.MoveNext())
        /// {
        ///     Console.WriteLine(e.Current);
        /// }
        /// ]]>
        /// </example>
        public bool MoveNext()
        {
            if (_Index >= _Length) return false;

            Current = Str[_Pos + _Index];
            _Index++;
            return true;
        }
    }
}
