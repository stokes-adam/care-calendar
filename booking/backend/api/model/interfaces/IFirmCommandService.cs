using model.enums;
using model.records;

namespace model.interfaces;

public interface IFirmCommandService
{
    Task<Firm> CreateFirm(Firm firm);
    Task AssignRoleToPerson(Guid firmId, Guid personId, Role role);
}