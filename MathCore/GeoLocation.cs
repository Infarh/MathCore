using System;
using System.Drawing;
using System.Globalization;

using MathCore.Geolocation;
using MathCore.Vectors;

using static System.Math;

namespace MathCore
{
    /// <summary>Географическое положение</summary>
    public readonly struct GeoLocation
    {
        /// <summary>Радиус Земли в метрах</summary>
        public const double EarthRadius = 6_378_137d;

        /// <summary>Преобразование градусов в радианы</summary>
        internal const double ToRad = PI / 180;

        /// <summary>Широта в градусах</summary>
        public double Latitude { get; init; }

        /// <summary>Долгота в градусах</summary>
        public double Longitude { get; init; }

        /// <summary>Градусы широты</summary>
        public int LatAngle => (int)Latitude;

        /// <summary>Минуты широты</summary>
        public int LatMinutes => (int)((Latitude - LatAngle) * 60);

        /// <summary>Секунды широты</summary>
        public double LatSeconds => ((Latitude - LatAngle) * 60 - LatMinutes) * 60;

        /// <summary>Градусы долготы</summary>
        public int LonAngle => (int)Longitude;

        /// <summary>Минуты долготы</summary>
        public int LonMinutes => (int)((Longitude - LonAngle) * 60);

        /// <summary>Секунды долготы</summary>
        public double LonSeconds => ((Longitude - LonAngle) * 60 - LonMinutes) * 60;

        /// <summary>Инициализация нового географического положения</summary>
        /// <param name="Latitude">Широта в градусах</param>
        /// <param name="Longitude">Долгота в градусах</param>
        public GeoLocation(double Latitude, double Longitude)
        {
            this.Latitude = Latitude;
            this.Longitude = Longitude;
        }

        /// <summary>Инициализация нового географического положения из кортежа положения</summary>
        /// <param name="Point">Кортеж со значениями широты и долготы</param>
        public GeoLocation((double Latitude, double Longitude) Point) => (Latitude, Longitude) = Point;

        /// <summary>Инициализация нового географического положения из двумерного вектора</summary>
        /// <param name="Point">Вектор положения</param>
        public GeoLocation(Vector2D Point) => (Longitude, Latitude) = Point;

        /// <summary>Инициализация нового географического положения из точки</summary>
        /// <param name="Point">Точка положения</param>
        public GeoLocation(Point Point)
        {
            Latitude = Point.Y;
            Longitude = Point.X;
        }

        /// <summary>Инициализация нового географического положения из точки</summary>
        /// <param name="Point">Точка положения</param>
        public GeoLocation(PointF Point)
        {
            Latitude = Point.Y;
            Longitude = Point.X;
        }

        /// <summary>Деконструктор географического положения на широту и долготу</summary>
        /// <param name="latitude">Широта</param>
        /// <param name="longitude">Долгота</param>
        public void Deconstruct(out double latitude, out double longitude)
        {
            latitude = Latitude;
            longitude = Longitude;
        }

        /// <summary>Вычисление дистанции до другого географического положения в метрах</summary>
        /// <param name="Location">Вторая точка географического положения</param>
        /// <returns>Расстояние до указанной точки в метрах</returns>
        public double DistanceTo(in GeoLocation Location) => GPS.LengthBetween(this, Location);

        /// <summary>Определение курса в направлении на указанную географическую точку (в градусах)</summary>
        /// <param name="Location">Точка назначения</param>
        /// <returns>Угол курса в градусах</returns>
        public double HeadingTo(GeoLocation Location) => GPS.Heading(this, Location);

        /// <summary>Определение точки места назначения по курсу в градусах и дистанции в метрах</summary>
        /// <param name="Heading">Угол курса в градусах</param>
        /// <param name="Distance">Дистанция в метрах</param>
        /// <returns>Новая точка места назначения на заданном удалении и с заданным курсом</returns>
        public GeoLocation Destination(double Heading, double Distance) => GPS.DestinationPoint(this, Heading, Distance);

        public override string ToString()
        {
            var lat = Latitude;
            var lon = Longitude;

            var lat_sign = Sign(lat);
            var lon_sign = Sign(lon);

            lat = Abs(lat);
            lon = Abs(lon);

            var lat_angle = (int)lat;
            var lon_angle = (int)lon;

            lat -= lat_angle;
            lon -= lon_angle;

            lat *= 60;
            lon *= 60;

            var lat_min = (int)lat;
            var lon_min = (int)lon;

            lat -= lat_min;
            lon -= lon_min;

            lat *= 60;
            lon *= 60;

            FormattableString result = $"{lat_angle}°{lat_min:00}'{lat:00.############}''{(lat_sign >= 0 ? "N" : "S")}, {lon_angle}°{lon_min:00}'{lon:00.############}''{(lon_sign >= 0 ? "E" : "W")}";

            return result.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>Оператор неявного приведения кортежа географических координат в структуру географического положения</summary>
        /// <param name="Point">Кортеж, содержащий широту и долготу</param>
        public static implicit operator GeoLocation((double Latitude, double Longitude) Point) => new(Point);

        /// <summary>Оператор неявного преобразования географического положения в кортеж, содержащий широту и долготу</summary>
        /// <param name="Location">Структура географического положения</param>
        public static implicit operator (double Latitude, double Longitude)(GeoLocation Location) => (Location.Latitude, Location.Longitude);

        /// <summary>Оператор неявного преобразования из географического положения в двумерный вектор</summary>
        /// <param name="Location">Географическое положение</param>
        public static implicit operator Vector2D(GeoLocation Location) => new(Location.Longitude, Location.Latitude);

        /// <summary>Оператор неявного преобразования из двумерного вектора в географическое положение</summary>
        /// <param name="Point">Точка географического положения</param>
        public static implicit operator GeoLocation(Vector2D Point) => new(Point);

        /// <summary>Оператор неявного преобразования из географического положения в точку</summary>
        /// <param name="Location">Географическое положение</param>
        public static implicit operator Point(GeoLocation Location) => new(Location.LonAngle, Location.LatAngle);

        /// <summary>Оператор неявного преобразования из точки в географическое положение</summary>
        /// <param name="Point">Точка географического положения</param>
        public static implicit operator GeoLocation(Point Point) => new(Point);

        /// <summary>Оператор неявного преобразования из географического положения в точку</summary>
        /// <param name="Location">Географическое положение</param>
        public static implicit operator PointF(GeoLocation Location) => new((float)Location.Longitude, (float)Location.Latitude);

        /// <summary>Оператор неявного преобразования из точки в географическое положение</summary>
        /// <param name="Point">Точка географического положения</param>
        public static implicit operator GeoLocation(PointF Point) => new(Point);
    }
}
