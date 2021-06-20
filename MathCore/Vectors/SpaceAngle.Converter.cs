using System;
using System.ComponentModel;
using System.Globalization;

namespace MathCore.Vectors
{
    internal class SpaceAngleConverter : TypeConverter<string, SpaceAngle>
    {
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

            var values = ss.Replace(" ", string.Empty).Split(',', ';');

            var theta = 0d;
            var phi = 0d;
            bool th_set = false, ph_set = false;
            var type = AngleType.Deg;
            for(var i = 0; i < values.Length; i++)
            {
                var v = values[i].Split('=');
                var name_str = v[0];
                var value_str = v[^1];
                if(value_str.IsNullOrWhiteSpace()) continue;
                if(name_str.Equals("type", StringComparison.InvariantCultureIgnoreCase))
                {
                    if(value_str.Equals("rad", StringComparison.InvariantCultureIgnoreCase))
                        type = AngleType.Rad;
                    continue;
                }

                switch(name_str.ToLower()[0])
                {
                    case 't':
                        double.TryParse(value_str, out theta);
                        th_set = true;
                        continue;
                    case 'p':
                        double.TryParse(value_str, out phi);
                        ph_set = true;
                        continue;
                }

                var vv = values[i].AsDouble();
                if(vv is null) continue;
                if(!th_set)
                {
                    theta = vv.Value;
                    th_set = true;
                }
                else if(!ph_set)
                {
                    phi = vv.Value;
                    ph_set = true;
                }
            }
            return type == AngleType.Deg
                    ? new SpaceAngle(theta, phi, AngleType.Deg)
                    : new SpaceAngle(theta, phi, AngleType.Deg).InRad;
        }
    }
}