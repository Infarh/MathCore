using System;

namespace MathCore.Vectors
{
    public partial struct Vector3D
    {
        #region Вектор на число

        /// <summary>Оператор суммы вектора и числа</summary>
        /// <returns>Вектор, координаты которого увеличены на значения числа</returns>
        public static Vector3D operator +(Vector3D V, double x) => new(V._X + x, V._Y + x, V._Z + x);

        /// <summary>Оператор суммы вектора и числа</summary>
        /// <returns>Вектор, координаты которого увеличены на значения числа</returns>
        public static Vector3D operator +(double x, Vector3D V) => new(V._X + x, V._Y + x, V._Z + x);

        /// <summary>Оператор разности вектора и числа</summary>
        /// <returns>Вектор, координаты которого уменьшены на значения числа</returns>
        public static Vector3D operator -(Vector3D V, double x) => new(V._X - x, V._Y - x, V._Z - x);

        /// <summary>Оператор суммы числа и вектора</summary>
        /// <returns>Вектор, координаты которого равны разности числа и координат исходного вектора</returns>
        public static Vector3D operator -(double x, Vector3D V) => new(x - V._X, x - V._Y, x - V._Z);

        /// <summary>Оператор произведения вектора и числа</summary>
        /// <returns>Вектор, координаты которого умножены на значения числа</returns>
        public static Vector3D operator *(Vector3D V, double x) => new(V._X * x, V._Y * x, V._Z * x);

        /// <summary>Оператор произведения вектора и числа</summary>
        /// <returns>Вектор, координаты которого умножены на значения числа</returns>
        public static Vector3D operator *(double x, Vector3D V) => new(V._X * x, V._Y * x, V._Z * x);

        /// <summary>Оператор деления вектора на число</summary>
        /// <returns>Вектор, координаты которого разделены на значения числа</returns>
        public static Vector3D operator /(Vector3D V, double x) => new(V._X / x, V._Y / x, V._Z / x);

        /// <summary>Оператор деления числа на вектор</summary>
        /// <returns>Вектор, координаты которого являются результатом деления числа на координаты исходного вектора</returns>
        public static Vector3D operator /(double x, Vector3D V) => new(x / V._X, x / V._Y, x / V._Z);


        /// <summary>Оператор суммы вектора и числа одинарной точности</summary>
        /// <returns>Вектор, координаты которого увеличены на значения числа одинарной точности</returns>
        public static Vector3D operator +(Vector3D V, float x) => new(V._X + x, V._Y + x, V._Z + x);

        /// <summary>Оператор суммы вектора и числа одинарной точности</summary>
        public static Vector3D operator +(float x, Vector3D V) => new(V._X + x, V._Y + x, V._Z + x);

        /// <summary>Оператор разности вектора и числа одинарной точности</summary>
        /// <returns>Вектор, координаты которого уменьшены на значения числа одинарной точности</returns>
        public static Vector3D operator -(Vector3D V, float x) => new(V._X - x, V._Y - x, V._Z - x);

        /// <summary>Оператор суммы числа одинарной точности и вектора</summary>
        /// <returns>Вектор, координаты которого равны разности числа одинарной точности и координат исходного вектора</returns>
        public static Vector3D operator -(float x, Vector3D V) => new(x - V._X, x - V._Y, x - V._Z);

        /// <summary>Оператор произведения вектора и числа одинарной точности</summary>
        /// <returns>Вектор, координаты которого умножены на значения числа одинарной точности</returns>
        public static Vector3D operator *(Vector3D V, float x) => new(V._X * x, V._Y * x, V._Z * x);

        /// <summary>Оператор произведения вектора и числа одинарной точности</summary>
        /// <returns>Вектор, координаты которого умножены на значения числа одинарной точности</returns>
        public static Vector3D operator *(float x, Vector3D V) => new(V._X * x, V._Y * x, V._Z * x);

        /// <summary>Оператор деления вектора на число одинарной точности</summary>
        /// <returns>Вектор, координаты которого разделены на значения числа одинарной точности</returns>
        public static Vector3D operator /(Vector3D V, float x) => new(V._X / x, V._Y / x, V._Z / x);

