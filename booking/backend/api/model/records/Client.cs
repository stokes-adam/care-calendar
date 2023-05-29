namespace model;

public record Client
(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Phone
);