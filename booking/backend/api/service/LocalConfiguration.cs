using Amazon;
using model.interfaces;

namespace service;

public class LocalConfiguration : IConfiguration
{
    public string ConnectionString { get; } = Environment.GetEnvironmentVariable("ConnectionString")
                                              ?? throw new Exception("ConnectionString not set");
    public RegionEndpoint Region { get; } = RegionEndpoint.USEast1;
}