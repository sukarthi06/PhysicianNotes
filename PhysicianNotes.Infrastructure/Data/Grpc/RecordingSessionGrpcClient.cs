using PhysicianNotes.Application.Grpc;
using PhysicianNotes.Domain.Recording;
using RecordingGrpcService.Grpc.Protos;

namespace PhysicianNotes.Infrastructure.Data.Grpc;

internal class RecordingSessionGrpcClient(
    RecordingService.RecordingServiceClient grpcClient) : IRecordingSessionGrpcClient
{    
    public async Task<string> GetTranscriptPathAsync(RecordingId recordingId, CancellationToken ct)
    {
        var response = await grpcClient.GetTranscriptPathAsync(
            new GetTranscriptPathRequest { RecordingId = recordingId.Value.ToString() },
            cancellationToken: ct);
        return response.TranscriptPath;
    }
}
