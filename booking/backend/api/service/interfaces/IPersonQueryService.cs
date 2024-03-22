using model.records;

namespace model.interfaces;

public interface IPersonQueryService
{
    Task<Person> GetPerson(Guid personId);
    Task<IEnumerable<Person>> GetClientsForFirm(Guid firmId);
    Task<IEnumerable<Person>> GetConsultantsForFirm(Guid firmId);
}