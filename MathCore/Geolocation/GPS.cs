using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathCore.Vectors;
using static System.Math;

namespace MathCore.Geolocation
{
    /// <summary>Класс сервисных функций работы с координатами</summary>
    public static class GPS
    {
        private const double c_ToRad = PI / 180d;

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

        /// <summary>Радиус Земли в метрах</summary>
        public const double EarthRadius = 6_371_000D;

        public static double ParallelLength(double latitude) => ParallelEquatorLength * Cos(latitude * c_ToRad);

        public static double Degree(int degrees, int minutes, double seconds = 0d)
        {
            return degrees + minutes / 60d + seconds / 3600d;
        }

        /// <summary>Вычисление расстояния между двумя точками на поверхности земли, заданными своими координатами</summary>
        /// <param name="latitude1">Широта первой точки в градусах</param>
        /// <param name="longitude1">Долгота первой точки в градусах</param>
        /// <param name="latitude2">Широта второй точки в градусах</param>
        /// <param name="longitude2">Долгота второй точки в градусах</param>
        /// <returns>Длина дуги на поверхности Земли, начинающейся в первой точке и заканчивающейся во второй точке</returns>
        [Copyright("Chris Veness 2002-2017", url = "https://www.movable-type.co.uk/scripts/latlong.html")]
        public static double LengthBetween(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            if (double.IsNaN(latitude1) || double.IsNaN(longitude1) || double.IsNaN(latitude2) || double.IsNaN(longitude2))
                return double.NaN;

            latitude1 *= c_ToRad;
            latitude2 *= c_ToRad;
            longitude1 *= c_ToRad;
            longitude2 *= c_ToRad;

            var d_latitude = latitude2 - latitude1;
            var d_longitude = longitude2 - longitude1;
            var sin_d_lat05 = Sin(d_latitude / 2);
            var sin_d_lon05 = Sin(d_longitude / 2);
            var a = sin_d_lat05 * sin_d_lat05 + Cos(latitude1) * Cos(latitude2) * sin_d_lon05 * sin_d_lon05;
            return 2 * Atan2(Sqrt(a), Sqrt(1 - a)) * EarthRadius;
        }

        public static double LengthBetween(Vector2D begin, Vector2D end) => LengthBetween(begin.Y, begin.X, end.Y, end.X);

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

            latitude1 *= c_ToRad;
            latitude2 *= c_ToRad;
            longitude1 *= c_ToRad;
            longitude2 *= c_ToRad;

            //var x = (longitude2 - longitude1) * Cos((latitude1 + latitude2) / 2);
            //var y = latitude2 - latitude1;
            //return Sqrt(x * x + y * y) * EarthRadius;
            var th1 = PI / 2 - latitude1;
            var th2 = PI / 2 - latitude2;
            return EarthRadius * Sqrt(th1 * th1 + th2 * th2 - 2 * th1 * th2 * Cos(longitude2 - longitude1));

        }

        public static double SphericalLawOfCosines_LengthBetween(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            if (double.IsNaN(latitude1) || double.IsNaN(longitude1) || double.IsNaN(latitude2) || double.IsNaN(longitude2))
                return double.NaN;

            latitude1 *= c_ToRad;
            latitude2 *= c_ToRad;
            longitude1 *= c_ToRad;
            longitude2 *= c_ToRad;

            return Acos(Sin(latitude1) * Sin(latitude2) + Cos(latitude1) * Cos(latitude2) * Cos(longitude2 - longitude1)) * EarthRadius;
        }

        /// <summary>Определение курса по координатам начальной и конечной точки</summary>
        /// <param name="latitude1">Широта первой иходной точки</param>
        /// <param name="longitude1">Долгота первой иходной точки</param>
        /// <param name="latitude2">Широта второй иходной точки</param>
        /// <param name="longitude2">Долгота второй иходной точки</param>
        /// <returns>Курс в градусах</returns>
        public static double Heading(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            if (double.IsNaN(latitude1) || double.IsNaN(longitude1) || double.IsNaN(latitude2) || double.IsNaN(longitude2))
                return double.NaN;

            latitude1 *= c_ToRad;
            latitude2 *= c_ToRad;
            longitude1 *= c_ToRad;
            longitude2 *= c_ToRad;

            var d_lon = longitude2 - longitude1;
            var y = Sin(d_lon) * Cos(latitude2);
            var x = Cos(latitude1) * Sin(latitude2)
                   - Sin(latitude1) * Cos(latitude2) * Cos(d_lon);
            return (Atan2(y, x) / c_ToRad + 360) % 360;
        }

