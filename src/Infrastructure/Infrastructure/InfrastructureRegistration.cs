using Application.Interfaces.Services;
using Common.Settings;
using Infrastructure.MqManager;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;

namespace Infrastructure
{
    public static class InfrastructureRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITokenService, TokenService>();
            services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.Configure<CacheSettings>(configuration.GetSection("CacheSettings"));

            services.Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQSettings"));


            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            services.AddSingleton<RabbitModelPooledObjectPolicy>();
            services.AddSingleton<IRabbitService, RabbitManager>();


            var cacheSettings = configuration.GetSection("CacheSettings").Get<CacheSettings>();
            var rsettings = configuration.GetSection("RabbitMQSettings").Get<RabbitMQSettings>();

            if (cacheSettings.PreferRedis)
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = cacheSettings.RedisURL;
                    options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions()
                    {
                        AbortOnConnectFail = true,
                        EndPoints = { cacheSettings.RedisURL }
                    };
                });
                services.AddTransient<ICacheService, RedisCacheService>();
            }
            else
            {
                services.AddMemoryCache();
                services.AddTransient<ICacheService, MemoryCacheService>();
            }
        }

    }
}
