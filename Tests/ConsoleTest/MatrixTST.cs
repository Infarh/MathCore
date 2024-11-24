
// ReSharper disable IdentifierTypo

namespace ConsoleTest;

internal static class MatrixTST
{
    /// <summary>
    /// A = U * S * V^T
    /// </summary>
    /// <param name="a"></param>
    /// <param name="uneeded">0, 1 or 2. See the description of the parameter U</param>
    /// <param name="vtneeded">0, 1 or 2. See the description of the parameter VT</param>
    /// <param name="additionalmemory">
    /// == 0, the algorithm doesn't use additional memory(lower requirements, lower performance).
    /// == 1, the algorithm uses additional memory of size min(M, N)*min(M, N) of real numbers      It often speeds up the algorithm.
    /// == 2, the algorithm uses additional memory of size M* min(M, N) of real numbers. It allows to get a maximum performance.
    /// </param>
    /// <param name="w">contains singular values in descending order</param>
    /// <param name="u">
    /// if UNeeded=0, U isn't changed, the left singular vectors are not calculated.
    /// if Uneeded=1, U contains left singular vectors (first min(M,N) columns of matrix U). Array whose indexes range within [0..M-1, 0..Min(M,N)-1].
    /// if UNeeded=2, U contains matrix U wholly. Array whose indexes range within [0..M-1, 0..M-1].
    /// </param>
    /// <param name="vt">
    /// if VTNeeded=0, VT isn't changed, the right singular vectors are not calculated.
    /// if VTNeeded=1, VT contains right singular vectors (first min(M,N) rows of matrix V^T). Array whose indexes range within [0..min(M,N)-1, 0..N-1].
    /// if VTNeeded=2, VT contains matrix V^T wholly. Array whose indexes range within [0..N-1, 0..N-1].
    /// </param>
    /// <param name="_params"></param>
    /// <returns></returns>
    public static bool rmatrixsvd(double[,] a,
            int uneeded,
            int vtneeded,
            int additionalmemory, // = 2
            ref double[] w,
            ref double[,] u,
            ref double[,] vt,
            alglib.xparams _params)
    {
        var m = a.GetLength(0);
        var n = a.GetLength(1);

        var tauq = Array.Empty<double>();
        var taup = Array.Empty<double>();
        var tau = Array.Empty<double>();
        var e = Array.Empty<double>();
        double[] work;
        var t2 = new double[0, 0];
        var isupper = new bool();
        int i;
        int j;

        a = (double[,])a.Clone();
        w = [];
        u = new double[0, 0];
        vt = new double[0, 0];

        var result = true;
        if (m == 0 || n == 0) 
            return result;

        //alglib.ap.assert(uneeded >= 0 && uneeded <= 2, "SVDDecomposition: wrong parameters!");
        //alglib.ap.assert(vtneeded >= 0 && vtneeded <= 2, "SVDDecomposition: wrong parameters!");
        //alglib.ap.assert(additionalmemory >= 0 && additionalmemory <= 2, "SVDDecomposition: wrong parameters!");

        // initialize
        var minmn = Math.Min(m, n);
        w = new double[minmn + 1];
        var ncu = 0;
        var nru = 0;

        switch (uneeded)
        {
            case 1:
                nru = m;
                ncu = minmn;
                u = new double[nru - 1 + 1, ncu - 1 + 1];
                break;
            case 2:
                nru = m;
                ncu = m;
                u = new double[nru - 1 + 1, ncu - 1 + 1];
                break;
        }

        var nrvt = 0;
        var ncvt = 0;
        switch (vtneeded)
        {
            case 1:
                nrvt = minmn;
                ncvt = n;
                vt = new double[nrvt - 1 + 1, ncvt - 1 + 1];
                break;
            case 2:
                nrvt = n;
                ncvt = n;
                vt = new double[nrvt - 1 + 1, ncvt - 1 + 1];
                break;
        }

        // M much larger than N
        if (m > 1.6 * n) // Use bidiagonal reduction with QR-decomposition
        {
            if (uneeded == 0) // No left singular vectors to be computed
            {
                rmatrixqr(a, ref tau);

                for (i = 0; i < n; i++)
                    for (j = 0; j < i; j++)
                        a[i, j] = 0;

                rmatrixbd(a, n, n, ref tauq, ref taup);
                rmatrixbdunpackpt(a, n, n, taup, nrvt, ref vt);
                rmatrixbdunpackdiagonals(a, n, n, ref isupper, ref w, ref e);
                result = bdsvd.rmatrixbdsvd(w, e, n, isupper, false, u, 0, a, 0, vt, ncvt, _params);
                return result;
            }

            // Left singular vectors (may be full matrix U) to be computed
            ortfac.rmatrixqr(a, m, n, ref tau, _params);
            ortfac.rmatrixqrunpackq(a, m, n, tau, ncu, ref u, _params);

            for (i = 0; i < n; i++)
                for (j = 0; j < i; j++) 
                    a[i, j] = 0;

            rmatrixbd(a, n, n, ref tauq, ref taup);
            rmatrixbdunpackpt(a, n, n, taup, nrvt, ref vt);
            rmatrixbdunpackdiagonals(a, n, n, ref isupper, ref w, ref e);

            if (additionalmemory < 1) // No additional memory can be used
            {
                ortfac.rmatrixbdmultiplybyq(a, n, n, tauq, u, m, n, true, false, _params);
                result = bdsvd.rmatrixbdsvd(w, e, n, isupper, false, u, m, a, 0, vt, ncvt, _params);
                return result;

            }

            // Large U. Transforming intermediate matrix T2
            work = new double[Math.Max(m, n) + 1];

            ortfac.rmatrixbdunpackq(a, n, n, tauq, n, ref t2, _params);
            blas.copymatrix(u, 0, m - 1, 0, n - 1, ref a, 0, m - 1, 0, n - 1, _params);
            blas.inplacetranspose(ref t2, 0, n - 1, 0, n - 1, ref work, _params);

            result = bdsvd.rmatrixbdsvd(w, e, n, isupper, false, u, 0, t2, n, vt, ncvt, _params);

            ablas.rmatrixgemm(m, n, n, 1.0, a, 0, 0, 0, t2, 0, 0, 1, 0.0, u, 0, 0, _params);

            return result;
        }

        // N much larger than M
        if (n > 1.6 * m) // Use bidiagonal reduction with LQ-decomposition
        {
            if (vtneeded == 0) // No right singular vectors to be computed
            {
                ortfac.rmatrixlq(a, m, n, ref tau, _params);

                for (i = 0; i < m; i++)
                    for (j = i + 1; j < m; j++)
                        a[i, j] = 0;

                rmatrixbd(a, m, m, ref tauq, ref taup);
                rmatrixbdunpackq(a, m, m, tauq, ncu, ref u);
                rmatrixbdunpackdiagonals(a, m, m, ref isupper, ref w, ref e);

                work = new double[m + 1];
                blas.inplacetranspose(ref u, 0, nru - 1, 0, ncu - 1, ref work, _params);
                result = bdsvd.rmatrixbdsvd(w, e, m, isupper, false, a, 0, u, nru, vt, 0, _params);
                blas.inplacetranspose(ref u, 0, nru - 1, 0, ncu - 1, ref work, _params);

                return result;
            }

            // Right singular vectors (may be full matrix VT) to be computed
            ortfac.rmatrixlq(a, m, n, ref tau, _params);
            ortfac.rmatrixlqunpackq(a, m, n, tau, nrvt, ref vt, _params);

            for (i = 0; i < m; i++)
                for (j = i + 1; j < m; j++) 
                    a[i, j] = 0;

            rmatrixbd(a, m, m, ref tauq, ref taup);
            rmatrixbdunpackq(a, m, m, tauq, ncu, ref u);
            rmatrixbdunpackdiagonals(a, m, m, ref isupper, ref w, ref e);
            work = new double[Math.Max(m, n) + 1];
            blas.inplacetranspose(ref u, 0, nru - 1, 0, ncu - 1, ref work, _params);
            if (additionalmemory < 1) // No additional memory available
            {
                ortfac.rmatrixbdmultiplybyp(a, m, m, taup, vt, m, n, false, true, _params);
                result = bdsvd.rmatrixbdsvd(w, e, m, isupper, false, a, 0, u, nru, vt, n, _params);
            }
            else // Large VT. Transforming intermediate matrix T2
            {
                ortfac.rmatrixbdunpackpt(a, m, m, taup, m, ref t2, _params);
                result = bdsvd.rmatrixbdsvd(w, e, m, isupper, false, a, 0, u, nru, t2, m, _params);
                blas.copymatrix(vt, 0, m - 1, 0, n - 1, ref a, 0, m - 1, 0, n - 1, _params);
                ablas.rmatrixgemm(m, n, m, 1.0, t2, 0, 0, 0, a, 0, 0, 0, 0.0, vt, 0, 0, _params);
            }
            blas.inplacetranspose(ref u, 0, nru - 1, 0, ncu - 1, ref work, _params);
            return result;
        }

        if (m <= n) // We can use inplace transposition of U to get rid of columnwise operations
        {
            rmatrixbd(a, m, n, ref tauq, ref taup);
            rmatrixbdunpackq(a, m, n, tauq, ncu, ref u);
            rmatrixbdunpackpt(a, m, n, taup, nrvt, ref vt);
            rmatrixbdunpackdiagonals(a, m, n, ref isupper, ref w, ref e);
            work = new double[m + 1];
            blas.inplacetranspose(ref u, 0, nru - 1, 0, ncu - 1, ref work, _params);
            result = bdsvd.rmatrixbdsvd(w, e, minmn, isupper, false, a, 0, u, nru, vt, ncvt, _params);
            blas.inplacetranspose(ref u, 0, nru - 1, 0, ncu - 1, ref work, _params);
            return result;
        }

        // Simple bidiagonal reduction
        rmatrixbd(a, m, n, ref tauq, ref taup);
        rmatrixbdunpackq(a, m, n, tauq, ncu, ref u);
        rmatrixbdunpackpt(a, m, n, taup, nrvt, ref vt);
        rmatrixbdunpackdiagonals(a, m, n, ref isupper, ref w, ref e);
        if (additionalmemory < 2 || uneeded == 0)
        {
            // We cant use additional memory or there is no need in such operations
            result = bdsvd.rmatrixbdsvd(w, e, minmn, isupper, false, u, nru, a, 0, vt, ncvt, _params);
            return result;
        }

        // We can use additional memory
        t2 = new double[minmn - 1 + 1, m - 1 + 1];
        blas.copyandtranspose(u, 0, m - 1, 0, minmn - 1, ref t2, 0, minmn - 1, 0, m - 1, _params);
        result = bdsvd.rmatrixbdsvd(w, e, minmn, isupper, false, u, 0, t2, m, vt, ncvt, _params);
        blas.copyandtranspose(t2, 0, minmn - 1, 0, m - 1, ref u, 0, m - 1, 0, minmn - 1, _params);

        return result;
    }

