#nullable enable
using System.Diagnostics.CodeAnalysis;

// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Collections.Generic;

/// <summary>Класс методов-расширений для интерфейса <see cref="ICloneable"/></summary>
[SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
// ReSharper disable once InconsistentNaming
public static class ICloneableExtensions
{
    /// <summary>
    /// Клонировать объект <typeparamref name="T"/>
    /// (с возможностью последующей инициализации <see cref="IInitializable"/>, если интерфейс поддерживается)
    /// </summary>
    /// <typeparam name="T">Тип клонируемого объекта</typeparam>
    /// <param name="obj">Клонируемый объект</param>
    /// <returns>Клонированный объект</returns>
    public static T CloneObject<T>(this T obj) where T : ICloneable
    {
        var result = (T)obj.Clone();
        (result as IInitializable)?.Initialize();
        return result;
    }

    /// <summary>
    /// Клонировать объект <typeparamref name="T"/>
    /// (с возможностью последующей инициализации <see cref="IInitializable"/>, если интерфейс поддерживается)
    /// </summary>
    /// <typeparam name="T">Тип клонируемого объекта</typeparam>
    /// <param name="obj">Клонируемый объект</param>
    /// <param name="Initializer">Метод, вызываемый для клонированного объекта для его инициализации</param>
    /// <returns>Клонированный объект</returns>
    public static T CloneObject<T>(this T obj, Action<T> Initializer) where T : ICloneable
    {
        var result = (T)obj.Clone();
        (result as IInitializable)?.Initialize();
        Initializer(result);
        return result;
    }

    /// <summary>
    /// Клонировать объект <typeparamref name="T"/>
    /// (с возможностью последующей инициализации <see cref="IInitializable"/>, либо <see cref="IInitializable{TParameter}"/>
    /// если интерфейсы поддерживается)
    /// </summary>
    /// <typeparam name="T">Тип клонируемого объекта</typeparam>
    /// <typeparam name="TParameter">Тип параметра процесса инициализации</typeparam>
    /// <param name="obj">Клонируемый объект</param>
    /// <param name="Initializer">Метод, вызываемый для клонированного объекта для его инициализации</param>
    /// <param name="parameter">Параметр процесса инициализации</param>
    /// <returns>Клонированный объект</returns>
    public static T CloneObject<T, TParameter>(this T obj, Action<T, TParameter> Initializer, TParameter parameter)
        where T : ICloneable
    {
        var result = (T)obj.Clone();
        (result as IInitializable)?.Initialize();
        (result as IInitializable<TParameter>)?.Initialize(parameter);
        Initializer(result, parameter);
        return result;
    }

    /// <summary>
    /// Клонировать объект <typeparamref name="T"/> 
    /// с последующей инициализацией <see cref="IInitializable"/>, либо <see cref="IInitializable{TParameter}"/>
    /// </summary>
    /// <typeparam name="T">Тип клонируемого объекта</typeparam>
    /// <typeparam name="TParameter">Тип параметра процесса инициализации</typeparam>
    /// <param name="obj">Клонируемый объект</param>
    /// <param name="parameter">Параметр процесса инициализации</param>
    /// <returns>Клонированный объект</returns>
    public static T CloneObject<T, TParameter>(this T obj, TParameter parameter)
        where T : ICloneable, IInitializable<TParameter>
    {
        var result = (T)obj.Clone();
        result.Initialize(parameter);
        return result;
    }
}