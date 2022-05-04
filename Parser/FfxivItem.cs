using CsvHelper.Configuration.Attributes;

namespace Parser
{
    internal class FfxivItem
    {
        [Name("#")]
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
