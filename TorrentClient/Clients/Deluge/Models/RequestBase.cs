using System.Text.Json.Serialization;

namespace TorrentClient.Clients.Deluge.Models
{
    internal record RequestBase<T>
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("method")]
        public required string Method { get; set; }
        [JsonPropertyName("params")]
        public required List<T> Params { get; set; }
    }
}
