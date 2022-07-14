#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace MathCore;

public partial class Matrix
{
    public partial class Array
    {
    }

    /// <summary>SVD-разложение матрицы</summary>
    /// <param name="U"></param>
    /// <param name="w"></param>
    /// <param name="V"></param>
    public void SVD(out Matrix U, out double[] w, out Matrix V)
    {
        Array.SVD(_Data, out var u, out w, out var v);
        U = new Matrix(u);
        V = new Matrix(v);
    }

    /// <summary>SVD-разложение матрицы</summary>
    /// <param name="U"></param>
    /// <param name="S"></param>
    /// <param name="V"></param>
    public void SVD(out Matrix U, out Matrix S, out Matrix V)
    {
        SVD(out U, out double[] w, out V);
        S = CreateDiagonal(w);
    }
}