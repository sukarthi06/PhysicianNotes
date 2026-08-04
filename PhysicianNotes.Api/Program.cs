using PhysicianNotes.Api;
using PhysicianNotes.Api.Endpoints;
using PhysicianNotes.Infrastructure.DependencyInjection;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.AddHostInfrastructure(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAppServices(builder.Configuration);

//#region "CORS"
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAngularClient", policy =>
//    {
//        policy.WithOrigins("http://localhost:4200") // your Angular dev server
//              .AllowAnyMethod()
//              .AllowAnyHeader()
//              .AllowCredentials(); // needed if you send cookies/auth headers
//    });
//});
//#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Local"))
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseSwagger();
    app.UseSwaggerUI();    
}
app.UseCors("AllowedClientOrigins");

//if (!app.Environment.IsDevelopment() || !app.Environment.IsEnvironment("Local"))
//{
//    app.UseHttpsRedirection();
//}



//app.MapPost(
//    "/clinical-facts",
//    async (
//        Transcript transcript,
//        IClinicalFactsExtractor extractor,
//        CancellationToken cancellationToken) =>
//    {
//        var clinicalFacts =
//            await extractor.ExtractAsync(
//                transcript,
//                cancellationToken);

//        return Results.Ok(clinicalFacts);
//    });

//app.MapPost(
//    "/soap-note",
//    async (
//        ClinicalFacts clinicalFacts,
//        ISoapNoteGenerator generator,
//        CancellationToken cancellationToken) =>
//    {
//        var soapNote =
//            await generator.GenerateAsync(
//                clinicalFacts,
//                cancellationToken);

//        return Results.Ok(soapNote);
//    });

//app.MapPost(
//    "/physician-note",
//    async (
//        Transcript transcript,
//        IClinicalFactsExtractor extractor,
//        ISoapNoteGenerator generator,
//        CancellationToken cancellationToken) =>
//    {
//        var clinicalFacts =
//            await extractor.ExtractAsync(
//                transcript,
//                cancellationToken);

//        var soapNote =
//            await generator.GenerateAsync(
//                clinicalFacts,
//                cancellationToken);

//        return Results.Ok(soapNote);
//    });

app.MapGet("/health", () => Results.Ok());

app.MapPhysicianNoteServicesEndpoints();

app.Run();
