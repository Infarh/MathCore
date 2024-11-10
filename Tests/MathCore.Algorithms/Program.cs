using System.Numerics;
using System.Runtime.Intrinsics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

using MathCore;
using MathCore.Algorithms.Matrices;

double[,] MM = 
{
    { 1, 2, 3, },
    { 4, 5, 6, },
    { 7, 8, 8, },
};

var M = Matrix.Create(MM);
var invM = M.GetInverse(out var P);

//var X = new double[11];

//for (var i = 0; i < X.Length; i++)
//    X[i] = i + 1;

//var A = DoubleVector.Create(X);

//var sum = A.Sum();


Console.WriteLine("End.");
return;

readonly ref struct DoubleVector(double[] array)
{
    public static DoubleVector Create(params double[] array) => new(array);

    public double[] Items => array;

    public int Length => array.Length;

    public double Sum()
    {
        var sum = 0d;
        if (!Vector.IsHardwareAccelerated)
        {
            foreach (var x in array)
                sum += x;

            return sum;
        }

        var window_size = Vector<double>.Count;
        var ones = Vector<double>.One;

        var vector_part_length = array.Length - array.Length % window_size;
        for (var i = 0; i < vector_part_length; i += window_size)
            sum += Vector.Dot(new(array, i), ones);

        for (var i = vector_part_length; i < array.Length; i++)
            sum += array[i];

        return sum;
    }

    public DoubleVector ProductTo(DoubleVector other)
    {
        if (Length != other.Length) throw new InvalidOperationException("Размеры векторов не совпадают");

        var result = new double[Length];


        return result;
    }


    public static implicit operator DoubleVector(double[] array) => new(array);

    public static implicit operator double[](DoubleVector vector) => vector.Items;
}