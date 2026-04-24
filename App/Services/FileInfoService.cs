using System.Runtime.InteropServices;
using TOFF.Models;
using TOFF.Services.FileInfoHandlers;

namespace TOFF.Services
{
    /// <summary>
    /// responsible for containing platform specific code for getting file info, primarily needed because we want to know the number of hardlinks.
    /// </summary>
    internal static class FileInfoService
    {
        private static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        private static IFileInfoHandler FileInfoHandler => IsWindows ? new FileInfoWindows() : new FileInfoUnix();
        /// <summary>
        /// returns information about the requested file including size, creation date, modification date, and number of hard links
        /// </summary>
        public static FileInformation GetFileInfo(string filePath)
        {
            return FileInfoHandler.GetFileInfo(filePath);
        }
    }
}
