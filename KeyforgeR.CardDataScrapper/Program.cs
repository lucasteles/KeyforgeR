using KeyforgeR.CardDataScrapper;

// get your key here https://decksofkeyforge.com/about/sellers-and-devs
const string keyforgeParserKey = "0000-0000-0000-0000";

var cards= await DecksOfkeyforgeParser.Get(keyforgeParserKey);
await ArchonArcanaImages.DownloadAll(cards);