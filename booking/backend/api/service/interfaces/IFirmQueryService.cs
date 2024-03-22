using model.records;

namespace model.interfaces;

public interface IFirmQueryService
{
    Task<Firm> GetFirm(Guid firmId);
}