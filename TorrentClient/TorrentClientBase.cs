using TorrentClient.Models;

namespace TorrentClient
{
    public abstract class TorrentClientBase : ITorrentClient
    {
        protected readonly TorrentClientConfig ClientConfig;
        protected TorrentClientBase(TorrentClientConfig clientConfig) { ClientConfig = clientConfig; }
        public abstract Task ConnectToClient();

        public abstract Task<FileDetails[]> GetFilesForTorrent(TorrentDetails torrentDetails);
        public abstract Task<TorrentDetails[]> GetTorrentDetails();
    }
}
