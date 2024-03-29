﻿namespace MathCore.Interpolation;

public abstract class Interpolator
{
    public static double Linear(double x, double x1, double y1, double x2, double y2) => Interpolation.Linear.Interpolate(x, x1, y1, x2, y2);

    public static Linear Linear(double[] X, double[] Y) => new(X, Y);

    public static double Lagrange(double x, double x0, double dx, double[] Y) => Interpolation.Lagrange.Interpolate(x, x0, dx, Y);
    public static double Lagrange(double x, double[] X, double[] Y) => Interpolation.Lagrange.Interpolate(x, X, Y);
    public static Lagrange Lagrange(double x0, double dx, double[] Y) => new(x0, dx, Y);
    public static Lagrange Lagrange(double[] X, double[] Y) => new(X, Y);

    public static Newton Newton(double x0, double dx, double[] Y) => new(x0, dx, Y);
    public static Newton Newton(double[] X, double[] Y) => new(X, Y);
}