    public static void rmatrixbdunpackdiagonals(double[,] b,
        int m,
        int n,
        ref bool isupper,
        ref double[] d,
        ref double[] e)
    {
        int i;

        isupper = false;
        d = [];
        e = [];

        isupper = m >= n;
        if (m <= 0 || n <= 0) return;
        if (isupper)
        {
            d = new double[n];
            e = new double[n];
            for (i = 0; i < n - 1; i++)
            {
                d[i] = b[i, i];
                e[i] = b[i, i + 1];
            }

            d[n - 1] = b[n - 1, n - 1];
        }
        else
        {
            d = new double[m];
            e = new double[m];
            for (i = 0; i < m - 1; i++)
            {
                d[i] = b[i, i];
                e[i] = b[i + 1, i];
            }

            d[m - 1] = b[m - 1, m - 1];
        }
    }

    public static void rmatrixbdunpackpt(double[,] qp,
        int m,
        int n,
        double[] taup,
        int ptrows,
        ref double[,] pt)
    {
        pt = new double[0, 0];

        //alglib.ap.assert(ptrows <= n, "RMatrixBDUnpackPT: PTRows>N!");
        //alglib.ap.assert(ptrows >= 0, "RMatrixBDUnpackPT: PTRows<0!");
        if (m == 0 || n == 0 || ptrows == 0) 
            return;

        //
        // prepare PT
        //
        pt = new double[ptrows, n];
        for (var i = 0; i <= ptrows - 1; i++)
        for (var j = 0; j <= n - 1; j++)
            if (i == j)
                pt[i, j] = 1;
            else
                pt[i, j] = 0;

        //
        // Calculate
        //
        rmatrixbdmultiplybyp(qp, m, n, taup, pt, ptrows, n, true, true);
    }

    public static void rmatrixbdmultiplybyp(double[,] qp,
           int m,
           int n,
           double[] taup,
           double[,] z,
           int zrows,
           int zcolumns,
           bool fromtheright,
           bool dotranspose)
    {
        var i = 0;
        double[] v = [];
        double[] work = [];
        double[] dummy = [];
        var i1 = 0;
        var i2 = 0;
        var istep = 0;
        var i_ = 0;
        var i1_ = 0;

        if (m <= 0 || n <= 0 || zrows <= 0 || zcolumns <= 0)
            return;
        //alglib.ap.assert(fromtheright && zcolumns == n || !fromtheright && zrows == n, "RMatrixBDMultiplyByP: incorrect Z size!");

        var mx = Math.Max(m, n);
        mx = Math.Max(mx, zrows);
        mx = Math.Max(mx, zcolumns);
        v = new double[mx + 1];
        work = new double[mx + 1];
        if (m >= n)
        {
            if (fromtheright)
            {
                i1 = n - 2;
                i2 = 0;
                istep = -1;
            }
            else
            {
                i1 = 0;
                i2 = n - 2;
                istep = 1;
            }

            if (!dotranspose)
            {
                i = i1;
                i1 = i2;
                i2 = i;
                istep = -istep;
            }

            if (n - 1 <= 0) return;

            i = i1;
            do
            {
                i1_ = i + 1 - 1;
                for (i_ = 1; i_ < n - i; i_++)
                    v[i_] = qp[i, i_ + i1_];

                v[1] = 1;
                if (fromtheright)
                    applyreflectionfromtheright(z, taup[i], v, 0, zrows - 1, i + 1, n - 1, ref work);
                else
                    applyreflectionfromtheleft(z, taup[i], v, i + 1, n - 1, 0, zcolumns - 1, ref work);
                i += istep;
            }
            while (i != i2 + istep);
        }
        else
        {
            if (fromtheright)
            {
                i1 = m - 1;
                i2 = 0;
                istep = -1;
            }
            else
            {
                i1 = 0;
                i2 = m - 1;
                istep = 1;
            }
            if (!dotranspose)
            {
                i = i1;
                i1 = i2;
                i2 = i;
                istep = -istep;
            }

            i = i1;
            do
            {
                i1_ = i - 1;
                for (i_ = 1; i_ <= n - i; i_++) 
                    v[i_] = qp[i, i_ + i1_];

                v[1] = 1;
                if (fromtheright)
                    applyreflectionfromtheright(z, taup[i], v, 0, zrows - 1, i, n - 1, ref work);
                else
                    applyreflectionfromtheleft(z, taup[i], v, i, n - 1, 0, zcolumns - 1, ref work);
                i += istep;
            }
            while (i != i2 + istep);
        }
    }

    public static void rmatrixbdunpackq(double[,] qp,
        int m,
        int n,
        double[] tauq,
        int qcolumns,
        ref double[,] q)
    {
        q = new double[0, 0];

        //alglib.ap.assert(qcolumns <= m, "RMatrixBDUnpackQ: QColumns>M!");
        //alglib.ap.assert(qcolumns >= 0, "RMatrixBDUnpackQ: QColumns<0!");
        if (m == 0 || n == 0 || qcolumns == 0) 
            return;

        // prepare Q
        q = new double[m, qcolumns];
        for (var i = 0; i <= m - 1; i++)
            for (var j = 0; j <= qcolumns - 1; j++)
                q[i, j] = i == j ? 1 : 0;

        //
        // Calculate
        //
        rmatrixbdmultiplybyq(qp, m, n, tauq, q, m, qcolumns, false, false);
    }

    public static void rmatrixbdmultiplybyq(double[,] qp,
            int m,
            int n,
            double[] tauq,
            double[,] z,
            int zrows,
            int zcolumns,
            bool fromtheright,
            bool dotranspose)
    {
        int i;
        var i1 = 0;
        var i2 = 0;
        var istep = 0;
        double[] v = [];
        double[] work = [];
        double[] dummy = [];
        int i_;
        int i1_;

        if (m <= 0 || n <= 0 || zrows <= 0 || zcolumns <= 0) 
            return;

        //alglib.ap.assert(fromtheright && zcolumns == m || !fromtheright && zrows == m, "RMatrixBDMultiplyByQ: incorrect Z size!");
        var mx = Math.Max(m, n);
        mx = Math.Max(mx, zrows);
        mx = Math.Max(mx, zcolumns);
        v = new double[mx + 1];
        work = new double[mx + 1];

        if (m >= n)
        {
            if (fromtheright)
            {
                i1 = 0;
                i2 = n - 1;
                istep = 1;
            }
            else
            {
                i1 = n - 1;
                i2 = 0;
                istep = -1;
            }
            if (dotranspose)
            {
                i = i1;
                i1 = i2;
                i2 = i;
                istep = -istep;
            }

            i = i1;
            do
            {
                i1_ = i - 1;
                for (i_ = 1; i_ <= m - i; i_++) 
                    v[i_] = qp[i_ + i1_, i];

                v[1] = 1;
                if (fromtheright)
                    applyreflectionfromtheright(z, tauq[i], v, 0, zrows - 1, i, m - 1, ref work);
                else
                    applyreflectionfromtheleft(z, tauq[i], v, i, m - 1, 0, zcolumns - 1, ref work);
                i += istep;
            }
            while (i != i2 + istep);
        }
        else
        {
            if (fromtheright)
            {
                i1 = 0;
                i2 = m - 2;
                istep = 1;
            }
            else
            {
                i1 = m - 2;
                i2 = 0;
                istep = -1;
            }

            if (dotranspose)
            {
                i = i1;
                i1 = i2;
                i2 = i;
                istep = -istep;
            }

            if (m - 1 <= 0) return;

            i = i1;
            do
            {
                i1_ = i + 1 - 1;
                for (i_ = 1; i_ < m - i; i_++) 
                    v[i_] = qp[i_ + i1_, i];

                v[1] = 1;
                if (fromtheright)
                    applyreflectionfromtheright(z, tauq[i], v, 0, zrows - 1, i + 1, m - 1, ref work);
                else
                    applyreflectionfromtheleft(z, tauq[i], v, i + 1, m - 1, 0, zcolumns - 1, ref work);
                i += istep;
            }
            while (i != i2 + istep);
        }
    }

