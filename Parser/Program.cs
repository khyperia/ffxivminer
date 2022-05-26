using CsvHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            for (var i = 0;; i++)
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
            var itemDb = ParseCSV<ItemCSV>(Path.Combine(RootDirectory, @"Data\Item.csv"))
                .Where(s => !string.IsNullOrWhiteSpace(s.Name) && uniqueNameSet.Add(s.Name)).ToList();
            var idToName = itemDb.ToDictionary(r => r.Id, r => r.Name!);

            var recipeLevelTable =
                ParseCSV<RecipeLevelTableCSV>(Path.Combine(RootDirectory, @"Data\RecipeLevelTable.csv"))
                    .ToDictionary(r => r.ID, r => r);

            var uniqueRecipeSet = new HashSet<int>(); // some items have multiple recipes (e.g. ARM & BSM)
            var recipes =
                ParseCSV<RecipeCSV>(Path.Combine(RootDirectory, @"Data\Recipe.csv"))
                    .Select(r => r.Convert(recipeLevelTable)).Where(r => r != null && uniqueRecipeSet.Add(r.ResultID))!
                    .ToList<Recipe>();

            /*
            Console.WriteLine("Loading files");
            var jsonFiles = Directory.EnumerateFiles(Path.Combine(RootDirectory, @"DownloadedData"))
                .Where(f => f.Contains("data")).AsParallel().Select(file =>
                {
                    Console.WriteLine($"Loading {file}");
                    using var stream = File.OpenRead(file);
                    return (JsonArray) JsonValue.Load(stream)["items"];
                }).ToList();

            Console.WriteLine("Parsing");
            var marketItems = new List<MarketItem>();
            foreach (var data in jsonFiles)
            {
                foreach (var item in data)
                {
                    if (item["entries"].Count > MinMarketboardEntryCount)
                    {
                        marketItems.Add(new MarketItem(item));
                    }
                }
            }
            */

            Console.WriteLine("Exporting");

            var exportNameToId = new JsonObject();
            foreach (var kvp in idToName)
            {
                exportNameToId.Add(kvp.Value, kvp.Key);
            }

            /*
            var exportMarket = new JsonArray();
            foreach (var marketItem in marketItems)
            {
                exportMarket.Add(marketItem.ExportJson());
            }
            */

            var exportRecipes = new JsonArray();
            foreach (var recipe in recipes)
            {
                exportRecipes.Add(recipe.ExportJson());
            }

            /*
            {
                var export = new JsonObject()
                {
                    {"nametoid", exportNameToId},
                    {"market", exportMarket},
                    {"recipes", exportRecipes},
                };

                Console.WriteLine("Writing result");
                var writerJson = new StreamWriter(Path.Combine(RootDirectory, @"Data\data.json"));
                export.Save(writerJson);
                writerJson.Flush();
                var writerJs = new StreamWriter(Path.Combine(RootDirectory, @"data.js"));
                writerJs.Write("var globaldata = "); // :)
                export.Save(writerJs);
                writerJs.Flush();
                Process.Start(new ProcessStartInfo("cmd.exe", "/C \"jq < Data\\data.json > Data\\data_formatted.json\"")
                    {WorkingDirectory = RootDirectory})!.WaitForExit();
            }
            */

            {
                var export = new JsonObject()
                {
                    {"nametoid", exportNameToId},
                    {"recipes", exportRecipes},
                };

                Console.WriteLine("Writing result");
                var writerJson = new StreamWriter(Path.Combine(RootDirectory, @"Data\recipes.json"));
                export.Save(writerJson);
                writerJson.Flush();
                var writerJs = new StreamWriter(Path.Combine(RootDirectory, @"recipes.js"));
                writerJs.Write("var globaldata = "); // :)
                export.Save(writerJs);
                writerJs.Flush();
                Process.Start(new ProcessStartInfo("cmd.exe",
                        "/C \"jq < Data\\recipes.json > Data\\recipes_formatted.json\"")
                    {WorkingDirectory = RootDirectory})!.WaitForExit();
            }

            Console.WriteLine("Done");
        }
    }
}