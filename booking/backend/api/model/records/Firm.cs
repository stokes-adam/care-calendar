namespace model;

public record Firm
(
    Guid Id,
    Guid OwnerPersonId,
    string Name
);
