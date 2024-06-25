using CsvHelper.Configuration.Attributes;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Parser;

internal class RecipeCSV
{
    public JsonNode? ExportJson(Dictionary<int, RecipeLevelTableCSV> recipeLevelTable)
    {
        if (ItemResult <= 0)
        {
            return null;
        }
        
        var ingredients = new JsonArray();

        void AddIngredient(int amount, int item)
        {
            if (amount != 0 && item != 0)
            {
                var ing = new JsonObject()
                {
                    {"id", item},
                    {"amount", amount},
                };
                ingredients.Add(ing);
            }
        }

        AddIngredient(AmountIngredient0, ItemIngredient0);
        AddIngredient(AmountIngredient1, ItemIngredient1);
        AddIngredient(AmountIngredient2, ItemIngredient2);
        AddIngredient(AmountIngredient3, ItemIngredient3);
        AddIngredient(AmountIngredient4, ItemIngredient4);
        AddIngredient(AmountIngredient5, ItemIngredient5);
        AddIngredient(AmountIngredient6, ItemIngredient6);
        AddIngredient(AmountIngredient7, ItemIngredient7);
        AddIngredient(AmountIngredient8, ItemIngredient8);
        AddIngredient(AmountIngredient9, ItemIngredient9);

        var recipeLevelTableEntry = recipeLevelTable[RecipeLevelTable];
        var level = recipeLevelTableEntry.ClassJobLevel;

        return new JsonObject()
        {
            {"resultid", ItemResult},
            {"resultamount", AmountResult},
            {"level", level},
            {"ingredients", ingredients},
        };
    }

    [Name("Item{Result}")] public int ItemResult { get; set; }
    [Name("Amount{Result}")] public int AmountResult { get; set; }
    [Name("RecipeLevelTable")] public int RecipeLevelTable { get; set; }

    [Name("Item{Ingredient}[0]")] public int ItemIngredient0 { get; set; }
    [Name("Amount{Ingredient}[0]")] public int AmountIngredient0 { get; set; }

    [Name("Item{Ingredient}[1]")] public int ItemIngredient1 { get; set; }
    [Name("Amount{Ingredient}[1]")] public int AmountIngredient1 { get; set; }

    [Name("Item{Ingredient}[2]")] public int ItemIngredient2 { get; set; }
    [Name("Amount{Ingredient}[2]")] public int AmountIngredient2 { get; set; }

    [Name("Item{Ingredient}[3]")] public int ItemIngredient3 { get; set; }
    [Name("Amount{Ingredient}[3]")] public int AmountIngredient3 { get; set; }

    [Name("Item{Ingredient}[4]")] public int ItemIngredient4 { get; set; }
    [Name("Amount{Ingredient}[4]")] public int AmountIngredient4 { get; set; }

    [Name("Item{Ingredient}[5]")] public int ItemIngredient5 { get; set; }
    [Name("Amount{Ingredient}[5]")] public int AmountIngredient5 { get; set; }

    [Name("Item{Ingredient}[6]")] public int ItemIngredient6 { get; set; }
    [Name("Amount{Ingredient}[6]")] public int AmountIngredient6 { get; set; }

    [Name("Item{Ingredient}[7]")] public int ItemIngredient7 { get; set; }
    [Name("Amount{Ingredient}[7]")] public int AmountIngredient7 { get; set; }

    [Name("Item{Ingredient}[8]")] public int ItemIngredient8 { get; set; }
    [Name("Amount{Ingredient}[8]")] public int AmountIngredient8 { get; set; }

    [Name("Item{Ingredient}[9]")] public int ItemIngredient9 { get; set; }
    [Name("Amount{Ingredient}[9]")] public int AmountIngredient9 { get; set; }
}
