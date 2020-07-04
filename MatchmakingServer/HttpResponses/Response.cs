using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MatchmakingServer
{
    [System.Serializable]
    public class ResponseMessage
    {
        public bool success { get; set; }
        public string errorMessage { get; set; }

        public ResponseMessage(bool success)
        {
            this.success = success;
            this.errorMessage = string.Empty;
        }

        public ResponseMessage(bool success, string errorMessage)
        {
            this.success = success;
            this.errorMessage = errorMessage;
        }
    }

    abstract class Response
    {
        public async Task HandleResponse(HttpListenerContext context)
        {
            try
            {
                await Work(context);
            }
            catch(System.Exception e)
            {
                Debug.Log(e.StackTrace, Debug.LogType.Error);
                Debug.Log(e.Message, Debug.LogType.Error);
                Debug.Log("");

                await WriteResponseMessage(context, new ResponseMessage(false, e.Message)); 
            }
        }

        public virtual async Task Work(HttpListenerContext context)
        {

        }

        protected void TryFillGamedataFromContext(HttpListenerContext context, GameData gameData)
        {
            // note, only fills address, not the port! 
            if (string.IsNullOrEmpty(gameData.ip_address))
            {
                var user = context.Request.UserHostAddress;
                var endpoint = IPEndPoint.Parse(user);
                gameData.ip_address = endpoint.Address.ToString();

                if(Program.settings.print_debug)
                {
                    Debug.Log($"[debug]: gameData.ip_address = {gameData.ip_address}", Debug.LogType.Minor);
                }
            }
        }

        public async Task WriteResponseMessage(HttpListenerContext context, ResponseMessage message)
        {
            var json = JsonSerializer.Serialize(message);
            await WriteResponseMessage(context, json);
        }

        public async Task WriteResponseMessageErrorMethod(HttpListenerContext context, string expectedMethod)
        {
            var method = new ResponseMessage(false, $"This is a {expectedMethod} request, but you submitted a {context.Request.HttpMethod}.");
            var json = JsonSerializer.Serialize(method);
            await WriteResponseMessage(context, json);
        }

        public async Task WriteResponseMessage(HttpListenerContext context, string message)
        {
            var response = context.Response;
            
            var buffer = System.Text.Encoding.UTF8.GetBytes(message);
            response.ContentLength64 = buffer.Length;

            var output = response.OutputStream;
            await output.WriteAsync(buffer, 0, buffer.Length);
            output.Close();
        }

        public async Task<GameData> GetGamedataFromBody(HttpListenerContext context)
        {
            var inputStream = context.Request.InputStream;

            var charSize = sizeof(char);
            var maxStringLength = 16000;
            var maxByteLength = maxStringLength * charSize;

            var contentBuffer = new byte[maxByteLength];
            var contentLength = await inputStream.ReadAsync(contentBuffer);
            var contentString = Encoding.UTF8.GetString(contentBuffer, 0, contentLength);

            if(Program.settings.print_debug)
            {
                Debug.Log("Received: " + contentString, Debug.LogType.Minor); 
            }

            var options = new JsonSerializerOptions();
            options.AllowTrailingCommas = true;

            var gameData = JsonSerializer.Deserialize<GameData>(contentString, options);
            return gameData;
        }

        public async Task<Dictionary<string, string>> GetKeyValuePairFromBody(HttpListenerContext context)
        {
            var inputStream = context.Request.InputStream;

            var charSize = sizeof(char);
            var maxStringLength = 16000;
            var maxByteLength = maxStringLength * charSize;

            var contentBuffer = new byte[maxByteLength];
            var contentLength = await inputStream.ReadAsync(contentBuffer);
            var contentString = Encoding.UTF8.GetString(contentBuffer, 0, contentLength);

            var keyValuePairs = new Dictionary<string, string>();
            var contentPairs = contentString.Split('&');
            foreach (var contentPair in contentPairs)
            {
                var pairString = contentPair.Split('=');
                keyValuePairs.Add(pairString[0], pairString[1]);
            }

            return keyValuePairs;
        }
    }
}
