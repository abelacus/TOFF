using System.Runtime.InteropServices;

namespace TorrentClient.Models
{
    public class FileDetails
    {
        public required string FileName { get; set
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
        public required string SavePath { get; set {
                field = Path.GetFullPath(value); //base path should already be provided fully qualified, we just want to convert to the OS form first.
            }
        }
        /// <summary>
        /// Returns the full path, generated from <see cref="SavePath"/> and <see cref="FileName"/>
        /// </summary>
        public string QualifiedPath { get {
                return Path.Join(SavePath, FileName);
            }
        }
        public int Priority { get; set; }
    }
}
