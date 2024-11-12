using System.Drawing;

using MathCore.Vectors;

namespace MathCore.Geolocation;

public readonly struct GeoLocationSpan : IEquatable<GeoLocationSpan>
{
    /// <summary>Смещение по широте в градусах</summary>
    public double LatitudeDelta { get; init; }

    /// <summary>Смещение по долготе в градусах</summary>
    public double LongitudeDelta { get; init; }

    /// <summary>Инициализация нового географического положения</summary>
    /// <param name="LatitudeDelta">Смещение по широте в градусах</param>
    /// <param name="LongitudeDelta">Смещение по долготе в градусах</param>
    public GeoLocationSpan(double LatitudeDelta, double LongitudeDelta) => (this.LatitudeDelta, this.LongitudeDelta) = (LatitudeDelta, LongitudeDelta);

    /// <summary>Инициализация нового географического положения из кортежа положения</summary>
    /// <param name="Point">Кортеж со значениями смещения по широте и долготе</param>
    public GeoLocationSpan((double LatitudeDelta, double LongitudeDelta) Point) => (LatitudeDelta, LongitudeDelta) = Point;

    /// <summary>Инициализация нового географического положения из двумерного вектора</summary>
    /// <param name="Point">Вектор смещения</param>
    public GeoLocationSpan(Vector2D Point) => (LongitudeDelta, LatitudeDelta) = Point;

    /// <summary>Инициализация нового географического положения из точки</summary>
    /// <param name="Point">Точка со значениями смещения</param>
    public GeoLocationSpan(Point Point) => (LatitudeDelta, LongitudeDelta) = (Point.Y, Point.X);

    /// <summary>Инициализация нового географического положения из точки</summary>
    /// <param name="Point">Точка со значениями смещения</param>
    public GeoLocationSpan(PointF Point) => (LatitudeDelta, LongitudeDelta) = (Point.Y, Point.X);

    /// <summary>Деконструктор географического положения на широту и долготу</summary>
    /// <param name="DeltaLatitude">Смещение по широте в градусах</param>
    /// <param name="DeltaLongitude">Смещение по долготе в градусах</param>
    public void Deconstruct(out double DeltaLatitude, out double DeltaLongitude)
    {
        DeltaLatitude = LatitudeDelta;
        DeltaLongitude = LongitudeDelta;
    }

    public bool Equals(GeoLocationSpan delta) => delta.LatitudeDelta.Equals(LatitudeDelta) && delta.LongitudeDelta.Equals(LongitudeDelta);

    public override bool Equals(object obj) => obj is GeoLocationSpan delta && Equals(delta);

    public override int GetHashCode() => HashBuilder.New(LatitudeDelta).Append(LongitudeDelta);

    public static bool operator ==(GeoLocationSpan left, GeoLocationSpan right) => left.Equals(right);

    public static bool operator !=(GeoLocationSpan left, GeoLocationSpan right) => !left.Equals(right);

    /// <summary>Оператор неявного приведения кортежа географических координат в структуру географического положения</summary>
    /// <param name="Point">Кортеж, содержащий широту и долготу</param>
    public static implicit operator GeoLocationSpan((double Latitude, double Longitude) Point) => new(Point);

    /// <summary>Оператор неявного преобразования географического положения в кортеж, содержащий широту и долготу</summary>
    /// <param name="Location">Структура географического положения</param>
    public static implicit operator (double Latitude, double Longitude)(GeoLocationSpan Location) => (Location.LatitudeDelta, Location.LongitudeDelta);

    public static implicit operator Vector2D(GeoLocationSpan Location) => new(Location.LatitudeDelta, Location.LongitudeDelta);

    public static implicit operator PointF(GeoLocationSpan Location) => new((float)Location.LatitudeDelta, (float)Location.LongitudeDelta);

    public static implicit operator Point(GeoLocationSpan Location) => new((int)Location.LatitudeDelta, (int)Location.LongitudeDelta);
}