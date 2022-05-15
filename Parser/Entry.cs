using System;
using System.Json;
using System.Linq;

namespace Parser
{
    internal struct Entry
    {
        public bool Hq;
        public int PricePerUnit;
        public int Quantity;
        public DateTime Timestamp;

        public Entry(JsonValue value)
        {
            Hq = value["hq"];
            PricePerUnit = value["pricePerUnit"];
            Quantity = value["quantity"] ?? 1;
            Timestamp = DateTime.UnixEpoch + TimeSpan.FromSeconds(value["timestamp"]);
        }

        public static int MedianPPU(Entry[] entries)
        {
            var ppus = entries.Select(e => e.PricePerUnit).ToArray();
            Array.Sort(ppus);
            var size = ppus.Length;
            var mid = size / 2;
            var median = (size % 2 != 0) ? ppus[mid] : (ppus[mid] + ppus[mid - 1]) / 2;
            return median;
        }

        public static double ItemsPerDay(Entry[] entries)
        {
            var min = entries.Min(e => e.Timestamp);
            var max = entries.Max(e => e.Timestamp);
            var count = entries.Sum(e => e.Quantity);
            var itemsPerDay = count * (TimeSpan.FromDays(1) / (max - min));
            return itemsPerDay;
        }
    }
}
