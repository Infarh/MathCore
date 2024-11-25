
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
    public static bool rmatrixsvd(double[,] a, int uneeded, int vtneeded, int additionalmemory, // = 2
            ref double[] w,
            ref double[,] u,
            ref double[,] vt)
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
                rmatrixqr(a, out tau);

                for (i = 0; i < n; i++)
                    for (j = 0; j < i; j++)
                        a[i, j] = 0;

                rmatrixbd(a, n, n, ref tauq, ref taup);
                rmatrixbdunpackpt(a, n, n, taup, nrvt, out vt);
                rmatrixbdunpackdiagonals(a, n, n, out isupper, out w, out e);
                result = rmatrixbdsvd(w, e, n, isupper, false, u, 0, a, 0, vt, ncvt);
                return result;
            }

            // Left singular vectors (may be full matrix U) to be computed
            rmatrixqr(a, out tau);
            rmatrixqrunpackq(a, m, n, tau, ncu, ref u);

            for (i = 0; i < n; i++)
                for (j = 0; j < i; j++) 
                    a[i, j] = 0;

            rmatrixbd(a, n, n, ref tauq, ref taup);
            rmatrixbdunpackpt(a, n, n, taup, nrvt, out vt);
            rmatrixbdunpackdiagonals(a, n, n, out isupper, out w, out e);

            if (additionalmemory < 1) // No additional memory can be used
            {
                rmatrixbdmultiplybyq(a, n, n, tauq, u, m, n, true, false);
                result = rmatrixbdsvd(w, e, n, isupper, false, u, m, a, 0, vt, ncvt);
                return result;

            }

            // Large U. Transforming intermediate matrix T2
            work = new double[Math.Max(m, n) + 1];

            rmatrixbdunpackq(a, n, n, tauq, n, out t2);
            copymatrix(u, 0, m - 1, 0, n - 1, ref a, 0, m - 1, 0, n - 1);
            inplacetranspose(ref t2, 0, n - 1, 0, n - 1, ref work);

            result = rmatrixbdsvd(w, e, n, isupper, false, u, 0, t2, n, vt, ncvt);

            rmatrixgemm(m, n, n, 1.0, a, 0, 0, 0, t2, 0, 0, 1, 0.0, u, 0, 0);

            return result;
        }

        // N much larger than M
        if (n > 1.6 * m) // Use bidiagonal reduction with LQ-decomposition
        {
            if (vtneeded == 0) // No right singular vectors to be computed
            {
                rmatrixlq(a, m, n, ref tau);

                for (i = 0; i < m; i++)
                    for (j = i + 1; j < m; j++)
                        a[i, j] = 0;

                rmatrixbd(a, m, m, ref tauq, ref taup);
                rmatrixbdunpackq(a, m, m, tauq, ncu, out u);
                rmatrixbdunpackdiagonals(a, m, m, out isupper, out w, out e);

                work = new double[m + 1];
                inplacetranspose(ref u, 0, nru - 1, 0, ncu - 1, ref work);
                result = rmatrixbdsvd(w, e, m, isupper, false, a, 0, u, nru, vt, 0);
                inplacetranspose(ref u, 0, nru - 1, 0, ncu - 1, ref work);

                return result;
            }

            // Right singular vectors (may be full matrix VT) to be computed
            rmatrixlq(a, m, n, ref tau);
            rmatrixlqunpackq(a, m, n, tau, nrvt, ref vt);

            for (i = 0; i < m; i++)
                for (j = i + 1; j < m; j++) 
                    a[i, j] = 0;

            rmatrixbd(a, m, m, ref tauq, ref taup);
            rmatrixbdunpackq(a, m, m, tauq, ncu, out u);
            rmatrixbdunpackdiagonals(a, m, m, out isupper, out w, out e);
            work = new double[Math.Max(m, n) + 1];
            inplacetranspose(ref u, 0, nru - 1, 0, ncu - 1, ref work);
            if (additionalmemory < 1) // No additional memory available
            {
                rmatrixbdmultiplybyp(a, m, m, taup, vt, m, n, false, true);
                result = rmatrixbdsvd(w, e, m, isupper, false, a, 0, u, nru, vt, n);
            }
            else // Large VT. Transforming intermediate matrix T2
            {
                rmatrixbdunpackpt(a, m, m, taup, m, out t2);
                result = rmatrixbdsvd(w, e, m, isupper, false, a, 0, u, nru, t2, m);
                copymatrix(vt, 0, m - 1, 0, n - 1, ref a, 0, m - 1, 0, n - 1);
                rmatrixgemm(m, n, m, 1.0, t2, 0, 0, 0, a, 0, 0, 0, 0.0, vt, 0, 0);
            }
            inplacetranspose(ref u, 0, nru - 1, 0, ncu - 1, ref work);
            return result;
        }

        if (m <= n) // We can use inplace transposition of U to get rid of columnwise operations
        {
            rmatrixbd(a, m, n, ref tauq, ref taup);
            rmatrixbdunpackq(a, m, n, tauq, ncu, out u);
            rmatrixbdunpackpt(a, m, n, taup, nrvt, out vt);
            rmatrixbdunpackdiagonals(a, m, n, out isupper, out w, out e);
            work = new double[m + 1];
            inplacetranspose(ref u, 0, nru - 1, 0, ncu - 1, ref work);
            result = rmatrixbdsvd(w, e, minmn, isupper, false, a, 0, u, nru, vt, ncvt);
            inplacetranspose(ref u, 0, nru - 1, 0, ncu - 1, ref work);
            return result;
        }

        // Simple bidiagonal reduction
        rmatrixbd(a, m, n, ref tauq, ref taup);
        rmatrixbdunpackq(a, m, n, tauq, ncu, out u);
        rmatrixbdunpackpt(a, m, n, taup, nrvt, out vt);
        rmatrixbdunpackdiagonals(a, m, n, out isupper, out w, out e);
        if (additionalmemory < 2 || uneeded == 0)
        {
            // We cant use additional memory or there is no need in such operations
            result = rmatrixbdsvd(w, e, minmn, isupper, false, u, nru, a, 0, vt, ncvt);
            return result;
        }

        // We can use additional memory
        t2 = new double[minmn - 1 + 1, m - 1 + 1];
        copyandtranspose(u, 0, m - 1, 0, minmn - 1, ref t2, 0, minmn - 1, 0, m - 1);
        result = rmatrixbdsvd(w, e, minmn, isupper, false, u, 0, t2, m, vt, ncvt);
        copyandtranspose(t2, 0, minmn - 1, 0, m - 1, ref u, 0, m - 1, 0, minmn - 1);

        return result;
    }

    public static void rmatrixqrunpackq(double[,] a, int m, int n, double[] tau, int qcolumns, ref double[,] q)
    {
        //alglib.ap.assert(qcolumns <= m, "UnpackQFromQR: QColumns>M!");
        if (m <= 0 || n <= 0 || qcolumns <= 0)
        {
            q = new double[0, 0];
            return;
        }

        var ts = matrixtilesizeb();
        var minmn = Math.Min(m, n);
        var refcnt = Math.Min(minmn, qcolumns);
        q = new double[m, qcolumns];
        int i;
        for (i = 0; i < m; i++)
            for (var j = 0; j < qcolumns; j++)
                q[i, j] = i == j ? 1 : 0;

        var work = new double[Math.Max(m, qcolumns) + 1];
        var t = new double[Math.Max(m, qcolumns) + 1];
        var taubuf = new double[minmn];
        var tmpa = new double[m, ts];
        var tmpt = new double[ts, 2 * ts];
        var tmpr = new double[2 * ts, qcolumns];

        var blockstart = ts * (refcnt / ts);
        var blocksize = refcnt - blockstart;
        while (blockstart >= 0)
        {
            var rowscount = m - blockstart;
            if (blocksize > 0)
            {
                // Copy current block
                rmatrixcopy(rowscount, blocksize, a, blockstart, blockstart, tmpa, 0, 0);
                var i1 = blockstart - 0;
                int j;
                for (j = 0; j < blocksize; j++) taubuf[j] = tau[j + i1];

                // Update, choose between:
                // a) Level 2 algorithm (when the rest of the matrix is small enough)
                // b) blocked algorithm, see algorithm 5 from  'A storage efficient WY
                //    representation for products of Householder transformations',
                //    by R. Schreiber and C. Van Loan.
                if (qcolumns >= 2 * ts)
                {
                    // Prepare block reflector
                    rmatrixblockreflector(ref tmpa, ref taubuf, true, rowscount, blocksize, ref tmpt, ref work);

                    // Multiply matrix by Q.
                    // Q  = E + Y*T*Y'  = E + TmpA*TmpT*TmpA'
                    rmatrixgemm(blocksize, qcolumns, rowscount, 1, tmpa, 0, 0, 1, q, blockstart, 0, 0, 0, tmpr, 0, 0);
                    rmatrixgemm(blocksize, qcolumns, blocksize, 1, tmpt, 0, 0, 0, tmpr, 0, 0, 0, 0, tmpr, blocksize, 0);
                    rmatrixgemm(rowscount, qcolumns, blocksize, 1, tmpa, 0, 0, 0, tmpr, blocksize, 0, 0, 1, q, blockstart, 0);
                }
                else
                    // Level 2 algorithm
                    for (i = blocksize - 1; i >= 0; i--)
                    {
                        i1 = i - 1;
                        for (j = 1; j <= rowscount - i; j++) 
                            t[j] = tmpa[j + i1, i];
                        t[1] = 1;
                        applyreflectionfromtheleft(q, taubuf[i], t, blockstart + i, m - 1, 0, qcolumns - 1, ref work);
                    }
            }

            blockstart -= ts;
            blocksize = ts;
        }
    }

    private static void rmatrixlq(double[,] a, int m, int n, ref double[] tau)
    {

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
        var tmpa = new double[ts, n];
        var tmpt = new double[ts, 2 * ts];
        var tmpr = new double[m, 2 * ts];

        // Blocked code
        var blockstart = 0;
        while (blockstart != minmn)
        {
            // Determine block size
            var blocksize = minmn - blockstart;
            if (blocksize > ts) blocksize = ts;
            var columnscount = n - blockstart;

            // LQ decomposition of submatrix.
            // Matrix is copied to temporary storage to solve
            // some TLB issues arising from non-contiguous memory
            // access pattern.
            rmatrixcopy(blocksize, columnscount, a, blockstart, blockstart, tmpa, 0, 0);
            rmatrixlqbasecase(ref tmpa, blocksize, columnscount, ref work, ref t, ref taubuf);
            rmatrixcopy(blocksize, columnscount, tmpa, 0, 0, a, blockstart, blockstart);
            var i1 = 0 - blockstart;
            int k;
            for (k = blockstart; k <= blockstart + blocksize - 1; k++) tau[k] = taubuf[k + i1];

            // Update the rest, choose between:
            // a) Level 2 algorithm (when the rest of the matrix is small enough)
            // b) blocked algorithm, see algorithm 5 from  'A storage efficient WY
            //    representation for products of Householder transformations',
            //    by R. Schreiber and C. Van Loan.
            if (blockstart + blocksize <= m - 1)
                if (m - blockstart - blocksize >= 2 * ts)
                {
                    // Prepare block reflector
                    rmatrixblockreflector(ref tmpa, ref taubuf, false, columnscount, blocksize, ref tmpt, ref work);

                    // Multiply the rest of A by Q.
                    //
                    // Q  = E + Y*T*Y'  = E + TmpA'*TmpT*TmpA
                    rmatrixgemm(m - blockstart - blocksize, blocksize, columnscount, 1.0, a, blockstart + blocksize, blockstart, 0, tmpa, 0, 0, 1, 0.0, tmpr, 0, 0);
                    rmatrixgemm(m - blockstart - blocksize, blocksize, blocksize, 1.0, tmpr, 0, 0, 0, tmpt, 0, 0, 0, 0.0, tmpr, 0, blocksize);
                    rmatrixgemm(m - blockstart - blocksize, columnscount, blocksize, 1.0, tmpr, 0, blocksize, 0, tmpa, 0, 0, 0, 1.0, a, blockstart + blocksize, blockstart);
                }
                else // Level 2 algorithm
                    for (var i = 0; i < blocksize; i++)
                    {
                        i1 = i - 1;
                        for (k = 1; k <= columnscount - i; k++) t[k] = tmpa[i, k + i1];
                        t[1] = 1;
                        applyreflectionfromtheright(a, taubuf[i], t, blockstart + blocksize, m - 1, blockstart + i, n - 1, ref work);
                    }

            blockstart += blocksize;
        }
    }

    private static void rmatrixlqbasecase(ref double[,] a, int m, int n, ref double[] work, ref double[] t, ref double[] tau)
    {
        var k = Math.Min(m, n);
        for (var i = 0; i < k; i++)
        {
            // Generate elementary reflector H(i) to annihilate A(i,i+1:n-1)
            var i1_ = i - 1;
            int i_;
            for (i_ = 1; i_ <= n - i; i_++) 
                t[i_] = a[i, i_ + i1_];
            
            generatereflection(ref t, n - i, out var tmp);
            tau[i] = tmp;
            
            i1_ = 1 - i;
            for (i_ = i; i_ < n; i_++)
                a[i, i_] = t[i_ + i1_];
            t[1] = 1;

            if (i < n) // Apply H(i) to A(i+1:m,i:n) from the right
                applyreflectionfromtheright(a, tau[i], t, i + 1, m - 1, i, n - 1, ref work);
        }
    }

    public static void rmatrixlqunpackq(double[,] a, int m, int n, double[] tau, int qrows, ref double[,] q)
    {
        //alglib.ap.assert(qrows <= n, "RMatrixLQUnpackQ: QRows>N!");
        if (m <= 0 || n <= 0 || qrows <= 0)
        {
            q = new double[0, 0];
            return;
        }

        var ts = matrixtilesizeb();
        var minmn = Math.Min(m, n);
        var refcnt = Math.Min(minmn, qrows);
        var work = new double[Math.Max(m, n) + 1];
        var t = new double[Math.Max(m, n) + 1];
        var taubuf = new double[minmn];
        var tmpa = new double[ts, n];
        var tmpt = new double[ts, 2 * ts];
        var tmpr = new double[qrows, 2 * ts];
        q = new double[qrows, n];

        int i;
        for (i = 0; i < qrows; i++)
            for (var j = 0; j < n; j++)
                q[i, j] = i == j ? 1 : 0;

        var blockstart = ts * (refcnt / ts);
        var blocksize = refcnt - blockstart;
        while (blockstart >= 0)
        {
            var columnscount = n - blockstart;
            if (blocksize > 0)
            {
                // Copy submatrix
                rmatrixcopy(blocksize, columnscount, a, blockstart, blockstart, tmpa, 0, 0);
                var i1_ = blockstart - 0;
                int i_;
                for (i_ = 0; i_ < blocksize; i_++) 
                    taubuf[i_] = tau[i_ + i1_];

                // Update matrix, choose between:
                // a) Level 2 algorithm (when the rest of the matrix is small enough)
                // b) blocked algorithm, see algorithm 5 from  'A storage efficient WY
                //    representation for products of Householder transformations',
                //    by R. Schreiber and C. Van Loan.
                if (qrows >= 2 * ts)
                {
                    // Prepare block reflector
                    rmatrixblockreflector(ref tmpa, ref taubuf, false, columnscount, blocksize, ref tmpt, ref work);

                    // Multiply the rest of A by Q'.
                    //
                    // Q'  = E + Y*T'*Y'  = E + TmpA'*TmpT'*TmpA
                    rmatrixgemm(qrows, blocksize, columnscount, 1.0, q, 0, blockstart, 0, tmpa, 0, 0, 1, 0.0, tmpr, 0, 0);
                    rmatrixgemm(qrows, blocksize, blocksize, 1.0, tmpr, 0, 0, 0, tmpt, 0, 0, 1, 0.0, tmpr, 0, blocksize);
                    rmatrixgemm(qrows, columnscount, blocksize, 1.0, tmpr, 0, blocksize, 0, tmpa, 0, 0, 0, 1.0, q, 0, blockstart);
                }
                else // Level 2 algorithm
                    for (i = blocksize - 1; i >= 0; i--)
                    {
                        i1_ = i - 1;
                        for (i_ = 1; i_ <= columnscount - i; i_++)
                            t[i_] = tmpa[i, i_ + i1_];
                        t[1] = 1;

                        applyreflectionfromtheright(q, taubuf[i], t, 0, qrows - 1, blockstart + i, n - 1, ref work);
                    }
            }

            blockstart -= ts;
            blocksize = ts;
        }
    }

    private static void copymatrix(double[,] a,
        int is1,
        int is2,
        int js1,
        int js2,
        ref double[,] b,
        int id1,
        int id2,
        int jd1,
        int jd2)
    {
        if (is1 > is2 || js1 > js2) 
            return;

        //alglib.ap.assert(is2 - is1 == id2 - id1, "CopyMatrix: different sizes!");
        //alglib.ap.assert(js2 - js1 == jd2 - jd1, "CopyMatrix: different sizes!");
        for (var isrc = is1; isrc <= is2; isrc++)
        {
            var idst = isrc - is1 + id1;
            var i1_ = js1 - jd1;
            for (var i_ = jd1; i_ <= jd2; i_++)
                b[idst, i_] = a[isrc, i_ + i1_];
        }
    }

    public static void inplacetranspose(ref double[,] a, int i1, int i2, int j1, int j2, ref double[] work)
    {
        if (i1 > i2 || j1 > j2)
            return;

        //alglib.ap.assert(i1 - i2 == j1 - j2, "InplaceTranspose error: incorrect array size!");
        for (var i = i1; i < i2; i++)
        {
            var j = j1 + i - i1;
            var ips = i + 1;
            var jps = j1 + ips - i1;
            var l = i2 - i;
            var i1_ = ips - 1;

            int i_;
            for (i_ = 1; i_ <= l; i_++) 
                work[i_] = a[i_ + i1_, j];

            i1_ = jps - ips;
            for (i_ = ips; i_ <= i2; i_++)
                a[i_, j] = a[i, i_ + i1_];

            i1_ = 1 - jps;
            for (i_ = jps; i_ <= j2; i_++)
                a[i, i_] = work[i_ + i1_];
        }
    }

    public static void copyandtranspose(double[,] a,
        int is1,
        int is2,
        int js1,
        int js2,
        ref double[,] b,
        int id1,
        int id2,
        int jd1,
        int jd2)
    {
        if (is1 > is2 || js1 > js2)
            return;

        //alglib.ap.assert(is2 - is1 == jd2 - jd1, "CopyAndTranspose: different sizes!");
        //alglib.ap.assert(js2 - js1 == id2 - id1, "CopyAndTranspose: different sizes!");
        for (var isrc = is1; isrc <= is2; isrc++)
        {
            var jdst = isrc - is1 + jd1;
            var i1_ = js1 - id1;
            for (var i_ = id1; i_ <= id2; i_++) 
                b[i_, jdst] = a[isrc, i_ + i1_];
        }
    }

    private static bool rmatrixbdsvd(
        double[] d,
        double[] e,
        int n,
        bool isupper,
        bool isfractionalaccuracyrequired,
        double[,] u,
        int nru,
        double[,] c,
        int ncc,
        double[,] vt,
        int ncvt)
    {
        var en = Array.Empty<double>();
        var d1 = Array.Empty<double>();
        var e1 = Array.Empty<double>();
        e = (double[])e.Clone();

        en = new double[n];
        for (var i = 0; i <= n - 2; i++)
            en[i] = e[i];
        en[n - 1] = 0;

        d1 = new double[n + 1];
        var i1_ = 0 - 1;
        int i_;
        for (i_ = 1; i_ <= n; i_++) 
            d1[i_] = d[i_ + i1_];

        if (n > 1)
        {
            e1 = new double[n - 1 + 1];
            i1_ = 0 - 1;
            for (i_ = 1; i_ < n; i_++) 
                e1[i_] = e[i_ + i1_];
        }
        var result = bidiagonalsvddecompositioninternal(d1, e1, n, isupper, isfractionalaccuracyrequired, u, 0, nru, c, 0, ncc, vt, 0, ncvt);

        i1_ = 1 - 0;
        for (i_ = 0; i_ < n; i_++) 
            d[i_] = d1[i_ + i1_];

        return result;
    }

    private static bool bidiagonalsvddecompositioninternal(
        double[] d,
            double[] e,
            int n,
            bool isupper,
            bool isfractionalaccuracyrequired,
            double[,] uu,
            int ustart,
            int nru,
            double[,] c,
            int cstart,
            int ncc,
            double[,] vt,
            int vstart,
            int ncvt)
    {
        double cosl = 0;
        double cosr = 0;
        double cs = 0;
        double r = 0;
        double shift = 0;
        double sigmn = 0;
        double sigmx = 0;
        double sinl = 0;
        double sinr = 0;
        double sn = 0;
        double[] work0;
        double[] work1;
        double[] work2;
        double[] work3;
        double[] utemp;
        double[] vttemp;
        double[] ctemp;
        double[] etemp;
        var ut = new double[0, 0];
        double tmp = 0;
        e = (double[])e.Clone();

        var result = true;
        if (n == 0) return result;
        int i_;
        if (n == 1)
        {
            if (d[1] < 0)
            {
                d[1] = -d[1];
                if (ncvt > 0)
                    for (i_ = vstart; i_ <= vstart + ncvt - 1; i_++)
                        vt[vstart, i_] = -1 * vt[vstart, i_];
            }
            return result;
        }

        //
        // these initializers are not really necessary,
        // but without them compiler complains about uninitialized locals
        //
        var ll = 0;
        double oldsn = 0;

        //
        // init
        //
        work0 = new double[n - 1 + 1];
        work1 = new double[n - 1 + 1];
        work2 = new double[n - 1 + 1];
        work3 = new double[n - 1 + 1];
        var uend = ustart + Math.Max(nru - 1, 0);
        var vend = vstart + Math.Max(ncvt - 1, 0);
        var cend = cstart + Math.Max(ncc - 1, 0);
        utemp = new double[uend + 1];
        vttemp = new double[vend + 1];
        ctemp = new double[cend + 1];
        var maxitr = 12;
        var fwddir = true;
        if (nru > 0)
        {
            ut = new double[ustart + n, ustart + nru];
            rmatrixtranspose(nru, n, uu, ustart, ustart, ut, ustart, ustart);
        }

        //
        // resize E from N-1 to N
        //
        etemp = new double[n + 1];
        int i;
        for (i = 1; i < n; i++) etemp[i] = e[i];
        e = new double[n + 1];
        for (i = 1; i < n; i++) e[i] = etemp[i];
        e[n] = 0;
        var idir = 0;

        //
        // Get machine constants
        //
        var eps = math.machineepsilon;
        var unfl = math.minrealnumber;

        //
        // If matrix lower bidiagonal, rotate to be upper bidiagonal
        // by applying Givens rotations on the left
        //
        if (!isupper)
        {
            for (i = 1; i < n; i++)
            {
                generaterotation(d[i], e[i], ref cs, ref sn, ref r);
                d[i] = r;
                e[i] = sn * d[i + 1];
                d[i + 1] = cs * d[i + 1];
                work0[i] = cs;
                work1[i] = sn;
            }

            //
            // Update singular vectors if desired
            //
            if (nru > 0) applyrotationsfromtheleft(fwddir, 1 + ustart - 1, n + ustart - 1, ustart, uend, work0, work1, ut, utemp);
            if (ncc > 0) applyrotationsfromtheleft(fwddir, 1 + cstart - 1, n + cstart - 1, cstart, cend, work0, work1, c, ctemp);
        }

        //
        // Compute singular values to relative accuracy TOL
        // (By setting TOL to be negative, algorithm will compute
        // singular values to absolute accuracy ABS(TOL)*norm(input matrix))
        //
        var tolmul = Math.Max(10, Math.Min(100, Math.Pow(eps, -0.125)));
        var tol = tolmul * eps;

        //
        // Compute approximate maximum, minimum singular values
        //
        double smax = 0;
        for (i = 1; i <= n; i++) smax = Math.Max(smax, Math.Abs(d[i]));
        for (i = 1; i < n; i++) smax = Math.Max(smax, Math.Abs(e[i]));
        double sminl = 0;
        double mu;
        double thresh;
        if (tol >= 0)
        {
            //
            // Relative accuracy desired
            //
            var sminoa = Math.Abs(d[1]);
            if (sminoa != 0)
            {
                mu = sminoa;
                for (i = 2; i <= n; i++)
                {
                    mu = Math.Abs(d[i]) * (mu / (mu + Math.Abs(e[i - 1])));
                    sminoa = Math.Min(sminoa, mu);
                    if (sminoa == 0) break;
                }
            }
            sminoa /= Math.Sqrt(n);
            thresh = Math.Max(tol * sminoa, maxitr * n * n * unfl);
        }
        else
            //
            // Absolute accuracy desired
            //
            thresh = Math.Max(Math.Abs(tol) * smax, maxitr * n * n * unfl);

        //
        // Prepare for main iteration loop for the singular values
        // (MAXIT is the maximum number of passes through the inner
        // loop permitted before nonconvergence signalled.)
        //
        var maxit = maxitr * n * n;
        var iter = 0;
        var oldll = -1;
        var oldm = -1;

        //
        // M points to last element of unconverged part of matrix
        //
        var m = n;
        double smin;
        //
        // Begin main iteration loop
        //
        while (true)
        {

            //
            // Check for convergence or exceeding iteration count
            //
            if (m <= 1) break;
            if (iter > maxit)
            {
                result = false;
                return result;
            }

            //
            // Find diagonal block of matrix to work on
            //
            if (tol < 0 && Math.Abs(d[m]) <= thresh) d[m] = 0;
            smax = Math.Abs(d[m]);
            smin = smax;
            var matrixsplitflag = false;
            int lll;
            for (lll = 1; lll < m; lll++)
            {
                ll = m - lll;
                var abss = Math.Abs(d[ll]);
                var abse = Math.Abs(e[ll]);
                if (tol < 0 && abss <= thresh) d[ll] = 0;
                if (abse <= thresh)
                {
                    matrixsplitflag = true;
                    break;
                }
                smin = Math.Min(smin, abss);
                smax = Math.Max(smax, Math.Max(abss, abse));
            }
            if (!matrixsplitflag)
                ll = 0;
            else
            {

                //
                // Matrix splits since E(LL) = 0
                //
                e[ll] = 0;
                if (ll == m - 1)
                {

                    //
                    // Convergence of bottom singular value, return to top of loop
                    //
                    m -= 1;
                    continue;
                }
            }
            ll += 1;

            //
            // E(LL) through E(M-1) are nonzero, E(LL-1) is zero
            //
            if (ll == m - 1)
            {

                //
                // 2 by 2 block, handle separately
                //
                svdv2x2(d[m - 1], e[m - 1], d[m], out sigmn, out sigmx, out sinr, out cosr, out sinl, out cosl);
                d[m - 1] = sigmx;
                e[m - 1] = 0;
                d[m] = sigmn;

                int mm1;
                int mm0;
                //
                // Compute singular vectors, if desired
                //
                if (ncvt > 0)
                {
                    mm0 = m + (vstart - 1);
                    mm1 = m - 1 + (vstart - 1);
                    for (i_ = vstart; i_ <= vend; i_++) vttemp[i_] = cosr * vt[mm1, i_];
                    for (i_ = vstart; i_ <= vend; i_++) vttemp[i_] += sinr * vt[mm0, i_];
                    for (i_ = vstart; i_ <= vend; i_++) vt[mm0, i_] = cosr * vt[mm0, i_];
                    for (i_ = vstart; i_ <= vend; i_++) vt[mm0, i_] -= sinr * vt[mm1, i_];
                    for (i_ = vstart; i_ <= vend; i_++) vt[mm1, i_] = vttemp[i_];
                }
                if (nru > 0)
                {
                    mm0 = m + ustart - 1;
                    mm1 = m - 1 + ustart - 1;
                    for (i_ = ustart; i_ <= uend; i_++) utemp[i_] = cosl * ut[mm1, i_];
                    for (i_ = ustart; i_ <= uend; i_++) utemp[i_] += sinl * ut[mm0, i_];
                    for (i_ = ustart; i_ <= uend; i_++) ut[mm0, i_] = cosl * ut[mm0, i_];
                    for (i_ = ustart; i_ <= uend; i_++) ut[mm0, i_] -= sinl * ut[mm1, i_];
                    for (i_ = ustart; i_ <= uend; i_++) ut[mm1, i_] = utemp[i_];
                }
                if (ncc > 0)
                {
                    mm0 = m + cstart - 1;
                    mm1 = m - 1 + cstart - 1;
                    for (i_ = cstart; i_ <= cend; i_++) ctemp[i_] = cosl * c[mm1, i_];
                    for (i_ = cstart; i_ <= cend; i_++) ctemp[i_] += sinl * c[mm0, i_];
                    for (i_ = cstart; i_ <= cend; i_++) c[mm0, i_] = cosl * c[mm0, i_];
                    for (i_ = cstart; i_ <= cend; i_++) c[mm0, i_] -= sinl * c[mm1, i_];
                    for (i_ = cstart; i_ <= cend; i_++) c[mm1, i_] = ctemp[i_];
                }
                m -= 2;
                continue;
            }

            //
            // If working on new submatrix, choose shift direction
            // (from larger end diagonal element towards smaller)
            //
            // Previously was
            //     "if (LL>OLDM) or (M<OLDLL) then"
            // fixed thanks to Michael Rolle < m@rolle.name >
            // Very strange that LAPACK still contains it.
            //
            var bchangedir = idir == 1 && Math.Abs(d[ll]) < 1.0E-3 * Math.Abs(d[m]) ||
                             idir == 2 && Math.Abs(d[m]) < 1.0E-3 * Math.Abs(d[ll]);
            
            if (ll != oldll || m != oldm || bchangedir)
                idir = Math.Abs(d[ll]) >= Math.Abs(d[m]) 
                    ? 1 // Chase bulge from top (big end) to bottom (small end)
                    : 2; // Chase bulge from bottom (big end) to top (small end)

            bool iterflag;
           
            if (idir == 1) // Apply convergence tests
            {

                // Run convergence test in forward direction
                // First apply standard test to bottom of matrix
                if (Math.Abs(e[m - 1]) <= Math.Abs(tol) * Math.Abs(d[m]) || tol < 0 && Math.Abs(e[m - 1]) <= thresh)
                {
                    e[m - 1] = 0;
                    continue;
                }
                if (tol >= 0)
                {
                    // If relative accuracy desired,
                    // apply convergence criterion forward
                    mu = Math.Abs(d[ll]);
                    sminl = mu;
                    iterflag = false;
                    for (lll = ll; lll < m; lll++)
                    {
                        if (Math.Abs(e[lll]) <= tol * mu)
                        {
                            e[lll] = 0;
                            iterflag = true;
                            break;
                        }
                        mu = Math.Abs(d[lll + 1]) * (mu / (mu + Math.Abs(e[lll])));
                        sminl = Math.Min(sminl, mu);
                    }

                    if (iterflag) 
                        continue;
                }
            }
            else
            {
                // Run convergence test in backward direction
                // First apply standard test to top of matrix
                if (Math.Abs(e[ll]) <= Math.Abs(tol) * Math.Abs(d[ll]) || tol < 0 && Math.Abs(e[ll]) <= thresh)
                {
                    e[ll] = 0;
                    continue;
                }

                if (tol >= 0)
                {
                    // If relative accuracy desired,
                    // apply convergence criterion backward
                    mu = Math.Abs(d[m]);
                    sminl = mu;
                    iterflag = false;
                    for (lll = m - 1; lll >= ll; lll--)
                    {
                        if (Math.Abs(e[lll]) <= tol * mu)
                        {
                            e[lll] = 0;
                            iterflag = true;
                            break;
                        }
                        mu = Math.Abs(d[lll]) * (mu / (mu + Math.Abs(e[lll])));
                        sminl = Math.Min(sminl, mu);
                    }

                    if (iterflag) 
                        continue;
                }
            }

            oldll = ll;
            oldm = m;

            // Compute shift.  First, test if shifting would ruin relative
            // accuracy, and if so set the shift to zero.
            if (tol >= 0 && n * tol * (sminl / smax) <= Math.Max(eps, 0.01 * tol))
                shift = 0; // Use a zero shift to avoid loss of relative accuracy
            else
            {
                double sll;
                // Compute the shift from 2-by-2 block at end of matrix
                if (idir == 1)
                {
                    sll = Math.Abs(d[ll]);
                    svd2x2(d[m - 1], e[m - 1], d[m], ref shift, ref r);
                }
                else
                {
                    sll = Math.Abs(d[m]);
                    svd2x2(d[ll], e[ll], d[ll + 1], ref shift, ref r);
                }

                // Test if shift negligible, and if so set to zero
                if (sll > 0)
                    if (math.sqr(shift / sll) < eps)
                        shift = 0;
            }

            // Increment iteration count
            iter = iter + m - ll;

            // If SHIFT = 0, do simplified QR iteration
            if (shift == 0)
            {
                double h;
                double oldcs;
                if (idir == 1)
                {
                    // Chase bulge from top to bottom
                    // Save cosines and sines for later singular vector updates
                    cs = 1;
                    oldcs = 1;
                    for (i = ll; i < m; i++)
                    {
                        generaterotation(d[i] * cs, e[i], ref cs, ref sn, ref r);
                        if (i > ll) 
                            e[i - 1] = oldsn * r;
                        generaterotation(oldcs * r, d[i + 1] * sn, ref oldcs, ref oldsn, ref tmp);
                        d[i] = tmp;
                        work0[i - ll + 1] = cs;
                        work1[i - ll + 1] = sn;
                        work2[i - ll + 1] = oldcs;
                        work3[i - ll + 1] = oldsn;
                    }

                    h = d[m] * cs;
                    d[m] = h * oldcs;
                    e[m - 1] = h * oldsn;

                    // Update singular vectors
                    if (ncvt > 0) applyrotationsfromtheleft(fwddir, ll + vstart - 1, m + vstart - 1, vstart, vend, work0, work1, vt, vttemp);
                    if (nru > 0) applyrotationsfromtheleft(fwddir, ll + ustart - 1, m + ustart - 1, ustart, uend, work2, work3, ut, utemp);
                    if (ncc > 0) applyrotationsfromtheleft(fwddir, ll + cstart - 1, m + cstart - 1, cstart, cend, work2, work3, c, ctemp);

                    // Test convergence
                    if (Math.Abs(e[m - 1]) <= thresh) 
                        e[m - 1] = 0;
                }
                else
                {
                    // Chase bulge from bottom to top
                    // Save cosines and sines for later singular vector updates
                    cs = 1;
                    oldcs = 1;
                    for (i = m; i >= ll + 1; i--)
                    {
                        generaterotation(d[i] * cs, e[i - 1], ref cs, ref sn, ref r);
                        if (i < m) 
                            e[i] = oldsn * r;
                        generaterotation(oldcs * r, d[i - 1] * sn, ref oldcs, ref oldsn, ref tmp);
                        d[i] = tmp;
                        work0[i - ll] = cs;
                        work1[i - ll] = -sn;
                        work2[i - ll] = oldcs;
                        work3[i - ll] = -oldsn;
                    }

                    h = d[ll] * cs;
                    d[ll] = h * oldcs;
                    e[ll] = h * oldsn;

                    // Update singular vectors
                    if (ncvt > 0) applyrotationsfromtheleft(!fwddir, ll + vstart - 1, m + vstart - 1, vstart, vend, work2, work3, vt, vttemp);
                    if (nru > 0) applyrotationsfromtheleft(!fwddir, ll + ustart - 1, m + ustart - 1, ustart, uend, work0, work1, ut, utemp);
                    if (ncc > 0) applyrotationsfromtheleft(!fwddir, ll + cstart - 1, m + cstart - 1, cstart, cend, work0, work1, c, ctemp);

                    // Test convergence
                    if (Math.Abs(e[ll]) <= thresh) 
                        e[ll] = 0;
                }
            }
            else
            {
                double f, g;
                // Use nonzero shift
                if (idir == 1)
                {
                    // Chase bulge from top to bottom
                    // Save cosines and sines for later singular vector updates
                    f = (Math.Abs(d[ll]) - shift) * (extsignbdsqr(1, d[ll]) + shift / d[ll]);
                    g = e[ll];
                    for (i = ll; i < m; i++)
                    {
                        generaterotation(f, g, ref cosr, ref sinr, ref r);
                        if (i > ll) 
                            e[i - 1] = r;

                        f = cosr * d[i] + sinr * e[i];
                        e[i] = cosr * e[i] - sinr * d[i];
                        g = sinr * d[i + 1];
                        d[i + 1] = cosr * d[i + 1];
                        generaterotation(f, g, ref cosl, ref sinl, ref r);
                        d[i] = r;
                        f = cosl * e[i] + sinl * d[i + 1];
                        d[i + 1] = cosl * d[i + 1] - sinl * e[i];

                        if (i < m - 1)
                        {
                            g = sinl * e[i + 1];
                            e[i + 1] = cosl * e[i + 1];
                        }

                        work0[i - ll + 1] = cosr;
                        work1[i - ll + 1] = sinr;
                        work2[i - ll + 1] = cosl;
                        work3[i - ll + 1] = sinl;
                    }
                    e[m - 1] = f;

                    // Update singular vectors
                    if (ncvt > 0) applyrotationsfromtheleft(fwddir, ll + vstart - 1, m + vstart - 1, vstart, vend, work0, work1, vt, vttemp);
                    if (nru > 0) applyrotationsfromtheleft(fwddir, ll + ustart - 1, m + ustart - 1, ustart, uend, work2, work3, ut, utemp);
                    if (ncc > 0) applyrotationsfromtheleft(fwddir, ll + cstart - 1, m + cstart - 1, cstart, cend, work2, work3, c, ctemp);

                    // Test convergence
                    if (Math.Abs(e[m - 1]) <= thresh) e[m - 1] = 0;
                }
                else
                {
                    // Chase bulge from bottom to top
                    // Save cosines and sines for later singular vector updates
                    f = (Math.Abs(d[m]) - shift) * (extsignbdsqr(1, d[m]) + shift / d[m]);
                    g = e[m - 1];
                    for (i = m; i >= ll + 1; i--)
                    {
                        generaterotation(f, g, ref cosr, ref sinr, ref r);
                        if (i < m)
                            e[i] = r;

                        f = cosr * d[i] + sinr * e[i - 1];
                        e[i - 1] = cosr * e[i - 1] - sinr * d[i];
                        g = sinr * d[i - 1];
                        d[i - 1] = cosr * d[i - 1];
                        generaterotation(f, g, ref cosl, ref sinl, ref r);
                        d[i] = r;
                        f = cosl * e[i - 1] + sinl * d[i - 1];
                        d[i - 1] = cosl * d[i - 1] - sinl * e[i - 1];

                        if (i > ll + 1)
                        {
                            g = sinl * e[i - 2];
                            e[i - 2] = cosl * e[i - 2];
                        }

                        work0[i - ll] = +cosr;
                        work1[i - ll] = -sinr;
                        work2[i - ll] = +cosl;
                        work3[i - ll] = -sinl;
                    }
                    e[ll] = f;

                    // Test convergence
                    if (Math.Abs(e[ll]) <= thresh) 
                        e[ll] = 0;

                    // Update singular vectors if desired
                    if (ncvt > 0) applyrotationsfromtheleft(!fwddir, ll + vstart - 1, m + vstart - 1, vstart, vend, work2, work3, vt, vttemp);
                    if (nru > 0) applyrotationsfromtheleft(!fwddir, ll + ustart - 1, m + ustart - 1, ustart, uend, work0, work1, ut, utemp);
                    if (ncc > 0) applyrotationsfromtheleft(!fwddir, ll + cstart - 1, m + cstart - 1, cstart, cend, work0, work1, c, ctemp);
                }
            }

            // QR iteration finished, go back and check convergence
        }

        // All singular values converged, so make them positive
        for (i = 1; i <= n; i++)
            if (d[i] < 0)
            {
                d[i] = -d[i];

                // Change sign of singular vectors, if desired
                if (ncvt <= 0) continue;

                for (i_ = vstart; i_ <= vend; i_++)
                    vt[i + vstart - 1, i_] = -vt[i + vstart - 1, i_];
            }

        // Sort the singular values into decreasing order (insertion sort on
        // singular values, but only one transposition per singular vector)
        for (i = 1; i < n; i++)
        {
            // Scan for smallest D(I)
            var isub = 1;
            smin = d[1];
            int j;

            for (j = 2; j <= n + 1 - i; j++)
                if (d[j] <= smin)
                {
                    isub = j;
                    smin = d[j];
                }

            if (isub == n + 1 - i) continue;

            // Swap singular values and vectors
            d[isub] = d[n + 1 - i];
            d[n + 1 - i] = smin;

            if (ncvt > 0)
            {
                j = n + 1 - i;
                for (i_ = vstart; i_ <= vend; i_++) 
                    vttemp[i_] = vt[isub + vstart - 1, i_];

                for (i_ = vstart; i_ <= vend; i_++) 
                    vt[isub + vstart - 1, i_] = vt[j + vstart - 1, i_];

                for (i_ = vstart; i_ <= vend; i_++) 
                    vt[j + vstart - 1, i_] = vttemp[i_];
            }

            if (nru > 0)
            {
                j = n + 1 - i;
                for (i_ = ustart; i_ <= uend; i_++) 
                    utemp[i_] = ut[isub + ustart - 1, i_];

                for (i_ = ustart; i_ <= uend; i_++) 
                    ut[isub + ustart - 1, i_] = ut[j + ustart - 1, i_];

                for (i_ = ustart; i_ <= uend; i_++) 
                    ut[j + ustart - 1, i_] = utemp[i_];
            }

            if (ncc <= 0) continue;

            j = n + 1 - i;
            for (i_ = cstart; i_ <= cend; i_++) 
                ctemp[i_] = c[isub + cstart - 1, i_];

            for (i_ = cstart; i_ <= cend; i_++) 
                c[isub + cstart - 1, i_] = c[j + cstart - 1, i_];

            for (i_ = cstart; i_ <= cend; i_++)
                c[j + cstart - 1, i_] = ctemp[i_];
        }

        // Copy U back from temporary storage
        if (nru > 0) 
            rmatrixtranspose(n, nru, ut, ustart, ustart, uu, ustart, ustart);

        return result;
    }

    private static void svdv2x2(double f, double g, double h, out double ssmin, out double ssmax, out double snr, out double csr, out double snl, out double csl)
    {
        ssmin = 0;
        ssmax = 0;
        var ft = f;
        var fa = Math.Abs(ft);
        var ht = h;
        var ha = Math.Abs(h);

        // these initializers are not really necessary,
        // but without them compiler complains about uninitialized locals
        double clt = 0;
        double crt = 0;
        double slt = 0;
        double srt = 0;
        double tsign = 0;

        //  PMAX points to the maximum absolute element of matrix
        //  PMAX = 1 if F largest in absolute values
        //  PMAX = 2 if G largest in absolute values
        //  PMAX = 3 if H largest in absolute values
        var pmax = 1;
        var swp = ha > fa;
        if (swp)
        {
            // Now FA .ge. HA
            pmax = 3;
            var temp = ft;
            ft = ht;
            ht = temp;
            temp = fa;
            fa = ha;
            ha = temp;
        }

        var ga = Math.Abs(g);
        if (ga == 0)
        {
            // Diagonal matrix
            ssmin = ha;
            ssmax = fa;
            clt = 1;
            crt = 1;
            slt = 0;
            srt = 0;
        }
        else
        {
            var gasmal = true;
            double v;
            if (ga > fa)
            {
                pmax = 2;
                if (fa / ga < math.machineepsilon)
                {
                    // Case of very large GA
                    gasmal = false;
                    ssmax = ga;
                    if (ha > 1)
                    {
                        v = ga / ha;
                        ssmin = fa / v;
                    }
                    else
                    {
                        v = fa / ga;
                        ssmin = v * ha;
                    }
                    clt = 1;
                    slt = ht / g;
                    srt = 1;
                    crt = ft / g;
                }
            }

            if (gasmal)
            {
                // Normal case
                var d = fa - ha;
                double l;

                l = d == fa ? 1 : d / fa;

                var m = g / ft;
                var t = 2 - l;
                var mm = m * m;
                var tt = t * t;
                var s = Math.Sqrt(tt + mm);
                double r;

                r = l == 0 ? Math.Abs(m) : Math.Sqrt(l * l + mm);

                var a = 0.5 * (s + r);
                ssmin = ha / a;
                ssmax = fa * a;
                if (mm == 0)
                {
                    // Note that M is very tiny
                    if (l == 0)
                        t = extsignbdsqr(2, ft) * extsignbdsqr(1, g);
                    else
                        t = g / extsignbdsqr(d, ft) + m / t;
                }
                else
                    t = (m / (s + t) + m / (r + l)) * (1 + a);

                l = Math.Sqrt(t * t + 4);
                crt = 2 / l;
                srt = t / l;
                clt = (crt + srt * m) / a;
                v = ht / ft;
                slt = v * srt / a;
            }
        }

        if (swp)
        {
            csl = srt;
            snl = crt;
            csr = slt;
            snr = clt;
        }
        else
        {
            csl = clt;
            snl = slt;
            csr = crt;
            snr = srt;
        }

            // Correct signs of SSMAX and SSMIN
        tsign = pmax switch
        {
            1 => extsignbdsqr(1, csr) * extsignbdsqr(1, csl) * extsignbdsqr(1, f),
            2 => extsignbdsqr(1, snr) * extsignbdsqr(1, csl) * extsignbdsqr(1, g),
            3 => extsignbdsqr(1, snr) * extsignbdsqr(1, snl) * extsignbdsqr(1, h),
            _ => tsign
        };

        ssmax = extsignbdsqr(ssmax, tsign);
        ssmin = extsignbdsqr(ssmin, tsign * extsignbdsqr(1, f) * extsignbdsqr(1, h));
    }

    private static void svd2x2(double f, double g, double h, ref double ssmin, ref double ssmax)
    {
        var fa = Math.Abs(f);
        var ga = Math.Abs(g);
        var ha = Math.Abs(h);
        var fhmn = Math.Min(fa, ha);
        var fhmx = Math.Max(fa, ha);

        if (fhmn == 0)
        {
            ssmin = 0;
            if (fhmx == 0)
                ssmax = ga;
            else
                ssmax = Math.Max(fhmx, ga) * Math.Sqrt(1 + math.sqr(Math.Min(fhmx, ga) / Math.Max(fhmx, ga)));
        }
        else
        {
            double aas;
            double at;
            double au;
            double c;
            if (ga < fhmx)
            {
                aas = 1 + fhmn / fhmx;
                at = (fhmx - fhmn) / fhmx;
                au = (ga / fhmx).Pow2();
                c = 2 / (Math.Sqrt(aas * aas + au) + Math.Sqrt(at * at + au));
                ssmin = fhmn * c;
                ssmax = fhmx / c;
            }
            else
            {
                au = fhmx / ga;
                if (au == 0)
                {
                    // Avoid possible harmful underflow if exponent range
                    // asymmetric (true SSMIN may not underflow even if
                    // AU underflows)
                    ssmin = fhmn * fhmx / ga;
                    ssmax = ga;
                }
                else
                {
                    aas = 1 + fhmn / fhmx;
                    at = (fhmx - fhmn) / fhmx;
                    c = 1 / (Math.Sqrt(1 + math.sqr(aas * au)) + Math.Sqrt(1 + math.sqr(at * au)));
                    ssmin = fhmn * c * au;
                    ssmin += ssmin;
                    ssmax = ga / (c + c);
                }
            }
        }
    }

    private static void generaterotation(double f, double g, ref double cs, ref double sn, ref double r)
    {
        if (g == 0)
        {
            cs = 1;
            sn = 0;
            r = f;
        }
        else if (f == 0)
        {
            cs = 0;
            sn = 1;
            r = g;
        }
        else
        {
            r = Math.Abs(f) > Math.Abs(g)
                ? Math.Abs(f) * Math.Sqrt(1 + (g / f).Pow2())
                : Math.Abs(g) * Math.Sqrt(1 + (f / g).Pow2());

            cs = f / r;
            sn = g / r;

            if (Math.Abs(f) <= Math.Abs(g) || cs >= 0)
                return;

            cs = -cs;
            sn = -sn;
            r = -r;
        }
    }

    private static double extsignbdsqr(double a, double b) => b >= 0 ? Math.Abs(a) : -Math.Abs(a);

    private static void applyrotationsfromtheleft(
        bool isforward,
        int m1,
        int m2,
        int n1,
        int n2,
        double[] c,
        double[] s,
        double[,] a,
        double[] work)
    {
        if (m1 > m2 || n1 > n2) 
            return;

        int j;
        int jp1;
        double ctemp;
        double stemp;
        double temp;
        int i_;

        // Form  P * A
        if (isforward)
        {
            if (n1 != n2) // Common case: N1!=N2
                for (j = m1; j < m2; j++)
                {
                    ctemp = c[j - m1 + 1];
                    stemp = s[j - m1 + 1];

                    if (ctemp == 1 && stemp == 0) 
                        continue;

                    jp1 = j + 1;
                    for (i_ = n1; i_ <= n2; i_++)
                        work[i_] = ctemp * a[jp1, i_];

                    for (i_ = n1; i_ <= n2; i_++)
                        work[i_] -= stemp * a[j, i_];

                    for (i_ = n1; i_ <= n2; i_++)
                        a[j, i_] *= ctemp;

                    for (i_ = n1; i_ <= n2; i_++)
                        a[j, i_] += stemp * a[jp1, i_];

                    for (i_ = n1; i_ <= n2; i_++)
                        a[jp1, i_] = work[i_];
                }
            else // Special case: N1==N2
                for (j = m1; j < m2; j++)
                {
                    ctemp = c[j - m1 + 1];
                    stemp = s[j - m1 + 1];

                    if (ctemp == 1 && stemp == 0) 
                        continue;
                    
                    temp = a[j + 1, n1];
                    a[j + 1, n1] = ctemp * temp - stemp * a[j, n1];
                    a[j, n1] = stemp * temp + ctemp * a[j, n1];
                }
        }
        else
        {
            if (n1 != n2)// Common case: N1!=N2
                for (j = m2 - 1; j >= m1; j--)
                {
                    ctemp = c[j - m1 + 1];
                    stemp = s[j - m1 + 1];

                    if (ctemp == 1 && stemp == 0) 
                        continue;

                    jp1 = j + 1;
                    for (i_ = n1; i_ <= n2; i_++)
                        work[i_] = ctemp * a[jp1, i_];
                    
                    for (i_ = n1; i_ <= n2; i_++)
                        work[i_] -= stemp * a[j, i_];
                    
                    for (i_ = n1; i_ <= n2; i_++)
                        a[j, i_] *= ctemp;
                    
                    for (i_ = n1; i_ <= n2; i_++)
                        a[j, i_] += stemp * a[jp1, i_];
                    
                    for (i_ = n1; i_ <= n2; i_++)
                        a[jp1, i_] = work[i_];
                }
            else // Special case: N1==N2
                for (j = m2 - 1; j >= m1; j--)
                {
                    ctemp = c[j - m1 + 1];
                    stemp = s[j - m1 + 1];

                    if (ctemp == 1 && stemp == 0) 
                        continue;

                    temp = a[j + 1, n1];
                    a[j + 1, n1] = ctemp * temp - stemp * a[j, n1];
                    a[j, n1] = stemp * temp + ctemp * a[j, n1];
                }
        }
    }

    private static void rmatrixtranspose(int m, int n, double[,] a, int ia, int ja, double[,] b, int ib, int jb)
    {
        int s1, s2;
        if (m <= 2 * 32 && n <= 2 * 32)
            for (var i = 0; i < m; i++) // base case
            {
                var i1_ = ja - ib;
                for (var i_ = ib; i_ <= ib + n - 1; i_++)
                    b[i_, jb + i] = a[ia + i, i_ + i1_];
            }
        else if (m > n) // Cache-oblivious recursion
        {
            ablassplitlength(a, m, out s1, out s2);
            rmatrixtranspose(s1, n, a, ia, ja, b, ib, jb);
            rmatrixtranspose(s2, n, a, ia + s1, ja, b, ib, jb + s1);
        }
        else
        {
            ablassplitlength(a, n, out s1, out s2);
            rmatrixtranspose(m, s1, a, ia, ja, b, ib, jb);
            rmatrixtranspose(m, s2, a, ia, ja + s1, b, ib + s1, jb);
        }
    }

    private static void ablassplitlength(double[,] a, int n, out int n1, out int n2)
    {
        n1 = 0;
        n2 = 0;

        if (n > 32)
            ablasinternalsplitlength(n, 32, ref n1, ref n2);
        else
            ablasinternalsplitlength(n, 8, ref n1, ref n2);
    }

    private static void ablasinternalsplitlength(int n, int nb, ref int n1, ref int n2)
    {
        if (n <= nb) // Block size, no further splitting
        {
            n1 = n;
            n2 = 0;
        }
        else if (n % nb != 0) // Greater than block size
        {
            // Split remainder
            n2 = n % nb;
            n1 = n - n2;
        }
        else
        {
            // Split on block boundaries
            n2 = n / 2;
            n1 = n - n2;
            if (n1 % nb == 0) return;
            var r = nb - n1 % nb;
            n1 += r;
            n2 -= r;
        }
    }

    private static void rmatrixbdunpackdiagonals(double[,] b, int m, int n, out bool isupper, out double[] d, out double[] e)
    {
        d = [];
        e = [];

        isupper = m >= n;
        if (m <= 0 || n <= 0)
            return;

        int i;
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

    private static void rmatrixbdunpackpt(double[,] qp, int m, int n, double[] taup, int ptrows, out double[,] pt)
    {
        pt = new double[0, 0];

        //alglib.ap.assert(ptrows <= n, "RMatrixBDUnpackPT: PTRows>N!");
        //alglib.ap.assert(ptrows >= 0, "RMatrixBDUnpackPT: PTRows<0!");
        if (m == 0 || n == 0 || ptrows == 0) 
            return;

        // prepare PT
        pt = new double[ptrows, n];
        for (var i = 0; i < ptrows; i++)
        for (var j = 0; j < n; j++)
            if (i == j)
                pt[i, j] = 1;
            else
                pt[i, j] = 0;

        // Calculate
        rmatrixbdmultiplybyp(qp, m, n, taup, pt, ptrows, n, true, true);
    }

    private static void rmatrixbdmultiplybyp(
        double[,] qp,
        int m,
        int n,
        double[] taup,
        double[,] z,
        int zrows,
        int zcolumns,
        bool fromtheright,
        bool dotranspose)
    {
        double[] dummy = [];
        if (m <= 0 || n <= 0 || zrows <= 0 || zcolumns <= 0)
            return;
        //alglib.ap.assert(fromtheright && zcolumns == n || !fromtheright && zrows == n, "RMatrixBDMultiplyByP: incorrect Z size!");

        var mx = Math.Max(m, n);
        mx = Math.Max(mx, zrows);
        mx = Math.Max(mx, zcolumns);
        var v = new double[mx + 1];
        var work = new double[mx + 1];
        int i, i1, i2, istep, i_, i1_;

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

    private static void rmatrixbdunpackq(double[,] qp, int m, int n, double[] tauq, int qcolumns, out double[,] q)
    {
        //alglib.ap.assert(qcolumns <= m, "RMatrixBDUnpackQ: QColumns>M!");
        //alglib.ap.assert(qcolumns >= 0, "RMatrixBDUnpackQ: QColumns<0!");

        if (m == 0 || n == 0 || qcolumns == 0)
        {
            q = new double[0, 0];
            return;
        }

        q = new double[m, qcolumns];
        for (var i = 0; i < m; i++)
            for (var j = 0; j < qcolumns; j++)
                q[i, j] = i == j ? 1 : 0;

        rmatrixbdmultiplybyq(qp, m, n, tauq, q, m, qcolumns, false, false);
    }

    private static void rmatrixbdmultiplybyq(double[,] qp,
        int m,
        int n,
        double[] tauq,
        double[,] z,
        int zrows,
        int zcolumns,
        bool fromtheright,
        bool dotranspose)
    {
        if (m <= 0 || n <= 0 || zrows <= 0 || zcolumns <= 0) 
            return;

        //alglib.ap.assert(fromtheright && zcolumns == m || !fromtheright && zrows == m, "RMatrixBDMultiplyByQ: incorrect Z size!");

        double[] dummy = [];

        var mx = Math.Max(m, n);
        mx = Math.Max(mx, zrows);
        mx = Math.Max(mx, zcolumns);

        var v = new double[mx + 1];
        var work = new double[mx + 1];

        int i, i_, i1_, i1, i2, istep;
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

    private static void rmatrixbd(double[,] a, int m, int n, ref double[] tauq, ref double[] taup)
    {
        if (n <= 0 || m <= 0)
        {
            tauq = [];
            taup = [];
            return;
        }

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
                tauq[i] = 0;
                taup[i] = 0;
            }
        }
        else
        {
            tauq = new double[m];
            taup = new double[m];
            for (i = 0; i < m; i++)
            {
                tauq[i] = 0;
                taup[i] = 0;
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
                    for (i_ = i + 1; i_ < n; i_++) 
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

    private static void rmatrixqr(double[,] a, out double[] tau)
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

    private static void rmatrixblockreflector(
        ref double[,] a,
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

    private static void rmatrixgemm(
        int m, int n, int k,
        double alpha, double[,] a,
        int ia, int ja, int optypea,
        double[,] b,
        int ib, int jb, int optypeb,
        double beta, double[,] c,
        int ic, int jc)
    {
        //alglib.ap.assert(optypea == 0 || optypea == 1, "RMatrixGEMM: incorrect OpTypeA (must be 0 or 1)");
        //alglib.ap.assert(optypeb == 0 || optypeb == 1, "RMatrixGEMM: incorrect OpTypeB (must be 0 or 1)");
        //alglib.ap.assert(ic + m <= alglib.ap.rows(c), "RMatrixGEMM: incorect size of output matrix C");
        //alglib.ap.assert(jc + n <= alglib.ap.cols(c), "RMatrixGEMM: incorect size of output matrix C");

        rmatrixgemmrec(m, n, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc);
    }

    private static void rmatrixgemmrec(
        int m,
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
        if (imax3(m, n, k) <= tsb) 
            tscur = tsa;

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

    private static void tiledsplit(int tasksize, int tilesize, ref int task0, ref int task1)
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

    private static int idivup(int a, int b) => a % b > 0 ? a / b + 1 : a / b;

    private static void rmatrixgemmk(
        int m,
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
            if (beta == 1) 
                return;

            int i, j;
            if (beta != 0)
                for (i = 0; i < m; i++)
                    for (j = 0; j < n; j++) 
                        c[ic + i, jc + j] *= beta;
            else
                for (i = 0; i < m; i++)
                    for (j = 0; j < n; j++) 
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

    private static void rmatrixgemmk44v00(
        int m,
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
                    for (var t = 0; t < k; t++)
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

                        offsa++;
                        offsb++;
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
                    var i1 = Math.Min(i + 3, m - 1);
                    var j1 = Math.Min(j + 3, n - 1);

                    // Process submatrix
                    for (var ik = i; ik <= i1; ik++)
                        for (var jk = j; jk <= j1; jk++)
                        {
                            double v;
                            if (k == 0 || alpha == 0)
                                v = 0;
                            else
                            {
                                var i1_ = ib - ja;
                                v = 0;
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

    private static void rmatrixgemmk44v01(
        int m,
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

                    for (var t = 0; t < k; t++)
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

                        offsa++;
                        offsb++;
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
                    var i1 = Math.Min(i + 3, m - 1);
                    var j1 = Math.Min(j + 3, n - 1);

                    // Process submatrix
                    for (var ik = i; ik <= i1; ik++)
                        for (var jk = j; jk <= j1; jk++)
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

    private static void rmatrixgemmk44v10(
        int m,
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
                    for (var t = 0; t < k; t++)
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
                    for (var t = 0; t < k; t++)
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

        for (var i = 0; i < m; i++)
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
        for (var i = 0; i < minmn; i++)
        {
            // Generate elementary reflector H(i) to annihilate A(i+1:m,i)
            var j = i - 1;

            int k;
            for (k = 1; k <= m - i; k++)
                t[k] = a[k + j, i];

            generatereflection(ref t, m - i, out var tmp);
            tau[i] = tmp;
            j = 1 - i;

            for (k = i; k < m; k++)
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
        for (var i = 0; i < m; i++)
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
            for (j = 0; j < m; j++) 
                y[j] += v * a[i, j];
        }
    }

    private static void rmulv(int n, double v, double[] x)
    {
        for (var i = 0; i < n; i++) 
            x[i] *= v;
    }

    private static void rsetv(int n, double v, double[] x)
    {
        for (var j = 0; j < n; j++)
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
        for (i = 0; i < n; i++)
        {
            v = alpha * x[ix + i];
            for (j = 0; j < m; j++) 
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
