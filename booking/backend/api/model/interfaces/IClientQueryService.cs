using model;

namespace service;

public interface IClientQueryService
{
    Task<IEnumerable<Client>> GetClientsForFirm(Guid firmId);
    Task<Client> GetClient(Guid id);
}