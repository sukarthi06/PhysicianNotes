using PhysicianNotes.Domain.Common;

namespace PhysicianNotes.Domain.Notes;

public class PhysicianNote
{
    public PhysicianNoteId PhysicianNoteId { get; set; } = PhysicianNoteId.Of(Guid.NewGuid());
    public SoapNote SoapNote { get; set; } = new();
}