using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace api;

public class AppSettings
{
   private readonly IAmazonSecretsManager _secretsManager = new AmazonSecretsManagerClient(RegionEndpoint.USEast1);
   public string GetConnectionString()
   {
      var connectionString = GetSecret("production/database/connectionString");
      
      return !string.IsNullOrEmpty(connectionString)
         ? connectionString
         : GetSetting("CONNECTION_STRING");
   }
   
   public string GetEncryptionKey()
   {
      return GetSetting("ENCRYPTION_KEY");
   }

   private string GetSetting(string settingName, string? defaultValue = null)
   {
      var settingValue = defaultValue ?? Environment.GetEnvironmentVariable(settingName);
      
      if (string.IsNullOrEmpty(settingValue))
      {
         throw new Exception($"{settingName} environment variable not set");
      }
      
      return settingValue;
   }

   private string GetSecret(string secretName)
   {
      try
      {
         var request = new GetSecretValueRequest
         {
            SecretId = secretName,
         };
         
         var response = _secretsManager.GetSecretValueAsync(request).Result;
         
         return response.SecretString;
      }
      catch (Exception e)
      {
         Console.WriteLine(e);
         throw;
      }
   }
}