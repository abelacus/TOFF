using System;
using System.Collections.Generic;
using System.Text;
using TOFF.Models;
using TorrentClient.Models;

namespace TOFF.Services.FileInfoHandlers
{
    internal interface IFileInfoHandler
    {
        FileInformation GetFileInfo(string filePath);
    }
}
