using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MatchmakingServer
{
    class UpdateGame : Response
    {
        public override async Task Work(HttpListenerContext context)
        {
            var gameData = await GetGamedataFromBody(context);
            TryFillGamedataFromContext(context, gameData);
            var success = MatchmakingServer.Instance.UpdateGame(gameData);
            await WriteResponseMessage(context, new ResponseMessage(success)); 
        }
    }
}
