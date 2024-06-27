using System.Text.Json.Nodes;

namespace Parser;

internal class WorldCSV
{
    public JsonNode? ExportJson()
    {
        if (string.IsNullOrWhiteSpace(Name) || !IsPublic && !OverrideIsPublic)
        {
            return null;
        }

        return Name;
    }

    private bool OverrideIsPublic => Name is "Cuchulainn" or "Kraken" or "Rafflesia" or "Golem";

    public string? Name { get; set; }
    public bool IsPublic { get; set; }
}
