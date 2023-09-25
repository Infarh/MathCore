﻿using MathCore.Vectors;

using static System.Math;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.Geolocation;

/// <summary>Класс сервисных функций работы с координатами</summary>
public static class GPS
{
    // ReSharper disable InconsistentNaming
    /// <summary>PI / 180</summary>
    private const double ToRad = MathCore.Consts.ToRad;
    /// <summary>180 / PI</summary>
    private const double ToDeg = MathCore.Consts.ToDeg;
    /// <summary>PI / 2</summary>
    private const double PI05 = MathCore.Consts.pi05;
    // ReSharper restore InconsistentNaming

    /// <summary>Константы размеров</summary>
    public static class Consts
    {
        /// <summary>Длина окружности меридиана (в метрах)</summary>
        public const double MeridianLength = 40_008_548; //40_008_550;

        /// <summary>Длина дуги меридиана в 1 градус (в метрах)</summary>
        public const double Meridian1DegreeLength = MeridianLength / 360d;

        /// <summary>Длина дуги меридиана в 1 минуту (в метрах)</summary>
        public const double Meridian1MinuteLength = Meridian1DegreeLength / 60d;

        /// <summary>Длина дуги меридиана в 1 секунду (в метрах)</summary>
        public const double Meridian1SecondLength = Meridian1MinuteLength / 60d;

        /// <summary>Длина дуги меридиана в 1 радиан (в метрах)</summary>
        public const double Meridian1RadianLength = MeridianLength / PI;

        /// <summary>Длина окружности параллели на экваторе (в метрах)</summary>
        public const double ParallelEquatorLength = 40_075_695D;

        /// <summary>Длина дуги параллели на экваторе в 1 градус (в метрах)</summary>
        public const double ParallelEquator1DegreeLength = ParallelEquatorLength / 360d;

        /// <summary>Длина дуги параллели на экваторе в 1 минуту (в метрах)</summary>
        public const double ParallelEquator1MinuteLength = ParallelEquator1DegreeLength / 60d;

        /// <summary>Длина дуги параллели на экваторе в 1 секунду (в метрах)</summary>
        public const double ParallelEquator1SecondLength = ParallelEquator1MinuteLength / 60d;

        /// <summary>Длина дуги параллели на экваторе в 1 радиан (в метрах)</summary>
        public const double ParallelEquator1RadianLength = ParallelEquatorLength / PI;

        /// <summary>Радиус Земли (в метрах)</summary>
        public const double EarthRadius = 6_378_137d;

        public const double Flattening = 1d / 298.257223563;

        public static readonly double Eccentricity = Sqrt((2 - Flattening) * Flattening);

        public const double MetersPerDegree = EarthRadius * PI / 180d;

        /// <summary>Градус широты в метрах на экваторе (в метрах)</summary>
        public const double LatitudeDegreeLength = 111.321377778;

        /// <summary>Минута широты в метрах на экваторе (в метрах)</summary>
        public const double LatitudeMinuteLength = 1.8553562963;

        /// <summary>Секунда широты в метрах на экваторе (в метрах)</summary>
        public const double LatitudeSecondLength = 0.0309226049383;

        /// <summary>Градус долготы в метрах на экваторе (в метрах)</summary>
        public const double LongitudeDegreeLength = 111.134861111;

        /// <summary>Минута долготы в метрах на экваторе (в метрах)</summary>
        public const double LongitudeMinuteLength = 1.85224768519;

        /// <summary>Секунда долготы в метрах на экваторе (в метрах)</summary>
        public const double LongitudeSecondLength = 0.0308707947531;
    }

    public static double ParallelLength(double latitude) => Consts.ParallelEquatorLength * Cos(latitude * ToRad);

    public static double Degree(int degrees, int minutes, double seconds = 0d) => degrees + minutes / 60d + seconds / 3600d;

