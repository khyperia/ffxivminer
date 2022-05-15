using CsvHelper.Configuration.Attributes;

namespace Parser
{
    internal class RecipeLevelTableCSV
    {
        [Name("#")]
        public int ID { get; set; }
        public int ClassJobLevel { get; set; }
        public int Stars { get; set; }
        public int SuggestedCraftsmanship { get; set; }
        public int SuggestedControl { get; set; }
        public int Difficulty { get; set; }
        public int Quality { get; set; }
        public int ProgressDivider { get; set; }
        public int QualityDivider { get; set; }
        public int ProgressModifier { get; set; }
        public int QualityModifier { get; set; }
        public int Durability { get; set; }
        public int ConditionsFlag { get; set; }
    }
}
