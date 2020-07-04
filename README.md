# GenericMatchmakingServer
A generic c# matchmaking server.

## How it works
It runs a simple http server locally, which your game can send requests to for adding games, removing, or getting a list of existing ones. A list of queries is in `Program.cs` in the function `SetupQueries()`.

## Build from source
Just open in Visual Studio 2019 and build. Outputs to `MatchmakingServer\bin\Release\netcoreapp3.1\<platform>`

## Settings 
In Program.cs, there's a function LoadServerSettings(), which has the initial settings for server_addresses. You can either add in your info here, or if the defaults have already been created, you can add it into your `settings.json`.
```cs
var defaultSettings = new ServerSettings();
defaultSettings.server_addresses = new string[]
{
    "http://127.0.0.1:8080/",
    // "http://example.com:8000/",
};
```

