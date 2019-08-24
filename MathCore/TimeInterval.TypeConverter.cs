//using System;
//using System.ComponentModel;
//using System.Globalization;

//namespace MathCore
//{
//    internal class IntervalConverter : ExpandableObjectConverter
//    {
//        public override bool CanConvertFrom(ITypeDescriptorContext context, Type t)
//        {
//            if(t == typeof(string)) return true;
//            return base.CanConvertFrom(context, t);
//        }

//        public override object ConvertFrom(ITypeDescriptorContext Context, CultureInfo Info, object Value)
//        {
//            var lv_Str = Value as string;
//            if(string.IsNullOrEmpty(lv_Str)) return base.ConvertFrom(Context, Info, Value);

//            var lv_Interval = new Interval(0, 0)
//                        {
//                            MinInclude = lv_Str[0] == '[',
//                            MaxInclude = lv_Str[lv_Str.Length - 1] == ']'
//                        };

//            lv_Str = lv_Str.Substring(1, lv_Str.Length - 2);
//            var lv_Values = lv_Str.Split(';');
//            lv_Interval.Min = double.Parse(lv_Values[0]);
//            lv_Interval.Max = double.Parse(lv_Values[1]);

//            return lv_Interval;
//        }

//        public override object ConvertTo(
//                 ITypeDescriptorContext Context,
//                 CultureInfo Culture,
//                 object Value,
//                 Type DestType)
//        {
//            return DestType != null && DestType == typeof (string) && Value is Interval
//                    ? Value.ToString()
//                    : base.ConvertTo(Context, Culture, Value, DestType);
//        }
//    }
//}
