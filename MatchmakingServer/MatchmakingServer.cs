using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MatchmakingServer
{
    class MatchmakingServer
    {
        public static MatchmakingServer Instance;

        public void Initialize(bool initial_debug_game)
        {
            Instance = this;
            games = new List<GameData>();

            // debug
            if(initial_debug_game)
            {
                games.Add(new GameData()
                {
                    server_name = "test server",
                    server_description = "dont try to connect to me!",
                    metadata = "free for all",
                    region = "NA",
                    level_name = "secret level",
                    ip_address = "127.0.0.1",
                    port = 8008,
                    player_count = 2,
                    player_maximum = 16,

                    player_names = new string[] { 
                        "coty",
                        "weston",
                    },

                    last_update = System.DateTime.UtcNow.Ticks
                });
            }
        }

        public async Task RemoveOldGames(int timeout)
        {
            while(Program.running)
            {

                var current = System.DateTime.UtcNow.Ticks;

                List<GameData> toRemove = new List<GameData>();

                foreach(var game in games)
                {
                    var ticksSinceUpdate = current - game.last_update;
                    if(ticksSinceUpdate > System.TimeSpan.TicksPerSecond * timeout)
                    {
                        toRemove.Add(game);
                    }
                }

                foreach(var remove in toRemove)
                {
                    games.Remove(remove); 
                }

                await Task.Delay(timeout);
            }
        }


        public List<GameData> games;

        public void AddGame(GameData data)
        {
            data.last_update = System.DateTime.UtcNow.Ticks; 
            games.Add(data); 
        }

        public void RemoveGame(string ip_address, int port)
        {
            var data = FindGame(ip_address, port); 
            games.Remove(data); 
        }

        public bool UpdateGame(GameData newData)
        {
            var exstingGame = FindGame(newData.ip_address, newData.port);
            if(exstingGame != null)
            {
                RemoveGame(newData.ip_address, newData.port);
                AddGame(newData);
                return true;
            }
            else
            {
                return false; 
            }
        }

        public GameData FindGame(string ip_address, int port)
        {
            foreach(var game in games)
            {
                if(game.ip_address == ip_address && game.port == port)
                {
                    return game; 
                }
            }

            return null; 
        }
    }
}