    public static void rmatrixbd(double[,] a, int m, int n, ref double[] tauq, ref double[] taup)
    {
        tauq = [];
        taup = [];

        if (n <= 0 || m <= 0) 
            return;

        var maxmn = Math.Max(m, n);
        var work = new double[maxmn + 1];
        var t = new double[maxmn + 1];
        int i;
        if (m >= n)
        {
            tauq = new double[n];
            taup = new double[n];
            for (i = 0; i < n; i++)
            {
                tauq[i] = 0.0;
                taup[i] = 0.0;
            }
        }
        else
        {
            tauq = new double[m];
            taup = new double[m];
            for (i = 0; i < m; i++)
            {
                tauq[i] = 0.0;
                taup[i] = 0.0;
            }
        }

        double ltau;
        int i_, i1_;
        if (m >= n) // Reduce to upper bidiagonal form
            for (i = 0; i < n; i++)
            {
                // Generate elementary reflector H(i) to annihilate A(i+1:m-1,i)
                i1_ = i - 1;
                for (i_ = 1; i_ <= m - i; i_++) 
                    t[i_] = a[i_ + i1_, i];

                generatereflection(ref t, m - i, out ltau);
                tauq[i] = ltau;
                i1_ = 1 - i;
                for (i_ = i; i_ < m; i_++) 
                    a[i_, i] = t[i_ + i1_];
                t[1] = 1;

                // Apply H(i) to A(i:m-1,i+1:n-1) from the left
                applyreflectionfromtheleft(a, ltau, t, i, m - 1, i + 1, n - 1, ref work);
                if (i < n - 1)
                {
                    // Generate elementary reflector G(i) to annihilate
                    // A(i,i+2:n-1)
                    i1_ = i + 1 - 1;
                    for (i_ = 1; i_ < n - i; i_++) 
                        t[i_] = a[i, i_ + i1_];

                    generatereflection(ref t, n - 1 - i, out ltau);
                    taup[i] = ltau;
                    i1_ = 1 - (i + 1);
                    for (i_ = i + 1; i_ <= n - 1; i_++) 
                        a[i, i_] = t[i_ + i1_];
                    t[1] = 1;

                    // Apply G(i) to A(i+1:m-1,i+1:n-1) from the right
                    applyreflectionfromtheright(a, ltau, t, i + 1, m - 1, i + 1, n - 1, ref work);
                }
                else
                    taup[i] = 0;
            }
        else // Reduce to lower bidiagonal form
            for (i = 0; i < m; i++)
            {
                // Generate elementary reflector G(i) to annihilate A(i,i+1:n-1)
                i1_ = i - 1;
                for (i_ = 1; i_ <= n - i; i_++) 
                    t[i_] = a[i, i_ + i1_];

                generatereflection(ref t, n - i, out ltau);
                taup[i] = ltau;
                i1_ = 1 - i;

                for (i_ = i; i_ < n; i_++) 
                    a[i, i_] = t[i_ + i1_];
                t[1] = 1;

                // Apply G(i) to A(i+1:m-1,i:n-1) from the right
                applyreflectionfromtheright(a, ltau, t, i + 1, m - 1, i, n - 1, ref work);
                if (i < m - 1)
                {
                    // Generate elementary reflector H(i) to annihilate
                    // A(i+2:m-1,i)
                    i1_ = i + 1 - 1;
                    for (i_ = 1; i_ < m - i; i_++) 
                        t[i_] = a[i_ + i1_, i];

                    generatereflection(ref t, m - 1 - i, out ltau);
                    tauq[i] = ltau;
                    i1_ = 1 - (i + 1);
                    for (i_ = i + 1; i_ < m; i_++) a[i_, i] = t[i_ + i1_];
                    t[1] = 1;

                    // Apply H(i) to A(i+1:m-1,i+1:n-1) from the left
                    applyreflectionfromtheleft(a, ltau, t, i + 1, m - 1, i + 1, n - 1, ref work);
                }
                else
                    tauq[i] = 0;
            }
    }

    public static void rmatrixqr(double[,] a, ref double[] tau)
    {
        var m = a.GetLength(0);
        var n = a.GetLength(1);

        if (m <= 0 || n <= 0)
        {
            tau = [];
            return;
        }

        var minmn = Math.Min(m, n);
        var ts = matrixtilesizeb();
        var work = new double[Math.Max(m, n) + 1];
        var t = new double[Math.Max(m, n) + 1];
        tau = new double[minmn];
        var taubuf = new double[minmn];
        var tmpa = new double[m, ts];
        var tmpt = new double[ts, 2 * ts];
        var tmpr = new double[2 * ts, n];

        // Blocked code
        var blockstart = 0;
        while (blockstart != minmn)
        {
            var blocksize = minmn - blockstart; // Determine block size
            if (blocksize > ts)
                blocksize = ts;
            var rowscount = m - blockstart;

            // QR decomposition of submatrix.
            // Matrix is copied to temporary storage to solve
            // some TLB issues arising from non-contiguous memory
            // access pattern.
            rmatrixcopy(rowscount, blocksize, a, blockstart, blockstart, tmpa, 0, 0);
            rmatrixqrbasecase(ref tmpa, rowscount, blocksize, ref work, ref t, ref taubuf);
            rmatrixcopy(rowscount, blocksize, tmpa, 0, 0, a, blockstart, blockstart);
            var i1_ = 0 - blockstart;
            int i_;
            for (i_ = blockstart; i_ < blockstart + blocksize; i_++) 
                tau[i_] = taubuf[i_ + i1_];

            // Update the rest, choose between:
            // a) Level 2 algorithm (when the rest of the matrix is small enough)
            // b) blocked algorithm, see algorithm 5 from  'A storage efficient WY
            //    representation for products of Householder transformations',
            //    by R. Schreiber and C. Van Loan.
            if (blockstart + blocksize <= n - 1)
            {
                if (n - blockstart - blocksize >= 2 * ts || rowscount >= 4 * ts)
                {
                    // Prepare block reflector
                    rmatrixblockreflector(ref tmpa, ref taubuf, true, rowscount, blocksize, ref tmpt, ref work);

                    // Multiply the rest of A by Q'.
                    //
                    // Q  = E + Y*T*Y'  = E + TmpA*TmpT*TmpA'
                    // Q' = E + Y*T'*Y' = E + TmpA*TmpT'*TmpA'
                    rmatrixgemm(blocksize, n - blockstart - blocksize, rowscount, 1.0, tmpa, 0, 0, 1, a, blockstart, blockstart + blocksize, 0, 0.0, tmpr, 0, 0);
                    rmatrixgemm(blocksize, n - blockstart - blocksize, blocksize, 1.0, tmpt, 0, 0, 1, tmpr, 0, 0, 0, 0.0, tmpr, blocksize, 0);
                    rmatrixgemm(rowscount, n - blockstart - blocksize, blocksize, 1.0, tmpa, 0, 0, 0, tmpr, blocksize, 0, 0, 1.0, a, blockstart, blockstart + blocksize);
                }
                else // Level 2 algorithm
                    for (var i = 0; i < blocksize; i++)
                    {
                        i1_ = i - 1;
                        for (i_ = 1; i_ <= rowscount - i; i_++)
                            t[i_] = tmpa[i_ + i1_, i];
                        t[1] = 1;

                        applyreflectionfromtheleft(a, taubuf[i], t, blockstart + i, m - 1, blockstart + blocksize, n - 1, ref work);
                    }
            }

            // Advance
            blockstart += blocksize;
        }
    }

    private static void rmatrixblockreflector(ref double[,] a,
            ref double[] tau,
            bool columnwisea,
            int lengtha,
            int blocksize,
            ref double[,] t,
            ref double[] work)
    {
        // fill beginning of new column with zeros,
        // load 1.0 in the first non-zero element
        int i, j, k;
        for (k = 0; k < blocksize; k++)
        {
            if (columnwisea)
                for (i = 0; i < k; i++)
                    a[i, k] = 0;
            else
                for (i = 0; i < k; i++)
                    a[k, i] = 0;

            a[k, k] = 1;
        }

        // Calculate Gram matrix of A
        for (i = 0; i < blocksize; i++)
            for (j = 0; j < blocksize; j++)
                t[i, blocksize + j] = 0;

        int i_, i1_;
        double v;
        for (k = 0; k < lengtha; k++)
            for (j = 1; j < blocksize; j++)
                if (columnwisea)
                {
                    v = a[k, j];
                    if (v == 0) continue;

                    i1_ = 0 - blocksize;
                    for (i_ = blocksize; i_ < blocksize + j; i_++)
                        t[j, i_] += v * a[k, i_ + i1_];
                }
                else
                {
                    v = a[j, k];
                    if (v == 0) continue;

                    i1_ = 0 - blocksize;
                    for (i_ = blocksize; i_ < blocksize + j; i_++)
                        t[j, i_] += v * a[i_ + i1_, k];
                }

        // Prepare Y (stored in TmpA) and T (stored in TmpT)
        for (k = 0; k < blocksize; k++)
        {
            // fill non-zero part of T, use pre-calculated Gram matrix
            i1_ = blocksize - 0;
            for (i_ = 0; i_ < k; i_++) 
                work[i_] = t[k, i_ + i1_];

            for (i = 0; i < k; i++)
            {
                v = 0.0;
                for (i_ = i; i_ < k; i_++) 
                    v += t[i, i_] * work[i_];
                t[i, k] = -(tau[k] * v);
            }

            t[k, k] = -tau[k];

            // Rest of T is filled by zeros
            for (i = k + 1; i < blocksize; i++) 
                t[i, k] = 0;
        }
    }

