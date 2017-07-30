using System;
using System.Threading;
using Konsole;
using MultiLoader.Core.Infrustructure;

namespace MultiLoader.ConsoleFacade
{
    class Program
    {
        private static ProgressBar _progressBar;
        static void Main(string[] args1)
        {
            var args = "http://imgur.com/a/uAFvn D:\\test".Split();
            if (!ConsoleArgs.ParseArgs(args, out ConsoleArgs consoleArgs))
            {
                Console.WriteLine(consoleArgs.ValidationMessage);
                return;
            }
            var loader = Loader.CreateLoader(consoleArgs.Request, consoleArgs.SavePath);

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
                .Download();
        }

    }
}