using System;
using System.ComponentModel;
using System.Globalization;

namespace MathCore.Vectors
{
    internal class Vector3DConverter : TypeConverter<string, Vector2D>
    {
        public override object ConvertFrom(ITypeDescriptorContext Context, CultureInfo Info, object value)
        {
            //Не указано объекта для преобразования
            if(value is null)
                throw new ArgumentNullException(nameof(value));

            //Аргумент не является трокой, либо строка пуста
            if (!(value is string s) || string.IsNullOrWhiteSpace(s) || s.Length < 1)
                return base.ConvertFrom(Context, Info, value);

            //Contract.Requires(lv_Str.Length > 0);
            //Убираем все начальные и конечные скобки, ковычки и апострофы
            while(s[0] == '{' && s[s.Length - 1] == '}')
                s = s.Substring(1, s.Length - 2);
            while(s[0] == '[' && s[s.Length - 1] == ']')
                s = s.Substring(1, s.Length - 2);
            while(s[0] == '(' && s[s.Length - 1] == ')')
                s = s.Substring(1, s.Length - 2);
            while(s[0] == '\'' && s[s.Length - 1] == '\'')
                s = s.Substring(1, s.Length - 2);
            while(s[0] == '"' && s[s.Length - 1] == '"')
                s = s.Substring(1, s.Length - 2);

            s = s.Replace(" ", "");

            var val = s.Split(';', ':', '|').ConvertTo(double.Parse);

            if(val.Length != 3) throw new ArgumentNullException(nameof(value));
            return new Vector3D(val[0], val[1], val[2]);
        }
    }
}