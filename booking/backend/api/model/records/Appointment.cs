namespace model.records;

public record Appointment
(
    Guid Id,
    DateTime Created,
    DateTime Updated,
    DateTime? Deleted,
    Guid ClientId,
    Guid ConsultantId, 
    DateTime StartTime,
    DateTime EndTime, 
    string? Notes
) : Entity(Id, Created, Updated, Deleted);