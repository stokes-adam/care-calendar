﻿namespace model;

public record Person
(
    Guid Id,
    DateTime Created,
    DateTime Updated,
    DateTime? Deleted
) : Entity(Id, Created, Updated, Deleted);