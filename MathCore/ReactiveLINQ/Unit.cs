#nullable enable
using System.Runtime.InteropServices;

// ReSharper disable UnusedParameter.Global

// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter

// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive;

/// <summary>
/// Represents a type with a single value.
/// This type is often used to denote the successful completion of a void-returning method (C#)
/// or a Sub procedure (Visual Basic).
/// </summary>
[Serializable]
[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct Unit : IEquatable<Unit>
{
    private static readonly Unit __Default;

    /// <summary>Gets the single unit value</summary>
    public static Unit Default => __Default;

    /// <summary>
    /// Determines whether the two specified Unit values are equal.
    /// Because Unit has a single value, this always returns true.
    /// </summary>
    /// <param name="first">The first Unit value to compare</param>
    /// <param name="second">The second Unit value to compare</param>
    /// <returns>Because Unit has a single value, this always returns true</returns>
    public static bool operator ==(Unit first, Unit second) => true;

    /// <summary>
    /// Determines whether the two specified Unit values are not equal.
    /// Because Unit has a single value, this always returns false.
    /// </summary>
    /// <param name="first">The first Unit value to compare</param>
    /// <param name="second">The second Unit value to compare</param>
    /// <returns>Because Unit has a single value, this always returns false</returns>
    public static bool operator !=(Unit first, Unit second) => false;

    /// <summary>
    /// Determines whether the specified Unit values is equal to the current Unit.
    /// Because Unit has a single value, this always returns true.
    /// </summary>
    /// <param name="other">An object to compare to the current Unit value</param>
    /// <returns>Because Unit has a single value, this always returns true</returns>
    public bool Equals(Unit other) => true;

    /// <summary>Determines whether the specified System.Object is equal to the current Unit</summary>
    /// <param name="obj">The System.Object to compare with the current Unit</param>
    /// <returns>true if the specified System.Object is a Unit value; otherwise, false</returns>
    public override bool Equals(object? obj) => obj is Unit;

    /// <summary>Returns the hash code for the current Unit value</summary>
    /// <returns>A hash code for the current Unit value</returns>
    public override int GetHashCode() => __Default.GetHashCode();

    /// <summary>Returns a string representation of the current Unit value</summary>
    /// <returns>String representation of the current Unit value</returns>
    public override string ToString() => "void";
}