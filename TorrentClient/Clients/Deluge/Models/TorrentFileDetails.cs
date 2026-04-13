namespace TorrentClient.Clients.Deluge.Models
{
    /// <summary>
    /// <see cref="T"/> for <see cref="ResponseBase{T}"/>
    /// </summary>
    internal record TorrentFileDetails
    {
        public required string type { get; set; }
        public required Dictionary<string, TorrentFileDetails> contents { get; set; }
        public int? index { get; set; }
        public ulong? offset { get; set; }
        public ulong? size { get; set; }
        public int? priority { get; set; }
        public float? progress { get; set; }
        public string? path { get; set; }
    }
}
