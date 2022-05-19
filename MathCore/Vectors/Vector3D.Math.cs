using System;
using System.Linq.Expressions;
using MathCore.Annotations;
using MathCore.Extensions.Expressions;
using static System.Math;
// ReSharper disable UnusedMember.Global

namespace MathCore.Vectors
{
    public partial struct Vector3D
    {
        /// <summary>Получить вектор, координаты которого являются обратными к координатам текущего вектора</summary>
        /// <returns>Вектор с обратными координатами</returns>
        public Vector3D GetInverse() => new(1 / _X, 1 / _Y, 1 / _Z);

        /// <summary>Скалярное произведение векторов</summary>
        /// <param name="Vector">Вектор, на который умножается текущий вектор</param>
        /// <returns>Число, равное скалярному произведению векторов</returns>
        public double Product_Scalar(Vector3D Vector) => _X * Vector._X + Y * Vector._Y + _Z * Vector._Z;

        /// <summary>Скалярное произведение векторов</summary>
        /// <param name="Vector">Вектор, на который умножается текущий вектор</param>
        /// <returns>Число, равное скалярному произведению векторов</returns>
        public double Product_Scalar((double X, double Y, double Z) Vector) => _X * Vector.X + Y * Vector.Y + _Z * Vector.Z;

        /// <summary>Смешанное произведение трёх векторов</summary>
        /// <param name="A">Первый вектор произведения</param>
        /// <param name="B">Второй вектор произведения</param>
        /// <param name="C">Третий вектор произведения</param>
        /// <returns>Число, равное смешанному произведения векторов</returns>
        public static double Product_Mixed(Vector3D A, Vector3D B, Vector3D C) =>
            +1 * A._X * (B._Y * C._Z - B._Z * C._Y) +
            -1 * A._Y * (B._X * C._Z - B._Z * C._X) +
            +1 * A._Z * (B._X * C._Y - B._Y * C._X);

        /// <summary>Векторное произведение векторов</summary>
        /// <param name="Vector">Вектор, на который умножается исходный вектор</param>
        /// <returns>Вектор, равный векторному произведению векторов</returns>
        public Vector3D Product_Vector(Vector3D Vector)
        {
            /*
             * A = {Xa, Ya, Za}
             * B = {Xb, Yb, Zb}
             *         | i  j  k  |   {Ya * Zb - Za * Yb}   {Xc}
             * A * B = | Xa Ya Za | = {Za * Xb - Xa * Zb} = {Yc} = C
             *         | Xb Yb Zb |   {Xa * Yb - Ya * Xb}   {Zc}
             * C = {Xc, Yc, Zc}
             */

            var A = this;
            var B = Vector;
            return new Vector3D
                (
                    A._Y * B._Z - A._Z * B._Y, // X
                    A._Z * B._X - A._X * B._Z, // Y
                    A._X * B._Y - A._Y * B._X  // Z
                );
        }

        /// <summary>Векторное произведение векторов</summary>
        /// <param name="Vector">Вектор, на который умножается исходный вектор</param>
        /// <returns>Вектор, равный векторному произведению векторов</returns>
        public Vector3D Product_Vector((double X, double Y, double Z) Vector)
        {
            /*
             * A = {Xa, Ya, Za}
             * B = {Xb, Yb, Zb}
             *         | i  j  k  |   {Ya * Zb - Za * Yb}   {Xc}
             * A * B = | Xa Ya Za | = {Za * Xb - Xa * Zb} = {Yc} = C
             *         | Xb Yb Zb |   {Xa * Yb - Ya * Xb}   {Zc}
             * C = {Xc, Yc, Zc}
             */

            var A = this;
            var B = Vector;
            return new Vector3D
            (
                A._Y * B.Z - A._Z * B.Y, // X
                A._Z * B.X - A._X * B.Z, // Y
                A._X * B.Y - A._Y * B.X  // Z
            );
        }

