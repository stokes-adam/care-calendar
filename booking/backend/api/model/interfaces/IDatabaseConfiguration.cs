using model;

namespace service;

public interface IDatabaseConfiguration
{
    string ConnectionString { get; }
}