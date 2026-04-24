using System.Text.Json.Serialization;

namespace TorrentClient.Clients.qBittorrent.Models
{
    [JsonSourceGenerationOptions(WriteIndented = false)]
    [JsonSerializable(typeof(List<TorrentFileDetails>))]
    internal partial class TorrentFileDetailsContext : JsonSerializerContext
    {
    }
}