        /// <summary>Векторное произведение векторов</summary>
        /// <param name="Vector">Вектор, на который умножается исходный вектор</param>
        /// <returns>Вектор, равный векторному произведению векторов</returns>
        public Vector3D Product_VectorInv((double X, double Y, double Z) Vector)
        {
            /*
             * A = {Xa, Ya, Za}
             * B = {Xb, Yb, Zb}
             *         | i  j  k  |   {Ya * Zb - Za * Yb}   {Xc}
             * A * B = | Xa Ya Za | = {Za * Xb - Xa * Zb} = {Yc} = C
             *         | Xb Yb Zb |   {Xa * Yb - Ya * Xb}   {Zc}
             * C = {Xc, Yc, Zc}
             */

            var A = Vector;
            var B = this;
            return new Vector3D
                (
                    A.Y * B._Z - A.Z * B._Y, // X
                    A.Z * B._X - A.X * B._Z, // Y
                    A.X * B._Y - A.Y * B._X  // Z
                );
        }

        /// <summary>Покомпонентное умножение на вектор</summary>
        /// <param name="Vector">Векторный сомножитель</param>
        /// <returns>Вектор, компоненты которого являются произведениями компоненты векторов</returns>
        public Vector3D Product_Component(Vector3D Vector)
            => new(_X * Vector._X, _Y * Vector._Y, _Z * Vector._Z);

        /// <summary>Покомпонентное умножение на вектор</summary>
        /// <param name="Vector">Векторный сомножитель</param>
        /// <returns>Вектор, компоненты которого являются произведениями компоненты векторов</returns>
        public Vector3D Product_Component((double X, double Y, double Z) Vector)
            => new(_X * Vector.X, _Y * Vector.Y, _Z * Vector.Z);

        /// <summary>Угол между векторами</summary>
        /// <param name="Vector">Вектор, к которому вычисляется угол</param>
        /// <returns>Пространственный угол между векторами</returns>
        public SpaceAngle GetAngle(Vector3D Vector)
        {
            var angle1 = Angle;
            var angle2 = Vector.Angle;
            return angle1 - angle2;
        }

        /// <summary>Проекция на вектор</summary>
        /// <param name="Vector">Вектор, НА который производится проекции</param>
        /// <returns>Проекция на вектор</returns>
        public double GetProjectionTo(Vector3D Vector) => Product_Scalar(Vector) / Vector.R;

        /// <summary>Проекция на вектор</summary>
        /// <param name="Vector">Вектор, НА который производится проекции</param>
        /// <returns>Проекция на вектор</returns>
        public double GetProjectionTo((double X, double Y, double Z) Vector) => Product_Scalar(Vector) / Sqrt(Vector.X * Vector.X + Vector.Y * Vector.Y + Vector.Z * Vector.Z);

        /// <summary>Проекция на вектор</summary>
        /// <param name="Vector">Вектор, НА который производится проекции</param>
        /// <returns>Проекция на вектор</returns>
        public double GetProjectionInverseTo(Vector3D Vector) => Product_Scalar(Vector) / R;

        /// <summary>Проекция на вектор</summary>
        /// <param name="Vector">Вектор, НА который производится проекции</param>
        /// <returns>Проекция на вектор</returns>
        public double GetProjectionInverseTo((double X, double Y, double Z) Vector) => Product_Scalar(Vector) / R;

        /// <summary>Проекцию текущего вектора на вектор</summary>
        /// <returns>Функция, вычисляющая проекцию текущего вектора на вектор</returns>
        [NotNull]
        public Func<Vector3D, double> GetProjectorV()
        {
            var t = this;
            return v => t * v / v.R;
        }

        /// <summary>Проекция текущего вектора на вектор, передаваемый в качестве параметра</summary>
        /// <param name="v">Выражение, результатом вычисления которого является <see cref="Vector3D"/></param>
        /// <returns>Выражение, вычисляющее проекцию текущего вектора на вектор, передаваемый в параметре функции</returns>
        /// <exception cref="ArgumentNullException">Если передана пустая ссылка</exception>
        /// <exception cref="ArgumentException">Если тип результата <paramref name="v"/> не является <see cref="Vector3D"/></exception>
        [NotNull]
        public Expression GetProjectorV_Expression([NotNull] Expression v)
        {
            if (v is null) throw new ArgumentNullException(nameof(v));
            if(v.Type != typeof(Vector3D)) 
                throw new ArgumentException($"Тип выражения {v.Type} не является {typeof(Vector3D)}");

            var vector = this.ToExpression();
            var scalar_multiply = vector.Multiply(v);
            var length = v.GetProperty(nameof(R));
            return scalar_multiply.Divide(length);
        }

