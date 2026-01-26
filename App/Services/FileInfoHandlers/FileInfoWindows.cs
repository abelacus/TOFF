using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using TOFF.Models;
using TorrentClient.Models;

namespace TOFF.Services.FileInfoHandlers
{
    internal class FileInfoWindows : IFileInfoHandler
    {
        //windows specific
        [StructLayout(LayoutKind.Sequential)]
        struct FILE_STAT_INFORMATION
        {
            public ulong FileId;
            public ulong CreationTime;
            public ulong LastAccessTime;
            public ulong LastWriteTime;
            public ulong ChangeTime;
            public ulong AllocationSize;
            public ulong EndOfFile;
            public uint FileAttributes;
            public uint ReparseTag;
            public uint NumberOfLinks;
            public uint EffectveAccess;
        };

        enum _FILE_INFO_BY_NAME_CLASS //technically could support additional classes, but first one has everything we need
        {
            FileStatByNameInfo,
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetFileInformationByName(string FileName, _FILE_INFO_BY_NAME_CLASS FileInformationClass, out FILE_STAT_INFORMATION FileInformation, uint Size);

        public FileInformation GetFileInfo(string filePath)
        {
            GetFileInformationByName(filePath, _FILE_INFO_BY_NAME_CLASS.FileStatByNameInfo, out var info, (uint)Marshal.SizeOf<FILE_STAT_INFORMATION>());



            return new FileInformation
            {
                savePath = filePath,
                creationDate = new DateTime((int)info.CreationTime),
                lastModifiedDate = new DateTime((int)info.ChangeTime),
                links = info.NumberOfLinks,
            };

        }
    }
}
