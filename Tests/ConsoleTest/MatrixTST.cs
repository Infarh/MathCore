
// ReSharper disable IdentifierTypo

namespace ConsoleTest;

internal static class MatrixTST
{
    /// <summary>
    /// A = U * S * V^T
    /// </summary>
    /// <param name="a"></param>
    /// <param name="UNeeded">0, 1 or 2. See the description of the parameter U</param>
    /// <param name="VtNeeded">0, 1 or 2. See the description of the parameter VT</param>
    /// <param name="AdditionalMemory">
    /// == 0, the algorithm doesn't use additional memory(lower requirements, lower performance).
    /// == 1, the algorithm uses additional memory of size min(M, N)*min(M, N) of real numbers      It often speeds up the algorithm.
    /// == 2, the algorithm uses additional memory of size M* min(M, N) of real numbers. It allows to get a maximum performance.
    /// </param>
    /// <param name="s">contains singular values in descending order</param>
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
    public static bool SVD(
        double[,] a,
        out double[] s,
        out double[,] u,
        out double[,] vt,
        int UNeeded = 1,
        int VtNeeded = 2,
        int AdditionalMemory = 2)
    {
        var m = a.GetLength(0);
        var n = a.GetLength(1);

        double[] tau_q = [];
        double[] tau_p = [];
        double[] tau = [];
        double[] e = [];
        double[] work;
        var t2 = new double[0, 0];
        var is_upper = false;
        int i, j;

        a = (double[,])a.Clone();
        s = [];
        u = new double[0, 0];
        vt = new double[0, 0];

        var result = true;
        if (m == 0 || n == 0)
            return result;

        //alglib.ap.assert(UNeeded >= 0 && UNeeded <= 2, "SVDDecomposition: wrong parameters!");
        //alglib.ap.assert(VtNeeded >= 0 && VtNeeded <= 2, "SVDDecomposition: wrong parameters!");
        //alglib.ap.assert(AdditionalMemory >= 0 && AdditionalMemory <= 2, "SVDDecomposition: wrong parameters!");

        // initialize
        var min_mn = Math.Min(m, n);
        s = new double[min_mn + 1];
        var nc_u = 0;
        var nr_u = 0;

        switch (UNeeded)
        {
            case 1:
                nr_u = m;
                nc_u = min_mn;
                u = new double[nr_u - 1 + 1, nc_u - 1 + 1];
                break;
            case 2:
                nr_u = m;
                nc_u = m;
                u = new double[nr_u - 1 + 1, nc_u - 1 + 1];
                break;
        }

        var nr_vt = 0;
        var nc_vt = 0;
        switch (VtNeeded)
        {
            case 1:
                nr_vt = min_mn;
                nc_vt = n;
                vt = new double[nr_vt - 1 + 1, nc_vt - 1 + 1];
                break;
            case 2:
                nr_vt = n;
                nc_vt = n;
                vt = new double[nr_vt - 1 + 1, nc_vt - 1 + 1];
                break;
        }

        // M much larger than N
        if (m > 1.6 * n) // Use bidiagonal reduction with QR-decomposition
        {
            if (UNeeded == 0) // No left singular vectors to be computed
            {
                MatrixQR(a, out tau);

                for (i = 0; i < n; i++)
                    for (j = 0; j < i; j++)
                        a[i, j] = 0;

                MatrixBD(a, n, n, ref tau_q, ref tau_p);
                MatrixBDUnpackPT(a, n, n, tau_p, nr_vt, out vt);
                rmatrixbdunpackdiagonals(a, n, n, out is_upper, out s, out e);
                result = rmatrixbdsvd(s, e, n, is_upper, false, u, 0, a, 0, vt, nc_vt);
                return result;
            }

            // Left singular vectors (may be full matrix U) to be computed
            MatrixQR(a, out tau);
            rmatrixqrunpackq(a, m, n, tau, nc_u, ref u);

            for (i = 0; i < n; i++)
                for (j = 0; j < i; j++)
                    a[i, j] = 0;

            MatrixBD(a, n, n, ref tau_q, ref tau_p);
            MatrixBDUnpackPT(a, n, n, tau_p, nr_vt, out vt);
            rmatrixbdunpackdiagonals(a, n, n, out is_upper, out s, out e);

            if (AdditionalMemory < 1) // No additional memory can be used
            {
                MatrixBDMultiplyByQ(a, n, n, tau_q, u, m, n, true, false);
                result = rmatrixbdsvd(s, e, n, is_upper, false, u, m, a, 0, vt, nc_vt);
                return result;

            }

            // Large U. Transforming intermediate matrix T2
            work = new double[Math.Max(m, n) + 1];

            MatrixBDUnpackQ(a, n, n, tau_q, n, out t2);
            copymatrix(u, 0, m - 1, 0, n - 1, ref a, 0, m - 1, 0, n - 1);
            inplacetranspose(ref t2, 0, n - 1, 0, n - 1, ref work);

            result = rmatrixbdsvd(s, e, n, is_upper, false, u, 0, t2, n, vt, nc_vt);

            MatrixGEMM(m, n, n, 1.0, a, 0, 0, 0, t2, 0, 0, 1, 0.0, u, 0, 0);

            return result;
        }

        // N much larger than M
        if (n > 1.6 * m) // Use bidiagonal reduction with LQ-decomposition
        {
            if (VtNeeded == 0) // No right singular vectors to be computed
            {
                MatrixLQ(a, m, n, ref tau);

                for (i = 0; i < m; i++)
                    for (j = i + 1; j < m; j++)
                        a[i, j] = 0;

                MatrixBD(a, m, m, ref tau_q, ref tau_p);
                MatrixBDUnpackQ(a, m, m, tau_q, nc_u, out u);
                rmatrixbdunpackdiagonals(a, m, m, out is_upper, out s, out e);

                work = new double[m + 1];
                inplacetranspose(ref u, 0, nr_u - 1, 0, nc_u - 1, ref work);
                result = rmatrixbdsvd(s, e, m, is_upper, false, a, 0, u, nr_u, vt, 0);
                inplacetranspose(ref u, 0, nr_u - 1, 0, nc_u - 1, ref work);

                return result;
            }

            // Right singular vectors (may be full matrix VT) to be computed
            MatrixLQ(a, m, n, ref tau);
            MatrixLQUnpackQ(a, m, n, tau, nr_vt, ref vt);

            for (i = 0; i < m; i++)
                for (j = i + 1; j < m; j++)
                    a[i, j] = 0;

            MatrixBD(a, m, m, ref tau_q, ref tau_p);
            MatrixBDUnpackQ(a, m, m, tau_q, nc_u, out u);
            rmatrixbdunpackdiagonals(a, m, m, out is_upper, out s, out e);
            work = new double[Math.Max(m, n) + 1];
            inplacetranspose(ref u, 0, nr_u - 1, 0, nc_u - 1, ref work);
            if (AdditionalMemory < 1) // No additional memory available
            {
                MatrixBDMultiplyByP(a, m, m, tau_p, vt, m, n, false, true);
                result = rmatrixbdsvd(s, e, m, is_upper, false, a, 0, u, nr_u, vt, n);
            }
            else // Large VT. Transforming intermediate matrix T2
            {
                MatrixBDUnpackPT(a, m, m, tau_p, m, out t2);
                result = rmatrixbdsvd(s, e, m, is_upper, false, a, 0, u, nr_u, t2, m);
                copymatrix(vt, 0, m - 1, 0, n - 1, ref a, 0, m - 1, 0, n - 1);
                MatrixGEMM(m, n, m, 1.0, t2, 0, 0, 0, a, 0, 0, 0, 0.0, vt, 0, 0);
            }
            inplacetranspose(ref u, 0, nr_u - 1, 0, nc_u - 1, ref work);
            return result;
        }

        if (m <= n) // We can use inplace transposition of U to get rid of columnwise operations
        {
            MatrixBD(a, m, n, ref tau_q, ref tau_p);
            MatrixBDUnpackQ(a, m, n, tau_q, nc_u, out u);
            MatrixBDUnpackPT(a, m, n, tau_p, nr_vt, out vt);
            rmatrixbdunpackdiagonals(a, m, n, out is_upper, out s, out e);
            work = new double[m + 1];
            inplacetranspose(ref u, 0, nr_u - 1, 0, nc_u - 1, ref work);
            result = rmatrixbdsvd(s, e, min_mn, is_upper, false, a, 0, u, nr_u, vt, nc_vt);
            inplacetranspose(ref u, 0, nr_u - 1, 0, nc_u - 1, ref work);
            return result;
        }

        // Simple bidiagonal reduction
        MatrixBD(a, m, n, ref tau_q, ref tau_p);
        MatrixBDUnpackQ(a, m, n, tau_q, nc_u, out u);
        MatrixBDUnpackPT(a, m, n, tau_p, nr_vt, out vt);
        rmatrixbdunpackdiagonals(a, m, n, out is_upper, out s, out e);
        if (AdditionalMemory < 2 || UNeeded == 0)
        {
            // We cant use additional memory or there is no need in such operations
            result = rmatrixbdsvd(s, e, min_mn, is_upper, false, u, nr_u, a, 0, vt, nc_vt);
            return result;
        }

        // We can use additional memory
        t2 = new double[min_mn - 1 + 1, m - 1 + 1];
        copyandtranspose(u, 0, m - 1, 0, min_mn - 1, ref t2, 0, min_mn - 1, 0, m - 1);
        result = rmatrixbdsvd(s, e, min_mn, is_upper, false, u, 0, t2, m, vt, nc_vt);
        copyandtranspose(t2, 0, min_mn - 1, 0, m - 1, ref u, 0, m - 1, 0, min_mn - 1);

        return result;
    }

