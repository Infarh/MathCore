
namespace System
{
    /// <summary>Объект, поддерживающий инициализацию</summary>
    public interface IInitializable
    {
        /// <summary>Инициализация</summary>
        void Initialize();
    }

    /// <summary>Объект, поддерживающий инициализацию с параметром</summary>
    /// <typeparam name="T">Тип параметра инициализации</typeparam>
    public interface IInitializable<in T>
    {
        /// <summary>Инициализация</summary>
        /// <param name="t">Параметр</param>
        void Initialize(T t);
    }

    /// <summary>Объект, поддерживающий инициализацию с двумя параметрами</summary>
    /// <typeparam name="T1">Тип первого параметра</typeparam>
    /// <typeparam name="T2">Тип второго параметра</typeparam>
    public interface IInitializable<in T1, in T2>
    {
        /// <summary>Инициализация</summary>
        /// <param name="t1">Первый параметр</param>
        /// <param name="t2">Второй параметр</param>
        void Initialize(T1 t1, T2 t2);
    }

    /// <summary>Объект, поддерживаюий инициализацию с тремя параметрами</summary>
    /// <typeparam name="T1">Тип первого параметра</typeparam>
    /// <typeparam name="T2">Тип второго параметра</typeparam>
    /// <typeparam name="T3">Тип третьего параметра</typeparam>
    public interface IInitializable<in T1, in T2, in T3>
    {
        /// <summary>Инициализация</summary>
        /// <param name="t1">Первый параметр</param>
        /// <param name="t2">Второй параметр</param>
        /// <param name="t3">Третий параметр</param>
        void Initialize(T1 t1, T2 t2, T3 t3);
    }

    /// <summary>ОБъект, поддерживающий инициализацию с четырьмя параметрами</summary>
    /// <typeparam name="T1">Тип первого параметра</typeparam>
    /// <typeparam name="T2">Тип второго параметра</typeparam>
    /// <typeparam name="T3">Тип третьего параметра</typeparam>
    /// <typeparam name="T4">Тип четвёртого патаметра</typeparam>
    public interface IInitializable<in T1, in T2, in T3, in T4>
    {
        /// <summary>Инициализация</summary>
        /// <param name="t1">Первый параметр</param>
        /// <param name="t2">Второй параметр</param>
        /// <param name="t3">Третий параметр</param>
        /// <param name="t4">Четвёртый параметр</param>
        void Initialize(T1 t1, T2 t2, T3 t3, T4 t4);
    }

    /// <summary>ОБъект, поддерживающий инициализацию с пятью параметрами</summary>
    /// <typeparam name="T1">Тип первого параметра</typeparam>
    /// <typeparam name="T2">Тип второго параметра</typeparam>
    /// <typeparam name="T3">Тип третьего параметра</typeparam>
    /// <typeparam name="T4">Тип четвёртого патаметра</typeparam>
    /// <typeparam name="T5">Тип пятого патаметра</typeparam>
    public interface IInitializable<in T1, in T2, in T3, in T4, in T5>
    {
        /// <summary>Инициализация</summary>
        /// <param name="t1">Первый параметр</param>
        /// <param name="t2">Второй параметр</param>
        /// <param name="t3">Третий параметр</param>
        /// <param name="t4">Четвёртый параметр</param>
        /// <param name="t5">Пятый параметр</param>
        void Initialize(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
    }
}