using Azure.Storage.Blobs;
using ClinicalGrpcService.Grpc.Protos;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI;
using PhysicianNotes.Application.Blob;
using PhysicianNotes.Application.Grpc;
using PhysicianNotes.Application.UseCases.GeneratePhysicianNote;
using PhysicianNotes.Infrastructure.Ai;
using PhysicianNotes.Infrastructure.Blob;
using PhysicianNotes.Infrastructure.Data.Grpc;
using PhysicianNotes.Infrastructure.Data.Mappers;
using RecordingGrpcService.Grpc.Protos;
using Serilog;
using System.ClientModel;

namespace PhysicianNotes.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IChatClient>(_ =>
        {
            var apiKey = configuration["OpenAI:ApiKey"]!;
            var model = configuration["OpenAI:Model"]!;

            var credential = new ApiKeyCredential(apiKey);

            var options = new OpenAIClientOptions
            {
                Endpoint = new Uri(configuration["OpenAI:Endpoint"]!)
            };

            return new OpenAIClient(credential, options)
                .GetChatClient(model)
                .AsIChatClient();
        });

        #region "Grpc"

        services.AddGrpcClient<RecordingService.RecordingServiceClient>(o =>
        {
            o.Address = new Uri(configuration["RecordingGrpcService:Address"]!);
        });
        services.AddScoped<IRecordingSessionGrpcClient, RecordingSessionGrpcClient>();

        services.AddGrpcClient<PhysiciansNoteRecording.PhysiciansNoteRecordingClient>(o =>
        {
            o.Address = new Uri(configuration["ClinicalGrpcService:Address"]!);
        });
        services.AddSingleton<PhysicianNoteRecordingMapper>();
        services.AddScoped<IPhyNoteRecordingGrpcClient, PhyNoteRecordingGrpcClient>();

        #endregion

        #region "Blob"

        services.Configure<AzureBlobStorageOptions>(
           configuration.GetSection(AzureBlobStorageOptions.SectionName));
        services.AddSingleton<BlobServiceClient>(sp =>
            new BlobServiceClient(configuration["Azure:StorageConnectionString"]));

        services.AddSingleton<ITranscriptObjectStorage, AzureBlobTranscriptStorage>();

        #endregion

        services.AddScoped<IClinicalFactsExtractor, OpenAiClinicalFactsExtractor>();
        services.AddScoped<ISoapNoteGenerator, OpenAiSoapNoteGenerator>();
        
        return services;
    }

    public static void AddHostInfrastructure(
        this IHostBuilder hostBuilder,
        IConfiguration configuration)
    {
        hostBuilder.UseSerilog((context, services, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext();
        });
    }
}
