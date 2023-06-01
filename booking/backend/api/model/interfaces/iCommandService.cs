using model.records;

namespace model.interfaces;

public interface ICommandService
{
    Task CreateClient(Person person);
    Task CreateConsultant(Person person);
    Task CreateUser(Person person);
    Task CreateAdmin(Person person);
    Task CreateFirm(Firm firm);
}