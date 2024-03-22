using model.records;

namespace model.interfaces;

public interface IFirmCommandService
{
    Task<Firm> CreateFirm(Firm firm);
    Task AssignFirmRoleToPerson(Guid firmId, Guid personId, string role);
    Task RemoveFirmRoleFromPerson(Guid firmId, Guid personId, string role);
}