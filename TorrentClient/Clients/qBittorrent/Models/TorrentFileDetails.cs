using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentClient.Clients.qBittorrent.Models
{
    internal record TorrentFileDetails
    {
        public int index { get; init; }
        public string name { get; init; }
        public ulong size { get; init; }
        public float progress { get; init; }
        public int priority { get; init; }
        public bool is_seed { get; init; }
        public int[] piece_range { get; init; }
        public float availability { get; init; }
    }
}
