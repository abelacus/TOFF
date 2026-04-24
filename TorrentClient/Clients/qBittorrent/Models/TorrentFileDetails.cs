namespace TorrentClient.Clients.qBittorrent.Models
{
    internal record TorrentFileDetails
    {
        public int index { get; init; }
        public required string name { get; init; }
        public ulong size { get; init; }
        public float progress { get; init; }
        public int priority { get; init; }
        public bool is_seed { get; init; }
        public required int[] piece_range { get; init; }
        public float availability { get; init; }
    }
}
