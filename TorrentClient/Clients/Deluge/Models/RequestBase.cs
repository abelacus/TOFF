using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TorrentClient.Clients.Deluge.Models
{
    internal record RequestBase<T>
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("method")]
        public string Method { get; set; }
        [JsonPropertyName("params")]
        public List<T> Params { get; set; }
    }
}
