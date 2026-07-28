using ClinicalGrpcService.Grpc.Protos;
using PhysicianNotes.Application.Grpc;
using PhysicianNotes.Domain.Recording;
using PhysicianNotes.Infrastructure.Data.Mappers;

namespace PhysicianNotes.Infrastructure.Data.Grpc;

public class PhyNoteRecordingGrpcClient(
    PhysiciansNoteRecording.PhysiciansNoteRecordingClient grpcClient,
    PhysicianNoteRecordingMapper mapper) : IPhyNoteRecordingGrpcClient
{
    public async Task<PhysicianNoteRecording> GetByRecordingIdAsync(RecordingId recordingId, CancellationToken ct)
    {
        var response = await grpcClient.GetByRecordingIdAsync(
            new GetByRecordingIdRequest { RecordingId = mapper.MapRecordingId(recordingId) },
            cancellationToken:ct);

        return mapper.ToDomain(response.PhysicianNoteEcording);
    }

    public async Task<bool> IsPhysicianNoteReadyAsync(RecordingId recordingId, CancellationToken ct)
    {
        var response = await grpcClient.IsReadyAsync(
            new IsReadyRequest { RecordingId = mapper.MapRecordingId(recordingId)},
            cancellationToken:ct);
        return response.IsSuccess;
    }

    public async Task<bool> SaveAsync(PhysicianNoteRecording noteRecording, CancellationToken cancellationToken)
    {
        var response = await grpcClient.SaveAsync(
            new PhysicianNoteRecordingRequest { PhysicianNoteEcording = mapper.ToDto(noteRecording) },
            cancellationToken: cancellationToken);
        return response.IsSuccess;
    }
}
