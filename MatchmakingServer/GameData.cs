using System;
using System.Collections.Generic;
using System.Text;

namespace MatchmakingServer
{
    public class GameData
    {
        // for clients 
        public string server_name { get; set; }
        public string server_description { get; set; }
        public string metadata { get; set; }
        public string region { get; set; }
        public string level_name { get; set; }
        public string ip_address { get; set; }
        public int port { get; set; }
        public int player_count { get; set; }
        public int player_maximum { get; set; }
        public string[] player_names { get; set; }

        // for server
        public long last_update { get; set; }
    }
}
