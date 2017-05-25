using MultiLoader.Core;
using MultiLoader.Core.Abstraction;
using MultiLoader.Core.Adapter;
using MultiLoader.Core.Db;
using System.IO;
using System.Linq;

namespace MultiLoader.ConsoleFacade
{
    public class ConsoleArgs
    {
        private const string DavachSource = "2ch";
        private const string DanbooruSource = "danbooru";

        public string SourceName { get; }
        public string SourceRequest { get; }
        public string SavePath { get; }
        public string ValidationMessage { get; }

        private ConsoleArgs(string sourceName, string sourceRequest, string savePath)
        {
            SourceName = sourceName;
            SourceRequest = sourceRequest;
            SavePath = savePath;
        }

        private ConsoleArgs(string validationMessage) =>
            ValidationMessage = validationMessage;
        

        public static bool ParseArgs(string[] args, out ConsoleArgs consoleArgsResult)
        {
            if (args.Length != 3)
            {
                consoleArgsResult = new ConsoleArgs("Request should be: \n" + 
                                                    "[danbooru] [searchRequest] [savePath] \n" + 
                                                    "[2ch] [board_thread] [savePath]");
                return false;
            }

            consoleArgsResult = new ConsoleArgs(args[0], args[1], args[2]);
            return true; 
        }
    }
}
