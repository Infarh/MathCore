#nullable enable
using System.Runtime.CompilerServices;
using System.Text;

using MathCore.Hash;

namespace MathCore.Extensions.String;

public static class StringHashExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] GetHashCodeMD5(this string str) => str.GetHashCodeMD5(Encoding.UTF8);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] GetHashCode256(this string str) => str.GetHashCode256(Encoding.UTF8);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] GetHashCode512(this string str) => str.GetHashCode512(Encoding.UTF8);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] GetHashCodeMD5(this string str, Encoding? encoding) => MD5.Compute(str, encoding);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] GetHashCode256(this string str, Encoding? encoding) => SHA256.Compute(str, encoding);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] GetHashCode512(this string str, Encoding? encoding) => SHA512.Compute(str, encoding);
}
