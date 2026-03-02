using System;
using System.Collections.Generic;
using System.Text;
using Transmission.API.RPC;
using TorrentClient.Models;
using Transmission.API.RPC.Entity;
using Transmission.API.RPC.Params;
using TorrentClient.Attributes;


namespace TorrentClient.Clients.Transmission
{
    /// <summary>
    /// Transmission client implementation. Currently only supports Transmission >=4.1.0.
    /// </summary>
    [ClientName("Transmission")]
    internal class TransmissionClient : TorrentClientBase
    {
        private static string? _sessionId;

        private readonly Client _client;

        public TransmissionClient(TorrentClientConfig clientConfig) : base(clientConfig)
        {
            if (!clientConfig.ApiURL.StartsWith("http")) //requests fail if method isn't included
            {
                clientConfig.ApiURL = "http://" + clientConfig.ApiURL;
            }
            if (!clientConfig.ApiURL.EndsWith("/transmission/rpc")) //transmission library doesn't add this automatically if missing
            {
                clientConfig.ApiURL = clientConfig.ApiURL + (clientConfig.ApiURL.EndsWith("/") ? "tramsission/rpc" : "/transmission/rpc");
            }

            _client = new (clientConfig.ApiURL, _sessionId, clientConfig.Username, clientConfig.Password);

            _sessionId = _client.GetSessionInformationAsync().Result.SessionId;

        }

        public override async Task ConnectToClient()
        {
            //not necessary?
            return;
        }

        public override async Task<FileDetails[]> GetFilesForTorrent(TorrentDetails torrentDetails)
        {
            TransmissionTorrents response = await _client.TorrentGetAsync([TorrentFields.ID, TorrentFields.FILES, TorrentFields.WANTED], [int.Parse(torrentDetails.Hash)]); //use wanted as that dictates whether a file is downloaded, not priority

            List<FileDetails> files = new();

            int i = 0;

            if(response.Torrents.Length == 0)
            {
                return files.ToArray();
            }

            foreach (var item in response.Torrents[0].Files)
            {
                files.Add(new() { fileName = item.Name, priority = response.Torrents[0].Wanted[i] ? 1 : 0, savePath = torrentDetails.SavePath });
                i++;
            }

            return files.ToArray();
        }

        public override async Task<TorrentDetails[]> GetTorrentDetails()
        {
            TransmissionTorrents response = await _client.TorrentGetAsync([TorrentFields.ID, TorrentFields.HASH_STRING, TorrentFields.NAME, TorrentFields.DOWNLOAD_DIR], []);

            List<TorrentDetails> torrents = new();

            foreach (var item in response.Torrents)
            {

                torrents.Add(new() { Hash = item.Id.ToString(), Name = item.Name, SavePath = item.DownloadDir });
            }

            return torrents.ToArray();
        }
    }
}
