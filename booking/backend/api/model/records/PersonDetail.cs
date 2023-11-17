namespace model.records;

public record PersonDetail
(
    Guid Id,
    DateTime Created,
    DateTime Updated,
    DateTime? Deleted,
    string FirstName,
    string LastName,
    string Email,
    string Phone
);