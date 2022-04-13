using Logcast.Recruitment.DataAccess.Factories;
using Logcast.Recruitment.DataAccess.Repositories;
using Logcast.Recruitment.DataAccess.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logcast.Recruitment.DataAccess.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static void AddEntityFramework(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }

        public static void AddDataAccessServices(this IServiceCollection services)
        {
            services.AddRepositories();
            services.AddFactories();
            services.AddServices();
        }

        private static void AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<ISubscriptionRepository, SubscriptionRepository>();
            services.AddTransient<IAudioRepository, AudioRepository>();
        }

        private static void AddFactories(this IServiceCollection services)
        {
            services.AddTransient<IDbContextFactory, DbContextFactory>();
        }

        private static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IFileStorage, FileStorage>();
        }
    }
}