    private static void rmatrixqrunpackq(double[,] a, int m, int n, double[] Tau, int QColumns, ref double[,] q)
    {
        //alglib.ap.assert(QColumns <= m, "UnpackQFromQR: QColumns>M!");
        if (m <= 0 || n <= 0 || QColumns <= 0)
        {
            q = new double[0, 0];
            return;
        }

        const int ts = __MatrixTileSizeB;
        var min_mn = Math.Min(m, n);
        var ref_cnt = Math.Min(min_mn, QColumns);
        q = new double[m, QColumns];
        int i;
        for (i = 0; i < m; i++)
            for (var j = 0; j < QColumns; j++)
                q[i, j] = i == j ? 1 : 0;

        var work = new double[Math.Max(m, QColumns) + 1];
        var t = new double[Math.Max(m, QColumns) + 1];
        var tau_buf = new double[min_mn];
        var tmp_a = new double[m, ts];
        var tmp_t = new double[ts, 2 * ts];
        var tmp_r = new double[2 * ts, QColumns];

        var blockstart = ts * (ref_cnt / ts);
        var blocksize = ref_cnt - blockstart;
        while (blockstart >= 0)
        {
            var rowscount = m - blockstart;
            if (blocksize > 0)
            {
                // Copy current block
                MatrixCopy(rowscount, blocksize, a, blockstart, blockstart, tmp_a, 0, 0);
                var i1 = blockstart - 0;
                int j;
                for (j = 0; j < blocksize; j++) tau_buf[j] = Tau[j + i1];

                // Update, choose between:
                // a) Level 2 algorithm (when the rest of the matrix is small enough)
                // b) blocked algorithm, see algorithm 5 from  'A storage efficient WY
                //    representation for products of Householder transformations',
                //    by R. Schreiber and C. Van Loan.
                if (QColumns >= 2 * ts)
                {
                    // Prepare block reflector
                    MatrixBlockReflector(ref tmp_a, ref tau_buf, true, rowscount, blocksize, ref tmp_t, ref work);

                    // Multiply matrix by Q.
                    // Q  = E + Y*T*Y'  = E + TmpA*TmpT*TmpA'
                    MatrixGEMM(blocksize, QColumns, rowscount, 1, tmp_a, 0, 0, 1, q, blockstart, 0, 0, 0, tmp_r, 0, 0);
                    MatrixGEMM(blocksize, QColumns, blocksize, 1, tmp_t, 0, 0, 0, tmp_r, 0, 0, 0, 0, tmp_r, blocksize, 0);
                    MatrixGEMM(rowscount, QColumns, blocksize, 1, tmp_a, 0, 0, 0, tmp_r, blocksize, 0, 0, 1, q, blockstart, 0);
                }
                else
                    // Level 2 algorithm
                    for (i = blocksize - 1; i >= 0; i--)
                    {
                        i1 = i - 1;
                        for (j = 1; j <= rowscount - i; j++)
                            t[j] = tmp_a[j + i1, i];
                        t[1] = 1;
                        ApplyReflectionFromTheLeft(q, tau_buf[i], t, blockstart + i, m - 1, 0, QColumns - 1, ref work);
                    }
            }

            blockstart -= ts;
            blocksize = ts;
        }
    }

    private static void MatrixLQ(double[,] a, int m, int n, ref double[] Tau)
    {

        if (m <= 0 || n <= 0)
        {
            Tau = [];
            return;
        }

        var min_mn = Math.Min(m, n);
        var ts = __MatrixTileSizeB;
        var work = new double[Math.Max(m, n) + 1];
        var t = new double[Math.Max(m, n) + 1];
        Tau = new double[min_mn];
        var tau_buf = new double[min_mn];
        var tmp_a = new double[ts, n];
        var tmp_t = new double[ts, 2 * ts];
        var tmp_r = new double[m, 2 * ts];

        // Blocked code
        var block_start = 0;
        while (block_start != min_mn)
        {
            // Determine block size
            var block_size = min_mn - block_start;
            if (block_size > ts) block_size = ts;
            var columns_count = n - block_start;

            // LQ decomposition of submatrix.
            // Matrix is copied to temporary storage to solve
            // some TLB issues arising from non-contiguous memory
            // access pattern.
            MatrixCopy(block_size, columns_count, a, block_start, block_start, tmp_a, 0, 0);
            rmatrixlqbasecase(ref tmp_a, block_size, columns_count, ref work, ref t, ref tau_buf);
            MatrixCopy(block_size, columns_count, tmp_a, 0, 0, a, block_start, block_start);
            var i1 = 0 - block_start;
            int k;
            for (k = block_start; k <= block_start + block_size - 1; k++) Tau[k] = tau_buf[k + i1];

            // Update the rest, choose between:
            // a) Level 2 algorithm (when the rest of the matrix is small enough)
            // b) blocked algorithm, see algorithm 5 from  'A storage efficient WY
            //    representation for products of Householder transformations',
            //    by R. Schreiber and C. Van Loan.
            if (block_start + block_size <= m - 1)
                if (m - block_start - block_size >= 2 * ts)
                {
                    // Prepare block reflector
                    MatrixBlockReflector(ref tmp_a, ref tau_buf, false, columns_count, block_size, ref tmp_t, ref work);

                    // Multiply the rest of A by Q.
                    //
                    // Q  = E + Y*T*Y'  = E + TmpA'*TmpT*TmpA
                    MatrixGEMM(m - block_start - block_size, block_size, columns_count, 1.0, a, block_start + block_size, block_start, 0, tmp_a, 0, 0, 1, 0.0, tmp_r, 0, 0);
                    MatrixGEMM(m - block_start - block_size, block_size, block_size, 1.0, tmp_r, 0, 0, 0, tmp_t, 0, 0, 0, 0.0, tmp_r, 0, block_size);
                    MatrixGEMM(m - block_start - block_size, columns_count, block_size, 1.0, tmp_r, 0, block_size, 0, tmp_a, 0, 0, 0, 1.0, a, block_start + block_size, block_start);
                }
                else // Level 2 algorithm
                    for (var i = 0; i < block_size; i++)
                    {
                        i1 = i - 1;
                        for (k = 1; k <= columns_count - i; k++) t[k] = tmp_a[i, k + i1];
                        t[1] = 1;
                        ApplyReflectionFromTheRight(a, tau_buf[i], t, block_start + block_size, m - 1, block_start + i, n - 1, ref work);
                    }

            block_start += block_size;
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

            GenerateReflection(ref t, n - i, out var tmp);
            tau[i] = tmp;

            i1_ = 1 - i;
            for (i_ = i; i_ < n; i_++)
                a[i, i_] = t[i_ + i1_];
            t[1] = 1;

            if (i < n) // Apply H(i) to A(i+1:m,i:n) from the right
                ApplyReflectionFromTheRight(a, tau[i], t, i + 1, m - 1, i, n - 1, ref work);
        }
    }

