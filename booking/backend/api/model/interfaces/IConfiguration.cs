using Amazon;

namespace model.interfaces;

public interface IConfiguration
{
    string ConnectionString { get; }
    RegionEndpoint Region { get; }
    string EncryptionKeyId { get; }
}