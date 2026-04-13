using Microsoft.Extensions.Configuration;
using System.Text.Json;
using TOFF.Models;
using TorrentClient;
using TorrentClient.Models;

namespace TOFF.Services
{
    internal class AppStateService
    {
        public ITorrentClient? TorrentClient;
        public FileInformation[] FilesMissingFromClient = Array.Empty<FileInformation>();
        public FileInformation[] ToBeDeleted = Array.Empty<FileInformation>();
        public AppPreferences Preferences;

        private readonly string _configFileName = "settings.json";

        public AppStateService(TorrentClientService clientService)
        {
            var builder = new ConfigurationBuilder().AddJsonFile(_configFileName, optional: true, reloadOnChange: false).Build();

            Preferences = builder.Get<AppPreferences>() ?? new AppPreferences(clientService.GetTorrentClients()[0], new TorrentClientConfig());

            if(Preferences.TorrentClientConfig == null)
            {
                Preferences.TorrentClientConfig = new TorrentClientConfig();
            }
            if (!clientService.GetTorrentClients().Contains(Preferences.ClientSelection))
            {
                Preferences.ClientSelection = clientService.GetTorrentClients()[0];
            }

            SavePreferences();
        }
        
        public void SavePreferences()
        {
            var sourceGenOptions = new JsonSerializerOptions
            {
                TypeInfoResolver = SourceGenerationContext.Default
            };

            var json = JsonSerializer.Serialize(Preferences, sourceGenOptions);

            File.WriteAllText(_configFileName, json);
        }

    }
}
