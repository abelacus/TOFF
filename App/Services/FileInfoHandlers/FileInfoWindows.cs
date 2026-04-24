using System.Diagnostics;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using TOFF.Models;

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


        //apparently only available in windows 11, whoops
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetFileInformationByName(string FileName, _FILE_INFO_BY_NAME_CLASS FileInformationClass, out FILE_STAT_INFORMATION FileInformation, uint Size);

        [StructLayout(LayoutKind.Sequential)]
        struct BY_HANDLE_FILE_INFORMATION
        {
            public uint dwFileAttributes;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
            public uint dwVolumeSerialNumber;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
            public uint nNumberOfLinks;
            public uint nFileIndexHigh;
            public uint nFileIndexLow;
        };

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetFileInformationByHandle(SafeFileHandle hFile, out BY_HANDLE_FILE_INFORMATION FileInformation);

        private static bool _canGetByName = true;

        public FileInformation GetFileInfo(string filePath)
        {
            if (_canGetByName)
            {
                try //almost certainly a better way to handle this but this is fine for now i guess.
                {
                    GetFileInformationByName(filePath, _FILE_INFO_BY_NAME_CLASS.FileStatByNameInfo, out var info, (uint)Marshal.SizeOf<FILE_STAT_INFORMATION>());

                    return new FileInformation
                    {
                        SavePath = filePath,
                        CreationDate = new DateTime((int)info.CreationTime),
                        LastModifiedDate = new DateTime((int)info.ChangeTime),
                        Links = info.NumberOfLinks,
                    };
                }
                catch (Exception e) //will fail on windows 10 and earlier so need an alternative
                {
                    Debug.WriteLine("Failed to get by name, trying again with handles");
                    
                    _canGetByName = false;
                    SafeFileHandle handle = File.OpenHandle(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, FileOptions.None);

                    GetFileInformationByHandle(handle, out var info);

                    handle.Close();

                    return new FileInformation
                    {
                        SavePath = filePath,
                        CreationDate = DateTime.FromFileTime((((long)info.ftCreationTime.dwHighDateTime) << 32) | ((uint)info.ftCreationTime.dwLowDateTime)),
                        LastModifiedDate = DateTime.FromFileTime((((long)info.ftLastWriteTime.dwHighDateTime) << 32) | ((uint)info.ftLastWriteTime.dwLowDateTime)),
                        Links = info.nNumberOfLinks
                    };
                }
            }
            else
            {
                SafeFileHandle handle = File.OpenHandle(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, FileOptions.None);

                GetFileInformationByHandle(handle, out var info);

                handle.Close();

                return new FileInformation
                {
                    SavePath = filePath,
                    CreationDate = DateTime.FromFileTime((((long)info.ftCreationTime.dwHighDateTime) << 32) | ((uint)info.ftCreationTime.dwLowDateTime)),
                    LastModifiedDate = DateTime.FromFileTime((((long)info.ftLastWriteTime.dwHighDateTime) << 32) | ((uint)info.ftLastWriteTime.dwLowDateTime)),
                    Links = info.nNumberOfLinks
                };
            }

        }
    }
}
