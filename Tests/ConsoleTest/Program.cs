//const uint poly = 0xEDB88320;

//var table = new uint[256];
//var t1 = new uint[256];
//var t2 = new uint[256];

//TestCRC32.FillTableCRC32RefOut(table, poly);
//TestCRC32.FillTableCRC32RefOut(t1, 0x814141AB, false, false);
//TestCRC32.FillTableCRC32RefOut(t2, poly, true, false);

//var data = "Hello, CRC32!"u8.ToArray();

//Console.WriteLine($"data: {data.Select(b => b.ToString("x2")).JoinStrings("")}");

//var crc = 0xFFFFFFFFu;

//foreach (var b in data)
//{
//    var i = (crc ^ b) & 0xFF;
//    crc = (crc >> 8) ^ table[i];
//}

//crc ^= 0xFFFFFFFFu;

//Console.WriteLine($"{crc:x8}");

double[,] A1 =
{
    { 0, 2, 3 },
    { 4, 5, 6 },
    { 1, 2, 3 }
};

double[] b1 = [4, 8, 12];
var x = new double[3];

Matrix.Array.Solve(A1, b1, x);



Matrix.Array.Triangulate(A1, out var p, out var d);

double[,] A = 
{
    { 1, 2, 3, },
    { 5, 6, 7, },
    { 9, 11, 11, },
};

double[,] b =
{
    { 4, },
    { 8, },
    { 12, },
};



Matrix.Array.Solve(A, b1, x);

Console.WriteLine("End.");
return;
