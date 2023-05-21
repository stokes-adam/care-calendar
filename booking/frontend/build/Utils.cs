public class Utils
{
    public static void CleanDirectory(string path)
    {
        if (!Directory.Exists(path)) return;

        var filesToDelete = Directory
            .GetFiles(path, "*.*", SearchOption.AllDirectories)
            .Where(file => !file.Contains(".gitignore"));
        foreach (var file in filesToDelete)
        {
            Console.WriteLine($"Deleting {file}");
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        var directoriesToDelete = Directory.GetDirectories(path);
        foreach (var directory in directoriesToDelete)
        {
            Console.WriteLine($"Deleting {directory}");
            Directory.Delete(directory, true);
        }
    }

    public static void CopyDirectory(string fromDir, string toDir)
    {
       if (!Directory.Exists(fromDir)) return;
       
         var filesToCopy = Directory
              .GetFiles(fromDir, "*.*", SearchOption.AllDirectories)
              .Where(file => !file.Contains(".gitignore"));

         foreach (var file in filesToCopy)
         {
             var relativePath = file.Replace(fromDir, "");
             if (relativePath.StartsWith(Path.DirectorySeparatorChar))
             {
                 relativePath = relativePath[1..];
             }

             var destination = Path.Combine(toDir, relativePath);

             var destinationDirectory = Path.GetDirectoryName(destination);

             if (!Directory.Exists(destinationDirectory))
             {
                 Console.WriteLine($"Creating directory {destinationDirectory}");
                 Directory.CreateDirectory(destinationDirectory);
             }

             Console.WriteLine($"Copying {file} to {destination}");
             File.Copy(file, destination);
         }
    }
}