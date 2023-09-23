using model.records;

namespace model.interfaces;

public interface IFirmService
{
    Task<Firm> GetFirm(Guid firmId);
    Task<Firm> CreateFirm(Firm firm);
    Task AssignFirmRoleToPerson(Guid firmId, Guid personId, string role);
    Task RemoveFirmRoleFromPerson(Guid firmId, Guid personId, string role);
}