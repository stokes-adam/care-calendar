using model;

namespace service;

public interface IClientRepository
{
    Task<IEnumerable<Client>> GetClientsForFirm(Guid firmId);
    Task<Client> GetClient(Guid id);
    Task<Client> AddClient(Client client);
    Task<Client> UpdateClient(Client client);
    Task<Client> DeleteClient(Guid id);
}