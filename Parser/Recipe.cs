using System.Collections.Generic;
using System.Json;

namespace Parser
{
    internal class Recipe
    {
        public int ResultID { get; set; }
        public int ResultAmount { get; set; }
        public (int id, int amount)[] Ingredients { get; set; }

        public Recipe(int resultID, int resultAmount, (int id, int amount)[] ingredients)
        {
            ResultID = resultID;
            ResultAmount = resultAmount;
            Ingredients = ingredients;
        }

        public JsonObject ExportJson()
        {
            var ingredients = new JsonArray();
            foreach (var (id, amount) in Ingredients)
            {
                var ing = new JsonObject()
                {
                    { "id", id },
                    { "amount", amount },
                };
                ingredients.Add(ing);
            }
            return new()
            {
                { "resultid", ResultID },
                { "resultamount", ResultAmount },
                { "ingredients", ingredients },
            };
        }
    }
}
