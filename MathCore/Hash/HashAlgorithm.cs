using System;

namespace MathCore.Hash;

public abstract class HashAlgorithm
{
    protected static uint LeftRotate(uint x, int c) => (x << c) | (x >> (32 - c));

    protected static uint RightRotate(uint x, int c) => (x >> c) | (x << (32 - c));
    protected static ulong RightRotate(ulong x, int c) => (x >> c) | (x << (64 - c));
}
