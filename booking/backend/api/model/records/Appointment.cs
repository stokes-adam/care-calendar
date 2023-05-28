namespace model;

public record Appointment
(
    Guid Id,
    Guid ClientId,
    Guid FirmId,
    Guid ConsultantId, 
    DateTime StartTime,
    DateTime EndTime, 
    string? Notes
);