    private static void MatrixLQUnpackQ(double[,] a, int m, int n, double[] Tau, int QRows, ref double[,] q)
    {
        //alglib.ap.assert(QRows <= n, "RMatrixLQUnpackQ: QRows>N!");
        if (m <= 0 || n <= 0 || QRows <= 0)
        {
            q = new double[0, 0];
            return;
        }

        const int ts = __MatrixTileSizeB;
        var min_mn = Math.Min(m, n);
        var ref_cnt = Math.Min(min_mn, QRows);
        var work = new double[Math.Max(m, n) + 1];
        var t = new double[Math.Max(m, n) + 1];
        var tau_buf = new double[min_mn];
        var tmp_a = new double[ts, n];
        var tmp_t = new double[ts, 2 * ts];
        var tmp_r = new double[QRows, 2 * ts];
        q = new double[QRows, n];

        int i;
        for (i = 0; i < QRows; i++)
            for (var j = 0; j < n; j++)
                q[i, j] = i == j ? 1 : 0;

        var block_start = ts * (ref_cnt / ts);
        var block_size = ref_cnt - block_start;
        while (block_start >= 0)
        {
            var columns_count = n - block_start;
            if (block_size > 0)
            {
                // Copy submatrix
                MatrixCopy(block_size, columns_count, a, block_start, block_start, tmp_a, 0, 0);
                var i1_ = block_start - 0;
                int i_;
                for (i_ = 0; i_ < block_size; i_++)
                    tau_buf[i_] = Tau[i_ + i1_];

                // Update matrix, choose between:
                // a) Level 2 algorithm (when the rest of the matrix is small enough)
                // b) blocked algorithm, see algorithm 5 from  'A storage efficient WY
                //    representation for products of Householder transformations',
                //    by R. Schreiber and C. Van Loan.
                if (QRows >= 2 * ts)
                {
                    // Prepare block reflector
                    MatrixBlockReflector(ref tmp_a, ref tau_buf, false, columns_count, block_size, ref tmp_t, ref work);

                    // Multiply the rest of A by Q'.
                    //
                    // Q'  = E + Y*T'*Y'  = E + TmpA'*TmpT'*TmpA
                    MatrixGEMM(QRows, block_size, columns_count, 1.0, q, 0, block_start, 0, tmp_a, 0, 0, 1, 0.0, tmp_r, 0, 0);
                    MatrixGEMM(QRows, block_size, block_size, 1.0, tmp_r, 0, 0, 0, tmp_t, 0, 0, 1, 0.0, tmp_r, 0, block_size);
                    MatrixGEMM(QRows, columns_count, block_size, 1.0, tmp_r, 0, block_size, 0, tmp_a, 0, 0, 0, 1.0, q, 0, block_start);
                }
                else // Level 2 algorithm
                    for (i = block_size - 1; i >= 0; i--)
                    {
                        i1_ = i - 1;
                        for (i_ = 1; i_ <= columns_count - i; i_++)
                            t[i_] = tmp_a[i, i_ + i1_];
                        t[1] = 1;

                        ApplyReflectionFromTheRight(q, tau_buf[i], t, 0, QRows - 1, block_start + i, n - 1, ref work);
                    }
            }

            block_start -= ts;
            block_size = ts;
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

    private static void inplacetranspose(ref double[,] a, int i1, int i2, int j1, int j2, ref double[] work)
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

    private static void copyandtranspose(double[,] a,
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
            int v_start,
            int ncvt)
    {
        double cos_l = 0;
        double cos_r = 0;
        double cs = 0;
        double r = 0;
        double shift = 0;
        double sigmn = 0;
        double sigmx = 0;
        double sin_l = 0;
        double sin_r = 0;
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
                    for (i_ = v_start; i_ <= v_start + ncvt - 1; i_++)
                        vt[v_start, i_] = -1 * vt[v_start, i_];
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
        var vend = v_start + Math.Max(ncvt - 1, 0);
        var cend = cstart + Math.Max(ncc - 1, 0);
        utemp = new double[uend + 1];
        vttemp = new double[vend + 1];
        ctemp = new double[cend + 1];
        var maxitr = 12;
        var fwddir = true;
        if (nru > 0)
        {
            ut = new double[ustart + n, ustart + nru];
            MatrixTranspose(nru, n, uu, ustart, ustart, ut, ustart, ustart);
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
        var eps = __Epsilon;
        var unfl = __MinRealValue;

        //
        // If matrix lower bidiagonal, rotate to be upper bidiagonal
        // by applying Givens rotations on the left
        //
        if (!isupper)
        {
            for (i = 1; i < n; i++)
            {
                GenerateRotation(d[i], e[i], ref cs, ref sn, ref r);
                d[i] = r;
                e[i] = sn * d[i + 1];
                d[i + 1] = cs * d[i + 1];
                work0[i] = cs;
                work1[i] = sn;
            }

            // Update singular vectors if desired
            if (nru > 0)
                ApplyRotationsFromTheLeft(fwddir, 1 + ustart - 1, n + ustart - 1, ustart, uend, work0, work1, ut, utemp);

            if (ncc > 0)
                ApplyRotationsFromTheLeft(fwddir, 1 + cstart - 1, n + cstart - 1, cstart, cend, work0, work1, c, ctemp);
        }

        // Compute singular values to relative accuracy TOL
        // (By setting TOL to be negative, algorithm will compute
        // singular values to absolute accuracy ABS(TOL)*norm(input matrix))
        var tolmul = Math.Max(10, Math.Min(100, Math.Pow(eps, -0.125)));
        var tol = tolmul * eps;

        // Compute approximate maximum, minimum singular values
        var smax = 0d;
        for (i = 1; i <= n; i++)
            smax = Math.Max(smax, Math.Abs(d[i]));

        for (i = 1; i < n; i++)
            smax = Math.Max(smax, Math.Abs(e[i]));

        var sminl = 0d;
        double mu, thresh;
        if (tol >= 0)
        {
            // Relative accuracy desired
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
        else // Absolute accuracy desired
            thresh = Math.Max(Math.Abs(tol) * smax, maxitr * n * n * unfl);

        // Prepare for main iteration loop for the singular values
        // (MAXIT is the maximum number of passes through the inner
        // loop permitted before nonconvergence signalled.)
        var maxit = maxitr * n * n;
        var iter = 0;
        var oldll = -1;
        var oldm = -1;

        // M points to last element of unconverged part of matrix
        var m = n;
        double smin;
        while (true)
        {
            // Check for convergence or exceeding iteration count
            if (m <= 1) break;

            if (iter > maxit)
            {
                result = false;
                return result;
            }

            // Find diagonal block of matrix to work on
            if (tol < 0 && Math.Abs(d[m]) <= thresh)
                d[m] = 0;

            smax = Math.Abs(d[m]);
            smin = smax;
            var matrix_split_flag = false;
            int lll;
            for (lll = 1; lll < m; lll++)
            {
                ll = m - lll;
                var abss = Math.Abs(d[ll]);
                var abse = Math.Abs(e[ll]);
                if (tol < 0 && abss <= thresh)
                    d[ll] = 0;

                if (abse <= thresh)
                {
                    matrix_split_flag = true;
                    break;
                }

                smin = Math.Min(smin, abss);
                smax = Math.Max(smax, Math.Max(abss, abse));
            }

            if (!matrix_split_flag)
                ll = 0;
            else
            {
                // Matrix splits since E(LL) = 0
                e[ll] = 0;
                if (ll == m - 1)
                {
                    // Convergence of bottom singular value, return to top of loop
                    m -= 1;
                    continue;
                }
            }
            ll += 1;

            // E(LL) through E(M-1) are nonzero, E(LL-1) is zero
            if (ll == m - 1)
            {
                // 2 by 2 block, handle separately
                SVDv2x2(d[m - 1], e[m - 1], d[m], out sigmn, out sigmx, out sin_r, out cos_r, out sin_l, out cos_l);
                d[m - 1] = sigmx;
                e[m - 1] = 0;
                d[m] = sigmn;

                int mm1, mm0;
                // Compute singular vectors, if desired
                if (ncvt > 0)
                {
                    mm0 = m + (v_start - 1);
                    mm1 = m - 1 + (v_start - 1);
                    for (i_ = v_start; i_ <= vend; i_++) vttemp[i_] = cos_r * vt[mm1, i_];
                    for (i_ = v_start; i_ <= vend; i_++) vttemp[i_] += sin_r * vt[mm0, i_];
                    for (i_ = v_start; i_ <= vend; i_++) vt[mm0, i_] = cos_r * vt[mm0, i_];
                    for (i_ = v_start; i_ <= vend; i_++) vt[mm0, i_] -= sin_r * vt[mm1, i_];
                    for (i_ = v_start; i_ <= vend; i_++) vt[mm1, i_] = vttemp[i_];
                }

                if (nru > 0)
                {
                    mm0 = m + ustart - 1;
                    mm1 = m - 1 + ustart - 1;
                    for (i_ = ustart; i_ <= uend; i_++) utemp[i_] = cos_l * ut[mm1, i_];
                    for (i_ = ustart; i_ <= uend; i_++) utemp[i_] += sin_l * ut[mm0, i_];
                    for (i_ = ustart; i_ <= uend; i_++) ut[mm0, i_] = cos_l * ut[mm0, i_];
                    for (i_ = ustart; i_ <= uend; i_++) ut[mm0, i_] -= sin_l * ut[mm1, i_];
                    for (i_ = ustart; i_ <= uend; i_++) ut[mm1, i_] = utemp[i_];
                }

                if (ncc > 0)
                {
                    mm0 = m + cstart - 1;
                    mm1 = m - 1 + cstart - 1;
                    for (i_ = cstart; i_ <= cend; i_++) ctemp[i_] = cos_l * c[mm1, i_];
                    for (i_ = cstart; i_ <= cend; i_++) ctemp[i_] += sin_l * c[mm0, i_];
                    for (i_ = cstart; i_ <= cend; i_++) c[mm0, i_] = cos_l * c[mm0, i_];
                    for (i_ = cstart; i_ <= cend; i_++) c[mm0, i_] -= sin_l * c[mm1, i_];
                    for (i_ = cstart; i_ <= cend; i_++) c[mm1, i_] = ctemp[i_];
                }

                m -= 2;
                continue;
            }

            // If working on new submatrix, choose shift direction
            // (from larger end diagonal element towards smaller)
            //
            // Previously was
            //     "if (LL>OLDM) or (M<OLDLL) then"
            // fixed thanks to Michael Rolle < m@rolle.name >
            // Very strange that LAPACK still contains it.
            var bchangedir = idir == 1 && Math.Abs(d[ll]) < 1.0E-3 * Math.Abs(d[m]) ||
                             idir == 2 && Math.Abs(d[m]) < 1.0E-3 * Math.Abs(d[ll]);

            if (ll != oldll || m != oldm || bchangedir)
                idir = Math.Abs(d[ll]) >= Math.Abs(d[m])
                    ? 1  // Chase bulge from top (big end) to bottom (small end)
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

                if (sll > 0) // Test if shift negligible, and if so set to zero
                    if ((shift / sll).Pow2() < eps)
                        shift = 0;
            }

            iter = iter + m - ll; // Increment iteration count

            if (shift == 0) // If SHIFT = 0, do simplified QR iteration
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
                        GenerateRotation(d[i] * cs, e[i], ref cs, ref sn, ref r);

                        if (i > ll)
                            e[i - 1] = oldsn * r;

                        GenerateRotation(oldcs * r, d[i + 1] * sn, ref oldcs, ref oldsn, ref tmp);

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
                    if (ncvt > 0)
                        ApplyRotationsFromTheLeft(fwddir, ll + v_start - 1, m + v_start - 1, v_start, vend, work0, work1, vt, vttemp);

                    if (nru > 0)
                        ApplyRotationsFromTheLeft(fwddir, ll + ustart - 1, m + ustart - 1, ustart, uend, work2, work3, ut, utemp);

                    if (ncc > 0)
                        ApplyRotationsFromTheLeft(fwddir, ll + cstart - 1, m + cstart - 1, cstart, cend, work2, work3, c, ctemp);

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
                        GenerateRotation(d[i] * cs, e[i - 1], ref cs, ref sn, ref r);
                        if (i < m)
                            e[i] = oldsn * r;
                        GenerateRotation(oldcs * r, d[i - 1] * sn, ref oldcs, ref oldsn, ref tmp);
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
                    if (ncvt > 0)
                        ApplyRotationsFromTheLeft(!fwddir, ll + v_start - 1, m + v_start - 1, v_start, vend, work2, work3, vt, vttemp);

                    if (nru > 0)
                        ApplyRotationsFromTheLeft(!fwddir, ll + ustart - 1, m + ustart - 1, ustart, uend, work0, work1, ut, utemp);

                    if (ncc > 0)
                        ApplyRotationsFromTheLeft(!fwddir, ll + cstart - 1, m + cstart - 1, cstart, cend, work0, work1, c, ctemp);

                    if (Math.Abs(e[ll]) <= thresh) // Test convergence
                        e[ll] = 0;
                }
            }
            else
            {
                double f, g;
                if (idir == 1) // Use nonzero shift
                {
                    // Chase bulge from top to bottom
                    // Save cosines and sines for later singular vector updates
                    f = (Math.Abs(d[ll]) - shift) * (ExtSignBDSqr(1, d[ll]) + shift / d[ll]);
                    g = e[ll];
                    for (i = ll; i < m; i++)
                    {
                        GenerateRotation(f, g, ref cos_r, ref sin_r, ref r);
                        if (i > ll)
                            e[i - 1] = r;

                        f = cos_r * d[i] + sin_r * e[i];
                        e[i] = cos_r * e[i] - sin_r * d[i];
                        g = sin_r * d[i + 1];
                        d[i + 1] = cos_r * d[i + 1];
                        GenerateRotation(f, g, ref cos_l, ref sin_l, ref r);
                        d[i] = r;
                        f = cos_l * e[i] + sin_l * d[i + 1];
                        d[i + 1] = cos_l * d[i + 1] - sin_l * e[i];

                        if (i < m - 1)
                        {
                            g = sin_l * e[i + 1];
                            e[i + 1] = cos_l * e[i + 1];
                        }

                        work0[i - ll + 1] = cos_r;
                        work1[i - ll + 1] = sin_r;
                        work2[i - ll + 1] = cos_l;
                        work3[i - ll + 1] = sin_l;
                    }
                    e[m - 1] = f;

                    // Update singular vectors
                    if (ncvt > 0)
                        ApplyRotationsFromTheLeft(fwddir, ll + v_start - 1, m + v_start - 1, v_start, vend, work0, work1, vt, vttemp);

                    if (nru > 0)
                        ApplyRotationsFromTheLeft(fwddir, ll + ustart - 1, m + ustart - 1, ustart, uend, work2, work3, ut, utemp);

                    if (ncc > 0)
                        ApplyRotationsFromTheLeft(fwddir, ll + cstart - 1, m + cstart - 1, cstart, cend, work2, work3, c, ctemp);

                    if (Math.Abs(e[m - 1]) <= thresh)  // Test convergence
                        e[m - 1] = 0;
                }
                else
                {
                    // Chase bulge from bottom to top
                    // Save cosines and sines for later singular vector updates
                    f = (Math.Abs(d[m]) - shift) * (ExtSignBDSqr(1, d[m]) + shift / d[m]);
                    g = e[m - 1];

                    for (i = m; i >= ll + 1; i--)
                    {
                        GenerateRotation(f, g, ref cos_r, ref sin_r, ref r);
                        if (i < m)
                            e[i] = r;

                        f = cos_r * d[i] + sin_r * e[i - 1];
                        e[i - 1] = cos_r * e[i - 1] - sin_r * d[i];
                        g = sin_r * d[i - 1];
                        d[i - 1] = cos_r * d[i - 1];
                        GenerateRotation(f, g, ref cos_l, ref sin_l, ref r);
                        d[i] = r;
                        f = cos_l * e[i - 1] + sin_l * d[i - 1];
                        d[i - 1] = cos_l * d[i - 1] - sin_l * e[i - 1];

                        if (i > ll + 1)
                        {
                            g = sin_l * e[i - 2];
                            e[i - 2] = cos_l * e[i - 2];
                        }

                        work0[i - ll] = +cos_r;
                        work1[i - ll] = -sin_r;
                        work2[i - ll] = +cos_l;
                        work3[i - ll] = -sin_l;
                    }
                    e[ll] = f;

                    // Test convergence
                    if (Math.Abs(e[ll]) <= thresh)
                        e[ll] = 0;

                    // Update singular vectors if desired
                    if (ncvt > 0)
                        ApplyRotationsFromTheLeft(!fwddir, ll + v_start - 1, m + v_start - 1, v_start, vend, work2, work3, vt, vttemp);

                    if (nru > 0)
                        ApplyRotationsFromTheLeft(!fwddir, ll + ustart - 1, m + ustart - 1, ustart, uend, work0, work1, ut, utemp);

                    if (ncc > 0)
                        ApplyRotationsFromTheLeft(!fwddir, ll + cstart - 1, m + cstart - 1, cstart, cend, work0, work1, c, ctemp);
                }
            }
        } // QR iteration finished, go back and check convergence

        for (i = 1; i <= n; i++) // All singular values converged, so make them positive
            if (d[i] < 0)
            {
                d[i] = -d[i];

                if (ncvt <= 0)  // Change sign of singular vectors, if desired
                    continue;

                for (i_ = v_start; i_ <= vend; i_++)
                    vt[i + v_start - 1, i_] = -vt[i + v_start - 1, i_];
            }

        // Sort the singular values into decreasing order (insertion sort on singular values, but only one transposition per singular vector)
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

            if (isub == n + 1 - i)
                continue;

            // Swap singular values and vectors
            d[isub] = d[n + 1 - i];
            d[n + 1 - i] = smin;

            if (ncvt > 0)
            {
                j = n + 1 - i;
                for (i_ = v_start; i_ <= vend; i_++)
                    vttemp[i_] = vt[isub + v_start - 1, i_];

                for (i_ = v_start; i_ <= vend; i_++)
                    vt[isub + v_start - 1, i_] = vt[j + v_start - 1, i_];

                for (i_ = v_start; i_ <= vend; i_++)
                    vt[j + v_start - 1, i_] = vttemp[i_];
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

            if (ncc <= 0)
                continue;

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
            MatrixTranspose(n, nru, ut, ustart, ustart, uu, ustart, ustart);

        return result;
    }

    private static void SVDv2x2(double f, double g, double h, out double ss_min, out double ss_max, out double sn_r, out double cs_r, out double sn_l, out double cs_l)
    {
        ss_min = 0;
        ss_max = 0;
        var ft = f;
        var fa = Math.Abs(ft);
        var ht = h;
        var ha = Math.Abs(h);

        // these initializers are not really necessary,
        // but without them compiler complains about uninitialized locals
        double cl_t = 0;
        double cr_t = 0;
        double sl_t = 0;
        double sr_t = 0;
        double t_sign = 0;

        //  PMAX points to the maximum absolute element of matrix
        //  PMAX = 1 if F largest in absolute values
        //  PMAX = 2 if G largest in absolute values
        //  PMAX = 3 if H largest in absolute values
        var p_max = 1;
        var need_swap = ha > fa;
        if (need_swap)
        {
            // Now FA .ge. HA
            p_max = 3;
            (ht, ft) = (ft, ht);
            (ha, fa) = (fa, ha);
        }

        var ga = Math.Abs(g);
        if (ga == 0)
        {
            // Diagonal matrix
            ss_min = ha;
            ss_max = fa;
            cl_t = 1;
            cr_t = 1;
            sl_t = 0;
            sr_t = 0;
        }
        else
        {
            var gasmal = true;
            double v;
            if (ga > fa)
            {
                p_max = 2;
                if (fa / ga < __Epsilon)
                {
                    // Case of very large GA
                    gasmal = false;
                    ss_max = ga;
                    if (ha > 1)
                    {
                        v = ga / ha;
                        ss_min = fa / v;
                    }
                    else
                    {
                        v = fa / ga;
                        ss_min = v * ha;
                    }
                    cl_t = 1;
                    sl_t = ht / g;
                    sr_t = 1;
                    cr_t = ft / g;
                }
            }

            if (gasmal) // Normal case
            {
                var d = fa - ha;
                double l;

                l = d == fa ? 1 : d / fa;

                var m = g / ft;
                var t = 2 - l;
                var mm = m * m;
                var tt = t * t;
                var s = Math.Sqrt(tt + mm);

                var r = l == 0 ? Math.Abs(m) : Math.Sqrt(l * l + mm);

                var a = 0.5 * (s + r);
                ss_min = ha / a;
                ss_max = fa * a;
                if (mm == 0)
                {
                    // Note that M is very tiny
                    t = l == 0
                        ? ExtSignBDSqr(2, ft) * ExtSignBDSqr(1, g)
                        : g / ExtSignBDSqr(d, ft) + m / t;
                }
                else
                    t = (m / (s + t) + m / (r + l)) * (1 + a);

                l = Math.Sqrt(t * t + 4);
                cr_t = 2 / l;
                sr_t = t / l;
                cl_t = (cr_t + sr_t * m) / a;
                v = ht / ft;
                sl_t = v * sr_t / a;
            }
        }

        if (need_swap)
            (cs_l, sn_l, cs_r, sn_r) = (sr_t, cr_t, sl_t, cl_t);
        else
            (cs_l, sn_l, cs_r, sn_r) = (cl_t, sl_t, cr_t, sr_t);

        t_sign = p_max switch // Correct signs of SSMAX and SSMIN
        {
            1 => ExtSignBDSqr(1, cs_r) * ExtSignBDSqr(1, cs_l) * ExtSignBDSqr(1, f),
            2 => ExtSignBDSqr(1, sn_r) * ExtSignBDSqr(1, cs_l) * ExtSignBDSqr(1, g),
            3 => ExtSignBDSqr(1, sn_r) * ExtSignBDSqr(1, sn_l) * ExtSignBDSqr(1, h),
            _ => t_sign
        };

        ss_max = ExtSignBDSqr(ss_max, t_sign);
        ss_min = ExtSignBDSqr(ss_min, t_sign * ExtSignBDSqr(1, f) * ExtSignBDSqr(1, h));
    }

    private static void svd2x2(double f, double g, double h, ref double ss_min, ref double ss_max)
    {
        var f_abs = Math.Abs(f);
        var g_abs = Math.Abs(g);
        var h_abs = Math.Abs(h);
        var fh_min = Math.Min(f_abs, h_abs);
        var fh_max = Math.Max(f_abs, h_abs);

        if (fh_min == 0)
        {
            ss_min = 0;
            ss_max = fh_max == 0
                ? g_abs
                : Math.Max(fh_max, g_abs) * (1 + (Math.Min(fh_max, g_abs) / Math.Max(fh_max, g_abs)).Pow2()).Sqrt();
        }
        else
        {
            double aas, at, au, c;
            if (g_abs < fh_max)
            {
                aas = 1 + fh_min / fh_max;
                at = (fh_max - fh_min) / fh_max;
                au = (g_abs / fh_max).Pow2();
                c = 2 / (Math.Sqrt(aas * aas + au) + Math.Sqrt(at * at + au));
                ss_min = fh_min * c;
                ss_max = fh_max / c;
            }
            else
            {
                au = fh_max / g_abs;
                if (au == 0)
                {
                    // Avoid possible harmful underflow if exponent range
                    // asymmetric (true SSMIN may not underflow even if
                    // AU underflows)
                    ss_min = fh_min * fh_max / g_abs;
                    ss_max = g_abs;
                }
                else
                {
                    aas = 1 + fh_min / fh_max;
                    at = (fh_max - fh_min) / fh_max;
                    c = 1 / (Math.Sqrt(1 + (aas * au).Pow2()) + Math.Sqrt(1 + (at * au).Pow2()));
                    ss_min = fh_min * c * au;
                    ss_min += ss_min;
                    ss_max = g_abs / (c + c);
                }
            }
        }
    }

    private static void GenerateRotation(double f, double g, ref double cs, ref double sn, ref double r)
    {
        if (g == 0)
            (cs, sn, r) = (1, 0, f);
        else if (f == 0)
            (cs, sn, r) = (0, 1, g);
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

    private static double ExtSignBDSqr(double a, double b) => b >= 0 ? Math.Abs(a) : -Math.Abs(a);

    private static void ApplyRotationsFromTheLeft(
        bool IsForward,
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

        int j, jp1, i_;
        double c_tmp, s_tmp, tmp;

        if (IsForward) // Form  P * A
        {
            if (n1 != n2) // Common case: N1!=N2
                for (j = m1; j < m2; j++)
                {
                    c_tmp = c[j - m1 + 1];
                    s_tmp = s[j - m1 + 1];

                    if (c_tmp == 1 && s_tmp == 0)
                        continue;

                    jp1 = j + 1;
                    for (i_ = n1; i_ <= n2; i_++)
                        work[i_] = c_tmp * a[jp1, i_];

                    for (i_ = n1; i_ <= n2; i_++)
                        work[i_] -= s_tmp * a[j, i_];

                    for (i_ = n1; i_ <= n2; i_++)
                        a[j, i_] *= c_tmp;

                    for (i_ = n1; i_ <= n2; i_++)
                        a[j, i_] += s_tmp * a[jp1, i_];

                    for (i_ = n1; i_ <= n2; i_++)
                        a[jp1, i_] = work[i_];
                }
            else // Special case: N1==N2
                for (j = m1; j < m2; j++)
                {
                    c_tmp = c[j - m1 + 1];
                    s_tmp = s[j - m1 + 1];

                    if (c_tmp == 1 && s_tmp == 0)
                        continue;

                    tmp = a[j + 1, n1];
                    a[j + 1, n1] = c_tmp * tmp - s_tmp * a[j, n1];
                    a[j, n1] = s_tmp * tmp + c_tmp * a[j, n1];
                }
        }
        else
        {
            if (n1 != n2)// Common case: N1!=N2
                for (j = m2 - 1; j >= m1; j--)
                {
                    c_tmp = c[j - m1 + 1];
                    s_tmp = s[j - m1 + 1];

                    if (c_tmp == 1 && s_tmp == 0)
                        continue;

                    jp1 = j + 1;
                    for (i_ = n1; i_ <= n2; i_++)
                        work[i_] = c_tmp * a[jp1, i_];

                    for (i_ = n1; i_ <= n2; i_++)
                        work[i_] -= s_tmp * a[j, i_];

                    for (i_ = n1; i_ <= n2; i_++)
                        a[j, i_] *= c_tmp;

                    for (i_ = n1; i_ <= n2; i_++)
                        a[j, i_] += s_tmp * a[jp1, i_];

                    for (i_ = n1; i_ <= n2; i_++)
                        a[jp1, i_] = work[i_];
                }
            else // Special case: N1==N2
                for (j = m2 - 1; j >= m1; j--)
                {
                    c_tmp = c[j - m1 + 1];
                    s_tmp = s[j - m1 + 1];

                    if (c_tmp == 1 && s_tmp == 0)
                        continue;

                    tmp = a[j + 1, n1];
                    a[j + 1, n1] = c_tmp * tmp - s_tmp * a[j, n1];
                    a[j, n1] = s_tmp * tmp + c_tmp * a[j, n1];
                }
        }
    }

    private static void MatrixTranspose(int m, int n, double[,] a, int ia, int ja, double[,] b, int ib, int jb)
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
            AbLasspLitLength(a, m, out s1, out s2);
            MatrixTranspose(s1, n, a, ia, ja, b, ib, jb);
            MatrixTranspose(s2, n, a, ia + s1, ja, b, ib, jb + s1);
        }
        else
        {
            AbLasspLitLength(a, n, out s1, out s2);
            MatrixTranspose(m, s1, a, ia, ja, b, ib, jb);
            MatrixTranspose(m, s2, a, ia, ja + s1, b, ib + s1, jb);
        }
    }

    private static void AbLasspLitLength(double[,] a, int n, out int n1, out int n2)
    {
        n1 = 0;
        n2 = 0;

        if (n > 32)
            AbLasInternalSplitLength(n, 32, ref n1, ref n2);
        else
            AbLasInternalSplitLength(n, 8, ref n1, ref n2);
    }

    private static void AbLasInternalSplitLength(int n, int nb, ref int n1, ref int n2)
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
        else // Split on block boundaries
        {
            n2 = n / 2;
            n1 = n - n2;

            if (n1 % nb == 0)
                return;

            var r = nb - n1 % nb;
            n1 += r;
            n2 -= r;
        }
    }