    public static void rmatrixgemm(int m,
        int n,
        int k,
        double alpha,
        double[,] a,
        int ia,
        int ja,
        int optypea,
        double[,] b,
        int ib,
        int jb,
        int optypeb,
        double beta,
        double[,] c,
        int ic,
        int jc)
    {
        //alglib.ap.assert(optypea == 0 || optypea == 1, "RMatrixGEMM: incorrect OpTypeA (must be 0 or 1)");
        //alglib.ap.assert(optypeb == 0 || optypeb == 1, "RMatrixGEMM: incorrect OpTypeB (must be 0 or 1)");
        //alglib.ap.assert(ic + m <= alglib.ap.rows(c), "RMatrixGEMM: incorect size of output matrix C");
        //alglib.ap.assert(jc + n <= alglib.ap.cols(c), "RMatrixGEMM: incorect size of output matrix C");

        rmatrixgemmrec(m, n, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc);
    }

    private static void rmatrixgemmrec(int m,
            int n,
            int k,
            double alpha,
            double[,] a,
            int ia,
            int ja,
            int optypea,
            double[,] b,
            int ib,
            int jb,
            int optypeb,
            double beta,
            double[,] c,
            int ic,
            int jc)
    {
        var s1 = 0;
        var s2 = 0;

        var tsa = matrixtilesizea();
        var tsb = matrixtilesizeb();

        var tscur = tsb;
        if (imax3(m, n, k) <= tsb) tscur = tsa;

        //alglib.ap.assert(tscur >= 1, "RMatrixGEMMRec: integrity check failed");

        if (m <= tsa && n <= tsa && k <= tsa)
        {
            rmatrixgemmk(m, n, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc);
            return;
        }

        // Recursive algorithm: split on M or N
        if (m >= n && m >= k)
        {
            // A*B = (A1 A2)^T*B
            tiledsplit(m, tscur, ref s1, ref s2);
            if (optypea == 0)
            {
                rmatrixgemmrec(s2, n, k, alpha, a, ia + s1, ja, optypea, b, ib, jb, optypeb, beta, c, ic + s1, jc);
                rmatrixgemmrec(s1, n, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc);
            }
            else
            {
                rmatrixgemmrec(s2, n, k, alpha, a, ia, ja + s1, optypea, b, ib, jb, optypeb, beta, c, ic + s1, jc);
                rmatrixgemmrec(s1, n, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc);
            }
            return;
        }
        if (n >= m && n >= k)
        {
            // A*B = A*(B1 B2)
            tiledsplit(n, tscur, ref s1, ref s2);
            if (optypeb == 0)
            {
                rmatrixgemmrec(m, s2, k, alpha, a, ia, ja, optypea, b, ib, jb + s1, optypeb, beta, c, ic, jc + s1);
                rmatrixgemmrec(m, s1, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc);
            }
            else
            {
                rmatrixgemmrec(m, s2, k, alpha, a, ia, ja, optypea, b, ib + s1, jb, optypeb, beta, c, ic, jc + s1);
                rmatrixgemmrec(m, s1, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc);
            }
            return;
        }

        // Recursive algorithm: split on K

        // A*B = (A1 A2)*(B1 B2)^T
        tiledsplit(k, tscur, ref s1, ref s2);
        if (optypea == 0 && optypeb == 0)
        {
            rmatrixgemmrec(m, n, s1, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc);
            rmatrixgemmrec(m, n, s2, alpha, a, ia, ja + s1, optypea, b, ib + s1, jb, optypeb, 1.0, c, ic, jc);
        }
        if (optypea == 0 && optypeb != 0)
        {
            rmatrixgemmrec(m, n, s1, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc);
            rmatrixgemmrec(m, n, s2, alpha, a, ia, ja + s1, optypea, b, ib, jb + s1, optypeb, 1.0, c, ic, jc);
        }
        if (optypea != 0 && optypeb == 0)
        {
            rmatrixgemmrec(m, n, s1, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc);
            rmatrixgemmrec(m, n, s2, alpha, a, ia + s1, ja, optypea, b, ib + s1, jb, optypeb, 1.0, c, ic, jc);
        }
        if (optypea != 0 && optypeb != 0)
        {
            rmatrixgemmrec(m, n, s1, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc);
            rmatrixgemmrec(m, n, s2, alpha, a, ia + s1, ja, optypea, b, ib, jb + s1, optypeb, 1.0, c, ic, jc);
        }
    }

    public static void tiledsplit(int tasksize, int tilesize, ref int task0, ref int task1)
    {
        //task0 = 0;
        //task1 = 0;

        //alglib.ap.assert(tasksize >= 2, "TiledSplit: TaskSize<2");
        //alglib.ap.assert(tasksize > tilesize, "TiledSplit: TaskSize<=TileSize");
        var cc = chunkscount(tasksize, tilesize);
        //alglib.ap.assert(cc >= 2, "TiledSplit: integrity check failed");
        task0 = idivup(cc, 2) * tilesize;
        task1 = tasksize - task0;
        //alglib.ap.assert(task0 >= 1, "TiledSplit: internal error");
        //alglib.ap.assert(task1 >= 1, "TiledSplit: internal error");
        //alglib.ap.assert(task0 % tilesize == 0, "TiledSplit: internal error");
        //alglib.ap.assert(task0 >= task1, "TiledSplit: internal error");
    }

    private static int chunkscount(int tasksize, int chunksize)
    {
        //alglib.ap.assert(tasksize >= 0, "ChunksCount: TaskSize<0");
        //alglib.ap.assert(chunksize >= 1, "ChunksCount: ChunkSize<1");
        return tasksize % chunksize != 0 ? tasksize / chunksize + 1 : tasksize / chunksize;
    }

    public static int idivup(int a, int b) => a % b > 0 ? a / b + 1 : a / b;

    public static void rmatrixgemmk(int m,
            int n,
            int k,
            double alpha,
            double[,] a,
            int ia,
            int ja,
            int optypea,
            double[,] b,
            int ib,
            int jb,
            int optypeb,
            double beta,
            double[,] c,
            int ic,
            int jc)
    {
        // if matrix size is zero
        if (m == 0 || n == 0)
            return;

        // if K=0 or Alpha=0, then C=Beta*C
        if (k == 0 || alpha == 0)
        {
            if (beta == 1) return;

            int i, j;
            if (beta != 0)
                for (i = 0; i <= m - 1; i++)
                    for (j = 0; j <= n - 1; j++) 
                        c[ic + i, jc + j] *= beta;
            else
                for (i = 0; i <= m - 1; i++)
                    for (j = 0; j <= n - 1; j++) 
                        c[ic + i, jc + j] = 0;

            return;
        }

        // Call specialized code.
        //
        // NOTE: specialized code was moved to separate function because of strange
        //       issues with instructions cache on some systems; Having too long
        //       functions significantly slows down internal loop of the algorithm.
        if (optypea == 0 && optypeb == 0)
            rmatrixgemmk44v00(m, n, k, alpha, a, ia, ja, b, ib, jb, beta, c, ic, jc);
        if (optypea == 0 && optypeb != 0)
            rmatrixgemmk44v01(m, n, k, alpha, a, ia, ja, b, ib, jb, beta, c, ic, jc);
        if (optypea != 0 && optypeb == 0)
            rmatrixgemmk44v10(m, n, k, alpha, a, ia, ja, b, ib, jb, beta, c, ic, jc);
        if (optypea != 0 && optypeb != 0)
            rmatrixgemmk44v11(m, n, k, alpha, a, ia, ja, b, ib, jb, beta, c, ic, jc);
    }

