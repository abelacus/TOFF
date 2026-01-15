using System.Diagnostics.CodeAnalysis;
using TorrentClient.Models;

namespace TorrentClient
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public interface ITorrentClient
    {
        public void ConnectToClient();
        public Task<TorrentDetails[]> GetTorrentDetails();
        public Task<FileDetails[]> GetFilesForTorrent(string torrentHash);
    }
}
