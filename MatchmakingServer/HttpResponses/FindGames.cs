using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;

namespace MatchmakingServer
{
    class FindGames : Response
    {
        public override async Task Work(HttpListenerContext context)
        {
            var queryGameData = await GetGamedataFromBody(context);

            var mmServer = MatchmakingServer.Instance;
            var count = mmServer.games.Count;

            List<GameData> found;

            if(!string.IsNullOrEmpty(queryGameData.metadata))
            {
                found = mmServer.games.FindAll(game => game.metadata.Contains(queryGameData.metadata, StringComparison.OrdinalIgnoreCase));
            }
            else  if(!string.IsNullOrEmpty(queryGameData.level_name))
            {
                found = mmServer.games.FindAll(game => game.level_name.Contains(queryGameData.level_name, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                await WriteResponseMessage(context, new ResponseMessage(false, "FindGames needs either metadata or level_name to be filled."));
                return;
            }

            var json = JsonSerializer.Serialize(found);
                json = $"{{\"success\":true,\"games\":{json}}}";

            await WriteResponseMessage(context, json);
        }
    }
}
