using System.Text.Json;
using System.Text.Json.Serialization;

namespace CN.Models;

public static class Options
{
    public static JsonSerializerOptions JsonSerializer { get; } = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };
}
