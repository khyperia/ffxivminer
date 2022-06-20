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
                .Select(s => new KeyValuePair<string, JsonValue>(s.Name!, s.Id)));

            var recipeLevelTable =
                ParseCSV<RecipeLevelTableCSV>(@"Data\RecipeLevelTable.csv")
                    .ToDictionary(r => r.ID, r => r);

            var uniqueRecipeSet = new HashSet<int>(); // some items have multiple recipes (e.g. ARM & BSM)
            var recipes =
                new JsonArray(ParseCSV<RecipeCSV>(@"Data\Recipe.csv")
                    .Select(r => r.ExportJson(recipeLevelTable))
                    .Where(r => r != null && uniqueRecipeSet.Add(r["resultid"])));

            // var gatheringItem = new JsonArray(ParseCSV<GatheringItemCSV>(@"Data\GatheringItem.csv")
            //     .Select(csv => csv.ExportJson()).Where(j => j != null).OrderBy(j => (int) j));
            var gatheringItem = GetGatheringItem();

            var worlds = new JsonArray(ParseCSV<WorldCSV>(@"Data\World.csv")
                .Select(csv => csv.ExportJson()).Where(j => j != null).OrderBy(j => (string) j));

            Console.WriteLine("Exporting");

            {
                var export = new JsonObject
                {
                    {"nametoid", nameToId},
                    {"recipes", recipes},
                    {"gathering", gatheringItem},
                    {"worlds", worlds},
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

        private static JsonValue GetGatheringItem()
        {
            var nodes = (JsonObject) JsonValue.Parse(File.ReadAllText(Path.Combine(RootDirectory, @"Data\nodes.json")));
            var result = new Dictionary<int, bool>();
            foreach (var entry in nodes.Values)
            {
                if (entry.ContainsKey("map") && (int) entry["map"] == 0) continue;
                bool limited = entry.ContainsKey("limited") ? entry["limited"] : false;
                foreach (JsonPrimitive item in entry["items"])
                {
                    if (!result.TryGetValue(item, out var existing) || existing)
                    {
                        result[item] = limited;
                    }
                }
            }

            return new JsonObject(result.OrderBy(kvp => kvp.Key)
                .Select(kvp => new KeyValuePair<string, JsonValue>(kvp.Key.ToString(), kvp.Value)));
        }
    }
}