    private static void rmatrixbdunpackdiagonals(double[,] b, int m, int n, out bool IsUpper, out double[] d, out double[] e)
    {
        IsUpper = m >= n;
        if (m <= 0 || n <= 0)
        {
            d = [];
            e = [];
            return;
        }

        int i;
        if (IsUpper)
        {
            d = new double[n];
            e = new double[n];
            for (i = 0; i < n - 1; i++)
                (d[i], e[i]) = (b[i, i], b[i, i + 1]);

            d[n - 1] = b[n - 1, n - 1];
        }
        else
        {
            d = new double[m];
            e = new double[m];
            for (i = 0; i < m - 1; i++)
                (d[i], e[i]) = (b[i, i], b[i + 1, i]);

            d[m - 1] = b[m - 1, m - 1];
        }
    }

    private static void MatrixBDUnpackPT(double[,] qp, int m, int n, double[] TaUp, int PTRows, out double[,] pt)
    {
        //alglib.ap.assert(PTRows <= n, "MatrixBDUnpackPT: PTRows>N!");
        //alglib.ap.assert(PTRows >= 0, "MatrixBDUnpackPT: PTRows<0!");
        if (m == 0 || n == 0 || PTRows == 0)
        {
            pt = new double[0, 0];
            return;
        }

        // prepare PT
        pt = new double[PTRows, n];
        for (var i = 0; i < PTRows; i++)
            for (var j = 0; j < n; j++)
                pt[i, j] = i == j ? 1 : 0;

        // Calculate
        MatrixBDMultiplyByP(qp, m, n, TaUp, pt, PTRows, n, true, true);
    }

