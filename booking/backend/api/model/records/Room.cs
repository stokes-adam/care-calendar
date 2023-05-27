namespace model;

public record Room
(
    Guid Id,
    bool Active,
    string Name,
    string Description
);