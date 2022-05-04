using CsvHelper.Configuration.Attributes;
using System.Collections.Generic;

namespace Parser
{
    internal class RecipeCSV
    {
        public Recipe? Convert(Dictionary<string, int> nameToId)
        {
            if (string.IsNullOrWhiteSpace(ItemResult))
            {
                return null;
            }
            var itemResultId = nameToId[ItemResult];
            var ingredients = new List<(int id, int amount)>();
            if (AmountIngredient0 != 0 && ItemIngredient0 != null)
            {
                ingredients.Add((nameToId[ItemIngredient0], AmountIngredient0));
            }
            if (AmountIngredient1 != 0 && ItemIngredient1 != null)
            {
                ingredients.Add((nameToId[ItemIngredient1], AmountIngredient1));
            }
            if (AmountIngredient2 != 0 && ItemIngredient2 != null)
            {
                ingredients.Add((nameToId[ItemIngredient2], AmountIngredient2));
            }
            if (AmountIngredient3 != 0 && ItemIngredient3 != null)
            {
                ingredients.Add((nameToId[ItemIngredient3], AmountIngredient3));
            }
            if (AmountIngredient4 != 0 && ItemIngredient4 != null)
            {
                ingredients.Add((nameToId[ItemIngredient4], AmountIngredient4));
            }
            if (AmountIngredient5 != 0 && ItemIngredient5 != null)
            {
                ingredients.Add((nameToId[ItemIngredient5], AmountIngredient5));
            }
            if (AmountIngredient6 != 0 && ItemIngredient6 != null)
            {
                ingredients.Add((nameToId[ItemIngredient6], AmountIngredient6));
            }
            if (AmountIngredient7 != 0 && ItemIngredient7 != null)
            {
                ingredients.Add((nameToId[ItemIngredient7], AmountIngredient7));
            }
            if (AmountIngredient8 != 0 && ItemIngredient8 != null)
            {
                ingredients.Add((nameToId[ItemIngredient8], AmountIngredient8));
            }
            if (AmountIngredient9 != 0 && ItemIngredient9 != null)
            {
                ingredients.Add((nameToId[ItemIngredient9], AmountIngredient9));
            }
            return new Recipe(itemResultId, AmountResult, ingredients.ToArray());
        }

        [Name("Item{Result}")]
        public string? ItemResult { get; set; }
        [Name("Amount{Result}")]
        public int AmountResult { get; set; }

        [Name("Item{Ingredient}[0]")]
        public string? ItemIngredient0 { get; set; }
        [Name("Amount{Ingredient}[0]")]
        public int AmountIngredient0 { get; set; }

        [Name("Item{Ingredient}[1]")]
        public string? ItemIngredient1 { get; set; }
        [Name("Amount{Ingredient}[1]")]
        public int AmountIngredient1 { get; set; }

        [Name("Item{Ingredient}[2]")]
        public string? ItemIngredient2 { get; set; }
        [Name("Amount{Ingredient}[2]")]
        public int AmountIngredient2 { get; set; }

        [Name("Item{Ingredient}[3]")]
        public string? ItemIngredient3 { get; set; }
        [Name("Amount{Ingredient}[3]")]
        public int AmountIngredient3 { get; set; }

        [Name("Item{Ingredient}[4]")]
        public string? ItemIngredient4 { get; set; }
        [Name("Amount{Ingredient}[4]")]
        public int AmountIngredient4 { get; set; }

        [Name("Item{Ingredient}[5]")]
        public string? ItemIngredient5 { get; set; }
        [Name("Amount{Ingredient}[5]")]
        public int AmountIngredient5 { get; set; }

        [Name("Item{Ingredient}[6]")]
        public string? ItemIngredient6 { get; set; }
        [Name("Amount{Ingredient}[6]")]
        public int AmountIngredient6 { get; set; }

        [Name("Item{Ingredient}[7]")]
        public string? ItemIngredient7 { get; set; }
        [Name("Amount{Ingredient}[7]")]
        public int AmountIngredient7 { get; set; }

        [Name("Item{Ingredient}[8]")]
        public string? ItemIngredient8 { get; set; }
        [Name("Amount{Ingredient}[8]")]
        public int AmountIngredient8 { get; set; }

        [Name("Item{Ingredient}[9]")]
        public string? ItemIngredient9 { get; set; }
        [Name("Amount{Ingredient}[9]")]
        public int AmountIngredient9 { get; set; }
    }
}
