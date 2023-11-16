﻿#nullable enable
using MathCore.Vectors;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

namespace MathCore;

public abstract class TransformationMatrix : Matrix
{
    protected TransformationMatrix(double[,] Data) : base(Data) { }
}

public abstract class Transformation3DMatrix : TransformationMatrix
{
    protected Transformation3DMatrix(double[,] Data) : base(Data) { }
}

public class Rotation3DMatrix(double Angle, Rotation3DMatrix.RotationAxe Axe) : Transformation3DMatrix(GetData(Angle, Axe))
{
    public enum RotationAxe { X, Y, Z }

    private static double[,] GetData(double Angle, RotationAxe Axe)
    {
        var s = Math.Sin(Angle);
        var c = Math.Cos(Angle);

        return Axe switch
        {
            RotationAxe.X => new[,] { { 1,  0, 0 }, { 0, c, -s }, {  0, s, c } },
            RotationAxe.Y => new[,] { { c,  0, s }, { 0, 1,  0 }, { -s, 0, c } },
            RotationAxe.Z => new[,] { { c, -s, 0 }, { s, c,  0 }, {  0, 0, 1 } },
            _             => throw new ArgumentOutOfRangeException(nameof(Axe), Axe, null)
        };
    }

    public RotationAxe Axe { get; } = Axe;

    public static Vector3D operator ^(Rotation3DMatrix M, Vector3D v)
    {
        var m = M.GetData();
        var (x, y, z) = v;
        return new
        (
            m[0, 0] * x + m[0, 1] * y + m[0, 2] * z,
            m[1, 0] * x + m[1, 1] * y + m[1, 2] * z,
            m[2, 0] * x + m[2, 1] * y + m[2, 2] * z
        );
    }
}

public class RotationPhiThetaMatrix : Rotation3DMatrix
{
    public RotationPhiThetaMatrix(double Phi, double Theta) : base(Phi, RotationAxe.Z)
    {
        var s = Math.Sin(Theta);
        var c = Math.Cos(Theta);

        var m = GetData();
        m[0, 2] =  m[0, 0] * s;
        m[1, 2] =  m[1, 0] * s;
        m[2, 2] =  c;
        m[0, 0] *= c;
        m[1, 0] *= c;
        m[2, 0] =  -s;
    }
}

public class RotationThetaPhiMatrix : Rotation3DMatrix
{
    public RotationThetaPhiMatrix(double Phi, double Theta) : base(Phi, RotationAxe.Z)
    {
        var s = Math.Sin(Theta);
        var c = Math.Cos(Theta);

        var m = GetData();
        m[2, 0] =  m[0, 0]*-s;
        m[2, 1] =  m[0, 1]* s;
        m[0, 2] =  s;
        m[2, 2] =  c;
        m[0, 0] *= c;
        m[0, 1] *= c;
    }
}