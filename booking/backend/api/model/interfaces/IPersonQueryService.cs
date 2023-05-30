using model;

namespace service;

public interface IPersonQueryService
{
    Task<IEnumerable<Person>> GetPersonalDetails(Guid personId);
}