using System.Text.Json;

namespace KeyforgeR.CardDataScrapper;

public class DecksOfkeyforgeParser
{
    const string fileName = "all_cards.json";
    private const string rawFileName = $"raw_{fileName}";
    
    public static async Task<Card[]> Get(string key)
    {
        if (!File.Exists(rawFileName))
            await DownloadCards(key);

        var cardsJson = await File.ReadAllTextAsync(rawFileName);
        var cards = JsonSerializer.Deserialize<Card[]>(cardsJson)!;

        await DownloadImages(cards);
        
        return cards;
    }

    static async Task DownloadImages(Card[] cards)
    {
        var folder = "images";
        using var client = new HttpClient();
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        var semaphore = new SemaphoreSlim(10);

        var tasks = cards.Select(card => Task.Run(async () =>
        {
            await semaphore.WaitAsync();
            var filename = $"{card.expansion}-{card.cardNumber}";
            var ext = Path.GetExtension(card.frontImage);
            if (File.Exists(Path.Combine(folder, filename + ext)))
                return;
            Console.WriteLine($"=> {card.expansion}: #{card.cardNumber} - {card.cardTitle}");
            await Util.DownloadImage(client, folder, filename, new Uri(card.frontImage));
            semaphore.Release();
        }));

        await Task.WhenAll(tasks);
    }

    static async Task DownloadCards(string myKey)
    {
        var url = "https://decksofkeyforge.com/public-api/v1/cards";
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Api-Key", myKey);
        var response = await httpClient.GetStringAsync(url);
        await File.WriteAllTextAsync(rawFileName, response);
    }
}


    public class CardNumber
    {
        public string expansion { get; set; }
        public string cardNumber { get; set; }
    }


    public class Card
    {
        public string id { get; set; }
        public string cardTitle { get; set; }
        public string cardType { get; set; }
        public string frontImage { get; set; }
        public string cardText { get; set; }
        public int amber { get; set; }
        public int power { get; set; }
        public int armor { get; set; }
        public string rarity { get; set; }
        public string flavorText { get; set; }
        public string cardNumber { get; set; }
        public int expansion { get; set; }
        public string expansionEnum { get; set; }
        public bool anomaly { get; set; }
        public bool? big { get; set; }
        public List<string> traits { get; set; }
        public List<CardNumber> cardNumbers { get; set; }
        public List<string> houses { get; set; }
        public double aercScoreAverage { get; set; }
        public int effectivePower { get; set; }
        public double? aercScoreMax { get; set; }
        public bool evilTwin { get; set; }
        public double aercScore { get; set; }
    }

