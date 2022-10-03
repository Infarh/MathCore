using System;
using System.ComponentModel;

namespace MathCore;

/// <summary>Тип значения угла</summary>
[Serializable]
public enum AngleType : byte
{
    /// <summary>Радиан</summary>
    [Description("Радиан")]
    Rad = 0, 
    /// <summary>Градус</summary>
    [Description("Градус")]
    Deg = 1
}