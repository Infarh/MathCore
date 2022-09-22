using MathCore.CSV;
using MathCore.PE;

var bits00 = BitConverter.DoubleToInt64Bits(0.0);
var bits01 = BitConverter.DoubleToInt64Bits(1.0);
var bits02 = BitConverter.DoubleToInt64Bits(2.0);
var bits04 = BitConverter.DoubleToInt64Bits(4.0);
var bits08 = BitConverter.DoubleToInt64Bits(8.0);
var bits016 = BitConverter.DoubleToInt64Bits(16.0);
var bits0 = BitConverter.DoubleToInt64Bits(1.0);
var bits1 = BitConverter.DoubleToInt64Bits(30);
var bits2 = BitConverter.DoubleToInt64Bits(300);
var bits3 = BitConverter.DoubleToInt64Bits(3000);
var bits4 = BitConverter.DoubleToInt64Bits(30000);

const long exp_mask      = (1 << 11) - 1;
const long exp_mask_full = exp_mask << 52;
const long mantissa_mask = (1 << 52) - 1;


var exp1  = (int)((bits1 & exp_mask_full) >> 52);
var exp11 = exp1 - 1023;

var mantissa = bits1 & mantissa_mask;

var bits00_str = Convert.ToString(bits00, 2).PadLeft(64, '0');
var bits01_str = Convert.ToString(bits01, 2).PadLeft(64, '0');
var bits02_str = Convert.ToString(bits02, 2).PadLeft(64, '0');
var bits04_str = Convert.ToString(bits04, 2).PadLeft(64, '0');
var bits08_str = Convert.ToString(bits08, 2).PadLeft(64, '0');
var bits016_str = Convert.ToString(bits016, 2).PadLeft(64, '0');
var bits0_str = Convert.ToString(bits0, 2).PadLeft(64, '0');
var bits1_str = Convert.ToString(bits1, 2).PadLeft(64, '0');
var bits2_str = Convert.ToString(bits2, 2).PadLeft(64, '0');
var bits3_str = Convert.ToString(bits3, 2).PadLeft(64, '0');
var bits4_str = Convert.ToString(bits4, 2).PadLeft(64, '0');

Console.WriteLine("  0.0 =        {0}", bits00_str);
Console.WriteLine("  1.0 =        {0}", bits01_str);
Console.WriteLine("  2.0 =        {0}", bits02_str);
Console.WriteLine("  4.0 =        {0}", bits04_str);
Console.WriteLine("  8.0 =        {0}", bits08_str);
Console.WriteLine(" 16.0 =        {0}", bits016_str);
Console.WriteLine("  3.0 = s{0} e {1} m {2}", bits0_str[0], bits0_str[1..12], bits0_str[^52..]);
Console.WriteLine("   30 = s{0} e {1} m {2}", bits1_str[0], bits1_str[1..12], bits1_str[^52..]);
Console.WriteLine("  300 = s{0} e {1} m {2}", bits2_str[0], bits2_str[1..12], bits2_str[^52..]);
Console.WriteLine(" 3000 = s{0} e {1} m {2}", bits3_str[0], bits3_str[1..12], bits3_str[^52..]);
Console.WriteLine("30000 = s{0} e {1} m {2}", bits4_str[0], bits4_str[1..12], bits4_str[^52..]);

Console.ReadLine();

return;

//var pe_file = new PEFile("c:\\123\\user32.dll");

////pe_file.ReadData();

////var is_pe = pe_file.IsPE;
//var header = pe_file.GetHeader();

//var range = new Range(-10, 5);

//Console.WriteLine("End.");
//Console.ReadLine();