#nullable enable
using System;
using System.Diagnostics;

// ReSharper disable UnusedMember.Global

namespace MathCore.MathParser;

/// <summary>Скобки</summary>
public class Bracket : IEquatable<Bracket>, ICloneable<Bracket>
{
    /// <summary>Круглые скобки</summary>
    public static Bracket NewRound => new("(", ")");

    /// <summary>Квадратные скобки</summary>
    public static Bracket NewRect => new("[", "]");

    /// <summary>Фигурные скобки</summary>
    public static Bracket NewFigure => new("{", "}");

    /// <summary>Открывающая скобка</summary>
    public string Start { get; }

    /// <summary>Закрывающая скобка</summary>
    public string Stop { get; }

    /// <summary>Скобки</summary>
    /// <param name="Start">Строка открывающей скобки</param>
    /// <param name="Stop">Строка закрывающей скобки</param>
    public Bracket(string Start, string Stop)
    {
        this.Start = Start;
        this.Stop  = Stop;
    }

    /// <summary>Проверка на эквивалентность другим скобкам</summary>
    /// <param name="other">Проверяемые на эквивалентность скобки</param>
    /// <returns>Истина, если проверяемые скобки эквивалентны данным</returns>
    public bool Equals(Bracket? other) => other != null && (ReferenceEquals(this, other) || string.Equals(Start, other.Start) && string.Equals(Stop, other.Stop));

    /// <summary>Проверка на эквивалентность</summary>
    /// <param name="obj">Проверяемый объект</param>
    /// <returns>Истина, если объект - скобки и вид скобок совпадает</returns>
    public override bool Equals(object? obj) => obj != null && (ReferenceEquals(this, obj) || obj.GetType() == GetType() && Equals((Bracket)obj));

    /// <summary>Получить хэш-код</summary>
    /// <returns>Хэш-код</returns>
    public override int GetHashCode() => unchecked(Start.GetHashCode() * 397 ^ Stop.GetHashCode());

    object ICloneable.Clone() => Clone();

    /// <summary>Клонирование скобок</summary>
    /// <returns>Клон скобок</returns>
    public Bracket Clone() => new(Start, Stop);

    /// <summary>Строковое представление скобок</summary>
    /// <returns>Строковое представление</returns>
    public override string ToString() => Surround("...");

    /// <summary>Разместить текст в скобках</summary>
    /// <param name="str">Размещаемый текст</param>
    /// <returns>Текст в скобках</returns>
    public string Surround(string? str)
    {
        Trace.TraceWarning("В обёртку блока скобок передана пустая строка");
        return $"{Start}{str}{Stop}";
    }
}