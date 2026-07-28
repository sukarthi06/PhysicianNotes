using PhysicianNotes.Domain.Recording;

namespace PhysicianNotes.Application.Grpc;

public interface IPhyNoteRecordingGrpcClient
{
    Task<bool> SaveAsync(PhysicianNoteRecording noteRecording, CancellationToken cancellationToken);
    Task<PhysicianNoteRecording> GetByRecordingIdAsync(RecordingId recordingId, CancellationToken ct);
    Task<bool> IsPhysicianNoteReadyAsync(RecordingId recordingId, CancellationToken ct);
}
