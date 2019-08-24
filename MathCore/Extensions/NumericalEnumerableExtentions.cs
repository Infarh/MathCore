using System.Collections.Generic;
using System.Linq;
using MathCore.Values;

namespace System
{
    public static class NumericalEnumerableExtentions
    {
        public static IEnumerable<double> Average(this IEnumerable<double> collection, int length)
        {
            var aggregator = new double[length];
            var i = 0;
            foreach(var value in collection)
            {
                aggregator[i++] = value;
                if(i < length) continue;
                var summ = 0d;
                for(var j = 0; j < length; j++)
                    summ += aggregator[j];
                yield return summ / length;
                i = 0;
            }
            if(i > 0)
            {
                var summ = 0d;
                for(var j = 0; j < i; j++)
                    summ += aggregator[j];
                yield return summ / i;
            }
        }

        public static IEnumerable<double> RollingAverage(this IEnumerable<double> collection, int length)
        {
            var average = new AverageValue(length);
            return collection.Select(value => average.AddValue(value));
        }

        public static IEnumerable<float> Average(this IEnumerable<float> collection, int length)
        {
            var aggregator = new double[length];
            var i = 0;
            foreach(var value in collection)
            {
                aggregator[i++] = value;
                if(i < length) continue;
                var summ = 0d;
                for(var j = 0; j < length; j++)
                    summ += aggregator[j];
                yield return (float)(summ / length);
                i = 0;
            }
            if(i > 0)
            {
                var summ = 0d;
                for(var j = 0; j < i; j++)
                    summ += aggregator[j];
                yield return (float)(summ / i);
            }
        }

        public static IEnumerable<float> RollingAverage(this IEnumerable<float> collection, int length)
        {
            var average = new AverageValue(length);
            return collection.Select(value => (float)average.AddValue(value));
        }

        public static IEnumerable<double> Average(this IEnumerable<int> collection, int length)
        {
            var aggregator = new double[length];
            var i = 0;
            foreach(var value in collection)
            {
                aggregator[i++] = value;
                if(i < length) continue;
                var summ = 0d;
                for(var j = 0; j < length; j++)
                    summ += aggregator[j];
                yield return summ / length;
                i = 0;
            }
            if(i > 0)
            {
                var summ = 0d;
                for(var j = 0; j < i; j++)
                    summ += aggregator[j];
                yield return summ / i;
            }
        }

        public static IEnumerable<double> RollingAverage(this IEnumerable<int> collection, int length)
        {
            var average = new AverageValue(length);
            return collection.Select(value => average.AddValue(value));
        }
    }
}