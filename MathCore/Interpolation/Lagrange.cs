using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MathCore.Annotations;
using MathCore.Vectors;

namespace MathCore.Interpolation
{
    public class Lagrange : IInterpolator
    {
        /* ------------------------------------------------------------------------------------------ */

        [NotNull, ItemNotNull]
        private static Task<Polynom> GetPolynomAsync([NotNull] double[] X, [NotNull] double[] Y) => (X, Y).Async(p => GetPolynom(p.X, p.Y));

        [NotNull]
        public static Polynom GetPolynom([NotNull] double[] X, [NotNull] double[] Y) => Y.Select((y, i) => (i, y, p:Polynom.FromRoots(X.Where((x, j) => j != i)))).Select(v => (v.y / v.p.Value(X[v.i])) * v.p).Sum();

        /* ------------------------------------------------------------------------------------------ */

        public event EventHandler OnInitialized;

        private void Invoke_OnInitialized(EventArgs Args) => OnInitialized?.Invoke(this, Args);
        private void Invoke_OnInitialized() => Invoke_OnInitialized(EventArgs.Empty);

        /* ------------------------------------------------------------------------------------------ */

        [NotNull] private readonly Task<Polynom> _PolynomTask;

        /* ------------------------------------------------------------------------------------------ */

        [NotNull]
        public Polynom Polynom => _PolynomTask.Result;

        public bool Initialized => _PolynomTask.Status == TaskStatus.RanToCompletion;

        public double this[double x] => Value(x);

        /* ------------------------------------------------------------------------------------------ */

        public Lagrange([NotNull] double[] X, [NotNull] double[] Y) => _PolynomTask = GetPolynomAsync(X, Y);

        public Lagrange([NotNull] params Vector2D[] P)
        {
            var x = new double[P.Length];
            var y = new double[P.Length];
            for(var i = 0; i < P.Length; i++) { x[i] = P[i].X; y[i] = P[i].Y; }

            _PolynomTask = GetPolynomAsync(x, y);
        }

        /* ------------------------------------------------------------------------------------------ */

        public double Value(double x) => Polynom.Value(x);

        [NotNull]
        public Func<double, double> GetFunction() => Polynom.Value;

        [NotNull]
        public Expression<Func<double, double>> GetExpression() => Polynom.GetExpression();

        /* ------------------------------------------------------------------------------------------ */
    }
}