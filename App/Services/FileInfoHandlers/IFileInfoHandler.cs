using TOFF.Models;

namespace TOFF.Services.FileInfoHandlers
{
    internal interface IFileInfoHandler
    {
        FileInformation GetFileInfo(string filePath);
    }
}
