using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TorrentClient.Clients.Deluge.Models
{
    internal record ResponseBase<T>
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("result")]
        public T Result { get; set; }
        [JsonPropertyName("error")]
        public ResponseError Error { get; set; }
    }
}
