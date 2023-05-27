namespace model;

public record PersonalDetails
(
    Guid Id,
    Guid PersonId,
    string FirstName,
    string LastName,
    string Email,
    string Phone
);