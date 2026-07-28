namespace PhysicianNotes.Infrastructure.Blob;

public sealed class AzureBlobStorageOptions
{
    public const string SectionName = "Azure";
    public required string SourceContainer { get; init; }
    public required string ClinicalContainer { get; init; }
}
