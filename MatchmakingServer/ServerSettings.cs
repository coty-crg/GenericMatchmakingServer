using System;
using System.Collections.Generic;
using System.Text;

namespace MatchmakingServer
{
    class ServerSettings
    {
        public string[] server_addresses { get; set; }
        public int game_timeout_seconds { get; set; }
        public bool prune_old_games { get; set; }
        public bool initial_debug_game { get; set; }
        public bool print_debug { get; set; }

        public ServerSettings()
        {
            server_addresses = new string[0];
            game_timeout_seconds = 60;
            prune_old_games = false;
            initial_debug_game = true;
            print_debug = true; 
        }
    }
}
