using TOFF.Models;
using Mono.Unix.Native;

namespace TOFF.Services.FileInfoHandlers
{
    internal class FileInfoUnix : IFileInfoHandler
    {
        public FileInformation GetFileInfo(string filePath)
        {
            Syscall.stat(filePath, out var info);

            return new FileInformation
            {
                savePath = filePath,
                creationDate = DateTimeOffset.FromUnixTimeSeconds(info.st_ctime).DateTime, //is actually last metadata modification time, but should be good enough. easier than using statx and worrying about whether the filesystem returns it
                lastModifiedDate = DateTimeOffset.FromUnixTimeSeconds(info.st_mtime).DateTime,
                links = (uint)info.st_nlink
            };
        }
    }
}
