using ClinicalGrpcService.Grpc.Protos;
using PhysicianNotes.Domain.Common;
using PhysicianNotes.Domain.Recording;
using Riok.Mapperly.Abstractions;

namespace PhysicianNotes.Infrastructure.Data.Mappers;

[Mapper]
public partial class PhysicianNoteRecordingMapper : MapperBase
{    
    public partial PhysicianNoteRecording ToDomain(PhysicianNoteRecordingDto dto);    
    public partial PhysicianNoteRecordingDto ToDto(PhysicianNoteRecording entity);

    // ---- RecordingId (string <-> value object) ----
    public RecordingId MapRecordingId(string id) => RecordingId.Of(ParseGuid(id));
    public RecordingId MapRecordingId(Guid id) => RecordingId.Of(id);
    public string MapRecordingId(RecordingId id) => id.Value.ToString();

    // ---- PhysicianNoteId (string <-> value object) ----
    private PhysicianNoteId MapToPhysicianNoteId(string id) => PhysicianNoteId.Of(ParseGuid(id));
    public string MapPhysicianNoteId(PhysicianNoteId id) => id.Value.ToString();
}
