namespace TorrentClient.Clients.qBittorrent.Models
{
    internal enum TorrentStates
    {
        error,
        missingFiles,
        uploading,
        pausedUP,
        queuedUP,
        stalledUP,
        checkingUP,
        forcedUP,
        allocating,
        downloading,
        metaDL,
        pausedDL,
        queueDL,
        stalledDL,
        checkingDL,
        stoppedDL,
        stoppedUP,
        forcedDL,
        checkingResumeData,
        moving,
        unknown,
    }
}
