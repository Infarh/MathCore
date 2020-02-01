using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MathCore.Annotations;
// ReSharper disable UnusedMember.Global

namespace MathCore.IoC.ServiceRegistrations
{
    public abstract partial class ServiceRegistration
    {
        protected class ServiceConstructorInfo
        {
            private readonly ConstructorInfo _Constructor;
            private ParameterInfo[] _Parameters;

            [NotNull] public IReadOnlyCollection<ParameterInfo> Parameters => _Parameters ??= _Constructor.GetParameters();
            [NotNull] public IEnumerable<Type> ParameterTypes => Parameters.Select(p => p.ParameterType);

            public bool IsDefault => Parameters.Count == 0;

            public ServiceConstructorInfo(ConstructorInfo constructor) => _Constructor = constructor;

            [NotNull] public object[] GetParametersValues([NotNull] Func<Type, object> ParameterSelector) => ParameterTypes.ToArray(ParameterSelector);

            public object CreateInstance(object[] parameters) => _Constructor.Invoke(parameters);

            public object CreateInstance([NotNull] Func<Type, object> ParameterSelector) => CreateInstance(GetParametersValues(ParameterSelector));
        }
    }
}