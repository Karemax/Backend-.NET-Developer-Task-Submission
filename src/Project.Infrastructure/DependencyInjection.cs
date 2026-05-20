using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Project.Application.Abstractions.Authentication;
using Project.Application.Abstractions.Caching;
using Project.Application.Abstractions.Data;
using Project.Infrastructure.Authentication;
using Project.Infrastructure.Caching;
using Project.Infrastructure.Data;
using Project.Infrastructure.Data.Repositories;

namespace Project.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. Database Setup
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Database")));

        // 2. Authentication & Authorization Setup
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtOptions.SecretKey ?? "DefaultSecretKeyThatIsAtLeast32BytesLongForSecurity Purposes"))
            };
        });

        services.AddAuthorization();

        // 3. Authentication Services
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        // 4. Repositories & Unit of Work
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITaskItemRepository, TaskItemRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // 5. In-memory caching
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, MemoryCacheService>();

        return services;
    }
}