    public static void rmatrixgemmk44v00(int m,
            int n,
            int k,
            double alpha,
            double[,] a,
            int ia,
            int ja,
            double[,] b,
            int ib,
            int jb,
            double beta,
            double[,] c,
            int ic,
            int jc)
    {
        //alglib.ap.assert(alpha != 0, "RMatrixGEMMK44V00: internal error (Alpha=0)");

        // if matrix size is zero
        if (m == 0 || n == 0) return;

        // A*B
        var i = 0;
        while (i < m)
        {
            var j = 0;
            while (j < n)
            {
                // Choose between specialized 4x4 code and general code
                if (i + 4 <= m && j + 4 <= n)
                {
                    // Specialized 4x4 code for [I..I+3]x[J..J+3] submatrix of C.
                    //
                    // This submatrix is calculated as sum of K rank-1 products,
                    // with operands cached in local variables in order to speed
                    // up operations with arrays.
                    var idxa0 = ia + i + 0;
                    var idxa1 = ia + i + 1;
                    var idxa2 = ia + i + 2;
                    var idxa3 = ia + i + 3;
                    var offsa = ja;
                    var idxb0 = jb + j + 0;
                    var idxb1 = jb + j + 1;
                    var idxb2 = jb + j + 2;
                    var idxb3 = jb + j + 3;
                    var offsb = ib;
                    var v00 = 0.0;
                    var v01 = 0.0;
                    var v02 = 0.0;
                    var v03 = 0.0;
                    var v10 = 0.0;
                    var v11 = 0.0;
                    var v12 = 0.0;
                    var v13 = 0.0;
                    var v20 = 0.0;
                    var v21 = 0.0;
                    var v22 = 0.0;
                    var v23 = 0.0;
                    var v30 = 0.0;
                    var v31 = 0.0;
                    var v32 = 0.0;
                    var v33 = 0.0;

                    // Different variants of internal loop
                    for (var t = 0; t <= k - 1; t++)
                    {
                        var a0 = a[idxa0, offsa];
                        var a1 = a[idxa1, offsa];
                        var b0 = b[offsb, idxb0];
                        var b1 = b[offsb, idxb1];
                        v00 += a0 * b0;
                        v01 += a0 * b1;
                        v10 += a1 * b0;
                        v11 += a1 * b1;
                        var a2 = a[idxa2, offsa];
                        var a3 = a[idxa3, offsa];
                        v20 += a2 * b0;
                        v21 += a2 * b1;
                        v30 += a3 * b0;
                        v31 += a3 * b1;
                        var b2 = b[offsb, idxb2];
                        var b3 = b[offsb, idxb3];
                        v22 += a2 * b2;
                        v23 += a2 * b3;
                        v32 += a3 * b2;
                        v33 += a3 * b3;
                        v02 += a0 * b2;
                        v03 += a0 * b3;
                        v12 += a1 * b2;
                        v13 += a1 * b3;
                        offsa += 1;
                        offsb += 1;
                    }
                    if (beta == 0)
                    {
                        c[ic + i + 0, jc + j + 0] = alpha * v00;
                        c[ic + i + 0, jc + j + 1] = alpha * v01;
                        c[ic + i + 0, jc + j + 2] = alpha * v02;
                        c[ic + i + 0, jc + j + 3] = alpha * v03;
                        c[ic + i + 1, jc + j + 0] = alpha * v10;
                        c[ic + i + 1, jc + j + 1] = alpha * v11;
                        c[ic + i + 1, jc + j + 2] = alpha * v12;
                        c[ic + i + 1, jc + j + 3] = alpha * v13;
                        c[ic + i + 2, jc + j + 0] = alpha * v20;
                        c[ic + i + 2, jc + j + 1] = alpha * v21;
                        c[ic + i + 2, jc + j + 2] = alpha * v22;
                        c[ic + i + 2, jc + j + 3] = alpha * v23;
                        c[ic + i + 3, jc + j + 0] = alpha * v30;
                        c[ic + i + 3, jc + j + 1] = alpha * v31;
                        c[ic + i + 3, jc + j + 2] = alpha * v32;
                        c[ic + i + 3, jc + j + 3] = alpha * v33;
                    }
                    else
                    {
                        c[ic + i + 0, jc + j + 0] = beta * c[ic + i + 0, jc + j + 0] + alpha * v00;
                        c[ic + i + 0, jc + j + 1] = beta * c[ic + i + 0, jc + j + 1] + alpha * v01;
                        c[ic + i + 0, jc + j + 2] = beta * c[ic + i + 0, jc + j + 2] + alpha * v02;
                        c[ic + i + 0, jc + j + 3] = beta * c[ic + i + 0, jc + j + 3] + alpha * v03;
                        c[ic + i + 1, jc + j + 0] = beta * c[ic + i + 1, jc + j + 0] + alpha * v10;
                        c[ic + i + 1, jc + j + 1] = beta * c[ic + i + 1, jc + j + 1] + alpha * v11;
                        c[ic + i + 1, jc + j + 2] = beta * c[ic + i + 1, jc + j + 2] + alpha * v12;
                        c[ic + i + 1, jc + j + 3] = beta * c[ic + i + 1, jc + j + 3] + alpha * v13;
                        c[ic + i + 2, jc + j + 0] = beta * c[ic + i + 2, jc + j + 0] + alpha * v20;
                        c[ic + i + 2, jc + j + 1] = beta * c[ic + i + 2, jc + j + 1] + alpha * v21;
                        c[ic + i + 2, jc + j + 2] = beta * c[ic + i + 2, jc + j + 2] + alpha * v22;
                        c[ic + i + 2, jc + j + 3] = beta * c[ic + i + 2, jc + j + 3] + alpha * v23;
                        c[ic + i + 3, jc + j + 0] = beta * c[ic + i + 3, jc + j + 0] + alpha * v30;
                        c[ic + i + 3, jc + j + 1] = beta * c[ic + i + 3, jc + j + 1] + alpha * v31;
                        c[ic + i + 3, jc + j + 2] = beta * c[ic + i + 3, jc + j + 2] + alpha * v32;
                        c[ic + i + 3, jc + j + 3] = beta * c[ic + i + 3, jc + j + 3] + alpha * v33;
                    }
                }
                else
                {
                    // Determine submatrix [I0..I1]x[J0..J1] to process
                    var i0 = i;
                    var i1 = Math.Min(i + 3, m - 1);
                    var j0 = j;
                    var j1 = Math.Min(j + 3, n - 1);

                    // Process submatrix
                    for (var ik = i0; ik <= i1; ik++)
                        for (var jk = j0; jk <= j1; jk++)
                        {
                            double v;
                            if (k == 0 || alpha == 0)
                                v = 0;
                            else
                            {
                                var i1_ = ib - ja;
                                v = 0.0;
                                for (var i_ = ja; i_ <= ja + k - 1; i_++) 
                                    v += a[ia + ik, i_] * b[i_ + i1_, jb + jk];
                            }
                            if (beta == 0)
                                c[ic + ik, jc + jk] = alpha * v;
                            else
                                c[ic + ik, jc + jk] = beta * c[ic + ik, jc + jk] + alpha * v;
                        }
                }
                j += 4;
            }
            i += 4;
        }
    }

    public static void rmatrixgemmk44v01(int m,
        int n,
        int k,
        double alpha,
        double[,] a,
        int ia,
        int ja,
        double[,] b,
        int ib,
        int jb,
        double beta,
        double[,] c,
        int ic,
        int jc)
    {
        //alglib.ap.assert(alpha != 0, "RMatrixGEMMK44V00: internal error (Alpha=0)");

        // if matrix size is zero
        if (m == 0 || n == 0) 
            return;

        // A*B'
        var i = 0;
        while (i < m)
        {
            var j = 0;
            while (j < n)
            {
                // Choose between specialized 4x4 code and general code
                if (i + 4 <= m && j + 4 <= n)
                {
                    // Specialized 4x4 code for [I..I+3]x[J..J+3] submatrix of C.
                    //
                    // This submatrix is calculated as sum of K rank-1 products,
                    // with operands cached in local variables in order to speed
                    // up operations with arrays.
                    var idxa0 = ia + i + 0;
                    var idxa1 = ia + i + 1;
                    var idxa2 = ia + i + 2;
                    var idxa3 = ia + i + 3;
                    var offsa = ja;
                    var idxb0 = ib + j + 0;
                    var idxb1 = ib + j + 1;
                    var idxb2 = ib + j + 2;
                    var idxb3 = ib + j + 3;
                    var offsb = jb;
                    var v00 = 0.0;
                    var v01 = 0.0;
                    var v02 = 0.0;
                    var v03 = 0.0;
                    var v10 = 0.0;
                    var v11 = 0.0;
                    var v12 = 0.0;
                    var v13 = 0.0;
                    var v20 = 0.0;
                    var v21 = 0.0;
                    var v22 = 0.0;
                    var v23 = 0.0;
                    var v30 = 0.0;
                    var v31 = 0.0;
                    var v32 = 0.0;
                    var v33 = 0.0;
                    for (var t = 0; t <= k - 1; t++)
                    {
                        var a0 = a[idxa0, offsa];
                        var a1 = a[idxa1, offsa];
                        var b0 = b[idxb0, offsb];
                        var b1 = b[idxb1, offsb];
                        v00 += a0 * b0;
                        v01 += a0 * b1;
                        v10 += a1 * b0;
                        v11 += a1 * b1;
                        var a2 = a[idxa2, offsa];
                        var a3 = a[idxa3, offsa];
                        v20 += a2 * b0;
                        v21 += a2 * b1;
                        v30 += a3 * b0;
                        v31 += a3 * b1;
                        var b2 = b[idxb2, offsb];
                        var b3 = b[idxb3, offsb];
                        v22 += a2 * b2;
                        v23 += a2 * b3;
                        v32 += a3 * b2;
                        v33 += a3 * b3;
                        v02 += a0 * b2;
                        v03 += a0 * b3;
                        v12 += a1 * b2;
                        v13 += a1 * b3;
                        offsa += 1;
                        offsb += 1;
                    }
                    if (beta == 0)
                    {
                        c[ic + i + 0, jc + j + 0] = alpha * v00;
                        c[ic + i + 0, jc + j + 1] = alpha * v01;
                        c[ic + i + 0, jc + j + 2] = alpha * v02;
                        c[ic + i + 0, jc + j + 3] = alpha * v03;
                        c[ic + i + 1, jc + j + 0] = alpha * v10;
                        c[ic + i + 1, jc + j + 1] = alpha * v11;
                        c[ic + i + 1, jc + j + 2] = alpha * v12;
                        c[ic + i + 1, jc + j + 3] = alpha * v13;
                        c[ic + i + 2, jc + j + 0] = alpha * v20;
                        c[ic + i + 2, jc + j + 1] = alpha * v21;
                        c[ic + i + 2, jc + j + 2] = alpha * v22;
                        c[ic + i + 2, jc + j + 3] = alpha * v23;
                        c[ic + i + 3, jc + j + 0] = alpha * v30;
                        c[ic + i + 3, jc + j + 1] = alpha * v31;
                        c[ic + i + 3, jc + j + 2] = alpha * v32;
                        c[ic + i + 3, jc + j + 3] = alpha * v33;
                    }
                    else
                    {
                        c[ic + i + 0, jc + j + 0] = beta * c[ic + i + 0, jc + j + 0] + alpha * v00;
                        c[ic + i + 0, jc + j + 1] = beta * c[ic + i + 0, jc + j + 1] + alpha * v01;
                        c[ic + i + 0, jc + j + 2] = beta * c[ic + i + 0, jc + j + 2] + alpha * v02;
                        c[ic + i + 0, jc + j + 3] = beta * c[ic + i + 0, jc + j + 3] + alpha * v03;
                        c[ic + i + 1, jc + j + 0] = beta * c[ic + i + 1, jc + j + 0] + alpha * v10;
                        c[ic + i + 1, jc + j + 1] = beta * c[ic + i + 1, jc + j + 1] + alpha * v11;
                        c[ic + i + 1, jc + j + 2] = beta * c[ic + i + 1, jc + j + 2] + alpha * v12;
                        c[ic + i + 1, jc + j + 3] = beta * c[ic + i + 1, jc + j + 3] + alpha * v13;
                        c[ic + i + 2, jc + j + 0] = beta * c[ic + i + 2, jc + j + 0] + alpha * v20;
                        c[ic + i + 2, jc + j + 1] = beta * c[ic + i + 2, jc + j + 1] + alpha * v21;
                        c[ic + i + 2, jc + j + 2] = beta * c[ic + i + 2, jc + j + 2] + alpha * v22;
                        c[ic + i + 2, jc + j + 3] = beta * c[ic + i + 2, jc + j + 3] + alpha * v23;
                        c[ic + i + 3, jc + j + 0] = beta * c[ic + i + 3, jc + j + 0] + alpha * v30;
                        c[ic + i + 3, jc + j + 1] = beta * c[ic + i + 3, jc + j + 1] + alpha * v31;
                        c[ic + i + 3, jc + j + 2] = beta * c[ic + i + 3, jc + j + 2] + alpha * v32;
                        c[ic + i + 3, jc + j + 3] = beta * c[ic + i + 3, jc + j + 3] + alpha * v33;
                    }
                }
                else
                {
                    //
                    // Determine submatrix [I0..I1]x[J0..J1] to process
                    //
                    var i0 = i;
                    var i1 = Math.Min(i + 3, m - 1);
                    var j0 = j;
                    var j1 = Math.Min(j + 3, n - 1);

                    //
                    // Process submatrix
                    //
                    for (var ik = i0; ik <= i1; ik++)
                        for (var jk = j0; jk <= j1; jk++)
                        {
                            double v;
                            if (k == 0 || alpha == 0)
                                v = 0;
                            else
                            {
                                var i1_ = jb - ja;
                                v = 0.0;
                                for (var i_ = ja; i_ <= ja + k - 1; i_++) v += a[ia + ik, i_] * b[ib + jk, i_ + i1_];
                            }
                            if (beta == 0)
                                c[ic + ik, jc + jk] = alpha * v;
                            else
                                c[ic + ik, jc + jk] = beta * c[ic + ik, jc + jk] + alpha * v;
                        }
                }
                j += 4;
            }
            i += 4;
        }
    }

