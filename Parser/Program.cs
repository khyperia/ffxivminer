using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Json;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Parser
{
    internal static class Program
    {
        private static string CurrentFilePath([CallerFilePath] string s = "") => s;
        public static string RootDirectory => Path.GetDirectoryName(Path.GetDirectoryName(CurrentFilePath())!)!;

        private static TextReader RemoveGarbageLines(string path)
        {
            using var reader = new StreamReader(path);
            var memstream = new MemoryStream();
            var writer = new StreamWriter(memstream);

            for (var i = 0; ; i++)
            {
                var line = reader.ReadLine();
                if (line == null)
                {
                    break;
                }
                if (i == 0 || i == 2)
                {
                    continue;
                }
                writer.WriteLine(line);
            }
            writer.Flush();
            memstream.Position = 0;
            return new StreamReader(memstream);
        }

        private static List<T> ParseCSV<T>(string path)
        {
            using var csv = new CsvReader(RemoveGarbageLines(path), CultureInfo.InvariantCulture);
            return csv.GetRecords<T>().ToList();
        }

        const int MinMarketboardEntryCount = 5;

        private static void Main()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            Console.WriteLine("Loading CSV");

            var uniqueNameSet = new HashSet<string>();
            var itemDb = ParseCSV<FfxivItem>(Path.Combine(RootDirectory, @"Data\Item.csv")).Where(s => !string.IsNullOrWhiteSpace(s.Name) && uniqueNameSet.Add(s.Name)).ToList();
            var idToName = itemDb.ToDictionary(r => r.Id, r => r.Name!);
            var nameToId = itemDb.ToDictionary(r => r.Name!, r => r.Id);

            var uniqueRecipeSet = new HashSet<int>(); // some items have multiple recipes (e.g. ARM & BSM)
            var recipes = ParseCSV<RecipeCSV>(Path.Combine(RootDirectory, @"Data\Recipe.csv")).Select(r => r.Convert(nameToId)).Where(r => r != null && uniqueRecipeSet.Add(r.ResultID))!.ToList<Recipe>();
            var recipeSet = recipes.Select(r => r.ResultID).ToHashSet();
            // var recipeDict = recipes.ToDictionary(r => r.ResultID);

            var marketItems = new List<MarketItem>();
            foreach (var file in Directory.EnumerateFiles(Path.Combine(RootDirectory, @"DownloadedData")))
            {
                if (file.Contains("data"))
                {
                    Console.WriteLine($"Loading {file}");
                    using var stream = File.OpenRead(file);
                    var data = (JsonArray)JsonValue.Load(stream)["items"];
                    foreach (var item in data)
                    {
                        if (item["entries"].Count > MinMarketboardEntryCount)
                        {
                            marketItems.Add(new MarketItem(item));
                        }
                    }
                }
            }
            Console.WriteLine("Exporting");

            var exportNameToId = new JsonObject();
            foreach (var kvp in idToName)
            {
                exportNameToId.Add(kvp.Value, kvp.Key);
            }
            var exportMarket = new JsonArray();
            foreach (var marketItem in marketItems)
            {
                exportMarket.Add(marketItem.ExportJson());
            }
            var exportRecipes = new JsonArray();
            foreach (var recipe in recipes)
            {
                exportRecipes.Add(recipe.ExportJson());
            }
            var export = new JsonObject()
            {
                { "nametoid", exportNameToId },
                { "market", exportMarket },
                { "recipes", exportRecipes },
            };

            Console.WriteLine("Writing result");
            var writerJson = new StreamWriter(Path.Combine(RootDirectory, @"Data\data.json"));
            export.Save(writerJson);
            writerJson.Flush();
            var writerJs = new StreamWriter(Path.Combine(RootDirectory, @"data.js"));
            writerJs.Write("var globaldata = "); // :)
            export.Save(writerJs);
            writerJs.Flush();

            Console.WriteLine("Done");
        }

        private static void OldAnalysis(List<MarketItem> marketItems, Dictionary<int, Recipe> recipeDict, Dictionary<int, string> idToName, HashSet<int> recipeSet)
        {
            var marketDict = marketItems.ToDictionary(i => i.ItemID);
            PrintUnbuyables(marketDict, recipeDict, idToName);
            foreach (var item in marketItems)
            {
                item.ComputeCraftingCost(marketDict, recipeDict);
            }

            // marketItems.Sort((l, r) => l.Flux.CompareTo(r.Flux));
            marketItems.RemoveAll(i => !recipeSet.Contains(i.ItemID));
            marketItems.Sort((l, r) => r.ProfitFlux.CompareTo(l.ProfitFlux));
            marketItems.RemoveRange(100, marketItems.Count - 100);

            var now = DateTime.UtcNow;
            foreach (var item in marketItems)
            {
                // var dateInfo = $"{item.Entries.Length}sells last={item.Entries.Max(i => i.Timestamp)} up={item.lastUploadTime}";
                var grossInfo = $"gross=${item.GrossFlux:F3}/day = ${item.MedianPPU} * {item.ItemsPerDay:F2}/day";
                var profitInfo = $"profit=${item.ProfitFlux:F3}/day = ${item.Profit} * {item.ItemsPerDay:F2}/day";
                Console.WriteLine($"{item.ItemID} {idToName[item.ItemID]} -- {profitInfo} -- {grossInfo}");
            }
            // Console.WriteLine($"name,flux,pricePerUnit,itemsPerDay");
            // foreach (var item in marketItems)
            // {
            //     Console.WriteLine($"{idToName[item.ItemID]},{item.Flux:F3},{item.MedianPPU},{item.ItemsPerDay:F2}");
            // }
            Console.ReadLine();
        }

        private static void PrintUnbuyables(Dictionary<int, MarketItem> marketDict, Dictionary<int, Recipe> recipes, Dictionary<int, string> idToName)
        {
            var printed = new HashSet<int>();
            foreach (var recipe in recipes.Values)
            {
                Print(recipe.ResultID);
                foreach ((var id, var _) in recipe.Ingredients)
                {
                    Print(id);
                }
            }

            void Print(int id)
            {
                if (!marketDict.ContainsKey(id) && printed.Add(id))
                {
                    Console.WriteLine("Unbuyable: " + id + " - " + idToName[id]);
                }
            }
        }
    }
}
