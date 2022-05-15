using System;
using System.Json;
using System.Linq;

namespace Parser
{
    internal class MarketItem
    {
        public int ItemID;
        public Entry[] Entries;
        public int MedianPPU;
        public double ItemsPerDay;
        public double RegularSaleVelocity;
        public double NqSaleVelocity;
        public double HqSaleVelocity;
        public DateTime LastUploadTime;

        public MarketItem(JsonValue value)
        {
            ItemID = value["itemID"];
            LastUploadTime = DateTime.UnixEpoch + TimeSpan.FromMilliseconds(value["lastUploadTime"]);
            RegularSaleVelocity = value["regularSaleVelocity"];
            NqSaleVelocity = value["nqSaleVelocity"];
            HqSaleVelocity = value["hqSaleVelocity"];
            Entries = ((JsonArray)value["entries"]).Select(e => new Entry(e)).ToArray();
            MedianPPU = Entry.MedianPPU(Entries);
            ItemsPerDay = Entry.ItemsPerDay(Entries);
        }

        public JsonObject ExportJson()
        {
            return new()
            {
                { "id", ItemID },
                { "medianppu", MedianPPU },
                { "itemsperday", ItemsPerDay },
            };
        }
    }
}
