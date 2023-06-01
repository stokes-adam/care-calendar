namespace model.records;

public record Person
(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Phone
);