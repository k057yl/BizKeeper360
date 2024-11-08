using Microsoft.Extensions.Localization;
using System.Reflection;

namespace BizKeeper360.Resources
{
    public class SharedLocalizationService
    {
        private readonly IStringLocalizer _localizer;

        public SharedLocalizationService(IStringLocalizerFactory factory)
        {
            var type = typeof(SharedResources);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            _localizer = factory.Create("SharedResources", assemblyName.Name);
        }

        public LocalizedString this[string key] => _localizer[key];

        public LocalizedString this[string key, params object[] arguments] => _localizer[key, arguments];
    }
}
