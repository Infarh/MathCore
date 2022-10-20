// ReSharper disable once CheckNamespace
namespace System.Drawing;

public static class SizeEx
{
    public static void Deconstruct(this Size size, out int Width, out int Height)
    {
        Width  = size.Width;
        Height = size.Height;
    } 

    public static void Deconstruct(this SizeF size, out float Width, out float Height)
    {
        Width  = size.Width;
        Height = size.Height;
    }
}