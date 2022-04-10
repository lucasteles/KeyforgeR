// namespace KeyforgeR.CardDataScrapper;
// #nullable disable
// using System.Net;
// using System.Text.Json.Nodes;
// using System.Text.Json;
//
// // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
// public class CardData
// {
//     public string id { get; set; }
//     public string card_title { get; set; }
//     public string house { get; set; }
//     public string card_type { get; set; }
//     public string front_image { get; set; }
//     public string card_text { get; set; }
//     public object traits { get; set; }
//     public int amber { get; set; }
//     public string power { get; set; }
//     public string armor { get; set; }
//     public string rarity { get; set; }
//     public string flavor_text { get; set; }
//     public string card_number { get; set; }
//     public int expansion { get; set; }
//     public bool is_maverick { get; set; }
//     public bool is_anomaly { get; set; }
//     public bool is_enhanced { get; set; }
//     public bool is_non_deck { get; set; }
// }
//
// public class Scrapper
// {
//     public static async Task Run()
//     {
//         var skipJsonDownload = false;
//
//         string Url(int page, Expansions expansion) =>
//             $"https://www.keyforgegame.com/api/decks/?page={page}&links=cards&page_size=25&expansion={(int) expansion}";
//
//         using var client = new HttpClient();
//
//         var expansions = new[]
//         {
//             Expansions.CalloftheArchons,
//             Expansions.AgeofAscension,
//             Expansions.WorldsCollide,
//             Expansions.MassMutation,
//             Expansions.DarkTidings
//         };
//
//         Dictionary<Expansions, int> expansionCount = new()
//         {
//             [Expansions.CalloftheArchons] = 370,
//             [Expansions.AgeofAscension] = 370,
//             [Expansions.WorldsCollide] = 415,
//             [Expansions.MassMutation] = 422,
//             [Expansions.DarkTidings] = 428
//         };
//
//         async Task<CardData[]> GetCardsOfPage(int page, Expansions expansion)
//         {
//             Console.WriteLine($"Requesting page: {page} for expansion {expansion}");
//             var response = await client.GetAsync(Url(page + 1, expansion));
//             var waitCounter = 1;
//             while (response.StatusCode == HttpStatusCode.TooManyRequests)
//             {
//                 var responseJson = JsonNode.Parse(await response.Content.ReadAsStringAsync());
//                 if (responseJson["detail"].ToString().Contains("seconds"))
//                 {
//                     Console.WriteLine($"***** ");
//                     Console.WriteLine(responseJson.ToJsonString());
//                     var detail = responseJson["detail"].ToString().Trim().Split(" ")[^2].Trim();
//                     var waitFor = int.Parse(detail);
//                     Console.WriteLine($"***** waiting for {waitFor} + {waitCounter} seconds");
//                     await Task.Delay((waitFor + waitCounter) * 1000);
//                     waitCounter++;
//                 }
//                 else
//                 {
//                     var waitFor = waitCounter++ * 2000;
//                     Console.WriteLine($"***** Too many requests... waiting for {waitFor} seconds");
//                     await Task.Delay(waitFor);
//                 }
//
//                 response = await client.GetAsync(Url(page + 1, expansion));
//             }
//
//             var json = JsonNode.Parse(await response.Content.ReadAsStringAsync());
//             var jsonString = json?["_linked"]?["cards"]?.ToJsonString();
//             var cards = JsonSerializer.Deserialize<CardData[]>(jsonString).DistinctBy(x => x.id)
//                 .Where(x => !x.is_enhanced && !x.is_maverick && !x.is_non_deck).ToArray();
//             return cards;
//         }
//
//         var allCards = new List<CardData>();
//         var i = 0;
//
//         if (!skipJsonDownload)
//             while (allCards.Count < expansionCount.Values.Sum())
//                 foreach (var expansion in expansions)
//                 {
//                     Console.WriteLine($"Iteraction: {i}");
//                     var responses = await GetCardsOfPage(i, expansion);
//                     allCards = responses.Concat(allCards).DistinctBy(x => x.id).ToList();
//                     await File.WriteAllTextAsync("cards.json", JsonSerializer.Serialize(allCards));
//                     Console.WriteLine($"Current number of cards: {allCards.Count}");
//                     await Task.Delay(4000);
//                     i++;
//                 }
//
//         async Task Download(string directoryPath, string fileName, Uri uri)
//         {
//             var uriWithoutQuery = uri.GetLeftPart(UriPartial.Path);
//             var fileExtension = Path.GetExtension(uriWithoutQuery);
//             var path = Path.Combine(directoryPath, $"{fileName}{fileExtension}");
//
//             var imageBytes = await client.GetByteArrayAsync(uri);
//             await File.WriteAllBytesAsync(path, imageBytes);
//         }
//
//         if (File.Exists("cards.json"))
//         {
//             if (!Directory.Exists("images"))
//                 Directory.CreateDirectory("images");
//
//             var cards = JsonSerializer.Deserialize<CardData[]>(await File.ReadAllBytesAsync("cards.json"));
//             foreach (var card in cards)
//                 await Download("images", card.card_number, new Uri(card.front_image));
//         }
//
//         Console.WriteLine("Done");
//     }
// }