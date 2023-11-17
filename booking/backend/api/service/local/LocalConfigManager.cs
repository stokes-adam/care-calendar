using Amazon;
using model.interfaces;

namespace service;

public class LocalConfigManager : IConfigManager
{
    public string ConnectionString { get; } = Environment.GetEnvironmentVariable("ConnectionString")
                                              ?? throw new Exception("ConnectionString not set");
}