using System.IO;

namespace MultiLoader.ConsoleFacade
{
    public class ConsoleArgs
    {
        private const string Examples = "Request should be: [Source_Url] [savePath]";

        public string Request { get; }
        public string SavePath { get; }
        public string ValidationMessage { get; }

        private ConsoleArgs(string request, string savePath)
        {
            Request = request;
            SavePath = savePath;
        }

        private ConsoleArgs(string validationMessage) => ValidationMessage = validationMessage;

        public static bool ParseArgs(string[] args, out ConsoleArgs consoleArgsResult)
        {
            switch (args.Length)
            {
                case 2:
                    consoleArgsResult = new ConsoleArgs(args[0], args[1]);
                    return true;
                case 1:
                    consoleArgsResult = new ConsoleArgs(args[0], Directory.GetCurrentDirectory());
                    return true;
                default:
                    consoleArgsResult = new ConsoleArgs(Examples);
                    return false;
            }
        }
    }
}
