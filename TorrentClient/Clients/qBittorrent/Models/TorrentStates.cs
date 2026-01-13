using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        forcedDL,
        checkingResumeData,
        moving,
        unknown,
    }
}
