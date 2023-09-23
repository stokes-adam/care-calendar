namespace model.records;

public record Appointment
(
    Guid Id,
    Guid ClientId,
    Guid ConsultantId, 
    DateTime StartTime,
    DateTime EndTime, 
    string? Notes
);