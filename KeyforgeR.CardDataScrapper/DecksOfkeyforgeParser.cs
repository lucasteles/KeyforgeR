﻿using System.Text.Json;

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
        return cards;
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

