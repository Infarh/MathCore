using System;

namespace MathCore.Vectors
{
    public partial struct Vector3D
    {
        #region Вектор на число

        public static Vector3D operator +(Vector3D V, double x) => new Vector3D(V._X + x, V._Y + x, V._Z + x);
        public static Vector3D operator +(double x, Vector3D V) => new Vector3D(V._X + x, V._Y + x, V._Z + x);

        public static Vector3D operator -(Vector3D V, double x) => new Vector3D(V._X - x, V._Y - x, V._Z - x);
        public static Vector3D operator -(double x, Vector3D V) => new Vector3D(x - V._X, x - V._Y, x - V._Z);

        public static Vector3D operator *(Vector3D V, double x) => new Vector3D(V._X * x, V._Y * x, V._Z * x);
        public static Vector3D operator *(double x, Vector3D V) => new Vector3D(V._X * x, V._Y * x, V._Z * x);

        public static Vector3D operator /(Vector3D V, double x) => new Vector3D(V._X / x, V._Y / x, V._Z / x);
        public static Vector3D operator /(double x, Vector3D V) => new Vector3D(x / V._X, x / V._Y, x / V._Z);


        public static Vector3D operator +(Vector3D V, float x) => new Vector3D(V._X + x, V._Y + x, V._Z + x);
        public static Vector3D operator +(float x, Vector3D V) => new Vector3D(V._X + x, V._Y + x, V._Z + x);

        public static Vector3D operator -(Vector3D V, float x) => new Vector3D(V._X - x, V._Y - x, V._Z - x);
        public static Vector3D operator -(float x, Vector3D V) => new Vector3D(x - V._X, x - V._Y, x - V._Z);

        public static Vector3D operator *(Vector3D V, float x) => new Vector3D(V._X * x, V._Y * x, V._Z * x);
        public static Vector3D operator *(float x, Vector3D V) => new Vector3D(V._X * x, V._Y * x, V._Z * x);

        public static Vector3D operator /(Vector3D V, float x) => new Vector3D(V._X / x, V._Y / x, V._Z / x);
        public static Vector3D operator /(float x, Vector3D V) => new Vector3D(x / V._X, x / V._Y, x / V._Z);


        public static Vector3D operator +(Vector3D V, int x) => new Vector3D(V._X + x, V._Y + x, V._Z + x);
        public static Vector3D operator +(int x, Vector3D V) => new Vector3D(V._X + x, V._Y + x, V._Z + x);

        public static Vector3D operator -(Vector3D V, int x) => new Vector3D(V._X - x, V._Y - x, V._Z - x);
        public static Vector3D operator -(int x, Vector3D V) => new Vector3D(x - V._X, x - V._Y, x - V._Z);

        public static Vector3D operator *(Vector3D V, int x) => new Vector3D(V._X * x, V._Y * x, V._Z * x);
        public static Vector3D operator *(int x, Vector3D V) => new Vector3D(V._X * x, V._Y * x, V._Z * x);

        public static Vector3D operator /(Vector3D V, int x) => new Vector3D(V._X / x, V._Y / x, V._Z / x);
        public static Vector3D operator /(int x, Vector3D V) => new Vector3D(x / V._X, x / V._Y, x / V._Z);

        #endregion

        public static bool operator ==(Vector3D X, Vector3D Y) => X._X.Equals(Y._X) && X._Y.Equals(Y._Y) && X._Z.Equals(Y._Z);
        public static bool operator !=(Vector3D X, Vector3D Y) => !X._X.Equals(Y._X) || !X._Y.Equals(Y._Y) || !X._Z.Equals(Y._Z);

        public static bool operator ==(Vector3D X, byte Y) => Y.Equals(0) && X._X.Equals(0d) && X._Y.Equals(0d) && X._Z.Equals(0d);
        public static bool operator !=(Vector3D X, byte Y) => !Y.Equals(0) || !X._X.Equals(0d) || !X._Y.Equals(0d) || !X._Z.Equals(0d);

        public static bool operator ==(Vector3D X, sbyte Y) => Y.Equals(0) && X._X.Equals(0d) && X._Y.Equals(0d) && X._Z.Equals(0d);
        public static bool operator !=(Vector3D X, sbyte Y) => !Y.Equals(0) || !X._X.Equals(0d) || !X._Y.Equals(0d) || !X._Z.Equals(0d);

        public static bool operator ==(Vector3D X, short Y) => Y.Equals(0) && X._X.Equals(0d) && X._Y.Equals(0d) && X._Z.Equals(0d);
        public static bool operator !=(Vector3D X, short Y) => !Y.Equals(0) || !X._X.Equals(0d) || !X._Y.Equals(0d) || !X._Z.Equals(0d);

        public static bool operator ==(Vector3D X, ushort Y) => Y.Equals(0) && X._X.Equals(0d) && X._Y.Equals(0d) && X._Z.Equals(0d);
        public static bool operator !=(Vector3D X, ushort Y) => !Y.Equals(0) || !X._X.Equals(0d) || !X._Y.Equals(0d) || !X._Z.Equals(0d);

