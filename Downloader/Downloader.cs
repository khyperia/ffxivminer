using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Json;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Downloader
{
    internal class Program
    {
        private static string CurrentFilePath([CallerFilePath] string s = "") => s;
        public static string RootDirectory => Path.GetDirectoryName(Path.GetDirectoryName(CurrentFilePath())!)!;

        private static void Main()
        {
            using var client = new HttpClient();

            var marketableFile = Download(client, "https://universalis.app/api/marketable", "marketable.json");
            using var marketable = File.OpenRead(marketableFile);
            var data = (JsonArray)JsonValue.Load(marketable);

            var chunks = data.Chunks(100);

            var stopwatch = new Stopwatch();

            var progress = new Progress();
            var i = 0;

            foreach (var chunk in chunks)
            {
                var start = chunk[0];
                var end = chunk[^1];
                // 67 = Shiva
                var url = $"https://universalis.app/api/history/67/{string.Join(",", chunk)}?entries=100";
                stopwatch.Restart();
                Download(client, url, @$"data-{start}-{end}.json");
                Console.WriteLine($"Downloading took {stopwatch.Elapsed}, {progress.TimeString((double)i++ / (data.Count / 100))}");
                Thread.Sleep(1000);
            }
            Console.WriteLine("Done");
            Console.ReadKey(true);
        }

        private static string Download(HttpClient client, string url, string filename)
        {
            var directory = Path.Combine(RootDirectory, "DownloadedData");
            var file = Path.Combine(directory, filename);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            Console.WriteLine($"Downloading: {url} to {file}");
            using var stream = client.GetStreamAsync(url).Result;
            using var filestream = new FileStream(file, FileMode.Create);
            stream.CopyTo(filestream);
            return file;
        }
    }

    public static class IEnumerableExtensions
    {
        public static IEnumerable<List<T>> Chunks<T>(this IEnumerable<T> source, int max)
        {
            var toReturn = new List<T>(max);
            foreach (var item in source)
            {
                toReturn.Add(item);
                if (toReturn.Count == max)
                {
                    yield return toReturn;
                    toReturn = new List<T>(max);
                }
            }
            if (toReturn.Count != 0)
            {
                yield return toReturn;
            }
        }
    }
}
