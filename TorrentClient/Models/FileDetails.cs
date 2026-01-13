using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentClient.Models
{
    public class FileDetails
    {
        public string fileName { get; set; }
        public string savePath { get; set; }
        public int priority { get; set; }
    }
}