    public static void rmatrixgemmk44v10(int m,
        int n,
        int k,
        double alpha,
        double[,] a,
        int ia,
        int ja,
        double[,] b,
        int ib,
        int jb,
        double beta,
        double[,] c,
        int ic,
        int jc)
    {
        //alglib.ap.assert(alpha != 0, "RMatrixGEMMK44V00: internal error (Alpha=0)");

        // if matrix size is zero
        if (m == 0 || n == 0) 
            return;

        // A'*B
        var i = 0;
        while (i < m)
        {
            var j = 0;
            while (j < n)
            {
                // Choose between specialized 4x4 code and general code
                if (i + 4 <= m && j + 4 <= n)
                {
                    // Specialized 4x4 code for [I..I+3]x[J..J+3] submatrix of C.
                    //
                    // This submatrix is calculated as sum of K rank-1 products,
                    // with operands cached in local variables in order to speed
                    // up operations with arrays.
                    var idxa0 = ja + i + 0;
                    var idxa1 = ja + i + 1;
                    var idxa2 = ja + i + 2;
                    var idxa3 = ja + i + 3;

                    var offsa = ia;
                    var idxb0 = jb + j + 0;
                    var idxb1 = jb + j + 1;
                    var idxb2 = jb + j + 2;
                    var idxb3 = jb + j + 3;

                    var offsb = ib;
                    var v00 = 0.0;
                    var v01 = 0.0;
                    var v02 = 0.0;
                    var v03 = 0.0;
                    var v10 = 0.0;
                    var v11 = 0.0;
                    var v12 = 0.0;
                    var v13 = 0.0;
                    var v20 = 0.0;
                    var v21 = 0.0;
                    var v22 = 0.0;
                    var v23 = 0.0;
                    var v30 = 0.0;
                    var v31 = 0.0;
                    var v32 = 0.0;
                    var v33 = 0.0;
                    for (var t = 0; t <= k - 1; t++)
                    {
                        var a0 = a[offsa, idxa0];
                        var a1 = a[offsa, idxa1];
                        var b0 = b[offsb, idxb0];
                        var b1 = b[offsb, idxb1];
                        v00 += a0 * b0;
                        v01 += a0 * b1;
                        v10 += a1 * b0;
                        v11 += a1 * b1;
                        var a2 = a[offsa, idxa2];
                        var a3 = a[offsa, idxa3];
                        v20 += a2 * b0;
                        v21 += a2 * b1;
                        v30 += a3 * b0;
                        v31 += a3 * b1;
                        var b2 = b[offsb, idxb2];
                        var b3 = b[offsb, idxb3];
                        v22 += a2 * b2;
                        v23 += a2 * b3;
                        v32 += a3 * b2;
                        v33 += a3 * b3;
                        v02 += a0 * b2;
                        v03 += a0 * b3;
                        v12 += a1 * b2;
                        v13 += a1 * b3;
                        offsa += 1;
                        offsb += 1;
                    }
                    if (beta == 0)
                    {
                        c[ic + i + 0, jc + j + 0] = alpha * v00;
                        c[ic + i + 0, jc + j + 1] = alpha * v01;
                        c[ic + i + 0, jc + j + 2] = alpha * v02;
                        c[ic + i + 0, jc + j + 3] = alpha * v03;
                        c[ic + i + 1, jc + j + 0] = alpha * v10;
                        c[ic + i + 1, jc + j + 1] = alpha * v11;
                        c[ic + i + 1, jc + j + 2] = alpha * v12;
                        c[ic + i + 1, jc + j + 3] = alpha * v13;
                        c[ic + i + 2, jc + j + 0] = alpha * v20;
                        c[ic + i + 2, jc + j + 1] = alpha * v21;
                        c[ic + i + 2, jc + j + 2] = alpha * v22;
                        c[ic + i + 2, jc + j + 3] = alpha * v23;
                        c[ic + i + 3, jc + j + 0] = alpha * v30;
                        c[ic + i + 3, jc + j + 1] = alpha * v31;
                        c[ic + i + 3, jc + j + 2] = alpha * v32;
                        c[ic + i + 3, jc + j + 3] = alpha * v33;
                    }
                    else
                    {
                        c[ic + i + 0, jc + j + 0] = beta * c[ic + i + 0, jc + j + 0] + alpha * v00;
                        c[ic + i + 0, jc + j + 1] = beta * c[ic + i + 0, jc + j + 1] + alpha * v01;
                        c[ic + i + 0, jc + j + 2] = beta * c[ic + i + 0, jc + j + 2] + alpha * v02;
                        c[ic + i + 0, jc + j + 3] = beta * c[ic + i + 0, jc + j + 3] + alpha * v03;
                        c[ic + i + 1, jc + j + 0] = beta * c[ic + i + 1, jc + j + 0] + alpha * v10;
                        c[ic + i + 1, jc + j + 1] = beta * c[ic + i + 1, jc + j + 1] + alpha * v11;
                        c[ic + i + 1, jc + j + 2] = beta * c[ic + i + 1, jc + j + 2] + alpha * v12;
                        c[ic + i + 1, jc + j + 3] = beta * c[ic + i + 1, jc + j + 3] + alpha * v13;
                        c[ic + i + 2, jc + j + 0] = beta * c[ic + i + 2, jc + j + 0] + alpha * v20;
                        c[ic + i + 2, jc + j + 1] = beta * c[ic + i + 2, jc + j + 1] + alpha * v21;
                        c[ic + i + 2, jc + j + 2] = beta * c[ic + i + 2, jc + j + 2] + alpha * v22;
                        c[ic + i + 2, jc + j + 3] = beta * c[ic + i + 2, jc + j + 3] + alpha * v23;
                        c[ic + i + 3, jc + j + 0] = beta * c[ic + i + 3, jc + j + 0] + alpha * v30;
                        c[ic + i + 3, jc + j + 1] = beta * c[ic + i + 3, jc + j + 1] + alpha * v31;
                        c[ic + i + 3, jc + j + 2] = beta * c[ic + i + 3, jc + j + 2] + alpha * v32;
                        c[ic + i + 3, jc + j + 3] = beta * c[ic + i + 3, jc + j + 3] + alpha * v33;
                    }
                }
                else
                {
                    // Determine submatrix [I0..I1]x[J0..J1] to process
                    var i0 = i;
                    var i1 = Math.Min(i + 3, m - 1);
                    var j0 = j;
                    var j1 = Math.Min(j + 3, n - 1);

                    // Process submatrix
                    for (var ik = i0; ik <= i1; ik++)
                        for (var jk = j0; jk <= j1; jk++)
                        {
                            double v;
                            if (k == 0 || alpha == 0)
                                v = 0;
                            else
                            {
                                v = 0.0;
                                var i1_ = ib - ia;
                                v = 0.0;
                                for (var i_ = ia; i_ <= ia + k - 1; i_++) 
                                    v += a[i_, ja + ik] * b[i_ + i1_, jb + jk];
                            }
                            if (beta == 0)
                                c[ic + ik, jc + jk] = alpha * v;
                            else
                                c[ic + ik, jc + jk] = beta * c[ic + ik, jc + jk] + alpha * v;
                        }
                }
                j += 4;
            }
            i += 4;
        }
    }

