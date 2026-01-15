using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorrentClient.Models;

namespace TorrentClient
{
    public abstract class TorrentClientBase : ITorrentClient
    {
        protected readonly TorrentClientConfig _clientConfig;
        protected TorrentClientBase(TorrentClientConfig clientConfig) { _clientConfig = clientConfig; }
        public abstract void ConnectToClient();

        public abstract Task<FileDetails[]> GetFilesForTorrent(string torrentHash);
        public abstract Task<TorrentDetails[]> GetTorrentDetails();
    }
}
