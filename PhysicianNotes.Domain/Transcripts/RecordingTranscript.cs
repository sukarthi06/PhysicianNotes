using PhysicianNotes.Domain.Recording;

namespace PhysicianNotes.Domain.Transcripts;

public class RecordingTranscript
{
    public RecordingId RecordingId { get; set; } = default!;
    public string Text { get; set; } = string.Empty;
}
