using System.Text.Json.Serialization;

namespace TorrentClient.Clients.Deluge.Models
{
    [JsonSerializable(typeof(RequestBase<string>))]
    [JsonSerializable(typeof(RequestBase<List<string>>))]
    internal partial class RequestBaseContext : JsonSerializerContext
    {
    }
}
