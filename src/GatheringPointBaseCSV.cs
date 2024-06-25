using System.Text.Json.Nodes;

namespace Parser;

internal class GatheringItemCSV
{
    public int Item { get; set; }

    public JsonNode? ExportJson()
    {
        if (Item == 0)
        {
            return null;
        }
        return Item;
    }
}
