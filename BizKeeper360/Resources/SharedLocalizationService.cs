﻿using Microsoft.Extensions.Localization;
using System.Reflection;

namespace BizKeeper360.Resources
{
    public class SharedLocalizationService
    {
        private readonly IStringLocalizerFactory _factory;

        public SharedLocalizationService(IStringLocalizerFactory factory)
        {
            _factory = factory;
        }

        public IStringLocalizer GetLocalizer<T>() where T : class
        {
            var type = typeof(T);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            return _factory.Create(type.Name, assemblyName.Name);
        }

        public IStringLocalizer Buttons => GetLocalizer<ButtonResources>();
        public IStringLocalizer Tables => GetLocalizer<TableResources>();
        public IStringLocalizer Pages => GetLocalizer<PageResources>();
        public IStringLocalizer Messages => GetLocalizer<MessageResources>();
        public IStringLocalizer Categories => GetLocalizer<CategoriesResources>();
    }
}
