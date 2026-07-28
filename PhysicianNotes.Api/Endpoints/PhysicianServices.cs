namespace PhysicianNotes.Api.Endpoints;

public static class PhysicianServices
{
    public static void MapPhysicianServicesEndpoints(this WebApplication app)
    {
        var orderGroup = app.MapGroup("/api/physicianervices")
            .WithTags("Physician Service Endpoints");
    }


}
