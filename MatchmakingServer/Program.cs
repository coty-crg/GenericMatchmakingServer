using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;

namespace MatchmakingServer
{
    class Program
    {

        public static Dictionary<string, Response> HttpQueries;
        public static MatchmakingServer Matchmaking;
        public static ServerSettings settings;

        public static bool running;

        private const string settings_filename = "settings.json";

        public static void SaveServerSettings()
        {
            try
            {
                System.Text.Json.JsonSerializerOptions options = new System.Text.Json.JsonSerializerOptions();
                options.WriteIndented = true;

                var json = System.Text.Json.JsonSerializer.Serialize(settings, options);
                System.IO.File.WriteAllText(settings_filename, json);
            }
            catch (System.Exception e)
            {
                Debug.Log(e.StackTrace, Debug.LogType.Error);
                Debug.Log(e.Message, Debug.LogType.Error);
            }
        }

        static ServerSettings LoadServerSettings()
        {
            try
            {
                if (System.IO.File.Exists(settings_filename))
                {
                    var json = System.IO.File.ReadAllText(settings_filename);
                    var settings = System.Text.Json.JsonSerializer.Deserialize<ServerSettings>(json);

                    Debug.Log("Loaded settings from " + settings_filename);

                    return settings;
                }
                else
                {
                    var defaultSettings = new ServerSettings();
                    defaultSettings.server_addresses = new string[]
                    {
                        "http://127.0.0.1:8080/",
                        // "http://example.com:8000/",
                    };

                    System.Text.Json.JsonSerializerOptions options = new System.Text.Json.JsonSerializerOptions();
                    options.WriteIndented = true;

                    var json = System.Text.Json.JsonSerializer.Serialize(defaultSettings, options);
                    System.IO.File.WriteAllText(settings_filename, json);

                    Debug.Log("Wrote default settings to " + settings_filename);

                    return defaultSettings;
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message, Debug.LogType.Error);
                return new ServerSettings();
            }
        }

        static void Main(string[] args)
        {
            running = true;

            if(!RequireAdministrator())
            {
                Debug.Log("Please start as administrator.", Debug.LogType.Error);
                return;
            }

            Debug.Log("starting up..");

            settings = LoadServerSettings();

            HttpQueries = new Dictionary<string, Response>();
            SetupQueries();

            Matchmaking = new MatchmakingServer();
            Matchmaking.Initialize(settings.initial_debug_game);

            var listenerThread = new System.Threading.Thread(() => Listener(settings.server_addresses));
            listenerThread.Start();

            // schedule matchmaking prune routine 
            if (settings.prune_old_games)
            {
                Task.Run(() => Matchmaking.RemoveOldGames(settings.game_timeout_seconds));
            }

            // wait 
            var commandListener = new ConsoleCommandListener();
            commandListener.Initialize();
            commandListener.Listen(); // blocks 

            Debug.Log("shutting down.");
            running = false;
        }

        public static void SetupQueries()
        {
            HttpQueries.Clear();

            HttpQueries.Add("/", new Root());
            HttpQueries.Add("/AddGame", new AddGame());
            HttpQueries.Add("/RemoveGame", new RemoveGame());
            HttpQueries.Add("/UpdateGame", new UpdateGame());
            HttpQueries.Add("/ListGames", new ListGames());
            HttpQueries.Add("/FindGames", new FindGames());
            HttpQueries.Add("/GetMyIP", new GetMyIP());
        }

        static void Listener(string[] prefixes)
        {
            HttpListener listener = new HttpListener();

            foreach (var prefix in prefixes)
            {
                listener.Prefixes.Add(prefix);
            }

            try
            {
                listener.Start();

            }
            catch (System.Exception e)
            {
                Debug.Log(e.StackTrace, Debug.LogType.Error);
                Debug.Log(e.Message, Debug.LogType.Error);

                Debug.Log("Server failed to bind. If not running as an admin, please run as admin.", Debug.LogType.Error);
                return;
            }

            Debug.Log("Listening..", Debug.LogType.Warning);

            while (running)
            {
                try
                {
                    HttpListenerContext context = listener.GetContext();

                    var path = context.Request.Url.AbsolutePath;
                    var method = context.Request.HttpMethod;
                    var user = context.Request.UserHostAddress;

                    Debug.Log($"[{user}] {method}: {path}", Debug.LogType.Minor);

                    Response responseHandler;
                    var found = HttpQueries.TryGetValue(path, out responseHandler);

                    if (found)
                    {
                        Task.Run(() => responseHandler.HandleResponse(context));
                    }
                    else
                    {
                        HttpListenerRequest request = context.Request;
                        HttpListenerResponse response = context.Response;

                        response.StatusCode = 404; 

                        var responseString = "{}";
                        var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                        response.ContentLength64 = buffer.Length;
                        var output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                        output.Close();
                    }
                }
                catch (System.Exception e)
                {
                    Console.Write(e.StackTrace);
                    Console.Write(e.Message);

                    System.Threading.Thread.Sleep(1);
                }
            }

            listener.Stop();
        }

        [DllImport("libc")]
        public static extern uint getuid();

        public static bool RequireAdministrator()
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                    {
                        WindowsPrincipal principal = new WindowsPrincipal(identity);
                        if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
                        {
                            return false; 
                        }
                    }
                }
                else if (getuid() != 0)
                {
                    return false; 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false; 
            }

            return true; 
        }
    }
}
