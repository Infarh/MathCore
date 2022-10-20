using System;
using System.Collections.Generic;

using MathCore.Annotations;

// ReSharper disable UnusedType.Global

namespace MathCore;

/// <summary>Список действий <see cref="Action"/></summary>
public class ActionList : List<Action>
{
    /// <summary>Инициализация нового экземпляра <see cref="ActionList"/></summary>
    public ActionList() { }

    /// <summary>Инициализация нового экземпляра <see cref="ActionList"/></summary>
    /// <param name="Capacity">Ёмкость списка</param>
    public ActionList(int Capacity) : base(Capacity) { }

    /// <summary>Инициализация нового экземпляра <see cref="ActionList"/></summary>
    /// <param name="ActionsEnumeration">Перечисление действий списка</param>
    public ActionList([NotNull] IEnumerable<Action> ActionsEnumeration) : base(ActionsEnumeration) { }

    /// <summary>Выполнение последовательности действий списка</summary>
    public void Invoke()
    {
        for(int i = 0, actions_count = Count; i < actions_count; i++)
            this[i]();
    }
}

/// <summary>Список действий <see cref="Action{T1}"/></summary>
/// <typeparam name="T1">Тип 1 параметра действия</typeparam>
public class ActionList<T1> : List<Action<T1>>
{
    /// <summary>Инициализация нового экземпляра <see cref="ActionList{T1}"/></summary>
    public ActionList() { }

    /// <summary>Инициализация нового экземпляра <see cref="ActionList{T1}"/></summary>
    /// <param name="Capacity">Ёмкость списка</param>
    public ActionList(int Capacity) : base(Capacity) { }

    /// <summary>Инициализация нового экземпляра <see cref="ActionList{T1}"/></summary>
    /// <param name="ActionsEnumeration">Перечисление действий списка</param>
    public ActionList([NotNull] IEnumerable<Action<T1>> ActionsEnumeration) : base(ActionsEnumeration) { }

    /// <summary>Выполнение последовательности действий списка</summary>
    public void Invoke(T1 p1)
    {
        for(int i = 0, actions_count = Count; i < actions_count; i++)
            this[i](p1);
    }
}

/// <summary>Список действий <see cref="Action{T1, T2}"/></summary>
/// <typeparam name="T1">Тип 1 параметра действия</typeparam>
/// <typeparam name="T2">Тип 2 параметра действия</typeparam>
public class ActionList<T1, T2> : List<Action<T1, T2>>
{
    /// <summary>Инициализация нового экземпляра <see cref="ActionList{T1, T2}"/></summary>
    public ActionList() { }

    /// <summary>Инициализация нового экземпляра <see cref="ActionList{T1, T2}"/></summary>
    /// <param name="Capacity">Ёмкость списка</param>
    public ActionList(int Capacity) : base(Capacity) { }

    /// <summary>Инициализация нового экземпляра <see cref="ActionList{T1, T2}"/></summary>
    /// <param name="ActionsEnumeration">Перечисление действий списка</param>
    public ActionList([NotNull] IEnumerable<Action<T1, T2>> ActionsEnumeration) : base(ActionsEnumeration) { }

    /// <summary>Выполнение последовательности действий списка</summary>
    public void Invoke(T1 p1, T2 p2)
    {
        for(int i = 0, actions_count = Count; i < actions_count; i++)
            this[i](p1, p2);
    }
}

/// <summary>Список действий <see cref="Action{T1, T2, T3}"/></summary>
/// <typeparam name="T1">Тип 1 параметра действия</typeparam>
/// <typeparam name="T2">Тип 2 параметра действия</typeparam>
/// <typeparam name="T3">Тип 3 параметра действия</typeparam>
public class ActionList<T1, T2, T3> : List<Action<T1, T2, T3>>
{
    /// <summary>Инициализация нового экземпляра <see cref="ActionList{T1, T2, T3}"/></summary>
    public ActionList() { }

    /// <summary>Инициализация нового экземпляра <see cref="ActionList{T1, T2, T3}"/></summary>
    /// <param name="Capacity">Ёмкость списка</param>
    public ActionList(int Capacity) : base(Capacity) { }

    /// <summary>Инициализация нового экземпляра <see cref="ActionList{T1, T2, T3}"/></summary>
    /// <param name="ActionsEnumeration">Перечисление действий списка</param>
    public ActionList([NotNull] IEnumerable<Action<T1, T2, T3>> ActionsEnumeration) : base(ActionsEnumeration) { }

    /// <summary>Выполнение последовательности действий списка</summary>
    public void Invoke(T1 p1, T2 p2, T3 p3)
    {
        for(int i = 0, actions_count = Count; i < actions_count; i++)
            this[i](p1, p2, p3);
    }
}

/// <summary>Список действий <see cref="Action{T1, T2, T3, T4}"/></summary>
/// <typeparam name="T1">Тип 1 параметра действия</typeparam>
/// <typeparam name="T2">Тип 2 параметра действия</typeparam>
/// <typeparam name="T3">Тип 3 параметра действия</typeparam>
/// <typeparam name="T4">Тип 4 параметра действия</typeparam>
public class ActionList<T1, T2, T3, T4> : List<Action<T1, T2, T3, T4>>
{
    /// <summary>Инициализация нового экземпляра <see cref="ActionList{T1, T2, T3, T4}"/></summary>
    public ActionList() { }

    /// <summary>Инициализация нового экземпляра <see cref="ActionList{T1, T2, T3, T4}"/></summary>
    /// <param name="Capacity">Ёмкость списка</param>
    public ActionList(int Capacity) : base(Capacity) { }