    private static void MatrixBDMultiplyByP(
        double[,] qp,
        int m,
        int n,
        double[] TaUp,
        double[,] z,
        int ZRows,
        int ZColumns,
        bool FromTheRight,
        bool DoTranspose)
    {
        if (m <= 0 || n <= 0 || ZRows <= 0 || ZColumns <= 0)
            return;
        //alglib.ap.assert(FromTheRight && ZColumns == n || !FromTheRight && ZRows == n, "RMatrixBDMultiplyByP: incorrect Z size!");

        var mx = Math.Max(m, n);
        mx = Math.Max(mx, ZRows);
        mx = Math.Max(mx, ZColumns);
        var v = new double[mx + 1];
        var work = new double[mx + 1];
        int i, i1, i2, i_step, i_, i1_;

        if (m >= n)
        {
            if (FromTheRight)
            {
                i1 = n - 2;
                i2 = 0;
                i_step = -1;
            }
            else
            {
                i1 = 0;
                i2 = n - 2;
                i_step = 1;
            }

            if (!DoTranspose)
            {
                i = i1;
                i1 = i2;
                i2 = i;
                i_step = -i_step;
            }

            if (n - 1 <= 0) return;

            i = i1;
            do
            {
                i1_ = i + 1 - 1;
                for (i_ = 1; i_ < n - i; i_++)
                    v[i_] = qp[i, i_ + i1_];

                v[1] = 1;
                if (FromTheRight)
                    ApplyReflectionFromTheRight(z, TaUp[i], v, 0, ZRows - 1, i + 1, n - 1, ref work);
                else
                    ApplyReflectionFromTheLeft(z, TaUp[i], v, i + 1, n - 1, 0, ZColumns - 1, ref work);
                i += i_step;
            }
            while (i != i2 + i_step);
        }
        else
        {
            if (FromTheRight)
            {
                i1 = m - 1;
                i2 = 0;
                i_step = -1;
            }
            else
            {
                i1 = 0;
                i2 = m - 1;
                i_step = 1;
            }

            if (!DoTranspose)
            {
                i = i1;
                i1 = i2;
                i2 = i;
                i_step = -i_step;
            }

            i = i1;
            do
            {
                i1_ = i - 1;
                for (i_ = 1; i_ <= n - i; i_++)
                    v[i_] = qp[i, i_ + i1_];

                v[1] = 1;
                if (FromTheRight)
                    ApplyReflectionFromTheRight(z, TaUp[i], v, 0, ZRows - 1, i, n - 1, ref work);
                else
                    ApplyReflectionFromTheLeft(z, TaUp[i], v, i, n - 1, 0, ZColumns - 1, ref work);

                i += i_step;
            }
            while (i != i2 + i_step);
        }
    }

