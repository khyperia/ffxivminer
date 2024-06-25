using System.Text.Json.Nodes;

namespace Parser;

internal class WorldCSV
{
    public JsonNode? ExportJson()
    {
        if (string.IsNullOrWhiteSpace(Name) || !IsPublic)
        {
            return null;
        }

        return Name;
    }

    public string? Name { get; set; }
    public bool IsPublic { get; set; }
}
