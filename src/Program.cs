using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
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

                if (i is 0 or 2)
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
            using var csv = new CsvReader(RemoveGarbageLines(Path.Combine(RootDirectory, path)),
                CultureInfo.InvariantCulture);
            return csv.GetRecords<T>().ToList();
        }

        private static void Main()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            Console.WriteLine("Loading CSV");

            var uniqueNameSet = new HashSet<string>();
            var nameToId = new JsonObject(ParseCSV<ItemCSV>(@"Data\Item.csv")
                .Where(s => !string.IsNullOrWhiteSpace(s.Name) && uniqueNameSet.Add(s.Name))
                .Select(s => new KeyValuePair<string, JsonNode?>(s.Name!, s.Id)));

            var recipeLevelTable =
                ParseCSV<RecipeLevelTableCSV>(@"Data\RecipeLevelTable.csv")
                    .ToDictionary(r => r.ID, r => r);

            var uniqueRecipeSet = new HashSet<int>(); // some items have multiple recipes (e.g. ARM & BSM)
            var recipes =
                new JsonArray(ParseCSV<RecipeCSV>(@"Data\Recipe.csv")
                    .Select(r => r.ExportJson(recipeLevelTable))
                    .Where(r => r != null && uniqueRecipeSet.Add(r["resultid"].GetValue<int>()))
                    .ToArray());

            // var gatheringItem = new JsonArray(ParseCSV<GatheringItemCSV>(@"Data\GatheringItem.csv")
            //     .Select(csv => csv.ExportJson()).Where(j => j != null).OrderBy(j => (int) j));
            var gatheringItem = GetGatheringItem();

            var worlds = new JsonArray(ParseCSV<WorldCSV>(@"Data\World.csv")
                .Select(csv => csv.ExportJson()).Where(j => j != null).OrderBy(j => j.GetValue<string>()).ToArray());

            Console.WriteLine("Exporting");

            {
                var export = new JsonObject
                {
                    { "nametoid", nameToId },
                    { "recipes", recipes },
                    { "gathering", gatheringItem },
                    { "worlds", worlds },
                };

                Console.WriteLine("Writing result");
                {
                    using var writerJson =
                        new Utf8JsonWriter(File.Create(Path.Combine(RootDirectory, @"Data\data_formatted.json")),
                            new() { Indented = true });
                    export.WriteTo(writerJson);
                }
                {
                    using var stream = File.Create(Path.Combine(RootDirectory, "data.js"));
                    var streamWriter = new StreamWriter(stream);
                    streamWriter.Write("var globaldata = "); // :)
                    streamWriter.Flush();
                    using var writerJs = new Utf8JsonWriter(stream, new() { Indented = false });
                    export.WriteTo(writerJs);
                }
            }

            Console.WriteLine("Done");
        }

        private static JsonNode GetGatheringItem()
        {
            var nodes = (JsonObject?)JsonNode.Parse(File.ReadAllText(Path.Combine(RootDirectory, @"Data\nodes.json")));
            var result = new Dictionary<int, bool>();
            foreach (var (_, node) in nodes)
            {
                var entry = node.AsObject();
                if (entry.ContainsKey("map") && entry["map"].GetValue<int>() == 0) continue;
                bool limited = entry.ContainsKey("limited") ? entry["limited"].GetValue<bool>() : false;
                foreach (var item in entry["items"].AsArray())
                {
                    if (!result.TryGetValue(item.GetValue<int>(), out var existing) || existing)
                    {
                        result[item.GetValue<int>()] = limited;
                    }
                }
            }

            return new JsonObject(result.OrderBy(kvp => kvp.Key)
                .Select(kvp => new KeyValuePair<string, JsonNode?>(kvp.Key.ToString(), kvp.Value)));
        }
    }
}
