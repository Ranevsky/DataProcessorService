using System.Text.Json;
using System.Text.Json.Nodes;

namespace DataProcessorService.Extensions;

internal static class JsonNodeExtension
{
    public static JsonNode GetNode(this JsonNode node, string key)
    {
        var foundedNode = node[key];
        if (foundedNode is not null)
        {
            return foundedNode;
        }

        throw new JsonException($"Node {key} not found");
    }
}