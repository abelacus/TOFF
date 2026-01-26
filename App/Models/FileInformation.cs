using System;
using System.Collections.Generic;
using System.Text;

namespace TOFF.Models
{
    internal record FileInformation
    {
        /// <summary>
        /// savePath should include the files name; qualified path
        /// </summary>
        public string savePath { get; set; }
        public DateTime creationDate { get; set; }
        public DateTime lastModifiedDate { get; set; }
        public uint links { get; set; }

    }
}