        public static bool operator ==(Vector3D X, int Y) => Y.Equals(0) && X._X.Equals(0d) && X._Y.Equals(0d) && X._Z.Equals(0d);
        public static bool operator !=(Vector3D X, int Y) => !Y.Equals(0) || !X._X.Equals(0d) || !X._Y.Equals(0d) || !X._Z.Equals(0d);

        public static bool operator ==(Vector3D X, uint Y) => Y.Equals(0) && X._X.Equals(0d) && X._Y.Equals(0d) && X._Z.Equals(0d);
        public static bool operator !=(Vector3D X, uint Y) => !Y.Equals(0) || !X._X.Equals(0d) || !X._Y.Equals(0d) || !X._Z.Equals(0d);

        public static bool operator ==(Vector3D X, long Y) => Y.Equals(0) && X._X.Equals(0d) && X._Y.Equals(0d) && X._Z.Equals(0d);
        public static bool operator !=(Vector3D X, long Y) => !Y.Equals(0) || !X._X.Equals(0d) || !X._Y.Equals(0d) || !X._Z.Equals(0d);

        public static bool operator ==(Vector3D X, ulong Y) => Y.Equals(0) && X._X.Equals(0d) && X._Y.Equals(0d) && X._Z.Equals(0d);
        public static bool operator !=(Vector3D X, ulong Y) => !Y.Equals(0) || !X._X.Equals(0d) || !X._Y.Equals(0d) || !X._Z.Equals(0d);

        public static bool operator ==(Vector3D X, float Y) => Y.Equals(0f) && X._X.Equals(0d) && X._Y.Equals(0d) && X._Z.Equals(0d);
        public static bool operator !=(Vector3D X, float Y) => !Y.Equals(0f) || !X._X.Equals(0d) || !X._Y.Equals(0d) || !X._Z.Equals(0d);

        public static bool operator ==(Vector3D X, double Y) => Y.Equals(0d) && X._X.Equals(0d) && X._Y.Equals(0d) && X._Z.Equals(0d);
        public static bool operator !=(Vector3D X, double Y) => !Y.Equals(0d) || !X._X.Equals(0d) || !X._Y.Equals(0d) || !X._Z.Equals(0d);


        #region Операции над двумя векторами

        public static Vector3D operator +(Vector3D A, Vector3D B) => new Vector3D(A._X + B._X, A._Y + B._Y, A._Z + B._Z);

        public static Vector3D operator -(Vector3D A, Vector3D B) => new Vector3D(A._X - B._X, A._Y - B._Y, A._Z - B._Z);

        /// <summary>Скалярное произведение векторов</summary>
        /// <param name="A">Первый вектор-множитель</param>
        /// <param name="B">Второй вектор-множитель</param>
        /// <returns>Число - скалярное произведение векторов</returns>
        public static double operator *(Vector3D A, Vector3D B) => A.Product_Scalar(B);

        //public static Vector3D operator /(Vector3D A, Vector3D B) { return A * B.GetInverse(); }

        /// <summary>Угол между векторами</summary>
        /// <param name="A">Вектор 1</param>
        /// <param name="B">Вектор 2</param>
        /// <returns>Угол между вектором А и вектором В в пространстве</returns>
        public static SpaceAngle operator ^(Vector3D A, Vector3D B) => A.GetAngle(B);

        /// <summary>Проверка на параллельность</summary>
        /// <param name="A">Вектор 1</param><param name="B">Вектор 2</param>
        /// <returns>Истина, если вектора параллельны</returns>
        public static bool operator |(Vector3D A, Vector3D B) => Math.Abs((A * B) / (A.R * B.R) - 1).Equals(0d);

        /// <summary>Проверка на ортогональность</summary>
        /// <param name="A">Вектор 1</param><param name="B">Вектор 2</param>
        /// <returns>Истина, если вектор 1 ортогонален вектору 2</returns>
        public static bool operator &(Vector3D A, Vector3D B) => Math.Abs((A * B) / (A.R * B.R)).Equals(0d);

        /// <summary>Проекция вектора A на вектор B</summary>
        /// <param name="A">Проецируемый вектор</param>
        /// <param name="B">Вектор, на который производится проекция</param>
        /// <returns>Проекция вектора А на вектор В</returns>
        public static double operator %(Vector3D A, Vector3D B) => A.GetProjectionTo(B);

        /// <summary>Проекция вектора на направление</summary>
        /// <param name="Vector">Проецируемый вектор</param>
        /// <param name="Direction">Пространственный угол направления проекции</param>
        /// <returns>Вещественное значение проекции</returns>
        public static double operator %(Vector3D Vector, SpaceAngle Direction) => Vector.GetProjectionTo(Direction);
        public static double operator %(Vector3D Vector, Basis3D b) => Vector.InBasis(b);

        #endregion

        #region Операторы преобразований

        public static implicit operator double(Vector3D V) => V.R;

        public static implicit operator SpaceAngle(Vector3D V) => V.Angle;

        public static explicit operator Vector3D(SpaceAngle Angle) => new Vector3D(1, Angle);

        public static explicit operator Vector3D(double V) => new Vector3D(V / Consts.sqrt_3, V / Consts.sqrt_3, V / Consts.sqrt_3);

        #endregion
    }
}
