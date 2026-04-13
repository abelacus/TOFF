using System.Text.Json.Serialization;

namespace TorrentClient.Clients.qBittorrent.Models
{
    [JsonSerializable(typeof(List<TorrentInfo>))]
    internal partial class TorrentInfoContext : JsonSerializerContext
    {
    }
}
