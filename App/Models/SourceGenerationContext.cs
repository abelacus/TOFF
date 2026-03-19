using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using TorrentClient.Models;

namespace TOFF.Models
{
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(AppPreferences))]
    internal partial class SourceGenerationContext : JsonSerializerContext
    {
    }
}
