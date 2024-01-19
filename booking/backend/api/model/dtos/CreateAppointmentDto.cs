namespace model.dtos;

public record CreateAppointmentDto
(
    Guid RoomId,
    Guid ClientId,
    Guid ConsultantId,
    DateTime StartTime,
    DateTime EndTime
);
