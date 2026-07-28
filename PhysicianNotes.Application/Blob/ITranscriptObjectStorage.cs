using PhysicianNotes.Domain.Common;
using PhysicianNotes.Domain.Notes;
using PhysicianNotes.Domain.Recording;
using PhysicianNotes.Domain.Transcripts;

namespace PhysicianNotes.Application.Blob;

public interface ITranscriptObjectStorage
{
    Task<RecordingTranscript?> DownloadTranscriptAsync(
        RecordingId recordingId,
        string path,
        CancellationToken ct);
    Task<bool> UploadPhysicianNoteAsync(
        PhysicianNote physicianNote,
        string path,
        CancellationToken ct);
    Task<PhysicianNote?> DownloadPhysicianNoteAsync(
        PhysicianNoteId physicianNoteId,
        string path,
        CancellationToken ct);

}
