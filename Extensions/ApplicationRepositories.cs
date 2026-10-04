using codesphere_api.Repositories;
using codesphere_api.Repositories.Interfaces;
using codesphere_api.Services;
using codesphere_api.Services.interfaces;

namespace codesphere_api.Extensions
{
    public static class ApplicationRepositories
    {
        public static IServiceCollection AddApplicationRepositories(
            this IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            return services;
        }
    }
}
