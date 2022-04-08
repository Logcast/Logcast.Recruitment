using Logcast.Recruitment.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Logcast.Recruitment.Domain.Configuration
{
    public static class ServiceCollectionExtension
    {
        public static void AddDomainServices(this IServiceCollection services)
        {
            services.AddServices();
        }

        private static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<ISubscriptionService, SubscriptionService>();
            services.AddTransient<IFileService, FileService>();
        }
    }
}
