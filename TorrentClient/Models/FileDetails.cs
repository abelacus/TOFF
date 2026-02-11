using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TorrentClient.Models
{
    public class FileDetails
    {
        public string fileName { get; set
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    field = value.Replace("/", @"\");
                }
                else
                {
                    field = value.Replace(@"\", "/");
                }
            }
        }
        /// <summary>
        /// the directory of the file. this should not have a trailing '/'
        /// </summary>
        public string savePath { get; set {
                field = Path.GetFullPath(value); //base path should already be provided fully qualified, we just want to convert to the OS form first.
            }
        }
        /// <summary>
        /// Returns the full path, generated from <see cref="savePath"/> and <see cref="fileName"/>
        /// </summary>
        public string qualifiedPath { get {
                return Path.Join(savePath, fileName);
            }
        }
        public int priority { get; set; }
    }
}
