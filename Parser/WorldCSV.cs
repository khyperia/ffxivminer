using System.Json;

namespace Parser;

internal class WorldCSV
{
    public JsonValue? ExportJson()
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