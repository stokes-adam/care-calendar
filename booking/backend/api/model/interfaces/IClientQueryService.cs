using model;

namespace service;

public interface IClientQueryService
{
    Task<IEnumerable<PersonalDetails>> GetClientsPersonalDetailsForFirm(Guid firmId);
}