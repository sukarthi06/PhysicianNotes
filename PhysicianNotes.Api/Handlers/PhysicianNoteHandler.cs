using PhysicianNotes.Application.Blob;
using PhysicianNotes.Application.Grpc;
using PhysicianNotes.Application.UseCases.GeneratePhysicianNote;
using PhysicianNotes.Domain.Common;
using PhysicianNotes.Domain.Notes;
using PhysicianNotes.Domain.Recording;
using PhysicianNotes.Domain.Transcripts;

namespace PhysicianNotes.Api.Handlers;

public class PhysicianNoteHandler(
    IRecordingSessionGrpcClient recordingGrpcClient,
    IPhyNoteRecordingGrpcClient phyNoteGrpcClient,
    ITranscriptObjectStorage transcriptObjectStorage,
    IClinicalFactsExtractor extractor,
    ISoapNoteGenerator generator,
    ILogger<PhysicianNoteHandler> logger)
{
    public async Task<bool> HandleAsync(RecordingId recordingId, CancellationToken ct)
    {
        var transcriptPath = await recordingGrpcClient.GetTranscriptPathAsync(recordingId, ct);
        if (string.IsNullOrEmpty(transcriptPath))
        {
            logger.LogWarning("Transcript Path is not found for RecordingId: {RecordingId}", recordingId);
            return false;
        }

        var recordingTranscript = await transcriptObjectStorage.DownloadTranscriptAsync(recordingId, transcriptPath, ct);
        if(recordingTranscript is null)
            return false;

        var transcript = new Transcript(recordingTranscript.Text);

        var clinicalFacts =
            await extractor.ExtractAsync(
                transcript,
                ct);

        var soapNote =
            await generator.GenerateAsync(
                clinicalFacts,
                ct);

        var physicianNoteId = PhysicianNoteId.Of(Guid.NewGuid());
        var physicianNote = new PhysicianNote
        {
            PhysicianNoteId = physicianNoteId,
            SoapNote = soapNote
        };

        var destinationPath = $"{DateTime.UtcNow:yyyy-MM-dd}/{physicianNoteId.Value}.json";

        var response = 
            await transcriptObjectStorage.UploadPhysicianNoteAsync(physicianNote, destinationPath, ct);
        if (!response)
        {
            logger.LogWarning("Failed to upload Physician Note for RecordingId: {RecordingId}", recordingId);
            return false;
        }

        var physicianNoteRecording = new PhysicianNoteRecording
        {
            PhysicianNoteId = physicianNoteId,
            RecordingId = recordingId,
            StoragePath = destinationPath
        };
        response = await phyNoteGrpcClient.SaveAsync(physicianNoteRecording, ct);
        if (!response)
        {
            logger.LogWarning("Failed to save Physician Note Recording for RecordingId: {RecordingId}", recordingId);
            return false;
        }

        logger.LogInformation("Physician Note: {PhysicianNoteId} generated and stored for RecordingId: {RecordingId}"
            , physicianNoteId, recordingId);

        return true;
    }
}