        /// <summary>Оператор деления числа одинарной точности на вектор</summary>
        /// <returns>Вектор, координаты которого являются результатом деления числа одинарной точности на координаты исходного вектора</returns>
        public static Vector3D operator /(float x, Vector3D V) => new(x / V._X, x / V._Y, x / V._Z);


        /// <summary>Оператор суммы вектора и целого числа</summary>
        /// <returns>Вектор, координаты которого увеличены на значения целого числа</returns>
        public static Vector3D operator +(Vector3D V, int x) => new(V._X + x, V._Y + x, V._Z + x);

        /// <summary>Оператор суммы вектора и целого числа</summary>
        /// <returns>Вектор, координаты которого увеличены на значения целого числа</returns>
        public static Vector3D operator +(int x, Vector3D V) => new(V._X + x, V._Y + x, V._Z + x);

        /// <summary>Оператор разности вектора и целого числа</summary>
        /// <returns>Вектор, координаты которого уменьшены на значения целого числа</returns>
        public static Vector3D operator -(Vector3D V, int x) => new(V._X - x, V._Y - x, V._Z - x);

        /// <summary>Оператор суммы целого числа и вектора</summary>
        /// <returns>Вектор, координаты которого равны разности целого числа и координат исходного вектора</returns>
        public static Vector3D operator -(int x, Vector3D V) => new(x - V._X, x - V._Y, x - V._Z);

        /// <summary>Оператор произведения вектора и целого числа</summary>
        /// <returns>Вектор, координаты которого умножены на значения целого числа</returns>
        public static Vector3D operator *(Vector3D V, int x) => new(V._X * x, V._Y * x, V._Z * x);

        /// <summary>Оператор произведения вектора и целого числа</summary>
        /// <returns>Вектор, координаты которого умножены на значения целого числа</returns>
        public static Vector3D operator *(int x, Vector3D V) => new(V._X * x, V._Y * x, V._Z * x);

        /// <summary>Оператор деления вектора на целое число</summary>
        /// <returns>Вектор, координаты которого разделены на значения целое числа</returns>
        public static Vector3D operator /(Vector3D V, int x) => new(V._X / x, V._Y / x, V._Z / x);

        /// <summary>Оператор деления целого числа на вектор</summary>
        /// <returns>Вектор, координаты которого являются результатом деления целого числа на координаты исходного вектора</returns>
        public static Vector3D operator /(int x, Vector3D V) => new(x / V._X, x / V._Y, x / V._Z);

        #endregion

        /// <summary>Оператор проверки равенства двух векторов</summary>
        /// <returns>Истина, если координаты векторов равны</returns>
        public static bool operator ==(Vector3D X, Vector3D Y) => X._X.Equals(Y._X) && X._Y.Equals(Y._Y) && X._Z.Equals(Y._Z);

        /// <summary>Оператор проверки неравенства двух векторов</summary>
        /// <returns>Истина, если координаты векторов неравны</returns>
        public static bool operator !=(Vector3D X, Vector3D Y) => !X._X.Equals(Y._X) || !X._Y.Equals(Y._Y) || !X._Z.Equals(Y._Z);

        /// <summary>Оператор проверки равенства вектора и целого числа (1 байт)</summary>
        /// <returns>Истина, если длина вектора равна указанному целому числу (1 байт)</returns>
        public static bool operator ==(Vector3D X, byte Y) => X.R.Equals(Y);

        /// <summary>Оператор проверки равенства вектора и целого числа (1 байт со знаком)</summary>
        /// <returns>Истина, если длина вектора равна указанному целому числу (1 байт со знаком)</returns>
        public static bool operator ==(Vector3D X, sbyte Y) => X.R.Equals(Y);

        /// <summary>Оператор проверки равенства вектора и целого числа (2 байта со знаком)</summary>
        /// <returns>Истина, если длина вектора равна указанному целому числу (2 байта со знаком)</returns>
        public static bool operator ==(Vector3D X, short Y) => X.R.Equals(Y);

