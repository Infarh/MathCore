// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable UnusedMember.Global

namespace MathCore;

public partial class BigInt
{
    /// <summary>Случайный набор бит указанной длины <paramref name="bits"/></summary>
    /// <param name="bits">Требуемое число бит</param>
    /// <param name="rand">Генератор случайных чисел</param>
    /// <exception cref="ArithmeticException">Число бит &gt; <see cref="MaxLength"/> = (70)x(8x8) = 4'480 </exception>
    public void GenRandomBits(int bits, Random rand)
    {
        var d_words = bits >> 5;
        var rem_bits = bits & 0x1F;

        if (rem_bits != 0)
            d_words++;

        if (d_words > MaxLength)
            throw new ArithmeticException("Number of required bits > maxLength");

        for (var i = 0; i < d_words; i++)
            _Data[i] = (uint)(rand.NextDouble() * 0x1_0000_0000);

        for (var i = d_words; i < MaxLength; i++)
            _Data[i] = 0;

        if (rem_bits != 0)
        {
            var mask = (uint)(0x01 << (rem_bits - 1));
            _Data[d_words - 1] |= mask;

            mask = 0xFFFFFFFF >> (32 - rem_bits);
            _Data[d_words - 1] &= mask;
        }
        else
            _Data[d_words - 1] |= 0x8_000_0000;

        _DataLength = d_words;

        if (_DataLength == 0)
            _DataLength = 1;
    }


    //***********************************************************************
    // Returns the position of the most significant bit in the BigInteger.
    //
    // Eg.  The result is 0, if the value of BigInteger is 0...0000 0000
    //      The result is 1, if the value of BigInteger is 0...0000 0001
    //      The result is 2, if the value of BigInteger is 0...0000 0010
    //      The result is 2, if the value of BigInteger is 0...0000 0011
    //
    //***********************************************************************

    /// <summary>Число бит (номер последнего значащего бита)</summary>
    public int BitCount
    {
        get
        {

            var data_length = _DataLength;
            while (data_length > 1 && _Data[data_length - 1] == 0)
                data_length--;

            var value = _Data[data_length - 1];
            var mask = 0x8_000_0000;
            var bits = 32;

            while (bits > 0 && (value & mask) == 0)
            {
                bits--;
                mask >>= 1;
            }

            bits += (data_length - 1) << 5;

            return bits;
        }
    }


    /// <summary>Последние 4 байта значения числа <see cref="BigInt"/></summary>
    public int IntValue() => (int)_Data[0];


    /// <summary>Последние 8 байт значения числа <see cref="BigInt"/></summary>
    public long LongValue()
    {
        long val = _Data[0];
        try
        { // exception if maxLength = 1
            val |= (long)_Data[1] << 32;
        }
        catch (ArithmeticException) //todo: избавиться от исключения
        {
            if ((_Data[0] & 0x80000000) != 0) // negative
                val = (int)_Data[0];
        }

        return val;
    }


    //***********************************************************************
    // Returns the value of the BigInteger as a byte array.  The lowest
    // index contains the MSB.
    //***********************************************************************

    public byte[] GetBytes()
    {
        var num_bits = BitCount;

        var num_bytes = num_bits >> 3;
        if ((num_bits & 0x7) != 0)
            num_bytes++;

        var result = new byte[num_bytes];

        var pos = 0;
        uint temp_val;
        var val = _Data[_DataLength - 1];

        if ((temp_val = val >> 24 & 0xFF) != 0)
            result[pos++] = (byte)temp_val;
        if ((temp_val = val >> 16 & 0xFF) != 0)
            result[pos++] = (byte)temp_val;
        if ((temp_val = val >> 8 & 0xFF) != 0)
            result[pos++] = (byte)temp_val;
        if ((temp_val = val & 0xFF) != 0)
            result[pos++] = (byte)temp_val;

        for (var i = _DataLength - 2; i >= 0; i--, pos += 4)
        {
            val = _Data[i];
            result[pos + 3] = (byte)(val & 0xFF);
            val >>= 8;
            result[pos + 2] = (byte)(val & 0xFF);
            val >>= 8;
            result[pos + 1] = (byte)(val & 0xFF);
            val >>= 8;
            result[pos] = (byte)(val & 0xFF);
        }

        return result;
    }


    //***********************************************************************
    // Sets the value of the specified bit to 1
    // The Least Significant Bit position is 0.
    //***********************************************************************

    public void SetBit(uint BitNum)
    {
        var byte_pos = BitNum >> 5;           // divide by 32
        var bit_pos = (byte)(BitNum & 0x1F); // get the lowest 5 bits

        var mask = (uint)1 << bit_pos;
        _Data[byte_pos] |= mask;

        if (byte_pos >= _DataLength)
            _DataLength = (int)byte_pos + 1;
    }


    //***********************************************************************
    // Sets the value of the specified bit to 0
    // The Least Significant Bit position is 0.
    //***********************************************************************

    public void UnsetBit(uint BitNum)
    {
        var byte_pos = BitNum >> 5;

        if (byte_pos >= _DataLength) return;
        var bit_pos = (byte)(BitNum & 0x1F);

        var mask = (uint)1 << bit_pos;
        var mask2 = 0xFFFFFFFF ^ mask;

        _Data[byte_pos] &= mask2;

        if (_DataLength > 1 && _Data[_DataLength - 1] == 0)
            _DataLength--;
    }


    //***********************************************************************
    // Returns a value that is equivalent to the integer square root
    // of the BigInteger.
    //
    // The integer square root of "this" is defined as the largest integer n
    // such that (n * n) <= this
    //
    //***********************************************************************

    public BigInt Sqrt()
    {
        var num_bits = (uint)BitCount;

        if ((num_bits & 0x1) != 0) // odd number of bits
            num_bits = (num_bits >> 1) + 1;
        else
            num_bits >>= 1;

        var byte_pos = num_bits >> 5;
        var bit_pos = (byte)(num_bits & 0x1F);

        uint mask;

        var result = new BigInt();
        if (bit_pos == 0)
            mask = 0x80000000;
        else
        {
            mask = (uint)1 << bit_pos;
            byte_pos++;
        }
        result._DataLength = (int)byte_pos;

        for (var i = (int)byte_pos - 1; i >= 0; i--)
        {
            while (mask != 0)
            {
                // guess
                result._Data[i] ^= mask;

                // undo the guess if its square is larger than this
                if (result * result > this)
                    result._Data[i] ^= mask;

                mask >>= 1;
            }
            mask = 0x80000000;
        }
        return result;
    }
}
