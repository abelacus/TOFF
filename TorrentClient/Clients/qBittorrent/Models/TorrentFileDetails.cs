using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentClient.Clients.qBittorrent.Models
{
    internal record TorrentFileDetails
    {
        public int index;
        public string name;
        public int size;
        public float progress;
        public int priority;
        public bool is_seed;
        public int[] piece_range;
        public float availability;
    }
}
