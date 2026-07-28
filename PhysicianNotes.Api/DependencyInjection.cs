using PhysicianNotes.Api.Handlers;

namespace PhysicianNotes.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddAppServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<PhysicianNoteHandler>();
        services.AddScoped<GetPhysicianNoteHandler>();

        return services;
    }
}
