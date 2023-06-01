namespace model.records;

public record Firm
(
    Guid Id,
    Guid OwnerPersonId,
    string Name
);
