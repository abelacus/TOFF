using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TorrentClient;
using TorrentClient.Attributes;
using TorrentClient.Models;

namespace TOFF.Services
{
    internal class TorrentClientService
    {
        private Dictionary<string, Type> _clientDictionary = new();

        public TorrentClientService()
        {
            var implementations = typeof(TorrentClientBase).Assembly.GetTypes()
                .Where(t => typeof(TorrentClientBase).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

            if (implementations == null)
            {
                throw new Exception("No torrent client implementations have been defined");
            }

            foreach (var type in implementations)
            {
                var nameAttribute = type.GetCustomAttribute<ClientNameAttribute>();
                string name = nameAttribute.Name ?? type.Name + " undefined attribute";

                _clientDictionary[name] = type;
            }
        }

        public string[] GetTorrentClients()
        {
            return _clientDictionary.Keys.ToArray();
        }

        public ITorrentClient CreateClientInstance(string clientName, TorrentClientConfig clientConfig)
        {
            if(_clientDictionary.TryGetValue(clientName, out var type))
            {
                return Activator.CreateInstance(type, clientConfig) as ITorrentClient;
            }
            throw new ArgumentException(clientName + " is not a defined client");
        }

    }
}
