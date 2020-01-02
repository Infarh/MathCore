using System.Collections.Generic;
using System.Linq;
using MathCore.Annotations;
using MathCore.Values;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class NumericalEnumerableExtensions
    {
        public static IEnumerable<double> Average([NotNull] this IEnumerable<double> collection, int length)
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

        [NotNull]
        public static IEnumerable<double> RollingAverage([NotNull] this IEnumerable<double> collection, int length)
        {
            var average = new AverageValue(length);
            return collection.Select(value => average.AddValue(value));
        }

        public static IEnumerable<float> Average([NotNull] this IEnumerable<float> collection, int length)
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

        [NotNull]
        public static IEnumerable<float> RollingAverage([NotNull] this IEnumerable<float> collection, int length)
        {
            var average = new AverageValue(length);
            return collection.Select(value => (float)average.AddValue(value));
        }

        public static IEnumerable<double> Average([NotNull] this IEnumerable<int> collection, int length)
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

        [NotNull]
        public static IEnumerable<double> RollingAverage([NotNull] this IEnumerable<int> collection, int length)
        {
            var average = new AverageValue(length);
            return collection.Select(value => average.AddValue(value));
        }
    }
}