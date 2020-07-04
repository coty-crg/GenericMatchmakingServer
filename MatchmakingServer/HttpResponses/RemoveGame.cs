using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MatchmakingServer
{
    class RemoveGame : Response
    {
        public override async Task Work(HttpListenerContext context)
        {
            var gameData = await GetGamedataFromBody(context);
            TryFillGamedataFromContext(context, gameData); 
            MatchmakingServer.Instance.RemoveGame(gameData.ip_address, gameData.port);
            await WriteResponseMessage(context, new ResponseMessage(true));
        }
    }
}
