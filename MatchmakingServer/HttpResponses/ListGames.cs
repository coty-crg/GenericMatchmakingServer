using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MatchmakingServer
{
    class ListGames : Response
    {
        private string _cache;
        private long _last_query_ticks;

        private object _lock_cache = new object();

        public override async Task Work(HttpListenerContext context)
        {
            lock (_lock_cache)
            {
                var current_tick = System.DateTime.UtcNow.Ticks;
                if (current_tick - _last_query_ticks > System.TimeSpan.TicksPerSecond * 10)
                {
                    var mmServer = MatchmakingServer.Instance;
                    var json = JsonSerializer.Serialize(mmServer.games);
                    json = $"{{\"success\":true,\"games\":{json}}}";

                    _cache = json;
                    _last_query_ticks = current_tick;
                }
            }

            await WriteResponseMessage(context, _cache);
        }
    }
}
