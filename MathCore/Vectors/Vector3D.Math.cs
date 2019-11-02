using System;
using System.Linq.Expressions;
using MathCore.Extensions.Expressions;

namespace MathCore.Vectors
{
    public partial struct Vector3D
    {
        public Vector3D GetInverse() => new Vector3D(1 / _X, 1 / _Y, 1 / _Z);

        /// <summary>Скалярное произведение векторов</summary>
        /// <param name="Vector">Вектор, на который домножается текущий вектор</param>
        /// <returns>Число, равное скалярному произведению векторов</returns>
        public double Product_Scalar(Vector3D Vector) => X * Vector.X + Y * Vector.Y + Z * Vector.Z;

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
        /// <param name="Vector">Вектор, на который домножается исходный вектор</param>
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

            var lv_A = this;
            var lv_B = Vector;
            return new Vector3D
                (
                    lv_A._Y * lv_B._Z - lv_A._Z * lv_B._Y, // X
                    lv_A._Z * lv_B._X - lv_A._X * lv_B._Z, // Y
                    lv_A._X * lv_B._Y - lv_A._Y * lv_B._X  // Z
                );
        }

        /// <summary>Покомпонентное домножение на вектор</summary>
        /// <param name="Vector">Векторный сомножитель</param>
        /// <returns>Вектор, компоненты которого являются произведениями компоненты векторов</returns>
        public Vector3D Product_Component(Vector3D Vector)
            => new Vector3D(_X * Vector._X, _Y * Vector._Y, _Z * Vector._Z);

        /// <summary>Угол между векторами</summary>
        /// <param name="Vector">Вектор, к которорому вычисляется угол</param>
        /// <returns>Пространственный угол между векторами</returns>
        public SpaceAngle GetAngle(Vector3D Vector)
        {
            var lv_Angle1 = Angle;
            var lv_Angle2 = Vector.Angle;
            return lv_Angle1 - lv_Angle2;
        }

        /// <summary>Проекция на вектор</summary>
        /// <param name="Vector">Вектор, НА который производится проекция</param>
        /// <returns>Проекция на вектор</returns>
        public double GetProjectionTo(Vector3D Vector) => Product_Scalar(Vector) / Vector.R;

        public Func<Vector3D, double> GetProjectorV()
        {
            var t = this;
            return v => t * v / v.R;
        }

        public Expression GetProjectorV_Expression(Expression v)
        {
            var t = this;
            return Expression.Divide
            (
                Expression.Multiply
                (
                    Expression.Constant(t),
                    v
                ),
                Expression.Property(v, "R")
            );
        }

        /// <summary>Проекция на наравление</summary>
        /// <param name="Direction">Направление, на которое проектируется вектор</param>
        /// <returns>Проекция вектора на направление</returns>
        public double GetProjectionTo(SpaceAngle Direction)
        {
            var thetta = Direction.ThettaRad;
            var phi = Direction.PhiRad;
            return Math.Sin(thetta) * (_X * Math.Cos(phi) + _Y * Math.Sin(phi)) + _Z * Math.Cos(thetta);
        }

        private static MethodCallExpression sin_expr(Expression xx) => Expression.Call(((Func<double, double>)Math.Sin).Method, xx);
        private static MethodCallExpression cos_expr(Expression xx) => Expression.Call(((Func<double, double>)Math.Cos).Method, xx);

        public Func<SpaceAngle, double> GetProjectorA()
        {
            var t = this;
            return d =>
            {
                var th = d.ThettaRad;
                var ph = d.PhiRad;
                return Math.Sin(th) * (t._X * Math.Cos(ph) + t._Y * Math.Sin(ph)) + t._Z * Math.Cos(th);
            };
        }

        //public Expression GetProjectorA_Expression(Expression d) => Expression.Add
        //(
        //    Expression.Multiply
        //    (
        //        sin_expr(Expression.Property(d, nameof(SpaceAngle.ThettaRad))),
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
        //        cos_expr(Expression.Property(d, nameof(SpaceAngle.ThettaRad)))
        //    )
        //);

        public Expression GetProjectorA_Expression(Expression d)
        {
            var sin = (Func<double, double>)Math.Sin;
            var cos = (Func<double, double>)Math.Cos;
            return sin.GetCallExpression(d.GetProperty(nameof(SpaceAngle.ThettaRad)))
                .Multiply
                (
                    _X.ToExpression().Multiply(cos.GetCallExpression(d.GetProperty(nameof(SpaceAngle.PhiRad))))
                    .Add
                    (
                        _Y.ToExpression().Multiply(sin.GetCallExpression(d.GetProperty(nameof(SpaceAngle.PhiRad))))
                    )
                ).Add(_Z.ToExpression().Multiply(cos.GetCallExpression(d.GetProperty(nameof(SpaceAngle.ThettaRad)))));
        }
    }
}