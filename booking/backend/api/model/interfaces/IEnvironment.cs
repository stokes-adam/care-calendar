using Amazon;

namespace model.interfaces;

public interface IEnvironment
{
    string ConnectionString { get; }
    RegionEndpoint Region { get; }
}