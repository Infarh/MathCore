namespace MathCore.Algorithms.Numbers;

public static class Pascal
{
    public static void Run(int N)
    {
        var nn = new int[N][];
        nn[0]    = new int[N];
        nn[0][0] = 1;

        for (var i = 1; i < N; i++)
        {
            var prev    = nn[i - 1];
            var current = nn[i] = new int[N];
            current[0] = 1;

            for (var j = 1; j < N; j++)
                current[j] = prev[j] + prev[j - 1];

            for (var k = 0; k < i; k++)
            {
                current = nn[k];
                var last = current[0];
                for (var j = 1; j < N; j++)
                {
                    var tmp = current[j];
                    current[j] -= last;
                    last       =  tmp;
                }
            }
        }

        #region Print

        var ss = new string[N, N];
        var ll = new int[N];
        for (var i = 0; i < N; i++)
            for (var j = 0; j < N; j++)
            {
                var s = nn[i][j].ToString();
                ss[j, i] = s;
                ll[i] = Math.Max(ll[i], s.Length);
            }

        Console.WriteLine("int[] z_matrix = ");
        Console.WriteLine("{");
        for (var i = 0; i < N; i++)
        {
            Console.Write("    /*{0,2}*/ {{ ", i);
            for (var j = 0; j < N; j++)
                Console.Write("{0}, ", ss[i, j].PadLeft(ll[j]));
            Console.CursorLeft -= 2;
            Console.WriteLine(" },");
        }

        Console.WriteLine("};");

        Console.WriteLine("---------------"); 

        #endregion

        Run2(N);
    }

    public static void Run2(int N)
    {
        var nn = new int[N, N];
        nn[0, 0] = 1;

        for (var j = 1; j < N; j++)
        {
            nn[0, j] = 1;

            for (var i = 1; i < N; i++)
                nn[i, j] = nn[i, j - 1] + nn[i - 1, j - 1];

            for (var k = 0; k < j; k++)
            {
                var last = nn[0, k];
                for (var i = 1; i < N; i++)
                {
                    var tmp = nn[i, k];
                    nn[i, k] -= last;
                    last     =  tmp;
                }
            }
        }

        var ss = new string[N, N];
        var ll = new int[N];
        for (var i = 0; i < N; i++)
            for (var j = 0; j < N; j++)
            {
                var s    = nn[i, j].ToString();
                ss[i, j] = s;
                ll[j]    = Math.Max(ll[j], s.Length);
            }

        Console.WriteLine("int[] z_matrix = ");
        Console.WriteLine("{");
        var colum_nums = string.Join(", ", Enumerable.Range(1, N).Select(i => i.ToString().PadLeft(ll[i - 1])));
        Console.WriteLine("    //       {0}", colum_nums);
        Console.WriteLine("    //       {0}", new string('-', colum_nums.Length));
        for (var i = 0; i < N; i++)
        {
            Console.Write("    /*{0,2}*/ {{ ", i + 1);
            for (var j = 0; j < N; j++)
                Console.Write("{0}, ", ss[i, j].PadLeft(ll[j]));
            Console.CursorLeft -= 2;
            Console.WriteLine(" },");
        }

        Console.WriteLine("};");
    }
}

//int[] z_matrix =
//{
//    /* 0*/ {     1,     1,    1,    1,   1,   1,   1,   1,   1,   1,   1,   1,    1,    1,     1,    1 },
//    /* 1*/ {   -15,   -13,  -11,   -9,  -7,  -5,  -3,  -1,   1,   3,   5,   7,    9,   11,    13,   15 },
//    /* 2*/ {   105,    77,   53,   33,  17,   5,  -3,  -7,  -7,  -3,   5,  17,   33,   53,    77,  105 },
//    /* 3*/ {  -455,  -273, -143,  -57,  -7,  15,  17,   7,  -7, -17, -15,   7,   57,  143,   273,  455 },
//    /* 4*/ {  1365,   637,  221,   21, -43, -35,  -3,  21,  21,  -3, -35, -43,   21,  221,   637, 1365 },
//    /* 5*/ { -3003, -1001, -143,   99,  77,  -1, -39, -21,  21,  39,   1, -77,  -99,  143,  1001, 3003 },
//    /* 6*/ {  5005,  1001, -143, -187, -11,  65,  25, -35, -35,  25,  65, -11, -187, -143,  1001, 5005 },
//    /* 7*/ { -6435,  -429,  429,   99, -99, -45,  45,  35, -35, -45,  45,  99,  -99, -429,   429, 6435 },
//    /* 8*/ {  6435,  -429, -429,   99,  99, -45, -45,  35,  35, -45, -45,  99,   99, -429,  -429, 6435 },
//    /* 9*/ { -5005,  1001,  143, -187,  11,  65, -25, -35,  35,  25, -65, -11,  187, -143, -1001, 5005 },
//    /*10*/ {  3003, -1001,  143,   99, -77,  -1,  39, -21, -21,  39,  -1, -77,   99,  143, -1001, 3003 },
//    /*11*/ { -1365,   637, -221,   21,  43, -35,   3,  21, -21,  -3,  35, -43,  -21,  221,  -637, 1365 },
//    /*12*/ {   455,  -273,  143,  -57,   7,  15, -17,   7,   7, -17,  15,   7,  -57,  143,  -273,  455 },
//    /*13*/ {  -105,    77,  -53,   33, -17,   5,   3,  -7,   7,  -3,  -5,  17,  -33,   53,   -77,  105 },
//    /*14*/ {    15,   -13,   11,   -9,   7,  -5,   3,  -1,  -1,   3,  -5,   7,   -9,   11,   -13,   15 },
//    /*15*/ {    -1,     1,   -1,    1,  -1,   1,  -1,   1,  -1,   1,  -1,   1,   -1,    1,    -1,    1 },
//};