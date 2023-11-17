namespace model.records;

public record Role(
    Guid Id,
    DateTime Created,
    DateTime Updated,
    DateTime? Deleted,
    string Name
);