        /// <summary>Оператор проверки равенства вектора и целого числа (2 байта без знака)</summary>
        /// <returns>Истина, если длина вектора равна указанному целому числу (2 байта без знака)</returns>
        public static bool operator ==(Vector3D X, ushort Y) => X.R.Equals(Y);
        
        /// <summary>Оператор проверки равенства вектора и целого числа</summary>
        /// <returns>Истина, если длина вектора равна указанному целому числу</returns>
        public static bool operator ==(Vector3D X, int Y) => X.R.Equals(Y);

        /// <summary>Оператор проверки равенства вектора и целого числа без знака</summary>
        /// <returns>Истина, если длина вектора равна указанному целому числу без знака</returns>
        public static bool operator ==(Vector3D X, uint Y) => X.R.Equals(Y);

        /// <summary>Оператор проверки равенства вектора и целого числа (8 байт со знаком)</summary>
        /// <returns>Истина, если длина вектора равна указанному целому числу (8 байт со знаком)</returns>
        public static bool operator ==(Vector3D X, long Y) => X.R.Equals(Y);

        /// <summary>Оператор проверки равенства вектора и целого числа (8 байт без знака)</summary>
        /// <returns>Истина, если длина вектора равна указанному целому числу (8 байт без знака)</returns>
        public static bool operator ==(Vector3D X, ulong Y) => X.R.Equals(Y);

        /// <summary>Оператор проверки равенства вектора и числа одинарной точности</summary>
        /// <returns>Истина, если длина вектора равна указанному числу одинарной точности</returns>
        public static bool operator ==(Vector3D X, float Y) => X.R.Equals(Y);

        /// <summary>Оператор проверки равенства вектора и числа двойной точности</summary>
        /// <returns>Истина, если длина вектора равна указанному числу двойной точности</returns>
        public static bool operator ==(Vector3D X, double Y) => X.R.Equals(Y);

        /// <summary>Оператор проверки неравенства вектора и целого числа (1 байт без знака)</summary>
        /// <returns>Истина, если длина вектора не равна целому числу (1 байт без знака)</returns>
        public static bool operator !=(Vector3D X, byte Y) => !X.R.Equals(Y);

        /// <summary>Оператор проверки неравенства вектора и целого числа (1 байт со знаком)</summary>
        /// <returns>Истина, если длина вектора не равна целому числу (1 байт со знаком)</returns>
        public static bool operator !=(Vector3D X, sbyte Y) => !X.R.Equals(Y);

        /// <summary>Оператор проверки неравенства вектора и целого числа (2 байта со знаком)</summary>
        /// <returns>Истина, если длина вектора не равна целому числу (2 байта со знаком)</returns>
        public static bool operator !=(Vector3D X, short Y) => !X.R.Equals(Y);

        /// <summary>Оператор проверки неравенства вектора и целого числа (2 байта без знака)</summary>
        /// <returns>Истина, если длина вектора не равна целому числу (2 байта без знака)</returns>
        public static bool operator !=(Vector3D X, ushort Y) => !X.R.Equals(Y);
        
        /// <summary>Оператор проверки неравенства вектора и целого числа</summary>
        /// <returns>Истина, если длина вектора не равна целому числу</returns>
        public static bool operator !=(Vector3D X, int Y) => !X.R.Equals(Y);
        
        /// <summary>Оператор проверки неравенства вектора и целого числа</summary>
        /// <returns>Истина, если длина вектора не равна целому числу</returns>
        public static bool operator !=(Vector3D X, uint Y) => !X.R.Equals(Y);
        
        /// <summary>Оператор проверки неравенства вектора и целого числа (8 байт со знаком)</summary>
        /// <returns>Истина, если длина вектора не равна целому числу (8 байт со знаком)</returns>
        public static bool operator !=(Vector3D X, long Y) => !X.R.Equals(Y);
        
        /// <summary>Оператор проверки неравенства вектора и целого числа (8 байт без знака)</summary>
        /// <returns>Истина, если длина вектора не равна целому числу (8 байт без знака)</returns>
        public static bool operator !=(Vector3D X, ulong Y) => !X.R.Equals(Y);
        
