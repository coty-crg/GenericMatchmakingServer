using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MatchmakingServer
{
    class Root : Response
    {
        public override async Task Work(HttpListenerContext context)
        {
            var queries = Program.HttpQueries;

            var keys = new List<string>();
            foreach(var entry in queries)
            {
                keys.Add(entry.Key);
            }

            var json = JsonSerializer.Serialize(keys);
            json = $"{{\"success\":true,\"queries\":{json}}}";

            await WriteResponseMessage(context, json);
        }
    }
}
