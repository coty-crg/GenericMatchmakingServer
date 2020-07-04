using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MatchmakingServer
{
    class AddGame : Response
    {
        public override async Task Work(HttpListenerContext context)
        {
            var gameData = await GetGamedataFromBody(context);
            TryFillGamedataFromContext(context, gameData);

            var player_ip = gameData.ip_address;
            var player_port = gameData.port;

            var connection_ok = true;

            // 
            // try
            // {
            // 
            //     if (Program.settings.print_debug)
            //     {
            //         Debug.Log($"Try connect, {player_ip}:{player_port}", Debug.LogType.Minor);
            //     }
            // 
            //     var tcpClient = new TcpClient();
            //     tcpClient.NoDelay = true;
            //     tcpClient.LingerState = new LingerOption(false, 0);
            // 
            //     var ipAddress = IPAddress.Parse(player_ip);
            //     var endpoint = new IPEndPoint(ipAddress, player_port);
            // 
            //     if (Program.settings.print_debug)
            //     {
            //         Debug.Log($"endpoint, {endpoint.ToString()}", Debug.LogType.Minor);
            //     }
            // 
            //     tcpClient.Connect(endpoint);
            // 
            //     if (Program.settings.print_debug)
            //     {
            //         Debug.Log($"Connected!", Debug.LogType.Minor);
            //     }
            // 
            //     connection_ok = true;
            // 
            //     tcpClient.Close();
            // 
            //     if (Program.settings.print_debug)
            //     {
            //         Debug.Log($"Closed!", Debug.LogType.Minor);
            //     }
            // }
            // catch(System.Exception e)
            // {
            //     Debug.Log(e.StackTrace, Debug.LogType.Error);
            //     Debug.Log(e.Message, Debug.LogType.Error);
            // }

            if(connection_ok)
            {
                MatchmakingServer.Instance.AddGame(gameData);
                await WriteResponseMessage(context, new ResponseMessage(true)); 
            }
            else
            {
                await WriteResponseMessage(context, new ResponseMessage(false, "Could not connect"));
            }
        }
    }
}
