using HtmlAgilityPack;

namespace KeyforgeR.CardDataScrapper;

enum Expansions
{
    CalloftheArchons = 341,
    AgeofAscension = 435,
    WorldsCollide = 452,
    MassMutation = 479,
    DarkTidings = 496,
    Anomaly = 453
}

public class ArchonArcanaImages
{
    public static async Task DownloadAll(Card[] cards)
    {
        using var httpClient = new HttpClient();
        await Parallel.ForEachAsync(cards, 
             new ParallelOptions { MaxDegreeOfParallelism = 10 },
            async (card, _) => 
            await DownloadFromArchonarcana(httpClient, card));
    }

    static async Task DownloadFromArchonarcana(HttpClient httpClient, Card card)
    {
        //var url = "https://archonarcana.com/File:479-001-Dis.png";
        //var url = "https://archonarcana.com/File:496-385.png";
        var baseUrl = $"https://archonarcana.com/File:{card.expansion}-{card.cardNumber}";
        var fileName = $"{card.expansion}-{card.cardNumber}";
        if (card.houses.Count != 1)
        {
            foreach (var house in card.houses)
            {
                Console.WriteLine($"-> {card.expansion}: #{card.cardNumber} of {house} - {card.cardTitle}");
                await ScrapImg(httpClient, $"{baseUrl}-{house}.png", $"{fileName}-{house}");
            }
        }
        else
        {
            Console.WriteLine($"-> {card.expansion}: #{card.cardNumber} - {card.cardTitle}");
            await ScrapImg(httpClient, baseUrl + ".png", $"{fileName}-{card.houses.First()}");
        }
    }

    static async Task ScrapImg(HttpClient httpClient, string url, string fileName)
    {
        
        if (File.Exists(Path.Combine("images", fileName.Replace("StarAlliance","Star_Alliance") + ".png")))
        {
            Console.WriteLine($"Skip: {fileName}");
            return;
        }
        
        var response = await httpClient.GetAsync(url);
        
        if (!response.IsSuccessStatusCode)
        {
            Console.Error.WriteLine($"*** Cant access '{url}', gives {response.StatusCode}");
            return;
        }

        var content = await response.Content.ReadAsStringAsync();
        var doc = new HtmlDocument();
        doc.LoadHtml(content);
        var div = doc.DocumentNode.SelectSingleNode("//div[@class='fullImageLink']");
        var imgPath = div.SelectSingleNode("a").GetAttributeValue("href", null);
        var imgSrc = $"https://archonarcana.com/{imgPath}";
        await Download(httpClient, "images", fileName, new Uri(imgSrc));
    }

    static async ValueTask Download(HttpClient client, string directoryPath, string fileName, Uri uri)
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