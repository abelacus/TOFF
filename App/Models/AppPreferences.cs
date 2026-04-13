using TorrentClient.Models;

namespace TOFF.Models
{
    internal class AppPreferences
    {
        public string ClientSelection { get; set; }
        public string[] IgnoreDirectories { get; set; } = [];
        /// <summary>
        /// <see cref="TKey"/> is path as it appears in torrent client, <see cref="TValue"/> is local path to translate to
        /// </summary>
        public Dictionary<string, string> PathTranslations { get; set; } = new Dictionary<string, string>();
        public TorrentClientConfig TorrentClientConfig { get; set; }
        public string? TorrentDirectory { get; set; }

        public AppPreferences() { }

        public AppPreferences(string clientSelection, TorrentClientConfig torrentClientConfig)
        {
            ClientSelection = clientSelection;
            TorrentClientConfig = torrentClientConfig;
        }
    }
}