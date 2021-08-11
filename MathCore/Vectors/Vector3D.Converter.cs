using System;
using System.ComponentModel;
using System.Globalization;

namespace MathCore.Vectors
{
    /// <summary>Конвертер для <see cref="Vector3D"/></summary>
    internal class Vector3DConverter : TypeConverter<string, Vector2D>
    {
        /// <inheritdoc />
        public override object ConvertFrom(ITypeDescriptorContext Context, CultureInfo Info, object value)
        {
            //Не указано объекта для преобразования
            if(value is null)
                throw new ArgumentNullException(nameof(value));

            //Аргумент не является строкой, либо строка пуста
            if (value is not string s || string.IsNullOrWhiteSpace(s) || s.Length < 1)
                return base.ConvertFrom(Context, Info, value);

            //Убираем все начальные и конечные скобки, ковычки и апострофы
            while(s[0] == '{' && s[^1] == '}') s = s[1..^1];
            while(s[0] == '[' && s[^1] == ']') s = s[1..^1];
            while(s[0] == '(' && s[^1] == ')') s = s[1..^1];
            while(s[0] == '\'' && s[^1] == '\'') s = s[1..^1];
            while(s[0] == '"' && s[^1] == '"') s = s[1..^1];

            return s.Replace(" ", string.Empty).Split(';', ':', '|').ConvertTo(double.Parse) is { Length: 3} val
                ? new Vector3D(val[0], val[1], val[2]) 
                : throw new ArgumentNullException(nameof(value));
        }
    }
}