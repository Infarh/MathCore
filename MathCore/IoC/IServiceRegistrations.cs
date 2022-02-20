#nullable enable
using System;
using MathCore.IoC.ServiceRegistrations;

namespace MathCore.IoC;

/// <summary>Регистратор сервисов</summary>
public interface IServiceRegistrations
{
    /// <summary>Запрос регистрации сервиса по его типу</summary>
    /// <param name="ServiceType">Тип запрашиваемого сервиса</param>
    ServiceRegistration? this[Type ServiceType] { get; }
}