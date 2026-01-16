using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentClient.Models
{
    public record TorrentClientConfig ()
    {
        public string? ApiURL;
        public bool HasAuthentication = false;
        public string? Username;
        public string? Password;

        public string[] ToDetailsArray()
        {
            if (!HasAuthentication)
            {
                return [
                    "Requires Authentication: False",
                    "API Url: " + (ApiURL ?? "Not Set"),
                    ];
            }

            return [
                "Requires Authentication: True",
                "API Url: " + ApiURL,
                "Username: " + Username,
                "Password: " + Password.Replace(@"\w", "*")
            ];
        }
    }
}
