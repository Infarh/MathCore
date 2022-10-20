using static System.Math;

namespace MathCore.Algorithms.Matrixes;

public static class Solve
{
    private static bool Swap(double[,] array, int i1, int i2)
    {
        if (i1 == i2) return false;
        var n = array.GetLength(0);
        for (var j = 0; j < n; j++)
        {
            var tmp = array[i1, j];
            array[i1, j] = array[i2, j];
            array[i2, j] = tmp;
        }

        return true;
    }

    private static void Swap<T>(ref T v1, ref T v2) { var t = v1; v1 = v2; v2 = t; }

    public static (int Rank, double Determinant) Triangulate(double[,] M)
    {
        var n = M.GetLength(0); // число строк
        var m = M.GetLength(1); // число столбцов

        var d    = 1d;        // определитель
        var rank = Min(n, m); // ранг матрицы в случае успеха операции

        for (var i0 = 0; i0 < rank; i0++)
        {
            // Если очередной элемент в строке равен нулю, то надо найти другую строку
            // и поменять знак определителя
            if (M[i0, i0] == 0)
            {
                var max       = 0d;
                var max_index = -1;
                // Ищем в столбце ниже опорного элемента строку с самым большим по модулю значением
                for (var i1 = i0 + 1; i1 < n; i1++)
                    if (Abs(M[i1, i0]) is var abs && abs > max)
                    {
                        max       = abs;
                        max_index = i1;
                    }

                // Если ни одно из значений в столбце ниже не отличается от нуля,
                // то max_index останется равен 0. И это будет признаком того, что матрица вырождена
                if (max_index < 0)
                {
                    // В этом случае надо принудительно обнулить все нижние строки
                    for (var i = i0; i < n; i++)
                        for (var j = i0; j < m; j++)
                            M[i, j] = 0d;
                    // И завершить алгоритм
                    // Рангом матрицы будет в этом случае номер текущей строки, а определитель равен 0.
                    return (i0, 0);
                }

                // Если найдена строка, то надо её переместить в строку, следующую за текущей
                if (Swap(M, i0, max_index)) // Если найденная строка не совпадает со следующей, то меняем их местами
                    d = -d;                 // и надо в этом случае изменить знак определителя
            }

            var main = M[i0, i0]; // Ведущий элемент строки
            d *= main;            // Сразу умножаем определитель на ведущий элемент (диагональный элемент будущей треугольной матрицы)

            //Нормируем строку основной матрицы по первому элементу
            for (var i = i0 + 1; i < n; i++)
                if (M[i, i0] != 0) // В том случае, если очередной элемент строки в столбце ведущего элемента не равен нулю, то надо что-то делать...
                {
                    // Определяем коэффициент, на который будем умножать все элементы строки
                    var k = M[i, i0] / main;
                    M[i, i0] = 0d; // Обнуляем элемент в столбце ведущей строки
                    // Проходим по всем элементам строки и вычитаем из каждого элемента ведущий элемент
                    for (var j = i0 + 1; j < m; j++)
                        M[i, j] -= M[i0, j] * k;
                }
        }

        return (rank, d);
    }

    public static (int Rank, double Determinant) Triangulate(double[,] M, double[,] b)
    {
        var n = M.GetLength(0); // число строк
        var m = M.GetLength(1); // число столбцов

        var b_n = b.GetLength(0); // число строк
        var b_m = b.GetLength(1); // число столбцов
        if (b_n != n) throw new ArgumentException(@"Число строк присоединённой матрицы не равно числу строк исходной матрицы");

        var d    = 1d;
        var rank = Min(n, m);
        for (var i0 = 0; i0 < rank; i0++)
        {
            if (M[i0, i0] == 0)
            {
                var max       = 0d;
                var max_index = -1;
                for (var i1 = i0 + 1; i1 < n; i1++)
                    if (Abs(M[i1, i0]) is var abs && abs > max)
                    {
                        max       = abs;
                        max_index = i1;
                    }

                if (max_index < 0)
                {
                    for (var i = i0; i < n; i++)
                    {
                        for (var j = i0; j < m; j++) M[i, j]  = 0d;
                        for (var j = 0; j < b_m; j++) b[i, j] = 0d;
                    }
                    return (i0, 0);
                }

                Swap(M, i0, max_index);
                Swap(b, i0, max_index);
                d = -d;
            }

            var main = M[i0, i0]; // Ведущий элемент строки
            d *= main;
            //Нормируем строку основной матрицы по первому элементу
            for (var i = i0 + 1; i < n; i++)
                if (M[i, i0] != 0)
                {
                    var k = M[i, i0] / main;
                    M[i, i0] = 0d;

                    for (var j = i0 + 1; j < m; j++)
                        M[i, j] -= M[i0, j] * k;
                    for (var j = 0; j < b_m; j++)
                        b[i, j] -= b[i0, j] * k;
                }
        }
        if (rank >= n)
            return (rank, d);

        for (var i = rank; i < n; i++)
            for (var j = 0; j < b_m; j++)
                b[i, j] = 0d;
        return (rank, d);
    }

