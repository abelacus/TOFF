using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorrentClient.Attributes;
using TorrentClient.Models;

namespace TorrentClient.Clients.Deluge
{
    [ClientName("Deluge")]
    internal class DelugeClient : TorrentClientBase
    {
        public DelugeClient(TorrentClientConfig clientConfig) : base(clientConfig)
        {
        }

        public override void ConnectToClient()
        {
            throw new NotImplementedException();
        }

        public override Task<FileDetails[]> GetFilesForTorrent(string torrentHash)
        {
            throw new NotImplementedException();
        }

        public override Task<TorrentDetails[]> GetTorrentDetails()
        {
            throw new NotImplementedException();
        }
    }
}
