using model;

namespace service;

public interface IFirmQueryService
{
    Task<IEnumerable<Firm>> GetForOwnerPersonWithEmail(string email);
}