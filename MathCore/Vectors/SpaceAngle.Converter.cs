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
            var V = ss.Split(',', ';');

            var theta = 0d;
            var phi = 0d;
            bool th_set = false, ph_set = false;
            var type = AngleType.Deg;
            for(var i = 0; i < V.Length; i++)
            {
                var v = V[i].Split('=');
                var name_str = v[0];
                var value_str = v[v.Length - 1];
                if(value_str.IsNullOrWhiteSpace()) continue;
                if(name_str.Equals("type", StringComparison.InvariantCultureIgnoreCase))
                {
                    if(value_str.Equals("rad", StringComparison.InvariantCultureIgnoreCase))
                        type = AngleType.Rad;
                    continue;
                }
                else
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

                var vv = V[i].AsDouble();
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