    public static void rmatrixgemmk44v11(int m,
        int n,
        int k,
        double alpha,
        double[,] a,
        int ia,
        int ja,
        double[,] b,
        int ib,
        int jb,
        double beta,
        double[,] c,
        int ic,
        int jc)
    {
        //alglib.ap.assert(alpha != 0, "RMatrixGEMMK44V00: internal error (Alpha=0)");

        // if matrix size is zero
        if (m == 0 || n == 0) 
            return;

        // A'*B'
        var i = 0;
        while (i < m)
        {
            var j = 0;
            while (j < n)
            {
                // Choose between specialized 4x4 code and general code
                if (i + 4 <= m && j + 4 <= n)
                {
                    // Specialized 4x4 code for [I..I+3]x[J..J+3] submatrix of C.
                    //
                    // This submatrix is calculated as sum of K rank-1 products,
                    // with operands cached in local variables in order to speed
                    // up operations with arrays.
                    var idxa0 = ja + i + 0;
                    var idxa1 = ja + i + 1;
                    var idxa2 = ja + i + 2;
                    var idxa3 = ja + i + 3;

                    var offsa = ia;
                    var idxb0 = ib + j + 0;
                    var idxb1 = ib + j + 1;
                    var idxb2 = ib + j + 2;
                    var idxb3 = ib + j + 3;

                    var offsb = jb;
                    var v00 = 0.0;
                    var v01 = 0.0;
                    var v02 = 0.0;
                    var v03 = 0.0;
                    var v10 = 0.0;
                    var v11 = 0.0;
                    var v12 = 0.0;
                    var v13 = 0.0;
                    var v20 = 0.0;
                    var v21 = 0.0;
                    var v22 = 0.0;
                    var v23 = 0.0;
                    var v30 = 0.0;
                    var v31 = 0.0;
                    var v32 = 0.0;
                    var v33 = 0.0;
                    for (var t = 0; t <= k - 1; t++)
                    {
                        var a0 = a[offsa, idxa0];
                        var a1 = a[offsa, idxa1];
                        var b0 = b[idxb0, offsb];
                        var b1 = b[idxb1, offsb];
                        v00 += a0 * b0;
                        v01 += a0 * b1;
                        v10 += a1 * b0;
                        v11 += a1 * b1;
                        var a2 = a[offsa, idxa2];
                        var a3 = a[offsa, idxa3];
                        v20 += a2 * b0;
                        v21 += a2 * b1;
                        v30 += a3 * b0;
                        v31 += a3 * b1;
                        var b2 = b[idxb2, offsb];
                        var b3 = b[idxb3, offsb];
                        v22 += a2 * b2;
                        v23 += a2 * b3;
                        v32 += a3 * b2;
                        v33 += a3 * b3;
                        v02 += a0 * b2;
                        v03 += a0 * b3;
                        v12 += a1 * b2;
                        v13 += a1 * b3;
                        offsa += 1;
                        offsb += 1;
                    }

                    if (beta == 0)
                    {
                        c[ic + i + 0, jc + j + 0] = alpha * v00;
                        c[ic + i + 0, jc + j + 1] = alpha * v01;
                        c[ic + i + 0, jc + j + 2] = alpha * v02;
                        c[ic + i + 0, jc + j + 3] = alpha * v03;
                        c[ic + i + 1, jc + j + 0] = alpha * v10;
                        c[ic + i + 1, jc + j + 1] = alpha * v11;
                        c[ic + i + 1, jc + j + 2] = alpha * v12;
                        c[ic + i + 1, jc + j + 3] = alpha * v13;
                        c[ic + i + 2, jc + j + 0] = alpha * v20;
                        c[ic + i + 2, jc + j + 1] = alpha * v21;
                        c[ic + i + 2, jc + j + 2] = alpha * v22;
                        c[ic + i + 2, jc + j + 3] = alpha * v23;
                        c[ic + i + 3, jc + j + 0] = alpha * v30;
                        c[ic + i + 3, jc + j + 1] = alpha * v31;
                        c[ic + i + 3, jc + j + 2] = alpha * v32;
                        c[ic + i + 3, jc + j + 3] = alpha * v33;
                    }
                    else
                    {
                        c[ic + i + 0, jc + j + 0] = beta * c[ic + i + 0, jc + j + 0] + alpha * v00;
                        c[ic + i + 0, jc + j + 1] = beta * c[ic + i + 0, jc + j + 1] + alpha * v01;
                        c[ic + i + 0, jc + j + 2] = beta * c[ic + i + 0, jc + j + 2] + alpha * v02;
                        c[ic + i + 0, jc + j + 3] = beta * c[ic + i + 0, jc + j + 3] + alpha * v03;
                        c[ic + i + 1, jc + j + 0] = beta * c[ic + i + 1, jc + j + 0] + alpha * v10;
                        c[ic + i + 1, jc + j + 1] = beta * c[ic + i + 1, jc + j + 1] + alpha * v11;
                        c[ic + i + 1, jc + j + 2] = beta * c[ic + i + 1, jc + j + 2] + alpha * v12;
                        c[ic + i + 1, jc + j + 3] = beta * c[ic + i + 1, jc + j + 3] + alpha * v13;
                        c[ic + i + 2, jc + j + 0] = beta * c[ic + i + 2, jc + j + 0] + alpha * v20;
                        c[ic + i + 2, jc + j + 1] = beta * c[ic + i + 2, jc + j + 1] + alpha * v21;
                        c[ic + i + 2, jc + j + 2] = beta * c[ic + i + 2, jc + j + 2] + alpha * v22;
                        c[ic + i + 2, jc + j + 3] = beta * c[ic + i + 2, jc + j + 3] + alpha * v23;
                        c[ic + i + 3, jc + j + 0] = beta * c[ic + i + 3, jc + j + 0] + alpha * v30;
                        c[ic + i + 3, jc + j + 1] = beta * c[ic + i + 3, jc + j + 1] + alpha * v31;
                        c[ic + i + 3, jc + j + 2] = beta * c[ic + i + 3, jc + j + 2] + alpha * v32;
                        c[ic + i + 3, jc + j + 3] = beta * c[ic + i + 3, jc + j + 3] + alpha * v33;
                    }
                }
                else
                {
                    // Determine submatrix [I0..I1]x[J0..J1] to process
                    var i0 = i;
                    var i1 = Math.Min(i + 3, m - 1);
                    var j0 = j;
                    var j1 = Math.Min(j + 3, n - 1);

                    // Process submatrix
                    for (var ik = i0; ik <= i1; ik++)
                        for (var jk = j0; jk <= j1; jk++)
                        {
                            double v;
                            if (k == 0 || alpha == 0)
                                v = 0;
                            else
                            {
                                var i1_ = jb - ia;
                                v = 0.0;

                                for (var i_ = ia; i_ < ia + k; i_++)
                                    v += a[i_, ja + ik] * b[ib + jk, i_ + i1_];
                            }
                            if (beta == 0)
                                c[ic + ik, jc + jk] = alpha * v;
                            else
                                c[ic + ik, jc + jk] = beta * c[ic + ik, jc + jk] + alpha * v;
                        }   
                }
                j += 4;
            }
            i += 4;
        }
    }

    public static int imax3(int i0, int i1, int i2) => Math.Max(i0, Math.Max(i1, i2));

    private static double rmul3(double v0, double v1, double v2) => v0 * v1 * v2;

    private static double smpactivationlevel()
    {
        var nn = 2d * matrixtilesizeb();
        var result = Math.Max(0.95 * 2 * nn * nn * nn, 1.0E7);
        return result;
    }

    private static int matrixtilesizea() => 32;

    private static int matrixtilesizeb() => 64;

    private static void rmatrixcopy(int m, int n, double[,] a, int ia, int ja, double[,] b, int ib, int jb)
    {
        if (m == 0 || n == 0) return;

        for (var i = 0; i <= m - 1; i++)
        {
            var i1 = ja - jb;

            for (var j = jb; j <= jb + n - 1; j++) 
                b[ib + i, j] = a[ia + i, j + i1];
        }
    }