    private static void MatrixBDUnpackQ(double[,] qp, int m, int n, double[] tauq, int QColumns, out double[,] q)
    {
        //alglib.ap.assert(QColumns <= m, "RMatrixBDUnpackQ: QColumns>M!");
        //alglib.ap.assert(QColumns >= 0, "RMatrixBDUnpackQ: QColumns<0!");

        if (m == 0 || n == 0 || QColumns == 0)
        {
            q = new double[0, 0];
            return;
        }

        q = new double[m, QColumns];
        for (var i = 0; i < m; i++)
            for (var j = 0; j < QColumns; j++)
                q[i, j] = i == j ? 1 : 0;

        MatrixBDMultiplyByQ(qp, m, n, tauq, q, m, QColumns, false, false);
    }

    private static void MatrixBDMultiplyByQ(double[,] qp,
        int m,
        int n,
        double[] tauq,
        double[,] z,
        int ZRows,
        int ZColumns,
        bool FromTheRight,
        bool DoTranspose)
    {
        if (m <= 0 || n <= 0 || ZRows <= 0 || ZColumns <= 0)
            return;

        //alglib.ap.assert(FromTheRight && ZColumns == m || !FromTheRight && ZRows == m, "RMatrixBDMultiplyByQ: incorrect Z size!");

        var mx = Math.Max(m, n);
        mx = Math.Max(mx, ZRows);
        mx = Math.Max(mx, ZColumns);

        var v = new double[mx + 1];
        var work = new double[mx + 1];

        int i, i_, i1_, i1, i2, i_step;
        if (m >= n)
        {
            if (FromTheRight)
            {
                i1 = 0;
                i2 = n - 1;
                i_step = 1;
            }
            else
            {
                i1 = n - 1;
                i2 = 0;
                i_step = -1;
            }
            if (DoTranspose)
            {
                i = i1;
                i1 = i2;
                i2 = i;
                i_step = -i_step;
            }

            i = i1;
            do
            {
                i1_ = i - 1;
                for (i_ = 1; i_ <= m - i; i_++)
                    v[i_] = qp[i_ + i1_, i];

                v[1] = 1;
                if (FromTheRight)
                    ApplyReflectionFromTheRight(z, tauq[i], v, 0, ZRows - 1, i, m - 1, ref work);
                else
                    ApplyReflectionFromTheLeft(z, tauq[i], v, i, m - 1, 0, ZColumns - 1, ref work);

                i += i_step;
            }
            while (i != i2 + i_step);
        }
        else
        {
            if (FromTheRight)
            {
                i1 = 0;
                i2 = m - 2;
                i_step = 1;
            }
            else
            {
                i1 = m - 2;
                i2 = 0;
                i_step = -1;
            }

            if (DoTranspose)
            {
                i = i1;
                i1 = i2;
                i2 = i;
                i_step = -i_step;
            }

            if (m - 1 <= 0) return;

            i = i1;
            do
            {
                i1_ = i + 1 - 1;
                for (i_ = 1; i_ < m - i; i_++)
                    v[i_] = qp[i_ + i1_, i];

                v[1] = 1;
                if (FromTheRight)
                    ApplyReflectionFromTheRight(z, tauq[i], v, 0, ZRows - 1, i + 1, m - 1, ref work);
                else
                    ApplyReflectionFromTheLeft(z, tauq[i], v, i + 1, m - 1, 0, ZColumns - 1, ref work);
                i += i_step;
            }
            while (i != i2 + i_step);
        }
    }

    private static void MatrixBD(double[,] a, int m, int n, ref double[] TauQ, ref double[] TauP)
    {
        if (n <= 0 || m <= 0)
        {
            TauQ = [];
            TauP = [];
            return;
        }

        var max_mn = Math.Max(m, n);
        var work = new double[max_mn + 1];
        var t = new double[max_mn + 1];
        int i;

        if (m >= n)
        {
            TauQ = new double[n];
            TauP = new double[n];
            for (i = 0; i < n; i++)
                (TauQ[i], TauP[i]) = (0, 0);
        }
        else
        {
            TauQ = new double[m];
            TauP = new double[m];
            for (i = 0; i < m; i++)
                (TauQ[i], TauP[i]) = (0, 0);
        }

        double l_tau;
        int i_, i1_;
        if (m >= n) // Reduce to upper bidiagonal form
            for (i = 0; i < n; i++)
            {
                // Generate elementary reflector H(i) to annihilate A(i+1:m-1,i)
                i1_ = i - 1;
                for (i_ = 1; i_ <= m - i; i_++)
                    t[i_] = a[i_ + i1_, i];

                GenerateReflection(ref t, m - i, out l_tau);

                TauQ[i] = l_tau;
                i1_ = 1 - i;

                for (i_ = i; i_ < m; i_++)
                    a[i_, i] = t[i_ + i1_];

                t[1] = 1;

                // Apply H(i) to A(i:m-1,i+1:n-1) from the left
                ApplyReflectionFromTheLeft(a, l_tau, t, i, m - 1, i + 1, n - 1, ref work);
                if (i < n - 1)
                {
                    // Generate elementary reflector G(i) to annihilate
                    // A(i,i+2:n-1)
                    i1_ = i + 1 - 1;
                    for (i_ = 1; i_ < n - i; i_++)
                        t[i_] = a[i, i_ + i1_];

                    GenerateReflection(ref t, n - 1 - i, out l_tau);

                    TauP[i] = l_tau;
                    i1_ = 1 - (i + 1);
                    for (i_ = i + 1; i_ < n; i_++)
                        a[i, i_] = t[i_ + i1_];

                    t[1] = 1;

                    // Apply G(i) to A(i+1:m-1,i+1:n-1) from the right
                    ApplyReflectionFromTheRight(a, l_tau, t, i + 1, m - 1, i + 1, n - 1, ref work);
                }
                else
                    TauP[i] = 0;
            }
        else // Reduce to lower bidiagonal form
            for (i = 0; i < m; i++)
            {
                // Generate elementary reflector G(i) to annihilate A(i,i+1:n-1)
                i1_ = i - 1;
                for (i_ = 1; i_ <= n - i; i_++)
                    t[i_] = a[i, i_ + i1_];

                GenerateReflection(ref t, n - i, out l_tau);
                TauP[i] = l_tau;
                i1_ = 1 - i;

                for (i_ = i; i_ < n; i_++)
                    a[i, i_] = t[i_ + i1_];
                t[1] = 1;

                // Apply G(i) to A(i+1:m-1,i:n-1) from the right
                ApplyReflectionFromTheRight(a, l_tau, t, i + 1, m - 1, i, n - 1, ref work);
                if (i < m - 1)
                {
                    // Generate elementary reflector H(i) to annihilate
                    // A(i+2:m-1,i)
                    i1_ = i + 1 - 1;
                    for (i_ = 1; i_ < m - i; i_++)
                        t[i_] = a[i_ + i1_, i];

                    GenerateReflection(ref t, m - 1 - i, out l_tau);
                    TauQ[i] = l_tau;
                    i1_ = 1 - (i + 1);
                    for (i_ = i + 1; i_ < m; i_++) a[i_, i] = t[i_ + i1_];
                    t[1] = 1;

                    // Apply H(i) to A(i+1:m-1,i+1:n-1) from the left
                    ApplyReflectionFromTheLeft(a, l_tau, t, i + 1, m - 1, i + 1, n - 1, ref work);
                }
                else
                    TauQ[i] = 0;
            }
    }

