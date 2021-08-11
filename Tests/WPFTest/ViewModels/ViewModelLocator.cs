using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace WPFTest.ViewModels
{
    public class ViewModelLocator
    {
        public DynamicModelLocator Get { get; } = new DynamicModelLocator(App.Services);

        public MainWindowViewModel MainWindowModel => App.Services.GetRequiredService<MainWindowViewModel>();

        public SecondViewModel SecondModel => App.Services.GetRequiredService<SecondViewModel>();
    }

    public class DynamicModelLocator : DynamicObject
    {
        private static readonly Dictionary<string, Type> __Types = GetTypes();

        private static Dictionary<string, Type> GetTypes()
        {
            var app_assembly = typeof(App).Assembly;
            return app_assembly.DefinedTypes.Select(t => (t.Name, Type: t))
               .Distinct(t => t.Name)
               .ToDictionary(t => t.Name, t => Type.GetType(t.Type.FullName));
        }

        private readonly IServiceProvider _Services;

        public DynamicModelLocator(IServiceProvider Services) => _Services = Services;

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var type_name = binder.Name;
            if (!__Types.TryGetValue(type_name, out var type) || !(_Services.GetService(type) is { } value)) 
                return base.TryGetMember(binder, out result);

            result = value;
            return true;
        }
    }
}