    /// <summary>Вычисление расстояния между двумя точками на поверхности земли, заданными своими координатами</summary>
    /// <param name="latitude1">Широта первой точки в градусах</param>
    /// <param name="longitude1">Долгота первой точки в градусах</param>
    /// <param name="latitude2">Широта второй точки в градусах</param>
    /// <param name="longitude2">Долгота второй точки в градусах</param>
    /// <returns>Длина дуги на поверхности Земли, начинающейся в первой точке и заканчивающейся во второй точке</returns>
    // ReSharper disable once StringLiteralTypo
    [Copyright("Chris Veness 2002-2017", url = "https://www.movable-type.co.uk/scripts/latlong.html")]
    public static double LengthBetween(double latitude1, double longitude1, double latitude2, double longitude2)
    {
        if (double.IsNaN(latitude1) || double.IsNaN(longitude1) || double.IsNaN(latitude2) || double.IsNaN(longitude2))
            return double.NaN;

        latitude1  *= ToRad;
        latitude2  *= ToRad;
        longitude1 *= ToRad;
        longitude2 *= ToRad;

        var d_latitude  = latitude2 - latitude1;
        var d_longitude = longitude2 - longitude1;
        var sin_d_lat05 = Sin(d_latitude / 2);
        var sin_d_lon05 = Sin(d_longitude / 2);
        var a           = sin_d_lat05 * sin_d_lat05 + Cos(latitude1) * Cos(latitude2) * sin_d_lon05 * sin_d_lon05;
        return 2 * Atan2(Sqrt(a), Sqrt(1 - a)) * Consts.EarthRadius;
    }

    /// <summary>Вычисление расстояния между двумя точками на поверхности земли, заданными своими координатами</summary>
    /// <param name="begin">Начало</param>
    /// <param name="end">Конец</param>
    public static double LengthBetween(Vector2D begin, Vector2D end) => LengthBetween(begin.Y, begin.X, end.Y, end.X);

    /// <summary>Вычисление расстояния между двумя точками на поверхности земли, заданными своими координатами</summary>
    /// <param name="begin">Начало</param>
    /// <param name="end">Конец</param>
    public static double LengthBetween(GeoLocation begin, GeoLocation end) => LengthBetween(begin.Latitude, begin.Longitude, end.Latitude, end.Longitude);

    /// <summary>Вычисление расстояния между двумя точками на поверхности земли (в равнопромежуточной проекции), заданными своими координатами</summary>
    /// <param name="latitude1">Широта первой точки в градусах</param>
    /// <param name="longitude1">Долгота первой точки в градусах</param>
    /// <param name="latitude2">Широта второй точки в градусах</param>
    /// <param name="longitude2">Долгота второй точки в градусах</param>
    /// <returns>Расстояние между двумя точками</returns>
    /// <remarks>Алгоритм требуем меньше вычислительных ресурсов, но даёт большую погрешность</remarks>
    public static double EquirectangularApproximation_LengthBetween(double latitude1, double longitude1, double latitude2, double longitude2)
    {
        if (double.IsNaN(latitude1) || double.IsNaN(longitude1) || double.IsNaN(latitude2) || double.IsNaN(longitude2))
            return double.NaN;

        latitude1  *= ToRad;
        latitude2  *= ToRad;
        longitude1 *= ToRad;
        longitude2 *= ToRad;

        var th1 = PI / 2 - latitude1;
        var th2 = PI / 2 - latitude2;
        return Consts.EarthRadius * Sqrt(th1 * th1 + th2 * th2 - 2 * th1 * th2 * Cos(longitude2 - longitude1));

    }

    public static double SphericalLawOfCosines_LengthBetween(double latitude1, double longitude1, double latitude2, double longitude2)
    {
        if (double.IsNaN(latitude1) || double.IsNaN(longitude1) || double.IsNaN(latitude2) || double.IsNaN(longitude2))
            return double.NaN;

        latitude1  *= ToRad;
        latitude2  *= ToRad;
        longitude1 *= ToRad;
        longitude2 *= ToRad;

        return Acos(Sin(latitude1) * Sin(latitude2) + Cos(latitude1) * Cos(latitude2) * Cos(longitude2 - longitude1)) * Consts.EarthRadius;
    }

