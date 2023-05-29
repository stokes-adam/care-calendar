namespace model;

public record PersonalDetails
(
    Guid Id,
    DateTime Created,
    DateTime Updated,
    DateTime? Deleted,
    Guid PersonId,
    string FirstName,
    string LastName,
    string Email,
    string Phone
) : Entity(Id, Created, Updated, Deleted);