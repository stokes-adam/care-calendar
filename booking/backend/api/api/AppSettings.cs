namespace api;

public class AppSettings
{
   public static string GetConnectionString()
   {
      var connectionString = "TODO: Get from AWS Secrets Manager";
      
      if (!string.IsNullOrEmpty(connectionString))
      {
         return connectionString;
      }
      
      connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
      if (string.IsNullOrEmpty(connectionString))
      {
         throw new Exception("CONNECTION_STRING environment variable not set");
      }
      return connectionString;
   }
   
   public static string GetEncryptionKey()
   {
      var encryptionKey = Environment.GetEnvironmentVariable("ENCRYPTION_KEY");
      if (string.IsNullOrEmpty(encryptionKey))
      {
         throw new Exception("ENCRYPTION_KEY environment variable not set");
      }
      return encryptionKey;
   }
}