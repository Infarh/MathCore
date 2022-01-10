using System;
using System.Threading;
using System.Threading.Tasks;
using MathCore.IoC;
using MathCore.IoC.ServiceRegistrations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable MemberCanBePrivate.Local

namespace MathCore.Tests.IoC
{
    /// <summary>Тест контейнера инверсии управления</summary>
    [TestClass]
    public class ServiceManagerTests
    {
        #region Тестовые сервисы

        private interface IService
        {
            int GetValue();
        }

        private interface IService2
        {
            int Value { get; }
        }

        private class Service_42 : IService
        {
            public int GetValue() => 42;
        }

        private class Service_GetHashCode : IService
        {
            public int GetValue() => GetHashCode();
        }

        private class Service_GetThreadId : IService
        {
            private readonly int _ThreadId = Thread.CurrentThread.ManagedThreadId;

            public int GetValue() => _ThreadId;
        }                                                                 
        
        private class Service_GetTaskId : IService
        {
            private readonly int _TaskId = Task.CurrentId ?? -1;

            public int GetValue() => _TaskId;
        }

        private class Service_Value : IService
        {
            private readonly int _Value;

            public Service_Value(int value) => _Value = value;

            public int GetValue() => _Value;
        }

        private class Service_TwoInterfaces : IService, IService2
        {
            private readonly int _Value;
            public Service_TwoInterfaces() : this(15) { }
            public Service_TwoInterfaces(int value) => _Value = value;

            int IService.GetValue() => _Value;

            int IService2.Value => _Value;
        }

        private class Service_ThrowException<TException> : IService
            where TException : Exception, new()
        {
            public int GetValue() => throw new TException();
        }


        #endregion

