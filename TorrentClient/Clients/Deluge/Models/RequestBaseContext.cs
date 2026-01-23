using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TorrentClient.Clients.Deluge.Models
{
    [JsonSerializable(typeof(RequestBase<string>))]
    [JsonSerializable(typeof(RequestBase<List<string>>))]
    internal partial class RequestBaseContext : JsonSerializerContext
    {
    }
}