        /// <summary>Проекция на направление</summary>
        /// <param name="Direction">Направление, на которое проектируется вектор</param>
        /// <returns>Проекция вектора на направление</returns>
        public double GetProjectionTo(SpaceAngle Direction)
        {
            var theta = Direction.ThetaRad;
            var phi = Direction.PhiRad;
            return Sin(theta) * (_X * Cos(phi) + _Y * Sin(phi)) + _Z * Cos(theta);
        }

        /// <summary>Проекцию текущего вектора на угол</summary>
        /// <returns>Функция, вычисляющая проекцию текущего вектора на угол</returns>
        [NotNull]
        public Func<SpaceAngle, double> GetProjectorA()
        {
            var t = this;
            return d =>
            {
                var th = d.ThetaRad;
                var ph = d.PhiRad;
                return Sin(th) * (t._X * Cos(ph) + t._Y * Sin(ph)) + t._Z * Cos(th);
            };
        }

        //public Expression GetProjectorA_Expression(Expression d) => Expression.Add
        //(
        //    Expression.Multiply
        //    (
        //        sin_expr(Expression.Property(d, nameof(SpaceAngle.ThetaRad))),
        //        Expression.Add
        //        (
        //            Expression.Multiply
        //            (
        //                Expression.Constant(_X),
        //                cos_expr(Expression.Property(d, nameof(SpaceAngle.PhiRad)))
        //            ),
        //            Expression.Multiply
        //            (
        //                Expression.Constant(_Y),
        //                sin_expr(Expression.Property(d, nameof(SpaceAngle.PhiRad)))
        //            )
        //        )
        //    ),
        //    Expression.Multiply
        //    (
        //        Expression.Constant(_Z),
        //        cos_expr(Expression.Property(d, nameof(SpaceAngle.ThetaRad)))
        //    )
        //);

        /// <summary>Создать выражение проекции вектора на пространственный угол</summary>
        /// <param name="d">Выражение, результатом вычисления которого будет объект <see cref="SpaceAngle"/></param>
        /// <returns>Выражение проекции вектора на угол</returns>
        /// <exception cref="ArgumentNullException">Если <paramref name="d"/> <see langword="=="/> <see langword="null"/></exception>
        /// <exception cref="ArgumentException">Если тип выражения <paramref name="d"/> является не <see cref="SpaceAngle"/></exception>
        [NotNull]
        public Expression GetProjectorA_Expression([NotNull] Expression d)
        {
            if (d is null) throw new ArgumentNullException(nameof(d));
            if(d.Type != typeof(SpaceAngle)) 
                throw new ArgumentException($"Тип выражения должен быть {typeof(SpaceAngle)}, а получен {d.Type}");

            // (X * cos(Phi) + Y * sin(Phi)) * sin(Theta) + Z * cos(Theta)

            var x = _X.ToExpression();
            var y = _Y.ToExpression();
            var z = _Z.ToExpression();

            var theta = d.GetProperty(nameof(SpaceAngle.ThetaRad));
            var phi = d.GetProperty(nameof(SpaceAngle.PhiRad));

            var sin_theta = MathExpression.Sin(theta);
            var cos_theta = MathExpression.Cos(theta);
            var sin_phi = MathExpression.Sin(phi);
            var cos_phi = MathExpression.Cos(phi);

            var x_cos_phi = x.Multiply(cos_phi);     // X * cos(Phi)
            var y_sin_phi = y.Multiply(sin_phi);     // Y * sin(Phi)
            var z_cos_theta = z.Multiply(cos_theta); // Z * cos(Theta)

            var x_cos_phi_add_y_sin_phi = x_cos_phi.Add(y_sin_phi); // X * cos(Phi) + Y * sin(Phi)
            var xoy = x_cos_phi_add_y_sin_phi.Multiply(sin_theta);  // (X * cos(Phi) + Y * sin(Phi)) * sin(Theta)

            return xoy.Add(z_cos_theta);

            //return MathExpression.Sin(d.GetProperty(nameof(SpaceAngle.ThetaRad)))
            //   .Multiply 
            //    (
            //        _X.ToExpression().Multiply(MathExpression.Cos(d.GetProperty(nameof(SpaceAngle.PhiRad))))
            //           .Add
            //            (
            //                _Y.ToExpression().Multiply(MathExpression.Sin(d.GetProperty(nameof(SpaceAngle.PhiRad))))
            //            )
            //    ).Add(_Z.ToExpression().Multiply(MathExpression.Cos(d.GetProperty(nameof(SpaceAngle.ThetaRad)))));
        }
    }
}