        [TestMethod]
        public void DefaultManager()
        {
            var expected = ServiceManager.Default;
            var actual = ServiceManager.Default;

            Assert.IsNotNull(expected);
            Assert.IsNotNull(actual);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TypeIndexator()
        {
            var service_manager = new ServiceManager();
            service_manager.Register<IService, Service_42>();
            var expected1 = service_manager.Get(typeof(IService));
            var expected2 = service_manager.Get<IService>();

            var actual = service_manager[typeof(IService)];

            Assert.IsNotNull(expected1);
            Assert.IsNotNull(expected2);

            Assert.AreEqual(expected1, actual);
            Assert.AreEqual(expected2, actual);

            Assert.IsInstanceOfType(actual, typeof(IService));
            Assert.IsInstanceOfType(actual, typeof(Service_42));
        }

        [TestMethod]
        public void ServiceRegistration_Default_Simple()
        {
            var service_manager = new ServiceManager();

            service_manager.Register<Service_42>();

            var service = service_manager.Get<Service_42>();

            Assert.IsNotNull(service);
        }

        /// <summary>Тестирование регистрации сервиса</summary>
        [TestMethod]
        public void ServiceRegistration_Default_Singleton()
        {
            var service_manager = new ServiceManager();

            service_manager.Register<IService, Service_42>();

            var service = service_manager.Get<IService>();
            Assert.IsNotNull(service);
            Assert.IsInstanceOfType(service, typeof(IService));
            Assert.IsInstanceOfType(service, typeof(Service_42));

            var actual = service.GetValue();
            const int expected = 42;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ServiceRegistration_Singleton_Factory()
        {
            var service_manager = new ServiceManager();
            service_manager.Register<IService>(() => new Service_Value(13));

            var instance1 = service_manager.Get<IService>();
            var instance2 = service_manager.Get<IService>();

            Assert.IsTrue(ReferenceEquals(instance1, instance2));
            Assert.AreEqual(instance1, instance2);
        }

        [TestMethod]
        public void ServiceRegistration_SingleCall_Factory()
        {
            var service_manager = new ServiceManager();

            var counter = 0;
            service_manager.Register<IService>(() => new Service_Value(++counter), ServiceRegistrationMode.SingleCall);

            var service = service_manager.Get<IService>();
            Assert.IsNotNull(service);
            Assert.IsInstanceOfType(service, typeof(IService));
            Assert.IsInstanceOfType(service, typeof(Service_Value));

            var actual = service.GetValue();
            var expected = counter;
            Assert.AreEqual(expected, actual);

            service = service_manager.Get<IService>();
            Assert.IsNotNull(service);
            Assert.IsInstanceOfType(service, typeof(IService));
            Assert.IsInstanceOfType(service, typeof(Service_Value));

            actual = service.GetValue();
            expected = counter;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ServiceRegistration_SingleCall()
        {
            var service_manager = new ServiceManager();
            service_manager.RegisterSingleCall<IService, Service_GetHashCode>();

            var instance1 = service_manager.Get<IService>();
            var instance2 = service_manager.Get<IService>();

            Assert.IsFalse(ReferenceEquals(instance1, instance2));
            Assert.AreNotEqual(instance1, instance2);
        }

        [TestMethod]
        public void ServiceRegistration_SingleCall_Simple()
        {
            var service_manager = new ServiceManager();
            service_manager.RegisterSingleCall<Service_42>();

            var instance1 = service_manager.Get<Service_42>();
            var instance2 = service_manager.Get<Service_42>();

            Assert.AreNotEqual(instance1, instance2);
            Assert.IsFalse(ReferenceEquals(instance1, instance2));
        }

        [TestMethod]
        public void ServiceRegistration_SingletonByThread()
        {
            var service_manager = new ServiceManager();

            service_manager.Register<IService, Service_GetHashCode>(ServiceRegistrationMode.SingleThread);

            var manager = service_manager.ServiceAccessor<IService>();

            var main_instance1 = manager.Service;

            IService thread1_instance1 = null;
            IService thread1_instance2 = null;

            IService thread2_instance1 = null;
            IService thread2_instance2 = null;

            var starter = new ManualResetEvent(false);
            var waiter1 = new ManualResetEvent(false);
            var waiter2 = new ManualResetEvent(false);
            var thread1 = new Thread(() =>
            {
                thread1_instance1 = manager.Service;
                waiter1.Set();
                starter.WaitOne();
                thread1_instance2 = manager.Service;
            });
            var thread2 = new Thread(() =>
            {
                thread2_instance1 = manager.Service;
                waiter2.Set();
                starter.WaitOne();
                thread2_instance2 = manager.Service;
            });

            thread1.Start();
            thread2.Start();

            waiter2.WaitOne();
            waiter1.WaitOne();

            starter.Set();

            thread1.Join();
            thread2.Join();

            var main_instance2 = manager.Service;

            Assert.That.Value(main_instance1).IsReferenceEquals(main_instance2);
            Assert.That.Value(thread1_instance1).IsReferenceEquals(thread1_instance2);
            Assert.That.Value(thread2_instance1).IsReferenceEquals(thread2_instance2);

            Assert.That.Value(main_instance1)
               .IsNotReferenceEquals(thread1_instance1)
               .IsNotReferenceEquals(thread2_instance1);

            Assert.That.Value(thread1_instance1)
               .IsNotReferenceEquals(thread2_instance1);
        }

        [TestMethod]
        public async Task ServiceRegistration_SingletonByThread_Simple()
        {
            var service_manager = new ServiceManager();

            service_manager.Register<Service_GetHashCode>(ServiceRegistrationMode.SingleThread);

            var instance = service_manager.Get<Service_GetHashCode>();

            await Task.Yield().ConfigureAwait(false);

            var instance2 = service_manager.Get<Service_GetHashCode>();

            Assert.IsNotNull(instance2);
            Assert.IsFalse(ReferenceEquals(instance, instance2));
            Assert.AreNotEqual(instance, instance2);
        }

        [TestMethod]
        public void ServiceRegistration_SingletonByThread_Factory()
        {
            var service_manager = new ServiceManager();
            service_manager.RegisterSingleThread(() => new Service_GetThreadId());

            var manager = service_manager.ServiceAccessor<Service_GetThreadId>();

            var main_instance1 = manager.Service;

            Service_GetThreadId thread1_instance1 = null;
            Service_GetThreadId thread1_instance2 = null;

            Service_GetThreadId thread2_instance1 = null;
            Service_GetThreadId thread2_instance2 = null;

            var starter = new ManualResetEvent(false);
            var waiter1 = new ManualResetEvent(false);
            var waiter2 = new ManualResetEvent(false);
            var thread1 = new Thread(() =>
            {
                thread1_instance1 = manager.Service;
                waiter1.Set();
                starter.WaitOne();
                thread1_instance2 = manager.Service;
            });
            var thread2 = new Thread(() =>
            {
                thread2_instance1 = manager.Service;
                waiter2.Set();
                starter.WaitOne();
                thread2_instance2 = manager.Service;
            });

            thread1.Start();
            thread2.Start();

            waiter2.WaitOne();
            waiter1.WaitOne();

            starter.Set();

            thread1.Join();
            thread2.Join();

            var main_instance2 = manager.Service;

            Assert.That.Value(main_instance1).IsReferenceEquals(main_instance2);
            Assert.That.Value(thread1_instance1).IsReferenceEquals(thread1_instance2);
            Assert.That.Value(thread2_instance1).IsReferenceEquals(thread2_instance2);

            Assert.That.Value(main_instance1)
               .IsNotReferenceEquals(thread1_instance1)
               .IsNotReferenceEquals(thread2_instance1);

            Assert.That.Value(thread1_instance1)
               .IsNotReferenceEquals(thread2_instance1);
        }

        [TestMethod]
        public void ServiceRegistration_Singleton()
        {
            var service_manager = new ServiceManager();
            service_manager.Register<IService, Service_GetHashCode>();

            var expected = service_manager.Get<IService>();
            var actual = service_manager.Get<IService>();

            Assert.IsInstanceOfType(expected, typeof(Service_GetHashCode));
            Assert.IsInstanceOfType(actual, typeof(Service_GetHashCode));
            Assert.IsTrue(ReferenceEquals(expected, actual));
        }

        [TestMethod]
        public void ServiceRegistration_Singleton_Instance()
        {
            var service_manager = new ServiceManager();
            var expected = new Service_GetHashCode();
            service_manager.RegisterSingleton<IService>(expected);

            var actual = service_manager.Get<IService>();
            Assert.IsInstanceOfType(actual, typeof(IService));
            Assert.IsInstanceOfType(actual, typeof(Service_GetHashCode));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ServiceRegistered()
        {
            var service_manager = new ServiceManager();
            Assert.IsFalse(service_manager.ServiceRegistered<IService>());
            service_manager.Register<IService, Service_42>();
            Assert.IsTrue(service_manager.ServiceRegistered<IService>());
        }

        [TestMethod]
        public void ServiceRegistered_ByType()
        {
            var service_manager = new ServiceManager();
            Assert.IsFalse(service_manager.ServiceRegistered(typeof(IService)));
            service_manager.Register<IService, Service_42>();
            Assert.IsTrue(service_manager.ServiceRegistered(typeof(IService)));
        }

        [TestMethod]
        public void ServiceRegistration_Singleton_TwoInterfaces()
        {
            var service_manager = new ServiceManager();

            service_manager.RegisterSingleton<IService, Service_TwoInterfaces>();
            service_manager.RegisterSingleton<IService2, Service_TwoInterfaces>();

            var instance_i_service = service_manager.Get<IService>();
            var instance_i_service2 = service_manager.Get<IService2>();

            Assert.IsInstanceOfType(instance_i_service, typeof(IService));
            Assert.IsInstanceOfType(instance_i_service, typeof(Service_TwoInterfaces));
            Assert.IsInstanceOfType(instance_i_service2, typeof(IService2));
            Assert.IsInstanceOfType(instance_i_service, typeof(Service_TwoInterfaces));

            Assert.IsTrue(ReferenceEquals(instance_i_service, instance_i_service2));
        }

        [TestMethod]
        public void ServiceRegistration_Singleton_Timeout()
        {
            var service_manager = new ServiceManager();

            var timeout = TimeSpan.FromMilliseconds(100);
            var registration = service_manager
                .RegisterSingleton<IService, Service_42>()
                .With(r => r.InstanceActualityTime = timeout);

            Assert.AreEqual(timeout, registration.InstanceActualityTime);

            var service1 = service_manager.Get<IService>();
            var service2 = service_manager.Get<IService>();
            Assert.IsNotNull(service1);
            Assert.IsNotNull(service2);
            Assert.IsInstanceOfType(service1, typeof(Service_42));
            Assert.IsInstanceOfType(service2, typeof(Service_42));
            Assert.IsTrue(ReferenceEquals(service1, service2));

            Thread.Sleep(1000);
            var service3 = service_manager.Get<IService>();
            Assert.IsNotNull(service3);
            Assert.IsInstanceOfType(service3, typeof(Service_42));
            //Assert.IsFalse(ReferenceEquals(service1, service3));
        }

        [TestMethod]
        public void ExceptionGeneration_SingleCall()
        {
            var service_manager = new ServiceManager();

            var call_number = 0;
            var registration = service_manager
                .Register<IService>(
                    () => throw new ApplicationException((++call_number).ToString()),
                    ServiceRegistrationMode.SingleCall);

            var exception_throwed = false;
            Assert.IsNull(registration.LastException);
            try
            {
                service_manager.Get<IService>();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (ApplicationException)
            {
                exception_throwed = true;
            }
#pragma warning restore CA1031 // Do not catch general exception types
            Assert.IsTrue(exception_throwed);
            Assert.IsNotNull(registration.LastException);
            Assert.IsInstanceOfType(registration.LastException, typeof(ApplicationException));
            Assert.AreEqual("1", registration.LastException.Message);

            exception_throwed = false;
            try
            {
                service_manager.Get<IService>();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (ApplicationException)
            {
                exception_throwed = true;
            }
#pragma warning restore CA1031 // Do not catch general exception types
            Assert.IsTrue(exception_throwed);
            Assert.IsNotNull(registration.LastException);
            Assert.IsInstanceOfType(registration.LastException, typeof(ApplicationException));
            Assert.AreEqual("2", registration.LastException.Message);
        }

        [TestMethod]
        public void ExceptionGeneration_Singleton()
        {
            var service_manager = new ServiceManager();

            var call_number = 0;
            var registration = service_manager
                .Register<IService>(
                    () => throw new ApplicationException((++call_number).ToString()),
                    ServiceRegistrationMode.Singleton);

            var exception_throwed = false;
            Assert.IsNull(registration.LastException);
            try
            {
                service_manager.Get<IService>();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (ApplicationException)
            {
                exception_throwed = true;
            }
#pragma warning restore CA1031 // Do not catch general exception types
            Assert.IsTrue(exception_throwed);
            var last_exception = registration.LastException;
            Assert.IsNotNull(last_exception);
            Assert.IsInstanceOfType(last_exception, typeof(ApplicationException));
            Assert.AreEqual("1", last_exception.Message);

            exception_throwed = false;
            try
            {
                service_manager.Get<IService>();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (ApplicationException)
            {
                exception_throwed = true;
            }
#pragma warning restore CA1031 // Do not catch general exception types
            Assert.IsTrue(exception_throwed);
            Assert.IsNotNull(registration.LastException);
            Assert.IsInstanceOfType(registration.LastException, typeof(ApplicationException));
            Assert.AreEqual("1", registration.LastException.Message);
            Assert.IsTrue(ReferenceEquals(last_exception, registration.LastException));
        }
    }
}
