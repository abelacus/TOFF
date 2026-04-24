namespace TorrentClient.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ClientNameAttribute : Attribute
    {
        public string Name { get; }
        public ClientNameAttribute(string name) => Name = name;

    }
}
