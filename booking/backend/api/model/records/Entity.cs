namespace model.records;

public abstract record Entity
(
    Guid Id,
    DateTime Created,
    DateTime Updated,
    DateTime? Deleted
);