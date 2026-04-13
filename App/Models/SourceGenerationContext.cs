using System.Text.Json.Serialization;

namespace TOFF.Models
{
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(AppPreferences))]
    internal partial class SourceGenerationContext : JsonSerializerContext
    {
    }
}
