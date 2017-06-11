using System;
using MultiLoader.Core;
using MultiLoader.Core.Adapter;
using MultiLoader.Core.Db;
using MultiLoader.Core.Abstraction;
using System.IO;
using MultiLoader.Core.Services;

namespace MultiLoader.ConsoleFacade
{
    class Program
    {
        private const string DavachSource = "2ch";
        private const string DanbooruSource = "danbooru";
        private const string AnonIbSource = "anonib";
        private const string ImgurSource = "imgur";

        static void Main(string[] args1)
        {

            var args = "imgur LQFLj C:\\Users\\Midian".Split(' ');

            if (!ConsoleArgs.ParseArgs(args, out ConsoleArgs consoleArgs))
            {
                Console.WriteLine(consoleArgs.ValidationMessage);
                return;
            }
               
            var loader = GetLoader(consoleArgs);

            loader
                .AddOnGetContentMetadataHandler((sender, count) => Console.WriteLine($"Files to download: {count}"))
                .AddOnContentDownloadErrorHandler((sender, ex) => Console.WriteLine($"Error to obtain file list: {ex.Message}"))
                .AddOnContentDownloadErrorHandler((sender, ex) => Console.WriteLine($"Downlad error: {ex.Message}"))
                .AddOnSavedHandler((sender, content) => Console.WriteLine(content.ContentMetadata.Name))
                .AddOnSaveErrorHandler((sender, exeption) => Console.WriteLine($"Save error: {exeption.Message}"))
                .Download(consoleArgs.SourceRequest);
        }

            
        private static Loader GetLoader(ConsoleArgs consoleArgs) =>
            DependencyResolver.ResolveAndGet<Loader>(resolver =>
            {
                switch (consoleArgs.SourceName)
                {
                    case DanbooruSource:
                        resolver.RegisterType<DanbooruAdapter, IApiAdapter>();
                        resolver.RegisterType<Downloader, IContentDownloader>();
                        break;
                    case DavachSource:
                        resolver.RegisterType<DvachAdapter, IApiAdapter>();
                        resolver.RegisterType<ParallelDownloader, IContentDownloader>();
                        break;
                    case AnonIbSource:
                        resolver.RegisterType<AnonIbAdapter, IApiAdapter>();
                        resolver.RegisterType<Downloader, IContentDownloader>();
                        break;
                    case ImgurSource:
                        resolver.RegisterType<ImgurAdapter, IApiAdapter>();
                        resolver.RegisterType<ParallelDownloader, IContentDownloader>();
                        break;
                    default:
                        Console.WriteLine("Unexpected source");
                        break;
                }

                var savePath = Path.Combine(consoleArgs.SavePath, consoleArgs.SourceRequest);

                resolver
                    .RegisterType<ContentMetadataRepository, IContentMetadataRepository>("dbpath", savePath)
                    .RegisterType<FileSaver, IContentSaver>("saveDir", savePath)
                    .RegisterType<Loader>();
            });
        
    }
}