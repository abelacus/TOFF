using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentClient.Clients.qBittorrent.Models
{
    public internal record TorrentInfo
    {
        public int added_on;
        public int amount_left;
        public bool auto_tmm;
        public float availability;
        public string category;
        public int completed;
        public int completion_on;
        public string content_path;
        public int dl_limit;
        public int dlspeed;
        public int downloaded;
        public int downloaded_session;
        public int eta;
        public bool f_l_pice_prio;
        public bool force_Start;
        public string hash;
        public bool isPrivate;
        public int last_activity;
        public string magnet_uri;
        public float max_ratio;
        public int max_seeding_time;
        public string name;
        public int num_complete;
        public int num_incomplete;
        public int num_leechs;
        public int num_seeds;
        public int priority;
        public float progress;
        public float ratio;
        public float ratio_limit;
        public string save_path;
        public int seeding_time;
        public int seeding_time_limit;
        public int seen_complete;
        public bool seq_dl;
        public int size;
        public TorrentStates state;
        public bool super_seeding;
        public string tags;
        public int time_active;
        public int total_size;
        public string tracker;
        public int up_limit;
        public int uploaded;
        public int uploaded_session;
        public int upspeed;
    }
}
