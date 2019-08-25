using System;
using System.Collections;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
using MathCore.Annotations;

namespace MathCore.Tests.Service
{
    public class UnitTest
    {
        [DST, NotNull]
        public static IComparer GetComparer(double tolerance = 1e-14) => new LambdaComparer<double>((x1, x2) =>
        {
            var delta = x2 - x1;
            if (Math.Abs(delta) < tolerance) delta = 0;
            return Math.Sign(delta);
        });
    }
}