        /// <summary>Оператор проверки неравенства вектора и вещественного числа одинарной точности</summary>
        /// <returns>Истина, если длина вектора не равна вещественному числу одинарной точности</returns>
        public static bool operator !=(Vector3D X, float Y) => !X.R.Equals(Y);
        
        /// <summary>Оператор проверки неравенства вектора и вещественного числа двойной точности</summary>
        /// <returns>Истина, если длина вектора не равна вещественному числу двойной точности</returns>
        public static bool operator !=(Vector3D X, double Y) => !X.R.Equals(Y);


        #region Операции над двумя векторами

        /// <summary>Оператор суммы двух векторов</summary>
        /// <returns>Вектор, координаты которого равны сумме координат двух исходных векторов</returns>
        public static Vector3D operator +(Vector3D A, Vector3D B) => new(A._X + B._X, A._Y + B._Y, A._Z + B._Z);

        /// <summary>Оператор разности двух векторов</summary>
        /// <returns>Вектор, координаты которого равны разности координат двух исходных векторов</returns>
        public static Vector3D operator -(Vector3D A, Vector3D B) => new(A._X - B._X, A._Y - B._Y, A._Z - B._Z);

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
#pragma warning disable IDE0047 // Удалить ненужные круглые скобки
        public static bool operator |(Vector3D A, Vector3D B) => Math.Abs((A * B) / (A.R * B.R) - 1).Equals(0d);
#pragma warning restore IDE0047 // Удалить ненужные круглые скобки

        /// <summary>Проверка на ортогональность</summary>
        /// <param name="A">Вектор 1</param><param name="B">Вектор 2</param>
        /// <returns>Истина, если вектор 1 ортогонален вектору 2</returns>
        public static bool operator &(Vector3D A, Vector3D B) => Math.Abs((A * B) / (A.R * B.R)).Equals(0d);

        /// <summary>Проекция вектора A на вектор B</summary>
        /// <param name="A">Проецируемый вектор</param>
        /// <param name="B">Вектор, на который производится проекции</param>
        /// <returns>Проекция вектора А на вектор В</returns>
        public static double operator %(Vector3D A, Vector3D B) => A.GetProjectionTo(B);

        /// <summary>Проекция вектора на направление</summary>
        /// <param name="Vector">Проецируемый вектор</param>
        /// <param name="Direction">Пространственный угол направления проекции</param>
        /// <returns>Вещественное значение проекции</returns>
        public static double operator %(Vector3D Vector, SpaceAngle Direction) => Vector.GetProjectionTo(Direction);

        /// <summary>Оператор представления вектора в базисе</summary>
        /// <returns>Вектор, представленный в базисе</returns>
        public static double operator %(Vector3D Vector, Basis3D b) => Vector.InBasis(b);

        #endregion

        #region Операторы преобразований

        /// <summary>Оператор неявного приведения <see cref="Vector3D"/> к <see cref="double"/>, результатом которого является длина вектора</summary>
        /// <param name="V">Трёхмерный вектор</param>
        public static implicit operator double(Vector3D V) => V.R;

        /// <summary>Оператор неявного приведения <see cref="Vector3D"/> к <see cref="SpaceAngle"/></summary>
        /// <param name="V">Трёхмерный вектор</param>
        public static implicit operator SpaceAngle(Vector3D V) => V.Angle;

        /// <summary>Оператор неявного приведения <see cref="SpaceAngle"/> к <see cref="Vector3D"/>, результатом которого является единичный вектор, ориентированный в пространстве</summary>
        /// <param name="Angle">Пространственный угол</param>
        public static explicit operator Vector3D(SpaceAngle Angle) => new(1, Angle);

        /// <summary>Оператор явного приведения типа <see cref="double"/> к <see cref="Vector3D"/>, результатом которого является вектор, с равными координатами, длина которого равна указанному числу</summary>
        /// <param name="V">Длина вектора</param>
        public static explicit operator Vector3D(double V) => new(V / Consts.sqrt_3, V / Consts.sqrt_3, V / Consts.sqrt_3);

        #endregion
    }
}