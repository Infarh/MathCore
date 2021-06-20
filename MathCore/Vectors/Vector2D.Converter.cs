using System;
using System.ComponentModel;
using System.Globalization;

namespace MathCore.Vectors
{
    /// <summary>Конвертер для <see cref="Vector2D"/></summary>
    internal class Vector2DConverter : TypeConverter
    {
        /// <inheritdoc />
        public override object ConvertFrom(ITypeDescriptorContext Context, CultureInfo Info, object value)
        {
            //Не указано объекта для преобразования
            if(value is null)
                throw new ArgumentNullException(nameof(value));

            //Аргумент не является строкой, либо строка пуста
            var ss = value as string;
            if(string.IsNullOrEmpty(ss) || ss.Length < 1)
                return base.ConvertFrom(Context, Info, value);

            //Убираем все начальные и конечные скобки, ковычки и апострофы
            while(ss[0] == '{' && ss[^1] == '}') ss = ss[1..^1];
            while(ss[0] == '[' && ss[^1] == ']') ss = ss[1..^1];
            while(ss[0] == '(' && ss[^1] == ')') ss = ss[1..^1];
            while(ss[0] == '\'' && ss[^1] == '\'') ss = ss[1..^1];
            while(ss[0] == '"' && ss[^1] == '"') ss = ss[1..^1];

            return ss.Replace(" ", string.Empty).Split(';', ':', '|').ConvertTo(double.Parse) is { Length: 2 } val
                ? new Vector2D(val[0], val[1]) 
                : throw new ArgumentNullException(nameof(value));
        }
    }
}