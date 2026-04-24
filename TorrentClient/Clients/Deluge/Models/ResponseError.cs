namespace TorrentClient.Clients.Deluge.Models
{
    internal record ResponseError
    {
        public required string message { get; set; }
        public required int code { get; set; }
    }
}