    /// <summary>Определение курса по координатам начальной и конечной точки</summary>
    /// <param name="latitude1">Широта первой исходной точки</param>
    /// <param name="longitude1">Долгота первой исходной точки</param>
    /// <param name="latitude2">Широта второй исходной точки</param>
    /// <param name="longitude2">Долгота второй исходной точки</param>
    /// <returns>Курс в градусах</returns>
    public static double Heading(double latitude1, double longitude1, double latitude2, double longitude2)
    {
        if (double.IsNaN(latitude1) || double.IsNaN(longitude1) || double.IsNaN(latitude2) || double.IsNaN(longitude2))
            return double.NaN;

        latitude1  *= ToRad;
        latitude2  *= ToRad;
        longitude1 *= ToRad;
        longitude2 *= ToRad;

        var d_lon = longitude2 - longitude1;
        var y     = Sin(d_lon) * Cos(latitude2);
        var x = Cos(latitude1) * Sin(latitude2)
            - Sin(latitude1) * Cos(latitude2) * Cos(d_lon);
        return (Atan2(y, x) / ToRad + 360) % 360;
    }

    /// <summary>Определение курса по координатам начальной и конечной точки</summary>
    /// <param name="begin">Исходная точка</param>
    /// <param name="end">Конечная точка</param>
    public static double Heading(Vector2D begin, Vector2D end) => Heading(begin.Y, begin.X, end.Y, end.X);

    /// <summary>Определение курса по координатам начальной и конечной точки</summary>
    /// <param name="begin">Исходная точка</param>
    /// <param name="end">Конечная точка</param>
    public static double Heading(GeoLocation begin, GeoLocation end) => Heading(begin.Latitude, begin.Longitude, end.Latitude, end.Longitude);

    /// <summary>Определение точки на середине отрезка, заданного двумя точками</summary>
    /// <param name="latitude1">Широта первой исходной точки</param>
    /// <param name="longitude1">Долгота первой исходной точки</param>
    /// <param name="latitude2">Широта второй исходной точки</param>
    /// <param name="longitude2">Долгота второй исходной точки</param>
    /// <returns>Точка в середине отрезка</returns>
    public static GeoLocation HalfWayPoint(double latitude1, double longitude1, double latitude2, double longitude2)
    {
        if (double.IsNaN(latitude1) || double.IsNaN(longitude1) || double.IsNaN(latitude2) || double.IsNaN(longitude2))
            return new(double.NaN, double.NaN);

        latitude1  *= ToRad;
        latitude2  *= ToRad;
        longitude1 *= ToRad;
        longitude2 *= ToRad;

        var d_lon    = longitude2 - longitude1;
        var cos_lat1 = Cos(latitude1);
        var cos_lat2 = Cos(latitude2);

        var bx           = cos_lat2 * Cos(d_lon);
        var by           = cos_lat2 * Sin(d_lon);
        var latitude_05  = Atan2(Sin(latitude1) + Sin(latitude2), Sqrt((cos_lat1 + bx) * (cos_lat1 + bx) + by * by));
        var longitude_05 = longitude1 + Atan2(by, cos_lat1 + bx);
        return new(latitude_05 / ToRad, longitude_05 / ToRad);
    }

    /// <summary>Определение курса по координатам начальной и конечной точки</summary>
    /// <param name="begin">Исходная точка</param>
    /// <param name="end">Конечная точка</param>
    public static GeoLocation HalfWayPoint(Vector2D begin, Vector2D end) => HalfWayPoint(begin.Y, begin.X, end.Y, end.X);

