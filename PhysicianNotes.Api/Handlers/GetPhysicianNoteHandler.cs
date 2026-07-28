using PhysicianNotes.Application.Blob;
using PhysicianNotes.Application.Grpc;
using PhysicianNotes.Domain.Notes;
using PhysicianNotes.Domain.Recording;

namespace PhysicianNotes.Api.Handlers;

public class GetPhysicianNoteHandler(
    IPhyNoteRecordingGrpcClient phyNoteGrpcClient,
    ITranscriptObjectStorage transcriptObjectStorage,
    ILogger<GetPhysicianNoteHandler> logger)
{
    public async Task<PhysicianNote?> HandleAsync(RecordingId recordingId, CancellationToken ct)
    {
        var physicianNoteRecording = await phyNoteGrpcClient.GetByRecordingIdAsync(recordingId, ct);
        if (string.IsNullOrEmpty(physicianNoteRecording.StoragePath))
        {
            logger.LogWarning("Physician note recording storage path is not found for RecordingId: {RecordingId}", recordingId);
            return null;
        }

        var physicianNote = await transcriptObjectStorage.DownloadPhysicianNoteAsync(
            physicianNoteId: physicianNoteRecording.PhysicianNoteId,
            path: physicianNoteRecording.StoragePath,
            ct: ct);

        if (physicianNote == null)
        {
            logger.LogWarning("Physician note is not found for RecordingId: {RecordingId}", recordingId);
            return null;
        }
        
        return physicianNote;
    }
}
