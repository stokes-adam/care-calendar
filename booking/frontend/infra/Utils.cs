public class Utils
{
    public static string GetContentType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".html" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".svg" => "image/svg+xml",
            ".json" => "application/json",
            ".woff" => "application/font-woff",
            ".woff2" => "application/font-woff2",
            ".ttf" => "application/font-ttf",
            ".eot" => "application/vnd.ms-fontobject",
            ".otf" => "application/font-otf",
            ".ico" => "image/x-icon",
            _ => "application/octet-stream",
        };
    } 
}