using System.Text.Json;
using Pulumi;
using Pulumi.Aws.SecretsManager;

namespace infra;

public class SecretProperties
{
    private readonly Output<GetSecretVersionResult> _secretString;

    public SecretProperties(Output<GetSecretVersionResult> secretString)
    {
        _secretString = secretString;
    }

    public Output<string> GetString(string propertyName)
    {
        return _secretString.Apply(s => JsonDocument.Parse(s.SecretString).RootElement.GetProperty(propertyName).GetString())!;
    }

    public Output<int> GetInt(string propertyName)
    {
        return _secretString.Apply(s => JsonDocument.Parse(s.SecretString).RootElement.GetProperty(propertyName).GetInt32())!;
    }
}