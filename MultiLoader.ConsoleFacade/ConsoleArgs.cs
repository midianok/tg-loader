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

        public SourceType SourceType { get; }
        public string SourceRequest { get; }
        public string SavePath { get; }
        public string ValidationMessage { get; }

        private ConsoleArgs(SourceType sourceName, string sourceRequest, string savePath)
        {
            SourceType = sourceName;
            SourceRequest = sourceRequest;
            SavePath = savePath;
        }

        private ConsoleArgs(string validationMessage) =>
            ValidationMessage = validationMessage;
        

        public static bool ParseArgs(string[] args, out ConsoleArgs consoleArgsResult)
        {
            if (args.Length != 3)
            {
                consoleArgsResult = new ConsoleArgs(Examples);
                return false;
            }
            var sourceType = ParseSourceType(args[0]);
            if (sourceType == SourceType.Unknown)
            {
                consoleArgsResult = new ConsoleArgs("Unknown source type\n" + Examples);
                return false;
            }

            consoleArgsResult = new ConsoleArgs(sourceType, args[1], args[2]);
            return true; 
        }

        private static SourceType ParseSourceType(string source)
        {
            switch (source)
            {
                case "2ch":
                    return SourceType.Dvach;
                case "danbooru":
                    return SourceType.Danbooru;
                case "anonib":
                    return SourceType.AnonIb;
                case "imgur":
                    return SourceType.Imgur;
                default:
                    return SourceType.Unknown;

            }
        }
    }
}
