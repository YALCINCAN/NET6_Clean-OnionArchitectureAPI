using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using EasyCaching.Core.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Context;
using Persistence.Repositories;
using Persistence.Services;
using Persistence.Settings;

namespace Persistence
{
    public static class InfrastructureRegistration
    {
        public static void AddInfrastructreServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CAContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("PostgreSqlConnection"));
            });

            services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.Configure<CacheSettings>(configuration.GetSection("CacheSettings"));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITokenService, TokenService>();

           

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