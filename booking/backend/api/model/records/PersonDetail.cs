namespace model.records;

public record PersonDetail
(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Phone
);