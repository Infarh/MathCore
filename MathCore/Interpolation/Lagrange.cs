using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using MathCore.Annotations;
using MathCore.Vectors;

namespace MathCore.Interpolation
{
    public class Lagrange : IInterpolator
    {
        /* ------------------------------------------------------------------------------------------ */

        private delegate void Initializator([NotNull]double[] X, [NotNull]double[] Y);

        /* ------------------------------------------------------------------------------------------ */

        [NotNull]
        public static Polynom GetPolynom([NotNull]double[] X, [NotNull]double[] Y)
        {
            Contract.Requires(X != null);
            Contract.Requires(Y != null);
            Contract.Ensures(Contract.Result<Polynom>() != null);
            return Y.Select((y, i) => new { i, y, p = Polynom.FromRoots(X.Where((x, j) => j != i)) }).Select(v => (v.y / v.p.Value(X[v.i])) * v.p).Sum();
        }

        /* ------------------------------------------------------------------------------------------ */

        public event EventHandler OnInitialized;

        private void Invoke_OnInitialized(EventArgs Args) => OnInitialized?.Invoke(this, Args);
        private void Invoke_OnInitialized() => Invoke_OnInitialized(EventArgs.Empty);

        /* ------------------------------------------------------------------------------------------ */

        [NotNull]
        private Polynom _Polynom;
        private readonly IAsyncResult _InitializationResult;

        /* ------------------------------------------------------------------------------------------ */

        [NotNull]
        public Polynom P
        {
            get
            {
                Contract.Ensures(Contract.Result<Polynom>() != null);
                return _Polynom;
            }
            set
            {
                Contract.Requires(value != null);
                _Polynom = value;
            }
        }
        public bool Initialized => _InitializationResult.IsCompleted;

        public double this[double x] => Value(x);

        /* ------------------------------------------------------------------------------------------ */

        public Lagrange([NotNull]double[] X, [NotNull]double[] Y) => _InitializationResult = ((Initializator)Initialize).BeginInvoke(X, Y, OnInitializationComplite, null);

        public Lagrange([NotNull]params Vector2D[] P)
        {
            var x = new double[P.Length];
            var y = new double[P.Length];
            for(var i = 0; i < P.Length; i++) { x[i] = P[i].X; y[i] = P[i].Y; }
            _InitializationResult = ((Initializator)Initialize).BeginInvoke(x, y, OnInitializationComplite, null);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(_Polynom != null);
        }

        /* ------------------------------------------------------------------------------------------ */

        private void Initialize([NotNull]double[] X, [NotNull]double[] Y) { _Polynom = GetPolynom(X, Y); }

        private void OnInitializationComplite([NotNull]IAsyncResult Result) => Invoke_OnInitialized();

        public double Value(double x)
        {
            if(!Initialized) _InitializationResult.AsyncWaitHandle.WaitOne();
            return _Polynom.Value(x);
        }

        [NotNull]
        public Func<double, double> GetFunction()
        {
            Contract.Ensures(Contract.Result<Func<double, double>>() != null);
            return Value;
        }

        [NotNull]
        public Expression<Func<double, double>> GetExpression() => _Polynom.GetExpression();

        /* ------------------------------------------------------------------------------------------ */
    }
}
