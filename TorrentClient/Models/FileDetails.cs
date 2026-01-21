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
        /// <summary>
        /// the directory of the file. this should not have a trailing '/'
        /// </summary>
        public string savePath { get; set; }
        /// <summary>
        /// Returns the full path, generated from <see cref="savePath"/> and <see cref="fileName"/>
        /// </summary>
        public string qualifiedPath { get {
                return savePath + "/" + fileName;
            }
        }
        public int priority { get; set; }
    }
}
