using PhysicianNotes.Domain.Common;

namespace PhysicianNotes.Domain.Recording;

public class PhysicianNoteRecording
{
    public RecordingId RecordingId { get; set; } = RecordingId.Of(Guid.NewGuid());
    public PhysicianNoteId PhysicianNoteId { get; set; } = PhysicianNoteId.Of(Guid.NewGuid());
    public string StoragePath { get; set; } = default!;
}
