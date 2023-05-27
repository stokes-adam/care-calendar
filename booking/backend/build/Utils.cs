public static class Utils
{
    public static void CleanDirectory(string path){
        if (!Directory.Exists(path)) return;

        var filesToDelete = Directory
            .GetFiles(path, "*.*", SearchOption.AllDirectories)
            .Where(file => !file.Contains(".gitignore"));
        foreach(var file in filesToDelete){
            Console.WriteLine($"Deleting {file}");
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        var directoriesToDelete = Directory.GetDirectories(path);
        foreach(var directory in directoriesToDelete){
            Console.WriteLine($"Deleting {directory}");
            Directory.Delete(directory, true);
        }
    }
}