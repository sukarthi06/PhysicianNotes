using PhysicianNotes.Domain.Recording;

namespace PhysicianNotes.Application.Grpc;

public interface IRecordingSessionGrpcClient
{
    Task<string> GetTranscriptPathAsync(RecordingId recordingId, CancellationToken ct);    
}
