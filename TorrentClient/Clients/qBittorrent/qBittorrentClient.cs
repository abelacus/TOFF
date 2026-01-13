using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TorrentClient.Clients.qBittorrent.Models;
using TorrentClient.Models;

namespace TorrentClient.Clients.qBittorrent
{
    public class qBittorrentClient : ITorrentClient
    {
        public static string ClientName => "qBittorrent";

        private HttpClient _httpClient;
        private readonly TorrentClientConfig _clientConfig;
        private CookieContainer _cookieContainer;
        private HttpClientHandler _httpClientHandler;
        private bool isLoggedIn = false;


        public qBittorrentClient(TorrentClientConfig clientConfig)
        {
            _cookieContainer = new CookieContainer();
            _httpClientHandler = new HttpClientHandler() { CookieContainer = _cookieContainer };
            _httpClient = new HttpClient(_httpClientHandler) { BaseAddress = new Uri(clientConfig.ApiURL) };
            _clientConfig = clientConfig;
        }

        public async void ConnectToClient()
        {
            var loginDetails = new FormUrlEncodedContent(new[]
            {
                KeyValuePair.Create("username", _clientConfig.Username),
                KeyValuePair.Create("password", _clientConfig.Password)
            });

            using HttpResponseMessage response = await _httpClient.PostAsync("/api/v2/auth/login", loginDetails);

            response.EnsureSuccessStatusCode();

            isLoggedIn = true;
        }


        public async Task<FileDetails[]> GetFilesForTorrent(string torrentHash)
        {
            if (!isLoggedIn)
            {
                ConnectToClient();
            }

            var response = await _httpClient.GetFromJsonAsync<List<TorrentFileDetails>>("/api/v2/torrents/files?hash=" + torrentHash);

            List<FileDetails> files = new List<FileDetails>();

            response.ForEach(fileRaw =>
            {
                files.Add(new FileDetails { fileName = fileRaw.name, priority = fileRaw.priority });
            });

            return files.ToArray();
        }

        public async Task<TorrentDetails[]> GetTorrentDetails()
        {
            if (!isLoggedIn)
            {
                ConnectToClient();
            }

            var response = await _httpClient.GetFromJsonAsync<List<TorrentInfo>>("/api/v2/torrents/info");

            List<TorrentDetails> torrents = new List<TorrentDetails>();

            response.ForEach((TorrentInfo torrentRaw) =>
            {
                torrents.Add(new TorrentDetails { Hash = torrentRaw.hash, Name = torrentRaw.name, SavePath = torrentRaw.save_path });
            });

            return torrents.ToArray();
        }
    }
}
