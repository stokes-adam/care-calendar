namespace model.records;

public record Firm
(
    Guid Id,
    DateTime Created,
    DateTime Updated,
    DateTime? Deleted,
    Guid OwnerPersonId,
    string Name,
    string Address,
    string City
);
