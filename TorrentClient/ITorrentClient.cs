using TorrentClient.Clients.qBittorrent.Models;
using TorrentClient.Models;

namespace TorrentClient
{
    public interface ITorrentClient
    {
        public static abstract string ClientName { get; }

        public void ConnectToClient();

        public Task<TorrentDetails[]> GetTorrentDetails();
        public Task<FileDetails[]> GetFilesForTorrent(string torrentHash);
    }
}
