using System;
using MultiLoader.Core.Abstraction;
using System.IO;

namespace MultiLoader.ConsoleFacade
{
    class Program
    {
        static void Main(string[] args)
        {

            if (!ConsoleArgs.ParseArgs(args, out ConsoleArgs consoleArgs))
            {
                Console.WriteLine(consoleArgs.ValidationMessage);
                return;
            }

            var savePath = Path.Combine(consoleArgs.SavePath, consoleArgs.SourceRequest);
            var loader = Loader.CreateLoader(consoleArgs.SourceType, savePath);

            loader
                .AddOnGetContentMetadataHandler((sender, count) => Console.WriteLine($"Files to download: {count}"))
                .AddOnGetContentMetadataErrorHandler((sender, ex) => Console.WriteLine($"Error to obtain file list: {ex.Message}"))
                .AddOnContentDownloadErrorHandler((sender, ex) => Console.WriteLine($"Item download error: {ex.Message}"))
                .AddOnSavedHandler((sender, content) => Console.WriteLine(content.ContentMetadata.Name))
                .AddOnSaveErrorHandler((sender, exeption) => Console.WriteLine($"Save error: {exeption.Message}"))
                .Download(consoleArgs.SourceRequest);
        }

    }
}