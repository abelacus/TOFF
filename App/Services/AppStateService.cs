using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorrentClient;
using TorrentClient.Models;

namespace TOFF.Services
{
    internal class AppStateService
    {
        public ITorrentClient? torrentClient;
        public FileDetails[]? torrentFiles;
        public TorrentClientConfig torrentClientConfig;
        public string clientSelection;
        public string? torrentDirectory;
        public string[] IgnoreDirectories = [];
        /// <summary>
        /// <see cref="TKey"/> is path as it appears in torrent client, <see cref="TValue"> is local path to translate to
        /// </summary>
        public Dictionary<string, string> PathTranslations = new Dictionary<string, string>();
        public bool exit = false;

        public AppStateService(TorrentClientService clientFactory)
        {
            //TODO: make this load from saved settings
            clientSelection = clientFactory.GetTorrentClients()[0]; //default client selection to the first one found
            torrentClientConfig = new TorrentClientConfig(); //initialise an empty config
        }
        
    }
}
