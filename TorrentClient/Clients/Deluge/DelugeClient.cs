using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TorrentClient.Attributes;
using TorrentClient.Clients.Deluge.Models;
using TorrentClient.Models;

namespace TorrentClient.Clients.Deluge
{
    [ClientName("Deluge")]
    internal class DelugeClient : TorrentClientBase
    {
        private HttpClient _httpClient;
        private CookieContainer _cookieContainer;
        private HttpClientHandler _httpClientHandler;
        private bool isLoggedIn = false;
        private int index = 0;

        public DelugeClient(TorrentClientConfig clientConfig) : base(clientConfig)
        {
            _cookieContainer = new CookieContainer();
            _httpClientHandler = new HttpClientHandler() { CookieContainer = _cookieContainer };

            string apiUrl = clientConfig.ApiURL;
            if (!apiUrl.StartsWith("http"))
            {
                apiUrl = "http://" + apiUrl;
            }

            _httpClient = new HttpClient(_httpClientHandler) { BaseAddress = new Uri(apiUrl) };
        }

        public override async Task ConnectToClient()
        {
            var loginRequest = new RequestBase<string>
            {
                Id = index++,
                Method = "auth.login",
                Params = new List<string>() { _clientConfig.Password }
            };

            using HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/json", loginRequest, RequestBaseContext.Default.RequestBaseString);

            ResponseBase<bool> rawData = await response.Content.ReadFromJsonAsync(ResponseBaseContext.Default.ResponseBaseBoolean);

            if (rawData.Error != null)
            {
                throw new Exception(rawData.Error.message);
            }

            isLoggedIn = true;
        }

        public override async Task<FileDetails[]> GetFilesForTorrent(TorrentDetails torrentDetails)
        {
            if (!isLoggedIn)
            {
                ConnectToClient();
            }

            var fileRequest = new RequestBase<string>
            {
                Id = index++,
                Method = "web.get_torrent_files",
                Params = new List<string>() { torrentDetails.Hash }
            };

            var response = await _httpClient.PostAsJsonAsync("/json", fileRequest, RequestBaseContext.Default.RequestBaseString);

            ResponseBase<TorrentFileDetails> rawData = await response.Content.ReadFromJsonAsync(ResponseBaseContext.Default.ResponseBaseTorrentFileDetails);

            if(rawData.Error != null)
            {
                throw new Exception(rawData.Error.message);
            }

            //actually hate this, stupid barely documented api
            List<FileDetails> files = getFilesRecursive(rawData.Result, torrentDetails.SavePath);

            return files.ToArray();
        }

        private List<FileDetails> getFilesRecursive(TorrentFileDetails details, string? basePath)
        {
            List<FileDetails> files = new List<FileDetails>();

            foreach (var item in details.contents)
            {
                if(item.Value.type == "dir")
                {
                    files.AddRange(getFilesRecursive(item.Value, basePath));
                    continue;
                }
                else
                {
                    files.Add(new FileDetails { fileName = item.Key, priority = (int)item.Value.priority, savePath = Path.Join(basePath, details.path ?? "") });
                }
            }

            return files;

        }

        public override async Task<TorrentDetails[]> GetTorrentDetails()
        {
            if (!isLoggedIn)
            {
                ConnectToClient();
            }

            var torrentsRequest = new RequestBase<List<string>>
            {
                Id = index++,
                Method = "core.get_torrents_status",
                Params = [new List<string>(){}, new List<string>() { //method expects an entry with two arrays, second of which contains the fields we want, first can be empty
                    "name",
                    "state",
                    "progress",
                    "download_location",
                    "hash",
                }]
            };

            var response = await _httpClient.PostAsJsonAsync("/json", torrentsRequest, RequestBaseContext.Default.RequestBaseListString);

            response.EnsureSuccessStatusCode();

            ResponseBase<Dictionary<string,TorrentInfo>> rawData = await response.Content.ReadFromJsonAsync(ResponseBaseContext.Default.ResponseBaseDictionaryStringTorrentInfo);

            if (rawData.Error != null)
            {
                throw new Exception(rawData.Error.message);
            }

            List<TorrentDetails> torrents = new List<TorrentDetails>();

            foreach(var item in rawData.Result)
            {
                torrents.Add(new TorrentDetails { Hash = item.Value.hash, Name = item.Value.name, SavePath = item.Value.download_location });
            }
            
            return torrents.ToArray();
        }
    }
}
