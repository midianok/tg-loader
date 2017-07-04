using System;
using MultiLoader.Core.Abstraction;
using System.IO;
using System.Threading;
using Konsole;

namespace MultiLoader.ConsoleFacade
{
    class Program
    {
        private static ProgressBar _progressBar;
        static void Main(string[] args1)
        {
            var args = "imgur YNJhp C:\\Users\\Midian\\test".Split();
            if (!ConsoleArgs.ParseArgs(args, out ConsoleArgs consoleArgs))
            {
                Console.WriteLine(consoleArgs.ValidationMessage);
                return;
            }

            var savePath = Path.Combine(consoleArgs.SavePath, consoleArgs.SourceRequest);
            var loader = Loader.CreateLoader(consoleArgs.SourceType, savePath);

            var filesDownloaded = 1;
            loader
                .AddOnAlreadyExistItemsFilteredHandler((sender, count) =>
                {
                    if (count != 0)
                    {
                        _progressBar = new ProgressBar(count);
                        Console.WriteLine($"{count} files to download");
                    }
                    else
                    {
                        Console.WriteLine("Nothing to download");
                    }
                })
                .AddOnSavedHandler((sender, content) =>
                {
                    _progressBar.Refresh(filesDownloaded, $"{filesDownloaded} {content.ContentMetadata.Name}");
                    Interlocked.Increment(ref filesDownloaded);
                })
                .AddOnGetContentMetadataErrorHandler((sender, ex) => Console.WriteLine($"Error to obtain file list: {ex.Message}"))
                .AddOnContentDownloadErrorHandler((sender, ex) => Console.WriteLine($"Item download error: {ex.Message}"))
                .AddOnSaveErrorHandler((sender, exeption) => Console.WriteLine($"Save error: {exeption.Message}"))
                .Download(consoleArgs.SourceRequest);
        }

    }
}