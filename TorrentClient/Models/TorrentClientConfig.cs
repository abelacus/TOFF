using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentClient.Models
{
    public record TorrentClientConfig ()
    {
        public string ApiURL;
        public bool HasAuthentication;
        public string Username;
        public string Password;
    }
}
