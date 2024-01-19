using model.dtos;

namespace model.interfaces;

public interface IPersonService
{
    Task<Guid> CreatePerson();
    Task<Guid> CreatePersonDetail(Guid personId, CreatePersonDetail personDetail);
}