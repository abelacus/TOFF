using TorrentClient.Models;

namespace TOFF.Models
{
    internal class AppPreferences
    {
        public string clientSelection { get; set; }
        public string[] IgnoreDirectories { get; set; } = [];
        /// <summary>
        /// <see cref="TKey"/> is path as it appears in torrent client, <see cref="TValue"> is local path to translate to
        /// </summary>
        public Dictionary<string, string> PathTranslations { get; set; } = new Dictionary<string, string>();
        public TorrentClientConfig torrentClientConfig { get; set; }
        public string? torrentDirectory { get; set; }

        public AppPreferences() { }

        public AppPreferences(string _clientSelection, TorrentClientConfig _torrentClientConfig)
        {
            clientSelection = _clientSelection;
            torrentClientConfig = _torrentClientConfig;
        }
    }
}