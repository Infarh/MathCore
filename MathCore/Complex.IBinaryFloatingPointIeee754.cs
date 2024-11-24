#if NET5_0_OR_GREATER

using System.Globalization;
using System.Numerics;

namespace MathCore;

public readonly partial struct Complex : IBinaryFloatingPointIeee754<Complex>
{
    int IComparable.CompareTo(object obj) => throw new NotImplementedException();

    int IComparable<Complex>.CompareTo(Complex other) => throw new NotImplementedException();

    static Complex IBitwiseOperators<Complex, Complex, Complex>.operator &(Complex left, Complex right) => throw new NotImplementedException();

    static Complex IBitwiseOperators<Complex, Complex, Complex>.operator |(Complex left, Complex right) => throw new NotImplementedException();

    static Complex IBitwiseOperators<Complex, Complex, Complex>.operator ~(Complex value) => throw new NotImplementedException();

    static bool IComparisonOperators<Complex, Complex, bool>.operator >(Complex left, Complex right) => throw new NotImplementedException();

    static bool IComparisonOperators<Complex, Complex, bool>.operator >=(Complex left, Complex right) => throw new NotImplementedException();

    static bool IComparisonOperators<Complex, Complex, bool>.operator <(Complex left, Complex right) => throw new NotImplementedException();

    static bool IComparisonOperators<Complex, Complex, bool>.operator <=(Complex left, Complex right) => throw new NotImplementedException();

    static Complex IDecrementOperators<Complex>.operator --(Complex value) => throw new NotImplementedException();

    static Complex IIncrementOperators<Complex>.operator ++(Complex value) => throw new NotImplementedException();

    static Complex IModulusOperators<Complex, Complex, Complex>.operator %(Complex left, Complex right) => throw new NotImplementedException();

    static Complex INumberBase<Complex>.Abs(Complex value) => value.Abs;

    static bool INumberBase<Complex>.IsCanonical(Complex value) => throw new NotImplementedException();

    static bool INumberBase<Complex>.IsComplexNumber(Complex value) => true;

    static bool INumberBase<Complex>.IsEvenInteger(Complex value) => throw new NotImplementedException();

    static bool INumberBase<Complex>.IsFinite(Complex value) => throw new NotImplementedException();

    static bool INumberBase<Complex>.IsImaginaryNumber(Complex value) => throw new NotImplementedException();

    static bool INumberBase<Complex>.IsInfinity(Complex value) => throw new NotImplementedException();

    static bool INumberBase<Complex>.IsInteger(Complex value) => throw new NotImplementedException();

    static bool INumberBase<Complex>.IsNaN(Complex value) => value.IsNaN;

    static bool INumberBase<Complex>.IsNegative(Complex value) => throw new NotImplementedException();

    static bool INumberBase<Complex>.IsNegativeInfinity(Complex value) => throw new NotImplementedException();

    static bool INumberBase<Complex>.IsNormal(Complex value) => throw new NotImplementedException();

    static bool INumberBase<Complex>.IsOddInteger(Complex value) => throw new NotImplementedException();

    static bool INumberBase<Complex>.IsPositive(Complex value) => throw new NotImplementedException();

    static bool INumberBase<Complex>.IsPositiveInfinity(Complex value) => throw new NotImplementedException();

    static bool INumberBase<Complex>.IsRealNumber(Complex value) => value.Im == 0;

    static bool INumberBase<Complex>.IsSubnormal(Complex value) => throw new NotImplementedException();

    static bool INumberBase<Complex>.IsZero(Complex value) => value is { Re: 0, Im: 0 };

    static Complex INumberBase<Complex>.MaxMagnitude(Complex x, Complex y) => throw new NotImplementedException();

    static Complex INumberBase<Complex>.MaxMagnitudeNumber(Complex x, Complex y) => throw new NotImplementedException();

    static Complex INumberBase<Complex>.MinMagnitude(Complex x, Complex y) => throw new NotImplementedException();

    static Complex INumberBase<Complex>.MinMagnitudeNumber(Complex x, Complex y) => throw new NotImplementedException();

    static Complex INumberBase<Complex>.Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider provider) => throw new NotImplementedException();

    static Complex INumberBase<Complex>.Parse(string s, NumberStyles style, IFormatProvider provider) => throw new NotImplementedException();

    static bool INumberBase<Complex>.TryConvertFromChecked<TOther>(TOther value, out Complex result) => throw new NotImplementedException();

    static bool INumberBase<Complex>.TryConvertFromSaturating<TOther>(TOther value, out Complex result) => throw new NotImplementedException();

    static bool INumberBase<Complex>.TryConvertFromTruncating<TOther>(TOther value, out Complex result) => throw new NotImplementedException();

    static bool INumberBase<Complex>.TryConvertToChecked<TOther>(Complex value, out TOther result) => throw new NotImplementedException();

    static bool INumberBase<Complex>.TryConvertToSaturating<TOther>(Complex value, out TOther result) => throw new NotImplementedException();

    static bool INumberBase<Complex>.TryConvertToTruncating<TOther>(Complex value, out TOther result) => throw new NotImplementedException();

    static bool INumberBase<Complex>.TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider provider, out Complex result) => throw new NotImplementedException();

    static bool INumberBase<Complex>.TryParse(string s, NumberStyles style, IFormatProvider provider, out Complex result) => throw new NotImplementedException();

    static Complex INumberBase<Complex>.One => Real;

    static int INumberBase<Complex>.Radix => throw new NotImplementedException();

    static Complex INumberBase<Complex>.Zero => Zero;

    static bool IBinaryNumber<Complex>.IsPow2(Complex value) => throw new NotImplementedException();

    //static Complex ILogarithmicFunctions<Complex>.Log(Complex x) => Log(x, Math.E);

    static Complex ILogarithmicFunctions<Complex>.Log(Complex x, Complex Base) => throw new NotImplementedException();

    public static Complex Log10(Complex x) => Log(x, 10);

    public static Complex Log2(Complex x) => Log(x, 2);

    static Complex IFloatingPointConstants<Complex>.E => throw new NotImplementedException();

    static Complex IFloatingPointConstants<Complex>.Pi => throw new NotImplementedException();

    static Complex IFloatingPointConstants<Complex>.Tau => throw new NotImplementedException();

    static Complex IExponentialFunctions<Complex>.Exp10(Complex x) => throw new NotImplementedException();

    static Complex IExponentialFunctions<Complex>.Exp2(Complex x) => throw new NotImplementedException();

    static Complex ISignedNumber<Complex>.NegativeOne => -Real;

    int IFloatingPoint<Complex>.GetExponentByteCount() => throw new NotImplementedException();

    int IFloatingPoint<Complex>.GetExponentShortestBitLength() => throw new NotImplementedException();

    int IFloatingPoint<Complex>.GetSignificandBitLength() => throw new NotImplementedException();

    int IFloatingPoint<Complex>.GetSignificandByteCount() => throw new NotImplementedException();

    static Complex IFloatingPoint<Complex>.Round(Complex x, int digits, MidpointRounding mode) => throw new NotImplementedException();

    bool IFloatingPoint<Complex>.TryWriteExponentBigEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

    bool IFloatingPoint<Complex>.TryWriteExponentLittleEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

    bool IFloatingPoint<Complex>.TryWriteSignificandBigEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

    bool IFloatingPoint<Complex>.TryWriteSignificandLittleEndian(Span<byte> destination, out int bytesWritten) => throw new NotImplementedException();

    static Complex IHyperbolicFunctions<Complex>.Acosh(Complex x) => throw new NotImplementedException();

    static Complex IHyperbolicFunctions<Complex>.Asinh(Complex x) => throw new NotImplementedException();

    static Complex IHyperbolicFunctions<Complex>.Atanh(Complex x) => throw new NotImplementedException();

    static Complex IHyperbolicFunctions<Complex>.Cosh(Complex x) => throw new NotImplementedException();

    static Complex IHyperbolicFunctions<Complex>.Sinh(Complex x) => throw new NotImplementedException();

    static Complex IHyperbolicFunctions<Complex>.Tanh(Complex x) => Trigonometry.Hyperbolic.Tgh(x);

    static Complex IPowerFunctions<Complex>.Pow(Complex x, Complex y) => x ^ y;

    static Complex IRootFunctions<Complex>.Cbrt(Complex x) => throw new NotImplementedException();

    static Complex IRootFunctions<Complex>.Hypot(Complex x, Complex y) => throw new NotImplementedException();

    static Complex IRootFunctions<Complex>.RootN(Complex x, int n) => throw new NotImplementedException();

    static Complex ITrigonometricFunctions<Complex>.Acos(Complex x) => throw new NotImplementedException();

    static Complex ITrigonometricFunctions<Complex>.AcosPi(Complex x) => throw new NotImplementedException();

    static Complex ITrigonometricFunctions<Complex>.Asin(Complex x) => throw new NotImplementedException();

    static Complex ITrigonometricFunctions<Complex>.AsinPi(Complex x) => throw new NotImplementedException();

    static Complex ITrigonometricFunctions<Complex>.Atan(Complex x) => throw new NotImplementedException();

    static Complex ITrigonometricFunctions<Complex>.AtanPi(Complex x) => throw new NotImplementedException();

    static Complex ITrigonometricFunctions<Complex>.Cos(Complex x) => Trigonometry.Cos(x);

    static Complex ITrigonometricFunctions<Complex>.CosPi(Complex x) => throw new NotImplementedException();

    static Complex ITrigonometricFunctions<Complex>.Sin(Complex x) => throw new NotImplementedException();

    static (Complex Sin, Complex Cos) ITrigonometricFunctions<Complex>.SinCos(Complex x) => Trigonometry.SinCos(x);

    static (Complex SinPi, Complex CosPi) ITrigonometricFunctions<Complex>.SinCosPi(Complex x) => throw new NotImplementedException();

    static Complex ITrigonometricFunctions<Complex>.SinPi(Complex x) => throw new NotImplementedException();

    static Complex ITrigonometricFunctions<Complex>.Tan(Complex x) => throw new NotImplementedException();

    static Complex ITrigonometricFunctions<Complex>.TanPi(Complex x) => throw new NotImplementedException();

    static Complex IFloatingPointIeee754<Complex>.Atan2(Complex y, Complex x) => throw new NotImplementedException();

    static Complex IFloatingPointIeee754<Complex>.Atan2Pi(Complex y, Complex x) => throw new NotImplementedException();

    static Complex IFloatingPointIeee754<Complex>.BitDecrement(Complex x) => throw new NotImplementedException();

    static Complex IFloatingPointIeee754<Complex>.BitIncrement(Complex x) => throw new NotImplementedException();

    static Complex IFloatingPointIeee754<Complex>.FusedMultiplyAdd(Complex left, Complex right, Complex addend) => throw new NotImplementedException();

    static Complex IFloatingPointIeee754<Complex>.Ieee754Remainder(Complex left, Complex right) => throw new NotImplementedException();

    static int IFloatingPointIeee754<Complex>.ILogB(Complex x) => throw new NotImplementedException();

    static Complex IFloatingPointIeee754<Complex>.ScaleB(Complex x, int n) => throw new NotImplementedException();

    static Complex IFloatingPointIeee754<Complex>.Epsilon => new(double.Epsilon, double.Epsilon);

    static Complex IFloatingPointIeee754<Complex>.NaN => NaN;

    static Complex IFloatingPointIeee754<Complex>.NegativeInfinity => throw new NotImplementedException();

    static Complex IFloatingPointIeee754<Complex>.NegativeZero => -Zero;

    static Complex IFloatingPointIeee754<Complex>.PositiveInfinity => throw new NotImplementedException();
}


#endif