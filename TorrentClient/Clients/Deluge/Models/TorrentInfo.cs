namespace TorrentClient.Clients.Deluge.Models
{
    internal record TorrentInfo
    {
        public required string name { get; set; }
        public required string state { get; set; }
        public required float progress { get; set; }
        public required string download_location { get; set; }
        public required string hash { get; set; }
    }
}
