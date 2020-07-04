using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MatchmakingServer
{
    class ConsoleCommandListener
    {
        public Dictionary<string, System.Action<string[]>> Commands;

        public void Initialize()
        {
            Commands = new Dictionary<string, Action<string[]>>();
            Commands.Add("shutdown", Shutdown); 
            Commands.Add("cleargames", ClearGames); 
            Commands.Add("games", ListGames); 
            Commands.Add("adddummygame", AddDummyGame);
            Commands.Add("savesettings", SaveSettings);
            Commands.Add("modifysetting", ModifySetting);
            Commands.Add("help", Help);
        }

        private void Shutdown(string[] args)
        {
            Debug.Log("Shutting down."); 
            Program.running = false;
        }

        private void ClearGames(string[] args)
        {
            MatchmakingServer.Instance.games.Clear();
            ListGames(args); 
        }

        private void ModifySetting(string[] args)
        {
            if(args.Length < 3)
            {
                Debug.Log("not enough arguments", Debug.LogType.Error);
                return; 
            }

            var replaceKey = args[1];
            var replaceValue = args[2];

            var settingsType = typeof(ServerSettings);
            var property = settingsType.GetProperty(replaceKey);
            if(property != null)
            {
                if(property.PropertyType.Equals(typeof(string)))
                {
                    property.SetValue(Program.settings, replaceValue);
                    Debug.Log("set!");
                }
                else if (property.PropertyType.Equals(typeof(int)))
                {
                    property.SetValue(Program.settings, int.Parse(replaceValue));
                    Debug.Log("set!");
                }
                else if (property.PropertyType.Equals(typeof(bool)))
                {
                    property.SetValue(Program.settings, bool.Parse(replaceValue));
                    Debug.Log("set!");
                }
                else
                {
                    Debug.Log("unsupported property type", Debug.LogType.Error);
                }
            }
            else
            {
                Debug.Log("setting not found", Debug.LogType.Error); 
            }
        }

        private void SaveSettings(string[] args)
        {
            Program.SaveServerSettings();
            Debug.Log("saved.");
        }

        private void AddDummyGame(string[] args)
        {
            MatchmakingServer.Instance.games.Add(new GameData());
            Debug.Log("Dummy game added!");
        }

        private void Help(string[] args)
        {
            Debug.Log("Commands: ");
            foreach (var entry in Commands)
            {
                Debug.Log("    " + entry.Key);
            }
        }

        private void ListGames(string[] args)
        {
            var mmServer = MatchmakingServer.Instance;

            var options = new JsonSerializerOptions();
            options.WriteIndented = true;

            var json = JsonSerializer.Serialize(mmServer.games, options);
            json = $"{{\"games\":{json}}}";
            Debug.Log(json);
        }

        // blocks 
        public void Listen()
        {

            while(Program.running)
            {
                try
                {
                    var input = Console.In.ReadLine();
                    var args = input.Split(' ');

                    if(args.Length == 0)
                    {
                        continue;
                    }

                    var command = args[0];

                    System.Action<string[]> action;
                    var found = Commands.TryGetValue(command, out action);
                    if(found)
                    {
                        action.Invoke(args); 
                    }
                    else
                    {
                        Debug.Log("Command not found.", Debug.LogType.Error); 
                    }
                }
                catch(System.Exception e)
                {
                    Debug.Log(e.StackTrace, Debug.LogType.Error);
                    Debug.Log(e.Message, Debug.LogType.Error);
                }

                System.Threading.Thread.Sleep(1); 
            }


        }
    }
}
