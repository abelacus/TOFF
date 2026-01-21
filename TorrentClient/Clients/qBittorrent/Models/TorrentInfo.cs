using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TorrentClient.Clients.qBittorrent.Models
{
    internal record TorrentInfo
    {
        public int added_on { get; init; }
        public ulong amount_left { get; init; }
        public bool auto_tmm { get; init; }
        public float availability { get; init; }
        public string category { get; init; }
        public ulong completed { get; init; }
        public int completion_on { get; init; }
        public string content_path { get; init; }
        public int dl_limit { get; init; }
        public int dlspeed { get; init; }
        public ulong downloaded { get; init; }
        public ulong downloaded_session { get; init; }
        public int eta { get; init; }
        public bool f_l_piece_prio { get; init; }
        public bool force_Start { get; init; }
        public string hash { get; init; }
        public bool isPrivate { get; init; }
        public int last_activity { get; init; }
        public string magnet_uri { get; init; }
        public float max_ratio { get; init; }
        public int max_seeding_time { get; init; }
        public string name { get; init; }
        public int num_complete { get; init; }
        public int num_incomplete { get; init; }
        public int num_leechs { get; init; }
        public int num_seeds { get; init; }
        public int priority { get; init; }
        public float progress { get; init; }
        public float ratio { get; init; }
        public float ratio_limit { get; init; }
        public string save_path { get; init; }
        public int seeding_time { get; init; }
        public int seeding_time_limit { get; init; }
        public int seen_complete { get; init; }
        public bool seq_dl { get; init; }
        public ulong size { get; init; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TorrentStates state { get; init; }
        public bool super_seeding { get; init; }
        public string tags { get; init; }
        public int time_active { get; init; }
        public ulong total_size { get; init; }
        public string tracker { get; init; }
        public int up_limit { get; init; }
        public ulong uploaded { get; init; }
        public ulong uploaded_session { get; init; }
        public int upspeed { get; init; }
    }
}
