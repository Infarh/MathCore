using System.Collections;
using System.Collections.Generic;
using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class RandomExtensions
    {


        //public static double NextNormal(this Random rnd, double D = 1, double M = 0) => Math.Tan(Math.PI * (rnd.NextDouble() - 0.5)) * D + M;

        /// <summary>
        /// Generates normally distributed numbers. Each operation makes two Gaussians for the price of one, and apparently they can be cached or something for better performance, but who cares.
        /// </summary>
        /// <param name="rnd"></param>
        /// <param name="mu">Mean of the distribution</param>
        /// <param name="sigma">Standard deviation</param>
        /// <returns></returns>
        [Copyright("Superbest@bitbucket.org", url = "https://bitbucket.org/Superbest/superbest-random")]
        public static double NextNormal(this Random rnd, double sigma = 1, double mu = 0)
        {
            var u1 = 1d - rnd.NextDouble(); //uniform(0,1] random doubles
            var u2 = 1d - rnd.NextDouble();
            var normal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            return mu + sigma * normal; //random normal(m,D^2)
        }

        public static double NextUniform(this Random rnd, double D = 1, double M = 0) => (rnd.NextDouble() - 0.5) * D + M;

        /// <summary>Generates values from a triangular distribution.</summary>
        /// <remarks>
        /// See http://en.wikipedia.org/wiki/Triangular_distribution for a description of the triangular probability distribution and the algorithm for generating one.
        /// </remarks>
        /// <param name="rnd"></param>
        /// <param name="min">Minimum</param>
        /// <param name="max">Maximum</param>
        /// <param name="mode">Mode (most frequent value)</param>
        /// <returns></returns>
        [Copyright("Superbest@bitbucket.org", url = "https://bitbucket.org/Superbest/superbest-random")]
        public static double NextTriangular(this Random rnd, double min, double max, double mode)
        {
            var u = rnd.NextDouble();

            return u < (mode - min) / (max - min)
                ? min + Math.Sqrt(u * (max - min) * (mode - min))
                : max - Math.Sqrt((1 - u) * (max - min) * (max - mode));
        }

        /// <summary>
        ///   Equally likely to return true or false. Uses <see cref="Random.Next()"/>.
        /// </summary>
        /// <returns></returns>
        [Copyright("Superbest@bitbucket.org", url = "https://bitbucket.org/Superbest/superbest-random")]
        public static bool NextBoolean(this Random rnd) => rnd.Next(2) > 0;

        /// <summary>
        ///   Shuffles a list in O(n) time by using the Fisher-Yates/Knuth algorithm.
        /// </summary>
        /// <param name="rnd"></param>
        /// <param name = "list"></param>
        [Copyright("Superbest@bitbucket.org", url = "https://bitbucket.org/Superbest/superbest-random")]
        public static void Shuffle(this Random rnd, IList list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var j = rnd.Next(0, i + 1);

                var temp = list[j];
                list[j] = list[i];
                list[i] = temp;
            }
        }

        /// <summary>
        /// Returns n unique random numbers in the range [1, n], inclusive. 
        /// This is equivalent to getting the first n numbers of some random permutation of the sequential numbers from 1 to max. 
        /// Runs in O(k^2) time.
        /// </summary>
        /// <param name="rnd"></param>
        /// <param name="n">Maximum number possible.</param>
        /// <param name="k">How many numbers to return.</param>
        /// <returns></returns>
        [Copyright("Superbest@bitbucket.org", url = "https://bitbucket.org/Superbest/superbest-random")]
        public static int[] Permutation(this Random rnd, int n, int k)
        {
            var result = new List<int>();
            var sorted = new SortedSet<int>();

            for (var i = 0; i < k; i++)
            {
                var r = rnd.Next(1, n + 1 - i);

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var q in sorted)
                    if (r >= q) r++;

                result.Add(r);
                sorted.Add(r);
            }

            return result.ToArray();
        }

        [NotNull, ItemCanBeNull]
        public static IEnumerable<T> Next<T>([NotNull] this Random rnd, int count, [NotNull, ItemCanBeNull] params T[] variants)
        {
            if (rnd is null) throw new ArgumentNullException(nameof(rnd));
            if (variants is null) throw new ArgumentNullException(nameof(variants));

            var variants_count = variants.Length;
            for (var i = 0; i < count; i++)
                yield return variants[rnd.Next(0, variants_count)];
        }

        [NotNull]
        public static IEnumerable<int> Sequence([NotNull] this Random rnd, int min, int max, int count = -1)
        {
            if (rnd is null) throw new ArgumentNullException(nameof(rnd));

            for (var i = 0; count == -1 || i < count; i++)
                yield return rnd.Next(min, max);
        }

        [NotNull]
        public static IEnumerable<double> Sequence([NotNull] this Random rnd, int count = -1)
        {
            if (rnd is null) throw new ArgumentNullException(nameof(rnd));

            for (var i = 0; count == -1 || i < count; i++)
                yield return rnd.NextDouble();
        }

        [NotNull]
        public static IEnumerable<double> SequenceNormal([NotNull] this Random rnd, double D = 1, double M = 0, int count = -1)
        {
            if (rnd is null) throw new ArgumentNullException(nameof(rnd));

            for (var i = 0; count == -1 || i < count; i++)
                yield return rnd.NextNormal(D, M);
        }
    }
}
