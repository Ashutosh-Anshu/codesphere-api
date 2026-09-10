using codesphere_api.Repositories;
using codesphere_api.Repositories.Interfaces;

namespace codesphere_api.Extensions
{
    public static class ApplicationRepositories
    {
        public static IServiceCollection AddApplicationRepositories(
            this IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();

            return services;
        }
    }
}
