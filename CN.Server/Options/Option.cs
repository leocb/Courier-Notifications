using System.Text.Json;
using System.Text.Json.Serialization;

namespace CN.Server.Options;

public static class Option
{
    public static JsonSerializerOptions JsonSerializer { get; } = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };
}
