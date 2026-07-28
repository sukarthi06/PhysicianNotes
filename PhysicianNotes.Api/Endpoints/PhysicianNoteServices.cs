using Microsoft.AspNetCore.Mvc;
using PhysicianNotes.Api.Handlers;
using PhysicianNotes.Application.Grpc;
using PhysicianNotes.Infrastructure.Data.Mappers;

namespace PhysicianNotes.Api.Endpoints;

public static class PhysicianNoteServices
{
    public static void MapPhysicianNoteServicesEndpoints(this WebApplication app)
    {
        var serviceGroup = app.MapGroup("/api/physiciannoteservice")
            .WithTags("Physician Note Service Endpoints");

        serviceGroup.MapPost("/physician-note", CreateNotesAsync)
            .WithSummary("Creates a physician note")
            .WithDescription("Creates a new physician note with the provided recording Id.")            
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        serviceGroup.MapGet("/physician-note/{recordingId:Guid}", GetNoteAsync)
            .WithSummary("Fetches a physician note")
            .WithDescription("Fetches a new physician note with the given recording Id.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        serviceGroup.MapGet("/physician-note/{recordingId:Guid}/status", IsPhysicianNoteReadyAsync)
            .WithSummary("Checks a physician note is prepared or not.")
            .WithDescription("Checks a physician note is prepared or not for the given recording Id.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> CreateNotesAsync(
        [FromBody]Guid recordingId,
        PhysicianNoteRecordingMapper mapper,
        PhysicianNoteHandler handler,
        CancellationToken ct)
    {
        var response = await handler.HandleAsync(mapper.MapRecordingId(recordingId), ct);
        return Results.Ok(response);
    }

    private static async Task<IResult> GetNoteAsync(
        [FromRoute]Guid recordingId,
        PhysicianNoteRecordingMapper mapper,
        GetPhysicianNoteHandler handler,
        CancellationToken ct)
    {
        var response = await handler.HandleAsync(mapper.MapRecordingId(recordingId), ct);
        return response is null
            ? Results.NotFound($"Physician note not found for RecordingId: {recordingId}")
            : Results.Ok(response);
    }

    private static async Task<IResult> IsPhysicianNoteReadyAsync(
        [FromRoute] Guid recordingId,
        PhysicianNoteRecordingMapper mapper,
        IPhyNoteRecordingGrpcClient grpcClient,
        CancellationToken ct)
    {
        var resposne = await grpcClient.IsPhysicianNoteReadyAsync(mapper.MapRecordingId(recordingId), ct);
        return Results.Ok(resposne);
    }
}
