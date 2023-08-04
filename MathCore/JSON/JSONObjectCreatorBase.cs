#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

using MathCore.CSV;
using MathCore.Values;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible

namespace MathCore.JSON;

/// <summary>Базовый класс генераторов JSON объектов</summary>
public abstract class JSONObjectCreatorBase
{
    /// <summary>Создать объект JSON</summary>
    /// <param name="obj">Объект-прототип, на основе которого генерируется JSON-объекта</param>
    /// <returns>Объект JSON</returns>
    internal abstract JSONObject Create(object? obj);
}