    /// <summary>Определение курса по координатам начальной и конечной точки</summary>
    /// <param name="begin">Исходная точка</param>
    /// <param name="end">Конечная точка</param>
    public static GeoLocation HalfWayPoint(GeoLocation begin, GeoLocation end) => HalfWayPoint(begin.Latitude, begin.Longitude, end.Latitude, end.Longitude);

    /// <summary>Определение точки места назначения по исходной точке, курсу и расстоянию</summary>
    /// <param name="latitude">Широта исходной точки</param>
    /// <param name="longitude">Долгота исходной точки</param>
    /// <param name="heading">Курс на точку назначения</param>
    /// <param name="distance">Пройденная дистанция в метрах</param>
    /// <returns>Точка назначения</returns>
    public static GeoLocation DestinationPoint(double latitude, double longitude, double heading, double distance)
    {
        if (double.IsNaN(latitude) || double.IsNaN(longitude) || double.IsNaN(heading) || double.IsNaN(distance))
            return new(double.NaN, double.NaN);

        latitude  *= ToRad;
        longitude *= ToRad;
        if (heading is < 0 or > 360) heading = (heading + 360) % 360;
        heading *= ToRad;

        distance /= Consts.EarthRadius;

        var sin_lat = Sin(latitude);
        var cos_lat = Cos(latitude);
        var sin_d   = Sin(distance);
        var cos_d   = Cos(distance);

        var sin_latitude2 = sin_lat * cos_d + cos_lat * sin_d * Cos(heading);
        var longitude2    = longitude + Atan2(Sin(heading) * sin_d * cos_lat, cos_d - sin_lat * sin_latitude2);
        return new(Asin(sin_latitude2) / ToRad, (longitude2 / ToRad + 540) % 360 - 180);
    }

    /// <summary>Определение точки места назначения по исходной точке, курсу и расстоянию</summary>
    /// <param name="point">Исходная точка</param>
    /// <param name="heading">Курс в градусах</param>
    /// <param name="distance">Расстояние в метрах</param>
    public static GeoLocation DestinationPoint(Vector2D point, double heading, double distance) =>
        DestinationPoint(point.Y, point.X, heading, distance);

    /// <summary>Определение точки места назначения по исходной точке, курсу и расстоянию</summary>
    /// <param name="point">Исходная точка</param>
    /// <param name="heading">Курс в градусах</param>
    /// <param name="distance">Расстояние в метрах</param>
    public static GeoLocation DestinationPoint(GeoLocation point, double heading, double distance) =>
        DestinationPoint(point.Latitude, point.Longitude, heading, distance);

    /// <summary>Определение точки места назначения по исходной точке, курсу и расстоянию</summary>
    /// <param name="latitude">Широта исходной точки</param>
    /// <param name="longitude">Долгота исходной точки</param>
    /// <param name="heading">Курс на точку назначения</param>
    /// <param name="distance">Пройденная дистанция в метрах</param>
    /// <param name="final_heading">Курс из точки назначения на исходную точку</param>
    /// <returns>Точка назначения</returns>
    public static GeoLocation DestinationPoint(double latitude, double longitude, double heading, double distance, out double final_heading)
    {
        var result = DestinationPoint(latitude, longitude, heading, distance);
        final_heading = (Heading(result.Latitude, result.Longitude, latitude, longitude) + 180) % 360;
        return result;
    }

    /// <summary>Определение точки места назначения по исходной точке, курсу и расстоянию</summary>
    /// <param name="point">Исходная точка</param>
    /// <param name="heading">Курс в градусах</param>
    /// <param name="distance">Расстояние</param>
    /// <param name="final_heading">Курс в конечной точке</param>
    public static GeoLocation DestinationPoint(Vector2D point, double heading, double distance, out double final_heading) =>
        DestinationPoint(point.Y, point.X, heading, distance, out final_heading);

