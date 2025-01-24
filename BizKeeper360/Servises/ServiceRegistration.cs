using BizKeeper360.Interfaces;
using BizKeeper360.Resources;

namespace BizKeeper360.Servises
{
    public static class ServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<CaptchaService>();
            services.AddScoped<HtmlValidator>();
            services.AddScoped<SharedResources>();
            services.AddScoped<SharedLocalizationService>();
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<CategoryLocalizationService>();
        }
    }
}
