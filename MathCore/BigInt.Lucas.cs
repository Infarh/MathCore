// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable UnusedMember.Global

namespace MathCore;

public partial class BigInt
{
    //***********************************************************************
    // Returns the k_th number in the Lucas Sequence reduced modulo n.
    //
    // Uses index doubling to speed up the process.  For example, to calculate V(k),
    // we maintain two numbers in the sequence V(n) and V(n+1).
    //
    // To obtain V(2n), we use the identity
    //      V(2n) = (V(n) * V(n)) - (2 * Q^n)
    // To obtain V(2n+1), we first write it as
    //      V(2n+1) = V((n+1) + n)
    // and use the identity
    //      V(m+n) = V(m) * V(n) - Q * V(m-n)
    // Hence,
    //      V((n+1) + n) = V(n+1) * V(n) - Q^n * V((n+1) - n)
    //                   = V(n+1) * V(n) - Q^n * V(1)
    //                   = V(n+1) * V(n) - Q^n * P
    //
    // We use k in its binary expansion and perform index doubling for each
    // bit position.  For each bit position that is set, we perform an
    // index doubling followed by an index addition.  This means that for V(n),
    // we need to update it to V(2n+1).  For V(n+1), we need to update it to
    // V((2n+1)+1) = V(2*(n+1))
    //
    // This function returns
    // [0] = U(k)
    // [1] = V(k)
    // [2] = Q^n
    //
    // Where U(0) = 0 % n, U(1) = 1 % n
    //       V(0) = 2 % n, V(1) = P % n
    //***********************************************************************

    public static BigInt[] LucasSequence(
        BigInt P,
        BigInt Q,
        BigInt k,
        BigInt n)
    {
        if (k._DataLength == 1 && k._Data[0] == 0)
        {
            var result = new BigInt[3];

            result[0] = 0; result[1] = 2 % n; result[2] = 1 % n;
            return result;
        }

        // calculate constant = b^(2k) / m
        // for Barrett Reduction
        var constant = new BigInt();

        var n_len = n._DataLength << 1;
        constant._Data[n_len] = 0x00000001;
        constant._DataLength = n_len + 1;

        constant /= n;

        // calculate values of s and t
        var s = 0;

        for (var index = 0; index < k._DataLength; index++)
        {
            uint mask = 0x01;

            for (var i = 0; i < 32; i++, mask <<= 1, s++)
                if ((k._Data[index] & mask) != 0)
                {
                    index = k._DataLength; // to break the outer loop
                    break;
                }
        }

        return LucasSequenceHelper(P, Q, k >> s, n, constant, s);
    }


    //***********************************************************************
    // Performs the calculation of the kth term in the Lucas Sequence.
    // For details of the algorithm, see reference [9].
    //
    // k must be odd.  i.e LSB == 1
    //***********************************************************************

    private static BigInt[] LucasSequenceHelper(
        BigInt P,
        BigInt Q,
        BigInt k,
        BigInt n,
        BigInt constant,
        int s)
    {
        var result = new BigInt[3];

        if ((k._Data[0] & 0x00000001) == 0)
            throw new ArgumentException("Argument k must be odd.");

        var num_of_bits = k.BitCount;
        var mask = (uint)0x1 << ((num_of_bits & 0x1F) - 1);

        var v = 2 % n;
        var q_k = 1 % n;
        var v1 = P % n;
        var u1 = q_k;
        var flag = true;

        for (var i = k._DataLength - 1; i >= 0; i--) // iterate on the binary expansion of k
        {
            while (mask != 0)
            {
                if (i == 0 && mask == 0x00000001) // last bit
                    break;

                if ((k._Data[i] & mask) != 0) // bit is set
                {
                    // index doubling with addition

                    u1 = u1 * v1 % n;

                    v = (v * v1 - P * q_k) % n;
                    v1 = BarrettReduction(v1 * v1, n, constant);
                    v1 = (v1 - ((q_k * Q) << 1)) % n;

                    if (flag)
                        flag = false;
                    else
                        q_k = BarrettReduction(q_k * q_k, n, constant);

                    q_k = q_k * Q % n;
                }
                else
                {
                    // index doubling
                    u1 = (u1 * v - q_k) % n;

                    v1 = (v * v1 - P * q_k) % n;
                    v = BarrettReduction(v * v, n, constant);
                    v = (v - (q_k << 1)) % n;

                    if (flag)
                    {
                        q_k = Q % n;
                        flag = false;
                    }
                    else
                        q_k = BarrettReduction(q_k * q_k, n, constant);
                }

                mask >>= 1;
            }
            mask = 0x80000000;
        }

        // at this point u1 = u(n+1) and v = v(n)
        // since the last bit always 1, we need to transform u1 to u(2n+1) and v to v(2n+1)

        u1 = (u1 * v - q_k) % n;
        v = (v * v1 - P * q_k) % n;
        if (!flag)
            q_k = BarrettReduction(q_k * q_k, n, constant);
        //        else
        //            flag = false;

        q_k = q_k * Q % n;


        for (var i = 0; i < s; i++)
        {
            // index doubling
            u1 = u1 * v % n;
            v = (v * v - (q_k << 1)) % n;

            q_k = BarrettReduction(q_k * q_k, n, constant);
        }

        result[0] = u1;
        result[1] = v;
        result[2] = q_k;

        return result;
    }
}
