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
            while(ss[0] == '{' && ss[ss.Length - 1] == '}')
                ss = ss.Substring(1, ss.Length - 2);
            while(ss[0] == '[' && ss[ss.Length - 1] == ']')
                ss = ss.Substring(1, ss.Length - 2);
            while(ss[0] == '(' && ss[ss.Length - 1] == ')')
                ss = ss.Substring(1, ss.Length - 2);
            while(ss[0] == '\'' && ss[ss.Length - 1] == '\'')
                ss = ss.Substring(1, ss.Length - 2);
            while(ss[0] == '"' && ss[ss.Length - 1] == '"')
                ss = ss.Substring(1, ss.Length - 2);

            ss = ss.Replace(" ", string.Empty);

            var val = ss.Split(';', ':', '|').ConvertTo(s => double.Parse(s));

            if(val.Length != 2)
                throw new ArgumentNullException(nameof(value));

            return new Vector2D(val[0], val[1]);
        }
    }
}