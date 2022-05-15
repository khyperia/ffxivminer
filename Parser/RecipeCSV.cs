using CsvHelper.Configuration.Attributes;
using System.Collections.Generic;

namespace Parser
{
    internal class RecipeCSV
    {
        // #, Number, CraftType, RecipeLevelTable, Item{Result}, Amount{Result}, Item{Ingredient}[0],
        // Amount{Ingredient}[0], Item{Ingredient}[1], Amount{Ingredient}[1], Item{Ingredient}[2],
        // Amount{Ingredient}[2], Item{Ingredient}[3], Amount{Ingredient}[3], Item{Ingredient}[4],
        // Amount{Ingredient}[4], Item{Ingredient}[5], Amount{Ingredient}[5], Item{Ingredient}[6],
        // Amount{Ingredient}[6], Item{Ingredient}[7], Amount{Ingredient}[7], Item{Ingredient}[8],
        // Amount{Ingredient}[8], Item{Ingredient}[9], Amount{Ingredient}[9], RecipeNotebookList, IsSecondary,
        // MaterialQualityFactor, DifficultyFactor, QualityFactor, DurabilityFactor, , RequiredCraftsmanship,
        // RequiredControl, QuickSynthCraftsmanship, QuickSynthControl, SecretRecipeBook, Quest, CanQuickSynth, CanHq,
        // ExpRewarded, Status{Required}, Item{Required}, IsSpecializationRequired, IsExpert, , , PatchNumber

        // 5327,14893,"Clothcraft","RecipeLevelTable#550","AR-Caean Velvet",1,"AR-Caean Cotton
        // Boll",5,"",0,"",0,"",0,"",0,"",0,"",0,"",0,"Lightning
        // Crystal",8,,0,"RecipeNotebookList#0",True,0,50,100,50,0,0,0,0,0,"",,True,True,True,"","",False,False,0,65535,600
        public Recipe? Convert(Dictionary<int, RecipeLevelTableCSV> recipeLevelTable)
        {
            if (ItemResult <= 0)
            {
                return null;
            }
            var ingredients = new List<(int id, int amount)>();
            if (AmountIngredient0 != 0 && ItemIngredient0 != 0)
            {
                ingredients.Add((ItemIngredient0, AmountIngredient0));
            }
            if (AmountIngredient1 != 0 && ItemIngredient1 != 0)
            {
                ingredients.Add((ItemIngredient1, AmountIngredient1));
            }
            if (AmountIngredient2 != 0 && ItemIngredient2 != 0)
            {
                ingredients.Add((ItemIngredient2, AmountIngredient2));
            }
            if (AmountIngredient3 != 0 && ItemIngredient3 != 0)
            {
                ingredients.Add((ItemIngredient3, AmountIngredient3));
            }
            if (AmountIngredient4 != 0 && ItemIngredient4 != 0)
            {
                ingredients.Add((ItemIngredient4, AmountIngredient4));
            }
            if (AmountIngredient5 != 0 && ItemIngredient5 != 0)
            {
                ingredients.Add((ItemIngredient5, AmountIngredient5));
            }
            if (AmountIngredient6 != 0 && ItemIngredient6 != 0)
            {
                ingredients.Add((ItemIngredient6, AmountIngredient6));
            }
            if (AmountIngredient7 != 0 && ItemIngredient7 != 0)
            {
                ingredients.Add((ItemIngredient7, AmountIngredient7));
            }
            if (AmountIngredient8 != 0 && ItemIngredient8 != 0)
            {
                ingredients.Add((ItemIngredient8, AmountIngredient8));
            }
            if (AmountIngredient9 != 0 && ItemIngredient9 != 0)
            {
                ingredients.Add((ItemIngredient9, AmountIngredient9));
            }
            var recipeLevelTableEntry = recipeLevelTable[RecipeLevelTable];
            var level = recipeLevelTableEntry.ClassJobLevel;
            return new Recipe(ItemResult, AmountResult, level, ingredients.ToArray());
        }

        [Name("Item{Result}")]
        public int ItemResult { get; set; }
        [Name("Amount{Result}")]
        public int AmountResult { get; set; }
        [Name("RecipeLevelTable")]
        public int RecipeLevelTable { get; set; }

        [Name("Item{Ingredient}[0]")]
        public int ItemIngredient0 { get; set; }
        [Name("Amount{Ingredient}[0]")]
        public int AmountIngredient0 { get; set; }

        [Name("Item{Ingredient}[1]")]
        public int ItemIngredient1 { get; set; }
        [Name("Amount{Ingredient}[1]")]
        public int AmountIngredient1 { get; set; }

        [Name("Item{Ingredient}[2]")]
        public int ItemIngredient2 { get; set; }
        [Name("Amount{Ingredient}[2]")]
        public int AmountIngredient2 { get; set; }

        [Name("Item{Ingredient}[3]")]
        public int ItemIngredient3 { get; set; }
        [Name("Amount{Ingredient}[3]")]
        public int AmountIngredient3 { get; set; }

        [Name("Item{Ingredient}[4]")]
        public int ItemIngredient4 { get; set; }
        [Name("Amount{Ingredient}[4]")]
        public int AmountIngredient4 { get; set; }

        [Name("Item{Ingredient}[5]")]
        public int ItemIngredient5 { get; set; }
        [Name("Amount{Ingredient}[5]")]
        public int AmountIngredient5 { get; set; }

        [Name("Item{Ingredient}[6]")]
        public int ItemIngredient6 { get; set; }
        [Name("Amount{Ingredient}[6]")]
        public int AmountIngredient6 { get; set; }

        [Name("Item{Ingredient}[7]")]
        public int ItemIngredient7 { get; set; }
        [Name("Amount{Ingredient}[7]")]
        public int AmountIngredient7 { get; set; }

        [Name("Item{Ingredient}[8]")]
        public int ItemIngredient8 { get; set; }
        [Name("Amount{Ingredient}[8]")]
        public int AmountIngredient8 { get; set; }

        [Name("Item{Ingredient}[9]")]
        public int ItemIngredient9 { get; set; }
        [Name("Amount{Ingredient}[9]")]
        public int AmountIngredient9 { get; set; }
    }
}
