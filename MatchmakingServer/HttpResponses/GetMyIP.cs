using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MatchmakingServer
{
    class GetMyIP : Response
    {
        public override async Task Work(HttpListenerContext context)
        {
            var endpoint = context.Request.RemoteEndPoint;
            var json = $"{{\"success\":true,\"ip_address\":\"{endpoint.Address.ToString()}\"}}";

            if(Program.settings.print_debug)
            {
                Debug.Log(json, Debug.LogType.Minor); 
            }

            await WriteResponseMessage(context, json);
        }
    }
}
