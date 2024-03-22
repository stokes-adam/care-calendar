using Amazon;

namespace service;

public record Configuration2(string EncryptionKeyId, RegionEndpoint Region, string ConnectionString);