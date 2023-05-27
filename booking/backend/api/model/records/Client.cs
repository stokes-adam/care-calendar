namespace model;

public record Client
(
    Guid Id,
    Guid PersonId,
    Guid FirmId,
    bool Active
);