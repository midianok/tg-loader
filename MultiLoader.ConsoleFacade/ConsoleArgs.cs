using MultiLoader.Core.Model;

namespace MultiLoader.ConsoleFacade
{
    public class ConsoleArgs
    {
        private const string Examples = "Request should be: \n" +
                                        "[danbooru] [searchRequest] [savePath]\n" +
                                        "[2ch] [board_thread] [savePath]\n" +
                                        "[anonib] [board_thread] [savePath]\n" +
                                        "[imgur] [albumId] [savePath]\n";
        public string Request { get; }
        public string SavePath { get; }
        public string ValidationMessage { get; }

        private ConsoleArgs(string request, string savePath)
        {
            Request = request;
            SavePath = savePath;
        }

        private ConsoleArgs(string validationMessage) =>
            ValidationMessage = validationMessage;
        

        public static bool ParseArgs(string[] args, out ConsoleArgs consoleArgsResult)
        {
            if (args.Length != 2)
            {
                consoleArgsResult = new ConsoleArgs(Examples);
                return false;
            }

            consoleArgsResult = new ConsoleArgs(args[0], args[1]);
            return true; 
        }
    }
}
