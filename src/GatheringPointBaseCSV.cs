using System.Json;

namespace Parser;

internal class GatheringItemCSV
{
    public int Item { get; set; }

    public JsonValue? ExportJson()
    {
        if (Item == 0)
        {
            return null;
        }
        return Item;
    }
}