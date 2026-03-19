using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TOFF.Models;
using TorrentClient;
using TorrentClient.Models;

namespace TOFF.Services
{
    internal class AppStateService
    {
        public ITorrentClient? torrentClient;
        public FileInformation[] filesMissingFromClient = Array.Empty<FileInformation>();
        public FileInformation[] toBeDeleted = Array.Empty<FileInformation>();
        public AppPreferences preferences;

        private readonly string configFileName = "settings.json";

        public AppStateService(TorrentClientService clientService)
        {
            var builder = new ConfigurationBuilder().AddJsonFile(configFileName, optional: true, reloadOnChange: false).Build();

            preferences = builder.Get<AppPreferences>() ?? new AppPreferences(clientService.GetTorrentClients()[0], new TorrentClientConfig());

            if(preferences.torrentClientConfig == null)
            {
                preferences.torrentClientConfig = new TorrentClientConfig();
            }
            if (!clientService.GetTorrentClients().Contains(preferences.clientSelection))
            {
                preferences.clientSelection = clientService.GetTorrentClients()[0];
            }

            SavePreferences();
        }
        
        public void SavePreferences()
        {
            var sourceGenOptions = new JsonSerializerOptions
            {
                TypeInfoResolver = SourceGenerationContext.Default
            };

            var json = JsonSerializer.Serialize(preferences, sourceGenOptions);

            File.WriteAllText(configFileName, json);
        }

    }
}
