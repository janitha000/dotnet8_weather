using System.Text;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration config)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddControllers( options =>
                {
                    options.Filters.Add<LogActionFilter>();
                }
            );

            services.AddScoped<ICityService, CityService>();
            services.AddScoped<IJWTService, JWTService>();
            services.AddScoped<LogActionFilter>();
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddHostedService<OutboxDispatcher>();

            services.AddTransient<ICityNormalizer, CityNormalizer>();


            services.AddMemoryCache();
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            services.Configure<CacheOptions>(
                config.GetSection(CacheOptions.SectionName));
            services.Configure<JWTOptions>(
                config.GetSection(JWTOptions.SectionName));

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(config.GetConnectionString("Default")));

            return services;
        }

        public static IServiceCollection AddJWTAuth(
            this IServiceCollection services,
        IConfiguration config)
        {
            var jwt = config.GetSection(JWTOptions.SectionName).Get<JWTOptions>()!;
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwt.Issuer,
                        ValidAudience = jwt.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwt.Key))
                    };
                });

            services.AddAuthorization(options => 
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
            });
            return services;
        }


        public static IServiceCollection AddCors(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalOrigins", builder =>
                    {
                        builder.WithOrigins(
                            "http://localhost:5173"    // Vite default, if you use it
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });
            return services;
        }


}