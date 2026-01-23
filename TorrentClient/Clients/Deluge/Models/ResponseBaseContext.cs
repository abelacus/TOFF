using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TorrentClient.Clients.Deluge.Models
{
    [JsonSerializable(typeof(ResponseBase<TorrentFileDetails>))]
    [JsonSerializable(typeof(ResponseBase<Dictionary<string,TorrentInfo>>))]
    [JsonSerializable(typeof(ResponseBase<bool>))]
    internal partial class ResponseBaseContext : JsonSerializerContext
    {
    }
}
