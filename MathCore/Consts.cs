using System;
// ReSharper disable UnusedMember.Global

namespace MathCore
{
    /// <summary>Константы</summary>
    public static partial class Consts
    {
        /// <summary>% = 0.01</summary>
        public const double Percent = 0.01;

        /// <summary>e = 2.7182818284590452353602874713527</summary>
        public const double e = 2.7182818284590452353602874713527;

        /// <summary>π/16 (90°/8 = 11.25°)</summary>
        public const double pi00625 = .5 * pi0125;
        /// <summary>π/8 (90/4° = 22.5°)</summary>
        public const double pi0125 = .5 * pi025;
        /// <summary>π/4 (90°/2 = 45°)</summary>
        public const double pi025 = .5 * pi05;

        /// <summary>π/2 (90°)</summary>
        public const double pi05 = .5 * pi;
        /// <summary>-π/2 (-90°)</summary>
        public const double pi05neg = -.5 * pi;

        /// <summary>π = 3.1415926535897932384626433832795</summary>
        public const double pi = 3.1415926535897932384626433832795;
        /// <summary>-π = -3.1415926535897932384626433832795</summary>
        public const double pi_neg = -3.1415926535897932384626433832795;
        /// <summary>2·π (360°)</summary>
        public const double pi2 = 2 * pi;
        /// <summary>-2·π (-360°)</summary>
        public const double pi2neg = -2 * pi;
        /// <summary>3·π/</summary>
        public const double pi3_2 = 3 * pi05;

        /// <summary>π/180</summary>
        public const double ToRad = pi / 180;
        /// <summary>180/π</summary>
        public const double ToDeg = 180 / pi;

        /// <summary>'π'</summary>
        public const string pi_name = "π";
        /// <summary>'°'</summary>
        public const string deg_name = "°";

        /// <summary>√e</summary>
        public const double sqrt_e = 1.6487212707001281468486507878142;
        /// <summary>√π</summary>
        public const double sqrt_pi = 1.7724538509055160272981674833411;
        /// <summary>√2·π</summary>
        public const double sqrt_pi2 = 2.506628274631000502415765284811;

        /// <summary>√2 = 1.4142135623730950488016887242097</summary>
        public const double sqrt_2 = 1.4142135623730950488016887242097;
        /// <summary>1 / √2 = 0.707...</summary>
        public const double sqrt_2_inv = 1 / sqrt_2;

        /// <summary>√3 = 1.732050807...</summary>
        public const double sqrt_3 = 1.7320508075688772935274463415059;

        /// <summary>√5 = 2.236067...</summary>
        public const double sqrt_5 = 2.2360679774997896964091736687313;

        /// <summary>1 / √5 = 0.4472135954999579...</summary>
        public const double sqrt_5_inv = 1/sqrt_5;

        /// <summary>Золотое сечение = (√5 + 1)/2</summary>
        public const double GoldenRatio = (sqrt_5 + 1) / 2;

        /// <summary>Величина, обратная золотому сечению = (√5 - 1)/2</summary>
        public const double GoldenRatio_Inv = 1 / GoldenRatio;

        /// <summary>Геометрические константы</summary>
        public static class Geometry
        {
            /// <summary>Константа преобразования радиан в градусы = 180/π</summary>
            public const double ToDeg = 180 / pi;
            /// <summary>Константа преобразования градусов в радианы = π/180</summary>
            public const double ToRad = pi / 180;
            /// <summary>Константа π = 3.1415926535897932384626433832</summary>
            public const double Pi = Math.PI;

            public static double ConvertToDeg(double RadValue) => RadValue * ToDeg;

            public static double ConvertToRad(double DegValue) => DegValue * ToRad;
        }

        /// <summary>Скорость света 300`000`000 м/с = 3e8 м/с</summary>
        public const double SpeedOfLigth = 3e8;
        /// <summary>Скорость света в вакууме 299`792`458 м/с</summary>
        public const double SpeedOfLightInVacuum = 299792458;

        /// <summary>Гравитационная постоянная 6.67384(80)×10^−11 m^3·кг^−1·с^−2</summary>
        public const double GravitationConstant = 6.673848080808080808080808080E-11;

        /// <summary>Постоянная Планка 6.626 069 57(29) ×10^−34 Жд·с</summary>
        public const double PlanckConstant = 6.6260695729292929292929E-34;

        /// <summary>Приведённая постоянная Планка 1.054 571 628(53) ×10^−34 Жд·с</summary>
        public const double PlanckConstant_Reduced = PlanckConstant / (2 * pi);

        /// <summary>Электродинамические константы</summary>
        public static class ElectroDynamic
        {

            /// <summary>Электрическая постоянная 1/(Mu0 · c^2) = 8,854187817620… ×10^−12 Ф·м^−1</summary>
            public const double Epsilon0 = 1 / (Mu0 * SpeedOfLightInVacuum * SpeedOfLightInVacuum);

            /// <summary>Магнитная постоянная 4·π ×10^-7 = 1.2566370614E-6 Гн/м</summary>
            public const double Mu0 = pi * 4E-7;

            /// <summary>Упрощённое сопротивление среды (вакуума) = √(Mu0/Epsilon0) = 377 Ом</summary>
            public const double Impedance = 377;

            /// <summary>Сопротивление среды (вакуума) = √(Mu0/Epsilon0) = 376.730313461771 Ом</summary>
            public const double Impedance0 = 376.730313461771;

            public static double ToWaveLength(double Frequancy) => SpeedOfLigth / Frequancy;

            public static double ToWaveLength(double Frequance, double Epsilon) => SpeedOfLigth / Frequance / Math.Sqrt(Epsilon);

            public static double ToWaveLength(double Frequance, double Epsilon, double Mu) => SpeedOfLigth / Frequance / Math.Sqrt(Epsilon / Mu);

            public static double ToFrequance(double WaveLength) => SpeedOfLigth / WaveLength;

            public static double ToFrequance(double WaveLength, double Epsilon) => SpeedOfLigth / WaveLength / Math.Sqrt(Epsilon);

            public static double ToFrequance(double WaveLength, double Epsilon, double Mu) => SpeedOfLigth / WaveLength / Math.Sqrt(Epsilon / Mu);
        }

        /// <summary>0x3ffeffff = 1073676287</summary>
        public const int BigPrime_int = 0x3ffeffff;

        /// <summary>0x3fffffefffffff = 18014398241046527</summary>
        public const long BigPrime_long = 0x3fffffefffffff;

        public static class DataLength
        {
            public static class Bytes
            {
                public const int B = 1;
                public const int kB = 0x400 * B;
                public const int MB = 0x400 * kB;
                public const int GB = 0x400 * MB;
                public const long TB = 1024L * GB;

                public static string[] GetDataNames() => new[] { "B", "kB", "MB", "GB", "TB" };
            }
        }

        /// <summary>Элементарный заряд e = 1,602 176 565(35)·10^−19 Кл</summary>
        public const double ElementaryCharge = 1.602176565353535353535353535353535353535e-19;

        /// <summary>Постоянная Больцмана k = 1,380 6488(13)·10^−23 Дж/К</summary>
        public const double BoltzmanConstant = 1.38064881313131313131313131313131313131313e-23;

        /// <summary>Число Авогадро Na = 6,022 141 29(27)·10^23 1/моль</summary>
        public const double AvogadroConstant = 6.02214129272727272727272727272727272727272727e23;

        /// <summary>Постоянная Фарадея F = e * Na = 96485,33(83) Кл/моль</summary>
        public const double FaradayConstant = ElementaryCharge * AvogadroConstant;
    }
}
