using System.Net;
using System.Net.Http.Json;
using TorrentClient.Attributes;
using TorrentClient.Clients.qBittorrent.Models;
using TorrentClient.Models;

namespace TorrentClient.Clients.qBittorrent
{
    [ClientName("qBittorrent")]
    public class qBittorrentClient : TorrentClientBase
    {
        private readonly HttpClient _httpClient;
        private bool _isLoggedIn = false;

        public qBittorrentClient(TorrentClientConfig clientConfig) : base(clientConfig)
        {
            var cookieContainer = new CookieContainer();
            var httpClientHandler = new HttpClientHandler() { CookieContainer = cookieContainer };

            string apiUrl = clientConfig.ApiUrl;
            if (!apiUrl.StartsWith("http"))
            {
                apiUrl = "http://" + apiUrl;
            }

            _httpClient = new HttpClient(httpClientHandler) { BaseAddress = new Uri(apiUrl) };
        }

        public override async Task ConnectToClient()
        {
            var loginDetails = new FormUrlEncodedContent([
                KeyValuePair.Create("username", ClientConfig.Username),
                KeyValuePair.Create("password", ClientConfig.Password)
            ]);

            using HttpResponseMessage response = await _httpClient.PostAsync("/api/v2/auth/login", loginDetails);

            response.EnsureSuccessStatusCode();

            if (await response.Content.ReadAsStringAsync() == "Fails.")
            {
                _isLoggedIn = false;
                throw new Exception("Unable to login");
            }

            _isLoggedIn = true;
        }


        public override async Task<FileDetails[]> GetFilesForTorrent(TorrentDetails torrentDetails)
        {
            if (!_isLoggedIn)
            {
                await ConnectToClient();
            }

            List<TorrentFileDetails>? response = await _httpClient.GetFromJsonAsync("/api/v2/torrents/files?hash=" + torrentDetails.Hash, TorrentFileDetailsContext.Default.ListTorrentFileDetails);

            if (response == null)
            {
                throw new Exception("Unable to get torrent files");
            }
            
            List<FileDetails> files = new List<FileDetails>();

            response.ForEach(fileRaw =>
            {
                files.Add(new FileDetails { FileName = fileRaw.name, Priority = fileRaw.priority, SavePath = torrentDetails.SavePath });
            });

            return files.ToArray();
        }

        public override async Task<TorrentDetails[]> GetTorrentDetails()
        {
            if (!_isLoggedIn)
            {
                await ConnectToClient();
            }

            List<TorrentInfo>? response = await _httpClient.GetFromJsonAsync("/api/v2/torrents/info", TorrentInfoContext.Default.ListTorrentInfo);

            if (response == null)
            {
                throw new Exception("Unable to get torrent info");
            }
            
            List<TorrentDetails> torrents = new List<TorrentDetails>();

            response.ForEach((torrentRaw) =>
            {
                torrents.Add(new TorrentDetails { Hash = torrentRaw.hash, Name = torrentRaw.name, SavePath = torrentRaw.save_path });
            });

            return torrents.ToArray();
        }
    }
}
