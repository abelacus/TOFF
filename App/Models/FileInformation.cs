namespace TOFF.Models
{
    internal record FileInformation
    {
        /// <summary>
        /// savePath should include the files name; qualified path
        /// </summary>
        public string SavePath { get; set; } = "";
        public DateTime CreationDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public uint Links { get; set; }

    }
}
