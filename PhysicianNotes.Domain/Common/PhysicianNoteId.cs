namespace PhysicianNotes.Domain.Common;

public record PhysicianNoteId
{
    public Guid Value { get; }
    private PhysicianNoteId(Guid value) => Value = value;

    public static PhysicianNoteId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("PhysicianNoteId cannot be empty.", nameof(value));
        }
        return new PhysicianNoteId(value);
    }

    public static bool Empty(Guid value)
    {
        return value == Guid.Empty;
    }
}