        public static double Heading(Vector2D begin, Vector2D end) => Heading(begin.Y, begin.X, end.Y, end.X);

        /// <summary>Определение точки на середине отрезка, заданного двумя точками</summary>
        /// <param name="latitude1">Широта первой иходной точки</param>
        /// <param name="longitude1">Долгота первой иходной точки</param>
        /// <param name="latitude2">Широта второй иходной точки</param>
        /// <param name="longitude2">Долгота второй иходной точки</param>
        /// <returns>Точка в середине отрезка</returns>
        public static Vector2D HalfWayPoint(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            if (double.IsNaN(latitude1) || double.IsNaN(longitude1) || double.IsNaN(latitude2) || double.IsNaN(longitude2))
                return new Vector2D(double.NaN, double.NaN);

            latitude1 *= c_ToRad;
            latitude2 *= c_ToRad;
            longitude1 *= c_ToRad;
            longitude2 *= c_ToRad;

            var d_lon = longitude2 - longitude1;
            var cos_lat1 = Cos(latitude1);
            var cos_lat2 = Cos(latitude2);

            var bx = cos_lat2 * Cos(d_lon);
            var by = cos_lat2 * Sin(d_lon);
            var latitude_05 = Atan2(Sin(latitude1) + Sin(latitude2), Sqrt((cos_lat1 + bx) * (cos_lat1 + bx) + by * by));
            var longitude_05 = longitude1 + Atan2(by, cos_lat1 + bx);
            return new Vector2D(longitude_05 / c_ToRad, latitude_05 / c_ToRad);
        }

        public static Vector2D HalfWayPoint(Vector2D begin, Vector2D end) => HalfWayPoint(begin.Y, begin.X, end.Y, end.X);

        /// <summary>Определение точки места назначения по исходной точке, курсу и расстоянию</summary>
        /// <param name="latitude">Широта исходной точки</param>
        /// <param name="longitude">Долгота исхоной точки</param>
        /// <param name="heading">Курс на точкуу назначения</param>
        /// <param name="distance">Пройденная дистанция в метрах</param>
        /// <returns>Точка назначения</returns>
        public static Vector2D DestinationPoint(double latitude, double longitude, double heading, double distance)
        {
            if (double.IsNaN(latitude) || double.IsNaN(longitude) || double.IsNaN(heading) || double.IsNaN(distance))
                return new Vector2D(double.NaN, double.NaN);

            latitude *= c_ToRad;
            longitude *= c_ToRad;
            if (heading < 0 || heading > 360) heading = (heading + 360) % 360;
            heading *= c_ToRad;

            distance /= EarthRadius;

            var sin_lat = Sin(latitude);
            var cos_lat = Cos(latitude);
            var sin_d = Sin(distance);
            var cos_d = Cos(distance);

            var sin_latitude2 = sin_lat * cos_d + cos_lat * sin_d * Cos(heading);
            var longitude2 = longitude + Atan2(Sin(heading) * sin_d * cos_lat, cos_d - sin_lat * sin_latitude2);
            return new Vector2D((longitude2 / c_ToRad + 540) % 360 - 180, Asin(sin_latitude2) / c_ToRad);
        }

        public static Vector2D DestinationPoint(Vector2D point, double heading, double distance) =>
            DestinationPoint(point.Y, point.X, heading, distance);

        /// <summary>Определение точки места назначения по исходной точке, курсу и расстоянию</summary>
        /// <param name="latitude">Широта исходной точки</param>
        /// <param name="longitude">Долгота исхоной точки</param>
        /// <param name="heading">Курс на точкуу назначения</param>
        /// <param name="distance">Пройденная дистанция в метрах</param>
        /// <param name="final_heading">Курс из точки назначения на исходную точку</param>
        /// <returns>Точка назначения</returns>
        public static Vector2D DestinationPoint(double latitude, double longitude, double heading, double distance, out double final_heading)
        {
            var result = DestinationPoint(latitude, longitude, heading, distance);
            final_heading = (Heading(result.Y, result.X, latitude, longitude) + 180) % 360;
            return result;
        }

