using System;
using System.Collections.Generic;
using System.Text;

namespace MatchmakingServer
{
    public static class Debug
    {
        public enum LogType
        {
            Log,
            Minor,
            Warning,
            Error,
        }

        public static void Log(string log, LogType type = LogType.Log)
        {
            switch (type)
            {
                case LogType.Log:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogType.Minor:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case LogType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }

            Console.WriteLine(log);
        }


    }
}
