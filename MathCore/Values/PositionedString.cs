﻿#nullable enable
// ReSharper disable UnusedType.Global

// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.Values;

public class PositionedString(string String, int Position = 0)
{
    public int Position { get; set; } = Position;

    public string String { get; set; } = String;

    public string GetSubstringForward(int Length) => String.Substring(Position, Length);

    public string GetSubstringBackward(int Length)
    {
        var pos = Position - Length + 1;
        if(pos >= 0) return String.Substring(pos, Length);
        Length += pos;
        pos    =  0;
        return String.Substring(pos, Length);
    }

    public string MoveForward(int Length)
    {
        var str = GetSubstringForward(Length); 
        Position += Length; 
        return str;
    }

    public string MoveBackward(int Length)
    {
        var str = GetSubstringBackward(Length); 
        Position -= Length;
        return str;
    }

    public void ShowCursor() => ShowCursor(Position);

    public void ShowCursor(int position)
    {
        var sub_str    = string.Empty;
        var visual_pos = position;
        if(position < 0)
        {
            sub_str  = new(' ', -position);
            position = 0;
        }
        Console.WriteLine("{1}{0}", String, sub_str);
        Console.WriteLine("{0}^({1})", new string(' ', position), visual_pos);
    }

    public void ShowSubstringForward(int length) => ShowSubstringForward(Position, length);
    public void ShowSubstringForward(int position, int length)
    {
        Console.WriteLine(String);
        Console.WriteLine("{0}{1}", new string(' ', position), new string('^', length));
    }

    public void ShowSubstringBackward(int length) => ShowSubstringBackward(Position, length);
    public void ShowSubstringBackward(int position, int length)
    {
        Console.WriteLine(String);
        Console.WriteLine("{0}{1}", new string(' ', position - 1 - length), new string('^', length));
    }
}