    /// <summary>Определение точки пресечения двух курсов, каждый из которых задан исходной точкой</summary>
    /// <param name="latitude1">Широта первой исходной точки</param>
    /// <param name="longitude1">Долгота первой исходной точки</param>
    /// <param name="heading1">Курс из первой исходной точки</param>
    /// <param name="latitude2">Широта второй исходной точки</param>
    /// <param name="longitude2">Долгота второй исходной точки</param>
    /// <param name="heading2">Курс второй исходной точки</param>
    /// <returns>Точка пересечения двух курсов</returns>
    public static GeoLocation Intersection
    (
        double latitude1, double longitude1, double heading1,
        double latitude2, double longitude2, double heading2
    )
    {
        if (double.IsNaN(latitude1) || double.IsNaN(longitude1)
            || double.IsNaN(latitude2) || double.IsNaN(longitude2)
            || double.IsNaN(heading1) || double.IsNaN(heading2))
            return new(double.NaN, double.NaN);

        latitude1  *= ToRad;
        latitude2  *= ToRad;
        longitude1 *= ToRad;
        longitude2 *= ToRad;
        heading1   *= ToRad;
        heading2   *= ToRad;

        var d_lat       = latitude2 - latitude1;
        var d_lon       = longitude2 - longitude1;
        var sin_d_lat05 = Sin(d_lat / 2);
        var sin_d_lon05 = Sin(d_lon / 2);

        var cos_lat1 = Cos(latitude1);
        var cos_lat2 = Cos(latitude2);
        var sin_lat1 = Sin(latitude1);
        var sin_lat2 = Sin(latitude2);

        var angular_distance                         = 2 * Asin(Sqrt(sin_d_lat05 * sin_d_lat05 + cos_lat1 * cos_lat2 * sin_d_lon05 * sin_d_lon05));
        var sin_angular_distance                     = Sin(angular_distance);
        var cos_angular_distance                     = Cos(angular_distance);
        var init_heading                             = Acos((sin_lat2 - sin_lat1 * Cos(angular_distance)) / (sin_angular_distance * cos_lat1));
        if (double.IsNaN(init_heading)) init_heading = 0;
        var final_heading                            = Acos((sin_lat1 - sin_lat2 * Cos(angular_distance)) / (sin_angular_distance * cos_lat2));

        double th12, th21;
        if (d_lon > 0)
        {
            th12 = init_heading;
            th21 = 2 * PI - final_heading;
        }
        else
        {
            th12 = 2 * PI - final_heading;
            th21 = init_heading;
        }

        var a1 = heading1 - th12;
        var a2 = th21 - heading2;

        var sin_a1 = Sin(a1);
        var sin_a2 = Sin(a2);
        var cos_a1 = Cos(a1);
        var cos_a2 = Cos(a2);

        if (sin_a1.Equals(0d) && sin_a2.Equals(0d) || sin_a1 * sin_a2 < 0)
            return new(double.NaN, double.NaN);

        var a3                         = Acos(-cos_a1 * cos_a2 + sin_a1 * sin_a2 * cos_angular_distance);
        var cos_a3                     = Cos(a3);
        var angular_distance_p1_p2     = Atan2(sin_angular_distance * sin_a1 * sin_a2, cos_a2 + cos_a1 * cos_a3);
        var sin_angular_distance_p1_p2 = Sin(angular_distance_p1_p2);
        var cos_angular_distance_p1_p2 = Cos(angular_distance_p1_p2);
        var latitude3                  = Asin(sin_lat1 * cos_angular_distance_p1_p2 + cos_lat1 * sin_angular_distance_p1_p2 * Cos(heading1));
        var d_lon_13                   = Atan2(Sin(heading1) * sin_angular_distance_p1_p2 * cos_lat1, cos_angular_distance_p1_p2 - sin_lat1 * Sin(latitude3));
        var longitude3                 = longitude1 + d_lon_13;
        return new(latitude3 / ToRad, (longitude3 / ToRad + 540) % 360 - 180);
    }

