namespace TorrentClient.Models
{
    public class TorrentDetails
    {
        public required string Name {  get; set; }
        public required string Hash { get; set; }
        public required string SavePath { get; set; }
    }
}
