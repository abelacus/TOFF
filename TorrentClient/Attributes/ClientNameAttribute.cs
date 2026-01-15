using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentClient.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ClientNameAttribute : Attribute
    {
        public string Name { get; }
        public ClientNameAttribute(string name) => Name = name;

    }
}