    private static void MatrixQR(double[,] a, out double[] Tau)
    {
        var m = a.GetLength(0);
        var n = a.GetLength(1);

        if (m <= 0 || n <= 0)
        {
            Tau = [];
            return;
        }

        var min_mn = Math.Min(m, n);
        const int ts = __MatrixTileSizeB;
        var work = new double[Math.Max(m, n) + 1];
        var t = new double[Math.Max(m, n) + 1];
        Tau = new double[min_mn];
        var tau_buffer = new double[min_mn];
        var tmp_a = new double[m, ts];
        var tmp_t = new double[ts, 2 * ts];
        var tmp_r = new double[2 * ts, n];

        // Blocked code
        var block_start = 0;
        while (block_start != min_mn)
        {
            var block_size = min_mn - block_start; // Determine block size
            if (block_size > ts)
                block_size = ts;
            var rows_count = m - block_start;

            // QR decomposition of submatrix.
            // Matrix is copied to temporary storage to solve
            // some TLB issues arising from non-contiguous memory
            // access pattern.
            MatrixCopy(rows_count, block_size, a, block_start, block_start, tmp_a, 0, 0);
            MatrixQRBaseCase(ref tmp_a, rows_count, block_size, ref work, ref t, ref tau_buffer);
            MatrixCopy(rows_count, block_size, tmp_a, 0, 0, a, block_start, block_start);

            var i1_ = 0 - block_start;
            int i_;
            for (i_ = block_start; i_ < block_start + block_size; i_++)
                Tau[i_] = tau_buffer[i_ + i1_];

            // Update the rest, choose between:
            // a) Level 2 algorithm (when the rest of the matrix is small enough)
            // b) blocked algorithm, see algorithm 5 from  'A storage efficient WY
            //    representation for products of Householder transformations',
            //    by R. Schreiber and C. Van Loan.
            if (block_start + block_size <= n - 1)
            {
                if (n - block_start - block_size >= 2 * ts || rows_count >= 4 * ts)
                {
                    // Prepare block reflector
                    MatrixBlockReflector(ref tmp_a, ref tau_buffer, true, rows_count, block_size, ref tmp_t, ref work);

                    // Multiply the rest of A by Q'.
                    //
                    // Q  = E + Y*T*Y'  = E + TmpA*TmpT*TmpA'
                    // Q' = E + Y*T'*Y' = E + TmpA*TmpT'*TmpA'
                    MatrixGEMM(block_size, n - block_start - block_size, rows_count, 1.0, tmp_a, 0, 0, 1, a, block_start, block_start + block_size, 0, 0.0, tmp_r, 0, 0);
                    MatrixGEMM(block_size, n - block_start - block_size, block_size, 1.0, tmp_t, 0, 0, 1, tmp_r, 0, 0, 0, 0.0, tmp_r, block_size, 0);
                    MatrixGEMM(rows_count, n - block_start - block_size, block_size, 1.0, tmp_a, 0, 0, 0, tmp_r, block_size, 0, 0, 1.0, a, block_start, block_start + block_size);
                }
                else // Level 2 algorithm
                    for (var i = 0; i < block_size; i++)
                    {
                        i1_ = i - 1;
                        for (i_ = 1; i_ <= rows_count - i; i_++)
                            t[i_] = tmp_a[i_ + i1_, i];
                        t[1] = 1;

                        ApplyReflectionFromTheLeft(a, tau_buffer[i], t, block_start + i, m - 1, block_start + block_size, n - 1, ref work);
                    }
            }

            // Advance
            block_start += block_size;
        }
    }

    private static void MatrixBlockReflector(
        ref double[,] a,
        ref double[] tau,
        bool ColumnWisea,
        int LengthA,
        int BlockSize,
        ref double[,] t,
        ref double[] work)
    {
        // fill beginning of new column with zeros,
        // load 1.0 in the first non-zero element
        int i, j, k;
        for (k = 0; k < BlockSize; k++)
        {
            if (ColumnWisea)
                for (i = 0; i < k; i++)
                    a[i, k] = 0;
            else
                for (i = 0; i < k; i++)
                    a[k, i] = 0;

            a[k, k] = 1;
        }

        // Calculate Gram matrix of A
        for (i = 0; i < BlockSize; i++)
            for (j = 0; j < BlockSize; j++)
                t[i, BlockSize + j] = 0;

        int i_, i1_;
        double v;
        for (k = 0; k < LengthA; k++)
            for (j = 1; j < BlockSize; j++)
                if (ColumnWisea)
                {
                    v = a[k, j];
                    if (v == 0) continue;

                    i1_ = 0 - BlockSize;
                    for (i_ = BlockSize; i_ < BlockSize + j; i_++)
                        t[j, i_] += v * a[k, i_ + i1_];
                }
                else
                {
                    v = a[j, k];
                    if (v == 0) continue;

                    i1_ = 0 - BlockSize;
                    for (i_ = BlockSize; i_ < BlockSize + j; i_++)
                        t[j, i_] += v * a[i_ + i1_, k];
                }

        // Prepare Y (stored in TmpA) and T (stored in TmpT)
        for (k = 0; k < BlockSize; k++)
        {
            // fill non-zero part of T, use pre-calculated Gram matrix
            i1_ = BlockSize - 0;
            for (i_ = 0; i_ < k; i_++)
                work[i_] = t[k, i_ + i1_];

            for (i = 0; i < k; i++)
            {
                v = 0;
                for (i_ = i; i_ < k; i_++)
                    v += t[i, i_] * work[i_];

                t[i, k] = -(tau[k] * v);
            }

            t[k, k] = -tau[k];

            // Rest of T is filled by zeros
            for (i = k + 1; i < BlockSize; i++)
                t[i, k] = 0;
        }
    }

    private static void MatrixGEMM(
        int m, int n, int k,
        double alpha, double[,] a,
        int ia, int ja, int OptyPeA,
        double[,] b,
        int ib, int jb, int OptyPeB,
        double beta, double[,] c,
        int ic, int jc)
    {
        //alglib.ap.assert(OptyPeA == 0 || OptyPeA == 1, "RMatrixGEMM: incorrect OpTypeA (must be 0 or 1)");
        //alglib.ap.assert(OptyPeB == 0 || OptyPeB == 1, "RMatrixGEMM: incorrect OpTypeB (must be 0 or 1)");
        //alglib.ap.assert(ic + m <= alglib.ap.rows(c), "RMatrixGEMM: incorect size of output matrix C");
        //alglib.ap.assert(jc + n <= alglib.ap.cols(c), "RMatrixGEMM: incorect size of output matrix C");

        MatrixGEMMRec(m, n, k, alpha, a, ia, ja, OptyPeA, b, ib, jb, OptyPeB, beta, c, ic, jc);
    }

    private static void MatrixGEMMRec(
        int m,
        int n,
        int k,
        double alpha,
        double[,] a,
        int ia,
        int ja,
        int PptyPeA,
        double[,] b,
        int ib,
        int jb,
        int OptyPeB,
        double beta,
        double[,] c,
        int ic,
        int jc)
    {
        var s1 = 0;
        var s2 = 0;

        const int ts_a = __MatrixTileSizeA;
        const int ts_b = __MatrixTileSizeB;

        var tscur = ts_b;
        if (Max3(m, n, k) <= ts_b)
            tscur = ts_a;

        //alglib.ap.assert(tscur >= 1, "RMatrixGEMMRec: integrity check failed");

        if (m <= ts_a && n <= ts_a && k <= ts_a)
        {
            MatrixGEMMk(m, n, k, alpha, a, ia, ja, PptyPeA, b, ib, jb, OptyPeB, beta, c, ic, jc);
            return;
        }

        // Recursive algorithm: split on M or N
        if (m >= n && m >= k)
        {
            // A*B = (A1 A2)^T*B
            TiledSplit(m, tscur, ref s1, ref s2);
            if (PptyPeA == 0)
            {
                MatrixGEMMRec(s2, n, k, alpha, a, ia + s1, ja, PptyPeA, b, ib, jb, OptyPeB, beta, c, ic + s1, jc);
                MatrixGEMMRec(s1, n, k, alpha, a, ia, ja, PptyPeA, b, ib, jb, OptyPeB, beta, c, ic, jc);
            }
            else
            {
                MatrixGEMMRec(s2, n, k, alpha, a, ia, ja + s1, PptyPeA, b, ib, jb, OptyPeB, beta, c, ic + s1, jc);
                MatrixGEMMRec(s1, n, k, alpha, a, ia, ja, PptyPeA, b, ib, jb, OptyPeB, beta, c, ic, jc);
            }
            return;
        }

        if (n >= m && n >= k)
        {
            // A*B = A*(B1 B2)
            TiledSplit(n, tscur, ref s1, ref s2);
            if (OptyPeB == 0)
            {
                MatrixGEMMRec(m, s2, k, alpha, a, ia, ja, PptyPeA, b, ib, jb + s1, OptyPeB, beta, c, ic, jc + s1);
                MatrixGEMMRec(m, s1, k, alpha, a, ia, ja, PptyPeA, b, ib, jb, OptyPeB, beta, c, ic, jc);
            }
            else
            {
                MatrixGEMMRec(m, s2, k, alpha, a, ia, ja, PptyPeA, b, ib + s1, jb, OptyPeB, beta, c, ic, jc + s1);
                MatrixGEMMRec(m, s1, k, alpha, a, ia, ja, PptyPeA, b, ib, jb, OptyPeB, beta, c, ic, jc);
            }
            return;
        }

        // Recursive algorithm: split on K

        // A*B = (A1 A2)*(B1 B2)^T
        TiledSplit(k, tscur, ref s1, ref s2);
        if (PptyPeA == 0 && OptyPeB == 0)
        {
            MatrixGEMMRec(m, n, s1, alpha, a, ia, ja, PptyPeA, b, ib, jb, OptyPeB, beta, c, ic, jc);
            MatrixGEMMRec(m, n, s2, alpha, a, ia, ja + s1, PptyPeA, b, ib + s1, jb, OptyPeB, 1.0, c, ic, jc);
        }

        if (PptyPeA == 0 && OptyPeB != 0)
        {
            MatrixGEMMRec(m, n, s1, alpha, a, ia, ja, PptyPeA, b, ib, jb, OptyPeB, beta, c, ic, jc);
            MatrixGEMMRec(m, n, s2, alpha, a, ia, ja + s1, PptyPeA, b, ib, jb + s1, OptyPeB, 1.0, c, ic, jc);
        }

        if (PptyPeA != 0 && OptyPeB == 0)
        {
            MatrixGEMMRec(m, n, s1, alpha, a, ia, ja, PptyPeA, b, ib, jb, OptyPeB, beta, c, ic, jc);
            MatrixGEMMRec(m, n, s2, alpha, a, ia + s1, ja, PptyPeA, b, ib + s1, jb, OptyPeB, 1.0, c, ic, jc);
        }

        if (PptyPeA != 0 && OptyPeB != 0)
        {
            MatrixGEMMRec(m, n, s1, alpha, a, ia, ja, PptyPeA, b, ib, jb, OptyPeB, beta, c, ic, jc);
            MatrixGEMMRec(m, n, s2, alpha, a, ia + s1, ja, PptyPeA, b, ib, jb + s1, OptyPeB, 1.0, c, ic, jc);
        }
    }

