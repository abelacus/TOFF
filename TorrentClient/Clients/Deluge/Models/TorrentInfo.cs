using System;
using System.Collections.Generic;
using System.Text;

namespace TorrentClient.Clients.Deluge.Models
{
    internal record TorrentInfo
    {
        public string name { get; set; }
        public string state { get; set; }
        public float progress { get; set; }
        public string download_location { get; set; }
        public string hash { get; set; }
    }
}