    private static double[,] CreatePermutationMatrix(int[] indexes)
    {
        var n           = indexes.Length;
        var permutation = new double[n, n];
        for (var i = 0; i < n; i++)
            permutation[i, indexes[i]] = 1;
        return permutation;
    }

    public static (int Rank, double Determinant, double[,] Permutation) TriangulateWithPermutation(double[,] matrix, double[,] b)
    {
        var n = matrix.GetLength(0); // число строк
        var m = matrix.GetLength(1); // число столбцов

        var b_n = b.GetLength(0); // число строк
        var b_m = b.GetLength(1); // число столбцов
        if (b_n != n) throw new ArgumentException(@"Число строк присоединённой матрицы не равно числу строк исходной матрицы");

        var d       = 1d;
        var p_index = new int[n];
        for (var i = 0; i < n; i++)
            p_index[i] = i;

        var n1 = Min(n, m);
        for (var i0 = 0; i0 < n1; i0++)
        {
            if (matrix[i0, i0] == 0)
            {
                var max       = 0d;
                var max_index = -1;
                for (var i1 = i0 + 1; i1 < n; i1++)
                    if (Abs(matrix[i1, i0]) is var abs && abs > max)
                    {
                        max       = abs;
                        max_index = i1;
                    }

                if (max_index < 0)
                {
                    for (var i = i0; i < n; i++)
                    {
                        for (var j = i0; j < m; j++)
                            matrix[i, j] = 0d;
                        for (var j = 0; j < b_m; j++)
                            b[i, j] = 0d;
                    }
                    return (i0, 0, CreatePermutationMatrix(p_index));
                }
                Swap(matrix, i0, max_index);
                Swap(b, i0, max_index);
                Swap(ref p_index[i0], ref p_index[max_index]);
                d = -d;
            }
            var main = matrix[i0, i0]; // Ведущий элемент строки
            d *= main;

            //Нормируем строку основной матрицы по первому элементу
            for (var i = i0 + 1; i < n; i++)
                if (matrix[i, i0] != 0)
                {
                    var k = matrix[i, i0] / main;
                    matrix[i, i0] = 0d;
                    for (var j = i0 + 1; j < m; j++)
                        matrix[i, j] -= matrix[i0, j] * k;
                    for (var j = 0; j < b_m; j++)
                        b[i, j] -= b[i0, j] * k;
                }
        }

        if (n1 >= n)
            return (n1, d, CreatePermutationMatrix(p_index));

        for (var i = n1; i < n; i++)
            for (var j = 0; j < b_m; j++)
                b[i, j] = 0d;
        return (n1, d, CreatePermutationMatrix(p_index));
    }

    public static double[,] Inverse(double[,] matrix)
    {
        var n = matrix.GetLength(0); // число строк
        var m = matrix.GetLength(1); // число столбцов

        var unitary = new double[n, m];
        for (var i = 0; i < n; i++)
            for (var j = 0; j < m; j++)
                unitary[i, j] = i == j ? 1 : 0;

        return SolveSystem(matrix, unitary, out _) ? unitary : null;
    }

    public static bool SolveSystem(double[,] matrix, double[,] b, out double[,] p)
    {
        (_, var det, p) = TriangulateWithPermutation(matrix, b);
        if (det == 0) return false;

        var n = matrix.GetLength(0); // число строк
        var m = matrix.GetLength(1); // число столбцов

        var b_m = b.GetLength(1);

        for (var i0 = Min(n, m) - 1; i0 >= 0; i0--)
        {
            if (matrix[i0, i0] is var k && k != 1)
                for (var j = 0; j < b_m; j++)
                    b[i0, j] /= k;

            for (var i = i0 - 1; i >= 0; i--)
                if (matrix[i, i0] != 0)
                {
                    var k0 = matrix[i, i0];
                    matrix[i, i0] = 0d;
                    for (var j = 0; j < b_m; j++) b[i, j] -= b[i0, j] * k0;
                }
        }

        return true;
    }
}