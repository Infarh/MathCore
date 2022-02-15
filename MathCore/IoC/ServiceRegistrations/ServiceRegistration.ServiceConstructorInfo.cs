#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MathCore.Annotations;
// ReSharper disable UnusedMember.Global

namespace MathCore.IoC.ServiceRegistrations;

public abstract partial class ServiceRegistration
{
    protected class ServiceConstructorInfo
    {
        private readonly ConstructorInfo _Constructor;

        private ParameterInfo[]? _Parameters;

        public IReadOnlyList<ParameterInfo> Parameters => _Parameters ??= _Constructor.GetParameters();

        public IEnumerable<Type> ParameterTypes => Parameters.Select(p => p.ParameterType);

        public bool IsDefault => Parameters.Count == 0;

        public ServiceConstructorInfo(ConstructorInfo constructor) => _Constructor = constructor;

        public object?[] GetParametersValues(Func<Type, object?> ParameterSelector, object?[] InputParameters)
        {
            var parameters = Parameters;
            var result = new object?[parameters.Count];
            var j = 0;
            for (var i = 0; i < result.Length; i++)
            {
                var parameter_type = parameters[i].ParameterType;
                object? obj = null;
                if (j < InputParameters.Length)
                {
                    var parameter = InputParameters[j];
                    if (parameter is null)
                        ++j;
                    else if (parameter_type.IsInstanceOfType(parameter))
                    {
                        obj = parameter;
                        ++j;
                    }
                }
                result[i] = obj ?? ParameterSelector(parameter_type);
                if (result[i] is null && parameters[i].GetCustomAttribute(typeof(NotNullAttribute)) != null)
                    throw new InvalidOperationException($"Ошибка в процессе создания объекта - не найден параметр конструктора c индексом {i} типа {parameter_type} с именем {parameters[i].Name} помеченный аттрибутом [NotNull]");
            }
            return result;
        }

        public object CreateInstance(object?[] parameters) => _Constructor.Invoke(parameters);

        public object CreateInstance(Func<Type, object> ParameterSelector, params object[] parameters) => CreateInstance(GetParametersValues(ParameterSelector, parameters));
    }
}