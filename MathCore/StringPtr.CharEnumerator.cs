#nullable enable
namespace MathCore;

public readonly ref partial struct StringPtr
{
    /// <summary>Перечислитель символов подстроки, необходимый для использования в цикле <c>foreach</c></summary>
    public ref struct CharEnumerator
    {
        /// <summary>Исходная строка</summary>
        private readonly string _Str;

        /// <summary>Начальное положение подстроки</summary>
        private readonly int _Pos;

        /// <summary>Длина подстроки</summary>
        private readonly int _Length;

        /// <summary>Текущий индекс в подстроке</summary>
        private int _Index;

        /// <summary>Текущий символ подстроки</summary>
        public char Current { get; private set; }

        /// <summary>Инициализация нового перечислителя подстроки</summary>
        /// <param name="Str">Исходная строка</param>
        /// <param name="Pos">Начальное положение подстроки</param>
        /// <param name="Length">Длина подстроки</param>
        public CharEnumerator(string Str, int Pos, int Length)
        {
            Current = '0';
            _Index  = 0;

            _Str    = Str;
            _Pos    = Pos;
            _Length = Length;
        }

        /// <summary>Смещение перечислителя к следующему символу</summary>
        /// <returns>Истина, если текущее положение символа находится в пределах подстроки</returns>
        public bool MoveNext()
        {
            if (_Index >= _Length) return false;

            Current = _Str[_Pos + _Index];
            _Index++;
            return true;
        }
    }
}
