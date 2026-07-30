using PhysicianNotes.Api.Handlers;

namespace PhysicianNotes.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddAppServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        #region "CORS"
        var corsOrigins = configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string>() ?? string.Empty;

        var allowedClientOrigins = corsOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);


        services.AddCors(options =>
        {
            options.AddPolicy("AllowedClientOrigins", policy =>
            {
                policy.WithOrigins(allowedClientOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });

        #endregion
        services.AddScoped<PhysicianNoteHandler>();
        services.AddScoped<GetPhysicianNoteHandler>();

        return services;
    }
}
