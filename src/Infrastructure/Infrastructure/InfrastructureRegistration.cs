using Application.Interfaces.Services;
using EasyCaching.Core.Configurations;
using Infrastructure.Services;
using Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class InfrastructureRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITokenService, TokenService>();
            services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.Configure<CacheSettings>(configuration.GetSection("CacheSettings"));

            var cacheSettings = configuration.GetSection("CacheSettings").Get<CacheSettings>();

            if (cacheSettings.PreferRedis)
            {
                services.AddEasyCaching(option =>
                {
                    option.WithJson();
                    option.UseRedis(config =>
                    {
                        config.DBConfig.Endpoints.Add(new ServerEndPoint(cacheSettings.RedisURL, cacheSettings.RedisPort));
                    }, "json");
                });
            }
            else
            {
                services.AddEasyCaching(option =>
                {
                    option.UseInMemory();
                });
            }
            services.AddTransient<IEasyCacheService, EasyCacheService>();
        }
    }
}
