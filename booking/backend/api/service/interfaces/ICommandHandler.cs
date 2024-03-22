using model.records;

namespace model.interfaces;

public interface ICommandHandler
{
    Task RegisterFirm(Firm firm);
}