using System;
using System.ComponentModel;
using System.Globalization;

namespace MathCore.Vectors
{
    internal class Vector2DConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext Context, CultureInfo Info, object value)
        {
            //Не указано объекта для преобразования
            if(value is null)
                throw new ArgumentNullException(nameof(value));

            //Оргумент не является трокой, либо строка пуста
            var strs = value as string;
            if(string.IsNullOrEmpty(strs) || strs.Length < 1)
                return base.ConvertFrom(Context, Info, value);

            //Contract.Requires(lv_Str.Length > 0);
            //Убираем все начальные и конечные скобки, ковычки и апострофы
            while(strs[0] == '{' && strs[strs.Length - 1] == '}')
                strs = strs.Substring(1, strs.Length - 2);
            while(strs[0] == '[' && strs[strs.Length - 1] == ']')
                strs = strs.Substring(1, strs.Length - 2);
            while(strs[0] == '(' && strs[strs.Length - 1] == ')')
                strs = strs.Substring(1, strs.Length - 2);
            while(strs[0] == '\'' && strs[strs.Length - 1] == '\'')
                strs = strs.Substring(1, strs.Length - 2);
            while(strs[0] == '"' && strs[strs.Length - 1] == '"')
                strs = strs.Substring(1, strs.Length - 2);

            strs = strs.Replace(" ", "");

            var val = strs.Split(';', ':', '|').ConvertTo(s => double.Parse(s));

            if(val.Length != 2)
                throw new ArgumentNullException(nameof(value));

            return new Vector2D(val[0], val[1]);
        }
    }
}