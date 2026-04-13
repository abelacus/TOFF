using System.Net;
using System.Net.Http.Json;
using TorrentClient.Attributes;
using TorrentClient.Clients.Deluge.Models;
using TorrentClient.Models;

namespace TorrentClient.Clients.Deluge
{
    [ClientName("Deluge")]
    internal class DelugeClient : TorrentClientBase
    {
        private readonly HttpClient _httpClient;
        private bool _isLoggedIn = false;
        private int _index = 0;

        public DelugeClient(TorrentClientConfig clientConfig) : base(clientConfig)
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
            var loginRequest = new RequestBase<string>
            {
                Id = _index++,
                Method = "auth.login",
                Params = [ClientConfig.Password ?? string.Empty]
            };

            using var response = await _httpClient.PostAsJsonAsync("/json", loginRequest, RequestBaseContext.Default.RequestBaseString);

            response.EnsureSuccessStatusCode();
            
            ResponseBase<bool>? rawData = await response.Content.ReadFromJsonAsync(ResponseBaseContext.Default.ResponseBaseBoolean);

            if (rawData == null || rawData.Error != null || !rawData.Result)
            {
                throw new Exception("Unable to login");
            }

            _isLoggedIn = rawData.Result;
        }

        public override async Task<FileDetails[]> GetFilesForTorrent(TorrentDetails torrentDetails)
        {
            if (!_isLoggedIn)
            {
                await ConnectToClient();
            }

            var fileRequest = new RequestBase<string>
            {
                Id = _index++,
                Method = "web.get_torrent_files",
                Params = [torrentDetails.Hash]
            };

            var response = await _httpClient.PostAsJsonAsync("/json", fileRequest, RequestBaseContext.Default.RequestBaseString);

            response.EnsureSuccessStatusCode();
            
            ResponseBase<TorrentFileDetails>? rawData = await response.Content.ReadFromJsonAsync(ResponseBaseContext.Default.ResponseBaseTorrentFileDetails);

            if (rawData == null)
            {
                throw new Exception("GetFiles request returned empty value");
            }
            
            if(rawData.Error != null)
            {
                throw new Exception(rawData.Error.message);
            }

            //actually hate this, stupid barely documented api
            List<FileDetails> files = GetFilesRecursive(rawData.Result, torrentDetails.SavePath);

            return files.ToArray();
        }

        private List<FileDetails> GetFilesRecursive(TorrentFileDetails details, string basePath)
        {
            List<FileDetails> files = new List<FileDetails>();

            foreach (var item in details.contents)
            {
                if(item.Value.type == "dir")
                {
                    files.AddRange(GetFilesRecursive(item.Value, basePath));
                    continue;
                }

                files.Add(new FileDetails { FileName = item.Key, Priority = item.Value.priority ?? 1, SavePath = Path.Join(basePath, details.path ?? "") });
            }

            return files;

        }

        public override async Task<TorrentDetails[]> GetTorrentDetails()
        {
            if (!_isLoggedIn)
            {
                await ConnectToClient();
            }

            var torrentsRequest = new RequestBase<List<string>>
            {
                Id = _index++,
                Method = "core.get_torrents_status",
                Params = [new List<string>(){}, [
                        "name",
                        "state",
                        "progress",
                        "download_location",
                        "hash"
                    ]
                ]
            };

            var response = await _httpClient.PostAsJsonAsync("/json", torrentsRequest, RequestBaseContext.Default.RequestBaseListString);

            response.EnsureSuccessStatusCode();

            ResponseBase<Dictionary<string, TorrentInfo>>? rawData = await response.Content.ReadFromJsonAsync(ResponseBaseContext.Default.ResponseBaseDictionaryStringTorrentInfo);

            if (rawData == null)
            {
                throw new Exception("Get Torrent Details request returned empty value");
            }
            
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
