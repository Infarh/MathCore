using System;
using System.Linq;

using MathCore.Annotations;
using MathCore.Values;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathCore.Tests.Values
{
    [TestClass]
    public class CovarianceValueTest
    {
        [TestMethod]
        public void Covariance_Sin_to_Sin_variance_eq_1()
        {
            const int samples_count = 10000;
            const int periods_count = 1000;
            const int period_length = samples_count / periods_count;
            const double Ax = 5;
            const double Ay = 3;
            const double A0x = -2;
            const double A0y = 0.5;
            var X = new double[samples_count];
            var Y = new double[samples_count];

            for (var i = 0; i < samples_count; i++)
            {
                X[i] = Ax * Math.Sin(2 * Consts.pi * i / period_length) + A0x;
                Y[i] = -Ay * Math.Sin(2 * Consts.pi * i / period_length) + A0y;
            }

            const int window_length = 100;
            var covariance = new CovarianceValue(window_length);
            var result = X.Zip(Y, covariance.AddValue).ToArray();

            var last_X = X[(samples_count - period_length)..];
            var last_Y = Y[(samples_count - period_length)..];
            var avg_X = X.Average();
            var avg_Y = Y.Average();
            var computed_x_avg = last_X.Average();
            var computed_y_avg = last_Y.Average();

            var computed_cov1 = X.Zip(Y, (x, y) => x * y - avg_X * avg_Y).Average();
            var computed_cov2 = X.Zip(Y, (x, y) => x * y).Average() - avg_X * avg_Y;
            var computed_cov3 = X.Zip(Y, (x, y) => (x - avg_X) * (y - avg_Y)).Average();
        }

        /*
         

        float fk_add(float * flt_arr)
        {
          long i;
          float sum, correction, corrected_next_term, new_sum;

          sum = flt_arr[0];
          correction = 0.0;
          for (i = 1; i < ARR_SIZE; i++)
          {
                corrected_next_term = flt_arr[i] - correction;
                new_sum = sum + corrected_next_term;
                correction = (new_sum - sum) - corrected_next_term;
                sum = new_sum;
           }
           return sum;
        }

         */

        private static double KahanSummation([NotNull] double[] X)
        {
            if (X is null) throw new ArgumentNullException(nameof(X));
            var length = X.Length;
            switch (length)
            {
                case 0: return double.NaN;
                case 1: return X[0];
            }

            var result = X[0];
            var tail = 0.0;
            for (var i = 1; i < length; i++)
            {
                var delta = X[i] - tail;
                var sum = result + delta;
                
                tail = sum - result - delta;
                result = sum;
            }

            return result;
        }
    }
}
