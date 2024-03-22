using model.records;

namespace model.interfaces;

public interface IPersonCommandService
{
    Task<Person> CreatePerson(Person person);
    Task<Person> UpdatePerson(Person person);
    Task DeletePerson(Guid personId);
    Task DeletePersonalDetails(Guid personId);
}