    private static void TiledSplit(int TaskSize, int TileSize, ref int Task0, ref int Task1)
    {
        //Task0 = 0;
        //Task1 = 0;

        //alglib.ap.assert(TaskSize >= 2, "TiledSplit: TaskSize<2");
        //alglib.ap.assert(TaskSize > TileSize, "TiledSplit: TaskSize<=TileSize");
        var cc = ChunksCount(TaskSize, TileSize);
        //alglib.ap.assert(cc >= 2, "TiledSplit: integrity check failed");
        Task0 = DivUp(cc, 2) * TileSize;
        Task1 = TaskSize - Task0;
        //alglib.ap.assert(Task0 >= 1, "TiledSplit: internal error");
        //alglib.ap.assert(Task1 >= 1, "TiledSplit: internal error");
        //alglib.ap.assert(Task0 % TileSize == 0, "TiledSplit: internal error");
        //alglib.ap.assert(Task0 >= Task1, "TiledSplit: internal error");
    }

    private static int ChunksCount(int TaskSize, int ChunkSize)
    {
        //alglib.ap.assert(TaskSize >= 0, "ChunksCount: TaskSize<0");
        //alglib.ap.assert(ChunkSize >= 1, "ChunksCount: ChunkSize<1");
        return TaskSize % ChunkSize != 0 ? TaskSize / ChunkSize + 1 : TaskSize / ChunkSize;
    }

    private static int DivUp(int a, int b) => a % b > 0 ? a / b + 1 : a / b;

    private static void MatrixGEMMk(
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
            MatrixGEMMk44v00(m, n, k, alpha, a, ia, ja, b, ib, jb, beta, c, ic, jc);

        if (optypea == 0 && optypeb != 0)
            MatrixGEMM44v01(m, n, k, alpha, a, ia, ja, b, ib, jb, beta, c, ic, jc);

        if (optypea != 0 && optypeb == 0)
            MatrixGEMMk44v10(m, n, k, alpha, a, ia, ja, b, ib, jb, beta, c, ic, jc);

        if (optypea != 0 && optypeb != 0)
            MatrixGEMMk44v11(m, n, k, alpha, a, ia, ja, b, ib, jb, beta, c, ic, jc);
    }

    private static void MatrixGEMMk44v00(
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
                            c[ic + ik, jc + jk] = beta == 0 
                                ? alpha * v 
                                : beta * c[ic + ik, jc + jk] + alpha * v;
                        }
                }
                j += 4;
            }
            i += 4;
        }
    }

    private static void MatrixGEMM44v01(
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
                                for (var i_ = ja; i_ <= ja + k - 1; i_++) 
                                    v += a[ia + ik, i_] * b[ib + jk, i_ + i1_];
                            }

                            c[ic + ik, jc + jk] = beta == 0 
                                ? alpha * v 
                                : beta * c[ic + ik, jc + jk] + alpha * v;
                        }
                }
                j += 4;
            }
            i += 4;
        }
    }

    private static void MatrixGEMMk44v10(
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
                            c[ic + ik, jc + jk] = beta == 0 
                                ? alpha * v 
                                : beta * c[ic + ik, jc + jk] + alpha * v;
                        }
                }
                j += 4;
            }
            i += 4;
        }
    }

    private static void MatrixGEMMk44v11(int m,
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
                                v = 0;

                                for (var i_ = ia; i_ < ia + k; i_++)
                                    v += a[i_, ja + ik] * b[ib + jk, i_ + i1_];
                            }
                            c[ic + ik, jc + jk] = beta == 0
                                ? alpha * v 
                                : beta * c[ic + ik, jc + jk] + alpha * v;
                        }
                }
                j += 4;
            }
            i += 4;
        }
    }

    private static int Max3(int i0, int i1, int i2) => Math.Max(i0, Math.Max(i1, i2));

    private static double Mul3(double v0, double v1, double v2) => v0 * v1 * v2;

    private static double smpactivationlevel()
    {
        var nn = 2d * __MatrixTileSizeB;
        var result = Math.Max(0.95 * 2 * nn * nn * nn, 1.0E7);
        return result;
    }

    private const int __MatrixTileSizeA = 32;

    private const int __MatrixTileSizeB = 64;

    private static void MatrixCopy(int m, int n, double[,] a, int ia, int ja, double[,] b, int ib, int jb)
    {
        if (m == 0 || n == 0) return;

        for (var i = 0; i < m; i++)
        {
            var i1 = ja - jb;

            for (var j = jb; j <= jb + n - 1; j++)
                b[ib + i, j] = a[ia + i, j + i1];
        }
    }

    private static void MatrixQRBaseCase(ref double[,] a, int m, int n, ref double[] work, ref double[] t, ref double[] tau)
    {
        var min_mn = Math.Min(m, n);

        for (var i = 0; i < min_mn; i++) // Test the input arguments
        {
            // Generate elementary reflector H(i) to annihilate A(i+1:m,i)
            var j = i - 1;

            int k;
            for (k = 1; k <= m - i; k++)
                t[k] = a[k + j, i];

            GenerateReflection(ref t, m - i, out var tmp);
            tau[i] = tmp;
            j = 1 - i;

            for (k = i; k < m; k++)
                a[k, i] = t[k + j];

            t[1] = 1;
            if (i < n) // Apply H(i) to A(i:m-1,i+1:n-1) from the left
                ApplyReflectionFromTheLeft(a, tau[i], t, i, m - 1, i + 1, n - 1, ref work);
        }
    }

    private static void GenerateReflection(ref double[] x, int n, out double tau)
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
                case <= __MinRealValue / __Epsilon:
                {
                    s = __MinRealValue / __Epsilon;
                    v = 1 / s;
                    for (k = 1; k <= n; k++)
                        x[k] = v * x[k];
                    mx *= v;
                    break;
                }

                case >= __MaxRealNumber * __Epsilon:
                {
                    s = __MaxRealNumber * __Epsilon;
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
                xnorm += (x[j] / mx).Pow2();

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
        var beta = -(mx * Math.Sqrt((alpha / mx).Pow2() + (xnorm / mx).Pow2()));
        if (alpha < 0)
            beta = -beta;

        tau = (beta - alpha) / beta;
        v = 1 / (alpha - beta);

        for (k = 2; k <= n; k++)
            x[k] *= v;

        x[1] = beta * s; // Scale back outputs
    }

    public const double __Epsilon = 5E-16;
    public const double __MaxRealNumber = 1E300;
    public const double __MinRealValue = 1E-300;

    private static void ApplyReflectionFromTheLeft(double[,] c, double tau, double[] v, int m1, int m2, int n1, int n2, ref double[] work)
    {
        if (tau == 0 || n1 > n2 || m1 > m2)
            return;

        VectorSetLengThatLeast(ref work, n2 - n1 + 1);
        MatrixGEMV(n2 - n1 + 1, m2 - m1 + 1, 1.0, c, m1, n1, 1, v, 1, 0.0, work, 0);
        MatrixGER(m2 - m1 + 1, n2 - n1 + 1, c, m1, n1, -tau, v, 1, work, 0);
    }

    private static void ApplyReflectionFromTheRight(double[,] c, double tau, double[] v, int m1, int m2, int n1, int n2, ref double[] work)
    {
        if (tau == 0 || n1 > n2 || m1 > m2)
            return;

        VectorSetLengThatLeast(ref work, m2 - m1 + 1);
        MatrixGEMV(m2 - m1 + 1, n2 - n1 + 1, 1.0, c, m1, n1, 0, v, 1, 0.0, work, 0);
        MatrixGER(m2 - m1 + 1, n2 - n1 + 1, c, m1, n1, -tau, work, 0, v, 1);
    }

    private static void VectorSetLengThatLeast(ref double[] x, int n)
    {
        if (x.Length < n) 
            x = new double[n];
    }

    private const int __Blas2MinVendorKernelSize = 8;

    private static void MatrixGER(int m, int n, double[,] a, int ia, int ja, double alpha, double[] u, int iu, double[] v, int iv)
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

    private static void MatrixGEMV(int m, int n, double alpha, double[,] a, int ia, int ja, int opa, double[] x, int ix, double beta, double[] y, int iy)
    {
        // Quick exit for M=0, N=0 or Alpha=0.
        // After this block we have M>0, N>0, Alpha<>0.
        if (m <= 0)
            return;

        if (n <= 0 || alpha == 0.0)
        {
            if (beta != 0)
                MulVx(m, beta, y, iy);
            else
                SetVx(m, 0, y, iy);

            return;
        }

        if (ia + ja + ix + iy == 0)
            GemV(m, n, alpha, a, opa, x, beta, y);
        else
            GemVx(m, n, alpha, a, ia, ja, opa, x, ix, beta, y, iy);
    }

    private static void GemV(int m, int n, double alpha, double[,] a, int opa, double[] x, double beta, double[] y)
    {
        // Properly premultiply Y by Beta.
        //
        // Quick exit for M=0, N=0 or Alpha=0.
        // After this block we have M>0, N>0, Alpha!=0.
        if (m <= 0)
            return;

        if (beta != 0)
            MulV(m, beta, y);
        else
            SetV(m, 0, y);

        if (n <= 0 || alpha == 0.0)
            return;

        // Generic code
        int i, j;
        double v;
        if (opa == 0)
        {
            // y += A*x - comment
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

    private static void MulV(int n, double v, double[] x)
    {
        for (var i = 0; i < n; i++)
            x[i] *= v;
    }

    private static void SetV(int n, double v, double[] x)
    {
        for (var j = 0; j < n; j++)
            x[j] = v;
    }

    private static void GemVx(
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
            MulVx(m, beta, y, iy);
        else
            SetVx(m, 0, y, iy);

        if (n <= 0 || alpha == 0)
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

    private static void MulVx(int n, double v, double[] x, int offsx)
    {
        for (var i = 0; i < n; i++)
            x[offsx + i] *= v;
    }

    private static void SetVx(int n, double v, double[] x, int offsx)
    {
        for (var j = 0; j < n; j++)
            x[offsx + j] = v;
    }
}
