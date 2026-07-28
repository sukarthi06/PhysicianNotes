using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PhysicianNotes.Application.Blob;
using PhysicianNotes.Application.Common;
using PhysicianNotes.Domain.Common;
using PhysicianNotes.Domain.Notes;
using PhysicianNotes.Domain.Recording;
using PhysicianNotes.Domain.Transcripts;
using System.Text;
using System.Text.Json;

namespace PhysicianNotes.Infrastructure.Blob;

public sealed class AzureBlobTranscriptStorage(
    BlobServiceClient blobServiceClient,
    IOptions<AzureBlobStorageOptions> options,
    ILogger<AzureBlobTranscriptStorage> logger) : ITranscriptObjectStorage
{
    private readonly BlobContainerClient _recordingContainerClient 
        = blobServiceClient.GetBlobContainerClient(options.Value.SourceContainer);
    private readonly BlobContainerClient _clinicalTranscriptContainerClient
        = blobServiceClient.GetBlobContainerClient(options.Value.ClinicalContainer);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = null, // preserve "Text"/"Segments"/"Start"/"End" casing on write
        PropertyNameCaseInsensitive = true, // tolerate case mismatches on read (e.g. "text" vs "Text")
        Converters =
        {
            new StronglyTypedGuidIdConverterFactory()
        }
    };

    public async Task<RecordingTranscript?> DownloadTranscriptAsync(
        RecordingId recordingId,string path, CancellationToken cancellationToken)
    {
        var blobClient = _recordingContainerClient.GetBlobClient(path);
        try
        {
            if (!await blobClient.ExistsAsync(cancellationToken))
            {
                logger.LogWarning(
                    "Transcript blob not found for RecordingId: {RecordingId}.", recordingId);
                return null;
            }

            var response = await blobClient.DownloadContentAsync(cancellationToken);            
            return response.Value.Content.ToObjectFromJson<RecordingTranscript>(JsonOptions);

        }
        catch (JsonException ex)
        {
            logger.LogError(
                ex, "Failed to deserialize transcript JSON for RecordingId {RecordingId}", recordingId);
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex, "Failed to download transcript for RecordingId {RecordingId}", recordingId);
            return null;
        }
    }

    public async Task<bool> UploadPhysicianNoteAsync(PhysicianNote physicianNote, string path, CancellationToken ct)
    {
        try
        {
            if (!await _clinicalTranscriptContainerClient.ExistsAsync(ct))
                await _clinicalTranscriptContainerClient.CreateAsync();

            var json = JsonSerializer.Serialize(physicianNote, JsonOptions);
            var bytes = Encoding.UTF8.GetBytes(json);

            using var stream = new MemoryStream(bytes, writable: false);

            var blobClient = _clinicalTranscriptContainerClient.GetBlobClient(path);

            await blobClient.UploadAsync(
                stream,
                new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders { ContentType = "application/json" }
                },
                ct);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to upload Physician Note {PhysicianNoteId}", physicianNote.PhysicianNoteId);

            return false;
        }
    }
    public async Task<PhysicianNote?> DownloadPhysicianNoteAsync(
        PhysicianNoteId physicianNoteId, string path, CancellationToken ct)
    {
        var blobClient = _clinicalTranscriptContainerClient.GetBlobClient(path);
        try
        {
            if (!await blobClient.ExistsAsync(ct))
            {
                logger.LogWarning(
                    "Transcript blob not found for PhysicianNoteId: {PhysicianNoteId}.", physicianNoteId);
                return null;
            }

            var response = await blobClient.DownloadContentAsync(ct);            
            return response.Value.Content.ToObjectFromJson<PhysicianNote>(JsonOptions);

        }
        catch (JsonException ex)
        {
            logger.LogError(
                ex, "Failed to deserialize PhysicianNote JSON for PhysicianNoteId {PhysicianNoteId}", physicianNoteId);
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex, "Failed to download PhysicianNote for PhysicianNoteId {PhysicianNoteId}", physicianNoteId);
            return null;
        }
    }
}