        public static Vector2D DestinationPoint(Vector2D point, double heading, double distance, out double final_heading) =>
            DestinationPoint(point.Y, point.X, heading, distance, out final_heading);


        /// <summary>Определение точки персечения двух курсов, каждый из которых задан исходной точкой</summary>
        /// <param name="latitude1">Широта первой иходной точки</param>
        /// <param name="longitude1">Долгота первой иходной точки</param>
        /// <param name="heading1">Курс из первой иходной точки</param>
        /// <param name="latitude2">Широта второй иходной точки</param>
        /// <param name="longitude2">Долгота второй иходной точки</param>
        /// <param name="heading2">Курс второй исходной точки</param>
        /// <returns>Точка пересечения двух курсов</returns>
        public static Vector2D Intersection
        (
            double latitude1, double longitude1, double heading1,
            double latitude2, double longitude2, double heading2
        )
        {
            if (double.IsNaN(latitude1) || double.IsNaN(longitude1)
                || double.IsNaN(latitude2) || double.IsNaN(longitude2)
                || double.IsNaN(heading1) || double.IsNaN(heading2))
                return double.NaN;

            latitude1 *= c_ToRad;
            latitude2 *= c_ToRad;
            longitude1 *= c_ToRad;
            longitude2 *= c_ToRad;
            heading1 *= c_ToRad;
            heading2 *= c_ToRad;

            var d_lat = latitude2 - latitude1;
            var d_lon = longitude2 - longitude1;
            var sin_d_lat05 = Sin(d_lat / 2);
            var sin_d_lon05 = Sin(d_lon / 2);

            var cos_lat1 = Cos(latitude1);
            var cos_lat2 = Cos(latitude2);
            var sin_lat1 = Sin(latitude1);
            var sin_lat2 = Sin(latitude2);

            var angular_distance = 2 * Asin(Sqrt(sin_d_lat05 * sin_d_lat05 + cos_lat1 * cos_lat2 * sin_d_lon05 * sin_d_lon05));
            var sin_angular_distance = Sin(angular_distance);
            var cos_angular_distance = Cos(angular_distance);
            var init_heading = Acos((sin_lat2 - sin_lat1 * Cos(angular_distance)) / (sin_angular_distance * cos_lat1));
            if (double.IsNaN(init_heading)) init_heading = 0;
            var final_heading = Acos((sin_lat1 - sin_lat2 * Cos(angular_distance)) / (sin_angular_distance * cos_lat2));

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
                return new Vector2D(double.NaN, double.NaN);

            var a3 = Acos(-cos_a1 * cos_a2 + sin_a1 * sin_a2 * cos_angular_distance);
            var cos_a3 = Cos(a3);
            var angular_distance_p1_p2 = Atan2(sin_angular_distance * sin_a1 * sin_a2, cos_a2 + cos_a1 * cos_a3);
            var sin_angular_distance_p1_p2 = Sin(angular_distance_p1_p2);
            var cos_angular_distance_p1_p2 = Cos(angular_distance_p1_p2);
            var latitude3 = Asin(sin_lat1 * cos_angular_distance_p1_p2 + cos_lat1 * sin_angular_distance_p1_p2 * Cos(heading1));
            var d_lon_13 = Atan2(Sin(heading1) * sin_angular_distance_p1_p2 * cos_lat1, cos_angular_distance_p1_p2 - sin_lat1 * Sin(latitude3));
            var longitude3 = longitude1 + d_lon_13;
            return new Vector2D((longitude3 / c_ToRad + 540) % 360 - 180, latitude3 / c_ToRad);
        }

        public static Vector2D Intersection(Vector2D point1, double heading1, Vector2D point2, double heading2) =>
            Intersection(point1.Y, point1.X, heading1, point2.Y, point2.X, heading2);
    }
}