    /// <summary>Инициализация нового экземпляра <see cref="ActionList{T1, T2, T3, T4}"/></summary>
    /// <param name="ActionsEnumeration">Перечисление действий списка</param>
    public ActionList([NotNull] IEnumerable<Action<T1, T2, T3, T4>> ActionsEnumeration) : base(ActionsEnumeration) { }

    /// <summary>Выполнение последовательности действий списка</summary>
    public void Invoke(T1 p1, T2 p2, T3 p3, T4 p4)
    {
        for(int i = 0, actions_count = Count; i < actions_count; i++)
            this[i](p1, p2, p3, p4);
    }
}

/// <summary>Список действий <see cref="Action{T1, T2, T3, T4, T5}"/></summary>
/// <typeparam name="T1">Тип 1 параметра действия</typeparam>
/// <typeparam name="T2">Тип 2 параметра действия</typeparam>
/// <typeparam name="T3">Тип 3 параметра действия</typeparam>
/// <typeparam name="T4">Тип 4 параметра действия</typeparam>
/// <typeparam name="T5">Тип 5 параметра действия</typeparam>
public class ActionList<T1, T2, T3, T4, T5> : List<Action<T1, T2, T3, T4, T5>>
{
    /// <summary>Инициализация нового экземпляра <see cref="ActionList{T1, T2, T3, T4, T5}"/></summary>
    public ActionList() { }

    /// <summary>Инициализация нового экземпляра <see cref="ActionList{T1, T2, T3, T4, T5}"/></summary>
    /// <param name="Capacity">Ёмкость списка</param>
    public ActionList(int Capacity) : base(Capacity) { }

    /// <summary>Инициализация нового экземпляра <see cref="ActionList{T1, T2, T3, T4, T5}"/></summary>
    /// <param name="ActionsEnumeration">Перечисление действий списка</param>
    public ActionList([NotNull] IEnumerable<Action<T1, T2, T3, T4, T5>> ActionsEnumeration) : base(ActionsEnumeration) { }

    /// <summary>Выполнение последовательности действий списка</summary>
    public void Invoke(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
    {
        for(int i = 0, actions_count = Count; i < actions_count; i++)
            this[i](p1, p2, p3, p4, p5);
    }
}

/// <summary>Список действий <see cref="Action{T1, T2, T3, T4, T5, T6}"/></summary>
/// <typeparam name="T1">Тип 1 параметра действия</typeparam>
/// <typeparam name="T2">Тип 2 параметра действия</typeparam>
/// <typeparam name="T3">Тип 3 параметра действия</typeparam>
/// <typeparam name="T4">Тип 4 параметра действия</typeparam>
/// <typeparam name="T5">Тип 5 параметра действия</typeparam>
/// <typeparam name="T6">Тип 6 параметра действия</typeparam>
public class ActionList<T1, T2, T3, T4, T5, T6> : List<Action<T1, T2, T3, T4, T5, T6>>
{
    /// <summary>Инициализация нового экземпляра <see cref="ActionList{T1, T2, T3, T4, T5, T6}"/></summary>
    public ActionList() { }

    /// <summary>Инициализация нового экземпляра <see cref="ActionList{T1, T2, T3, T4, T5, T6}"/></summary>
    /// <param name="Capacity">Ёмкость списка</param>
    public ActionList(int Capacity) : base(Capacity) { }

    /// <summary>Инициализация нового экземпляра <see cref="ActionList{T1, T2, T3, T4, T5, T6}"/></summary>
    /// <param name="ActionsEnumeration">Перечисление действий списка</param>
    public ActionList([NotNull] IEnumerable<Action<T1, T2, T3, T4, T5, T6>> ActionsEnumeration) : base(ActionsEnumeration) { }

    /// <summary>Выполнение последовательности действий списка</summary>
    public void Invoke(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6)
    {
        for(int i = 0, actions_count = Count; i < actions_count; i++)
            this[i](p1, p2, p3, p4, p5, p6);
    }
}

/// <summary>Список действий <see cref="Action{T1, T2, T3, T4, T5, T6, T7}"/></summary>
/// <typeparam name="T1">Тип 1 параметра действия</typeparam>
/// <typeparam name="T2">Тип 2 параметра действия</typeparam>
/// <typeparam name="T3">Тип 3 параметра действия</typeparam>
/// <typeparam name="T4">Тип 4 параметра действия</typeparam>
/// <typeparam name="T5">Тип 5 параметра действия</typeparam>
/// <typeparam name="T6">Тип 6 параметра действия</typeparam>
/// <typeparam name="T7">Тип 7 параметра действия</typeparam>
public class ActionList<T1, T2, T3, T4, T5, T6, T7> : List<Action<T1, T2, T3, T4, T5, T6, T7>>
{
    /// <summary>Инициализация нового экземпляра <see cref="ActionList{T1, T2, T3, T4, T5, T6, T7}"/></summary>
    public ActionList() { }

    /// <summary>Инициализация нового экземпляра <see cref="ActionList{T1, T2, T3, T4, T5, T6, T7}"/></summary>
    /// <param name="Capacity">Ёмкость списка</param>
    public ActionList(int Capacity) : base(Capacity) { }

    /// <summary>Инициализация нового экземпляра <see cref="ActionList{T1, T2, T3, T4, T5, T6, T7}"/></summary>
    /// <param name="ActionsEnumeration">Перечисление действий списка</param>
    public ActionList([NotNull] IEnumerable<Action<T1, T2, T3, T4, T5, T6, T7>> ActionsEnumeration) : base(ActionsEnumeration) { }

    /// <summary>Выполнение последовательности действий списка</summary>
    public void Invoke(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7)
    {
        for(int i = 0, actions_count = Count; i < actions_count; i++)
            this[i](p1, p2, p3, p4, p5, p6, p7);
    }
}