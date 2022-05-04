using System;
using System.Collections.Generic;
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

        private int? _craftingCost;
        public int Profit => MedianPPU - _craftingCost ?? throw new Exception("crafting cost not calculated");
        public double GrossFlux => ItemsPerDay * MedianPPU;
        public double ProfitFlux => ItemsPerDay * Profit;

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

        public int ComputeCraftingCost(Dictionary<int, MarketItem> marketDict, Dictionary<int, Recipe> recipes)
        {
            if (_craftingCost.HasValue)
            {
                return _craftingCost.Value;
            }

            int actualCost;
            if (recipes.TryGetValue(ItemID, out var recipe) && recipe.Ingredients.All(i => marketDict.ContainsKey(i.id)))
            {
                var craftingCost = recipe.Ingredients.Sum(i => i.amount * marketDict[i.id].ComputeCraftingCost(marketDict, recipes)) / recipe.ResultAmount;
                actualCost = Math.Min(craftingCost, MedianPPU);
            }
            else
            {
                actualCost = MedianPPU;
            }
            _craftingCost = actualCost;
            return actualCost;
        }
    }
}
