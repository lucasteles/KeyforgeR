namespace KeyforgeR.CardDataScrapper;

public class Util
{
    public static async ValueTask DownloadImage(HttpClient client, string directoryPath, string fileName, Uri uri)
    {
        var uriWithoutQuery = uri.GetLeftPart(UriPartial.Path);
        var fileExtension = Path.GetExtension(uriWithoutQuery);
        var path = Path.Combine(directoryPath, $"{fileName}{fileExtension}");
        
        var response = await client.GetAsync(uri);
        if (!response.IsSuccessStatusCode)
        {
            Console.Error.WriteLine($"*** Cant donwload image in '{uri}' gives {response.StatusCode}");
        }

        var imageBytes = await response.Content.ReadAsByteArrayAsync();
        await File.WriteAllBytesAsync(path, imageBytes);
    }
}