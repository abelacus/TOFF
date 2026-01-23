using System;
using System.Collections.Generic;
using System.Text;

namespace TorrentClient.Clients.Deluge.Models
{
    internal record ResponseError
    {
        public string message { get; set; }
        public int code { get; set; }
    }
}
