using AutoMapper;
using codesphere_api.Mappings;
using codesphere_api.Services;
using codesphere_api.Services.interfaces;

namespace codesphere_api.Extensions
{
    public static class ApplicationServices
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();

            services.AddAutoMapper(cfg => { }, typeof(MappingProfile));
            return services;
        }
    }

}