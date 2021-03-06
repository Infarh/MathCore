﻿using MathCore.Annotations;

namespace MathCore.IoC
{
    public class ServiceManagerAccessor<TService> where TService : class
    {
        private readonly ServiceManager _ServiceManager;

        [CanBeNull] public TService Service => _ServiceManager.Get<TService>();

        public ServiceManagerAccessor(ServiceManager ServiceManager) => _ServiceManager = ServiceManager;
    }
}