using System.Diagnostics.CodeAnalysis;
using TorrentClient.Models;

namespace TorrentClient
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public interface ITorrentClient
    {
        public Task ConnectToClient();
        public Task<TorrentDetails[]> GetTorrentDetails();
        public Task<FileDetails[]> GetFilesForTorrent(TorrentDetails torrentDetails);
    }
}