    /// <summary>Определение точки пресечения двух курсов, каждый из которых задан исходной точкой</summary>
    /// <param name="point1">Первая точка</param>
    /// <param name="heading1">Курс в первой точке</param>
    /// <param name="point2">Вторая точка</param>
    /// <param name="heading2">Курс во второй точке</param>
    public static GeoLocation Intersection(Vector2D point1, double heading1, Vector2D point2, double heading2) =>
        Intersection(point1.Y, point1.X, heading1, point2.Y, point2.X, heading2);

    /// <summary>Определение точки пресечения двух курсов, каждый из которых задан исходной точкой</summary>
    /// <param name="point1">Первая точка</param>
    /// <param name="heading1">Курс в первой точке</param>
    /// <param name="point2">Вторая точка</param>
    /// <param name="heading2">Курс во второй точке</param>
    public static GeoLocation Intersection(GeoLocation point1, double heading1, GeoLocation point2, double heading2) =>
        Intersection(point1.Latitude, point1.Longitude, heading1, point2.Latitude, point2.Longitude, heading2);

    public static class MercatorProjection
    {
        public static double LatitudeToY(double Lat)
        {
            switch (Lat)
            {
                case <= -90: return double.NegativeInfinity;
                case >= 90:  return double.PositiveInfinity;
                default:
                    {
                        var lat = Lat * ToRad;
                        return Log(Tan(lat / 2 + PI / 4) * ConformalFactor(lat)) * ToDeg;
                    }
            }
        }

        private static double ConformalFactor(double lat)
        {
            var sin_lat = Consts.Eccentricity * Sin(lat);
            return Pow((1 - sin_lat) / (1 + sin_lat), Consts.Eccentricity / 2d);
        }

        public static double YToLatitude(double y)
        {
            var t     = Exp(-y * ToRad);
            var lat   = PI05 - 2 * Atan(t);
            var delta = 1d;

            const int    max_iterations = 10;
            const double eps            = 1e-6;
            for (var i = 0; i < max_iterations && delta > eps; i++)
            {
                var new_lat = PI05 - 2 * Atan(t * ConformalFactor(lat));
                delta = Abs(1 - new_lat / lat);
                lat   = new_lat;
            }

            return lat * ToDeg;
        }

        public static Vector2D ToXY(Vector2D LongitudeLatitude) => new(LongitudeLatitude.X, LatitudeToY(LongitudeLatitude.Y));
        public static Vector2D ToXY(GeoLocation Location) => new(Location.Longitude, LatitudeToY(Location.Latitude));

        public static GeoLocation FromXY(Vector2D Point) => new(YToLatitude(Point.Y), Point.X);

        public static (double X, double Y) ToXY(double Longitude, double Latitude) => (Longitude, LatitudeToY(Latitude));
        public static (double X, double Y) ToXY(in (double Longitude, double Latitude) Point) => (Point.Longitude, LatitudeToY(Point.Latitude));

        public static (double X, double Y) ToXYInMeters(double Longitude, double Latitude) => (Longitude * Consts.MetersPerDegree, LatitudeToY(Latitude) * Consts.MetersPerDegree);
        public static (double X, double Y) ToXYInMeters(in (double Longitude, double Latitude) Point) => (Point.Longitude * Consts.MetersPerDegree, LatitudeToY(Point.Latitude) * Consts.MetersPerDegree);

        public static (double Longitude, double Latitude) FromXY(double X, double Y) => (X, YToLatitude(Y));
        public static (double Longitude, double Latitude) FromXY(in (double X, double Y) Point) => (Point.X, YToLatitude(Point.Y));

        public static (double Longitude, double Latitude) FromXYInMeters(double X, double Y) => (X / Consts.MetersPerDegree, YToLatitude(Y) / Consts.MetersPerDegree);
        public static (double Longitude, double Latitude) FromXYInMeters(in (double X, double Y) Point) => (Point.X / Consts.MetersPerDegree, YToLatitude(Point.Y) / Consts.MetersPerDegree);
    }
}