    private static void rmatrixqrbasecase(ref double[,] a, int m, int n, ref double[] work, ref double[] t, ref double[] tau)
    {
        var minmn = Math.Min(m, n);

        // Test the input arguments
        for (var i = 0; i <= minmn - 1; i++)
        {
            // Generate elementary reflector H(i) to annihilate A(i+1:m,i)
            var j = i - 1;

            int k;
            for (k = 1; k <= m - i; k++)
                t[k] = a[k + j, i];

            generatereflection(ref t, m - i, out var tmp);
            tau[i] = tmp;
            j = 1 - i;

            for (k = i; k <= m - 1; k++)
                a[k, i] = t[k + j];

            t[1] = 1;
            if (i < n) // Apply H(i) to A(i:m-1,i+1:n-1) from the left
                applyreflectionfromtheleft(a, tau[i], t, i, m - 1, i + 1, n - 1, ref work);
        }
    }

    private static void generatereflection(ref double[] x, int n, out double tau)
    {
        tau = 0;

        if (n <= 1) 
            return;

        // Scale if needed (to avoid overflow/underflow during intermediate calculations).
        var mx = 0d;
        int j;
        for (j = 1; j <= n; j++) 
            mx = Math.Max(Math.Abs(x[j]), mx);

        var s = 1d;
        int k;
        double v;
        if (mx != 0)
            switch (mx)
            {
                case <= math.minrealnumber / math.machineepsilon:
                {
                    s = math.minrealnumber / math.machineepsilon;
                    v = 1 / s;
                    for (k = 1; k <= n; k++) 
                        x[k] = v * x[k];
                    mx *= v;
                    break;
                }
                case >= math.maxrealnumber * math.machineepsilon:
                {
                    s = math.maxrealnumber * math.machineepsilon;
                    v = 1 / s;
                    for (k = 1; k <= n; k++) 
                        x[k] = v * x[k];
                    mx *= v;
                    break;
                }
            }

        // XNORM = DNRM2( N-1, X, INCX )
        var alpha = x[1];
        double xnorm = 0;
        if (mx != 0)
        {
            for (j = 2; j <= n; j++) 
                xnorm += math.sqr(x[j] / mx);

            xnorm = Math.Sqrt(xnorm) * mx;
        }

        if (xnorm == 0) // H  =  I
        {
            tau = 0;
            x[1] *= s;
            return;
        }

        // general case
        mx = Math.Max(Math.Abs(alpha), Math.Abs(xnorm));
        var beta = -(mx * Math.Sqrt(math.sqr(alpha / mx) + math.sqr(xnorm / mx)));
        if (alpha < 0)
            beta = -beta;

        tau = (beta - alpha) / beta;
        v = 1 / (alpha - beta);

        for (k = 2; k <= n; k++) 
            x[k] *= v;

        x[1] = beta * s; // Scale back outputs
    }

    private class math
    {
        //public static System.Random RndObject = new System.Random(System.DateTime.Now.Millisecond);
        //public static Random rndobject = new(DateTime.Now.Millisecond + 1000 * DateTime.Now.Second + 60 * 1000 * DateTime.Now.Minute);

        public const double machineepsilon = 5E-16;
        public const double maxrealnumber = 1E300;
        public const double minrealnumber = 1E-300;

        //public static bool isfinite(double d) => !double.IsNaN(d) && !double.IsInfinity(d);

        //public static double randomreal()
        //{
        //    double r;
        //    lock (rndobject) r = rndobject.NextDouble();
        //    return r;
        //}

        //public static int randominteger(int N)
        //{
        //    int r;
        //    lock (rndobject) r = rndobject.Next(N);
        //    return r;
        //}

        public static double sqr(double X) => X * X;

        //public static double abscomplex(complex z)
        //{
        //    double w;
        //    double xabs;
        //    double yabs;
        //    double v;

        //    xabs = Math.Abs(z.x);
        //    yabs = Math.Abs(z.y);
        //    w = xabs > yabs ? xabs : yabs;
        //    v = xabs < yabs ? xabs : yabs;
        //    if (v == 0)
        //        return w;
        //    else
        //    {
        //        var t = v / w;
        //        return w * Math.Sqrt(1 + t * t);
        //    }
        //}

        //public static complex conj(complex z) => new complex(z.x, -z.y);

        //public static complex csqr(complex z) => new complex(z.x * z.x - z.y * z.y, 2 * z.x * z.y);
    }

    private static void applyreflectionfromtheleft(double[,] c, double tau, double[] v, int m1, int m2, int n1, int n2, ref double[] work)
    {
        if (tau == 0 || n1 > n2 || m1 > m2) 
            return;

        rvectorsetlengthatleast(ref work, n2 - n1 + 1);
        rmatrixgemv(n2 - n1 + 1, m2 - m1 + 1, 1.0, c, m1, n1, 1, v, 1, 0.0, work, 0);
        rmatrixger(m2 - m1 + 1, n2 - n1 + 1, c, m1, n1, -tau, v, 1, work, 0);
    }

    public static void applyreflectionfromtheright(double[,] c, double tau, double[] v, int m1, int m2, int n1, int n2, ref double[] work)
    {
        if (tau == 0 || n1 > n2 || m1 > m2)
            return;

        rvectorsetlengthatleast(ref work, m2 - m1 + 1);
        rmatrixgemv(m2 - m1 + 1, n2 - n1 + 1, 1.0, c, m1, n1, 0, v, 1, 0.0, work, 0);
        rmatrixger(m2 - m1 + 1, n2 - n1 + 1, c, m1, n1, -tau, work, 0, v, 1);
    }

    public static void rvectorsetlengthatleast(ref double[] x, int n)
    {
        if (len(x) < n) x = new double[n];
    }

    const int blas2minvendorkernelsize = 8;

    private static void rmatrixger(int m, int n, double[,] a, int ia, int ja, double alpha, double[] u, int iu, double[] v, int iv)
    {
        // Quick exit
        if (m <= 0 || n <= 0) 
            return;

        // Generic code
        for (var i = 0; i <= m - 1; i++)
        {
            var s = alpha * u[iu + i];
            var i1 = iv - ja;
            for (var j = ja; j <= ja + n - 1; j++) 
                a[ia + i, j] += s * v[j + i1];
        }
    }

    private static int len<T>(T[] a) => a.Length;

    private static void rmatrixgemv(int m, int n, double alpha, double[,] a, int ia, int ja, int opa, double[] x, int ix, double beta, double[] y, int iy)
    {
        // Quick exit for M=0, N=0 or Alpha=0.
        // After this block we have M>0, N>0, Alpha<>0.
        if (m <= 0) 
            return;

        if (n <= 0 || alpha == 0.0)
        {
            if (beta != 0)
                rmulvx(m, beta, y, iy);
            else
                rsetvx(m, 0.0, y, iy);

            return;
        }

        if (ia + ja + ix + iy == 0)
            rgemv(m, n, alpha, a, opa, x, beta, y);
        else
            rgemvx(m, n, alpha, a, ia, ja, opa, x, ix, beta, y, iy);
    }

    private static void rgemv(int m, int n, double alpha, double[,] a, int opa, double[] x, double beta, double[] y)
    {
        // Properly premultiply Y by Beta.
        //
        // Quick exit for M=0, N=0 or Alpha=0.
        // After this block we have M>0, N>0, Alpha!=0.
        if (m <= 0) 
            return;

        if (beta != 0)
            rmulv(m, beta, y);
        else
            rsetv(m, 0, y);

        if (n <= 0 || alpha == 0.0) 
            return;

        // Generic code
        int i, j;
        double v;
        if (opa == 0)
        {
            // y += A*x
            for (i = 0, v = 0; i < m; i++, v = 0)
            {
                for (j = 0; j < n; j++)
                    v += a[i, j] * x[j];
                y[i] += alpha * v;
            }

            return;
        }

        if (opa != 1) return;

        // y += A^T*x
        for (i = 0; i < n; i++)
        {
            v = alpha * x[i];
            for (j = 0; j <= m - 1; j++) 
                y[j] += v * a[i, j];
        }
    }

    private static void rmulv(int n, double v, double[] x)
    {
        for (var i = 0; i <= n - 1; i++) 
            x[i] *= v;
    }

    private static void rsetv(int n, double v, double[] x)
    {
        for (var j = 0; j <= n - 1; j++)
            x[j] = v;
    }

    private static void rgemvx(
        int m,
        int n,
        double alpha,
        double[,] a,
        int ia,
        int ja,
        int opa,
        double[] x,
        int ix,
        double beta,
        double[] y,
        int iy)
    {
        // Properly premultiply Y by Beta.
        //
        // Quick exit for M=0, N=0 or Alpha=0.
        // After this block we have M>0, N>0, Alpha<>0.
        if (m <= 0) return;

        if (beta != 0)
            rmulvx(m, beta, y, iy);
        else
            rsetvx(m, 0.0, y, iy);

        if (n <= 0 || alpha == 0.0) 
            return;

        int i;
        int j;
        double v;

        // Generic code
        if (opa == 0)
        {
            // y += A*x
            for (i = 0, v = 0; i < m; i++, v = 0)
            {
                for (j = 0; j < n; j++) 
                    v += a[ia + i, ja + j] * x[ix + j];

                y[iy + i] += alpha * v;
            }

            return;
        }

        if (opa != 1) return;

        // y += A^T*x
        for (i = 0; i <= n - 1; i++)
        {
            v = alpha * x[ix + i];
            for (j = 0; j <= m - 1; j++) 
                y[iy + j] += v * a[ia + i, ja + j];
        }
    }

    private static void rmulvx(int n, double v, double[] x, int offsx)
    {
        for (var i = 0; i < n; i++) 
            x[offsx + i] *= v;
    }

    private static void rsetvx(int n, double v, double[] x, int offsx)
    {
        for (var j = 0; j < n; j++)
            x[offsx + j] = v;
    }
}
