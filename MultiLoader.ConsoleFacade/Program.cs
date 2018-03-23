using System;
using System.Threading;
using MultiLoader.Core.Infrustructure;

namespace MultiLoader.ConsoleFacade
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!ConsoleArgs.ParseArgs(args, out var consoleArgs))
            {
                Console.WriteLine(consoleArgs.ValidationMessage);
                return;
            }
            var loader = Loader.CreateLoader(consoleArgs.Request, consoleArgs.SavePath);

            var filesDownloaded = 1;
            loader
                .AddOnAlreadyExistItemsFilteredHandler((sender, count) => Console.WriteLine(count != 0 ? $"{count} files to download" : "Nothing to download"))
                .AddOnSavedHandler((sender, content) =>
                {
                    Console.Write($"\r{filesDownloaded} loaded");
                    Interlocked.Increment(ref filesDownloaded);
                })
                .AddOnDownloadFinishedHandler((sender, ex) => Console.WriteLine("\nDownload finished"))
                .AddOnGetContentMetadataErrorHandler((sender, ex) => Console.WriteLine($"Error to obtain file list: {ex.Message}"))
                .AddOnContentDownloadErrorHandler((sender, ex) => Console.WriteLine($"Item download error: {ex.Message}"))
                .AddOnSaveErrorHandler((sender, exeption) => Console.WriteLine($"Save error: {exeption.Message}"))
                .Download();
        }

    }
}