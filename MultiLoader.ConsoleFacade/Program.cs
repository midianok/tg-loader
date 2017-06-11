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
            var args = @"2ch e_268534 C:\Users\Midian\t".Split();
            if (!ConsoleArgs.ParseArgs(args, out ConsoleArgs consoleArgs))
            {
                Console.WriteLine(consoleArgs.ValidationMessage);
                return;
            }
               
            var loader = GetLoader(consoleArgs);

            loader
                .AddOnGetContentMetadataHandler((sender, count) => Console.WriteLine($"Files to download: {count}"))
                .AddOnGetContentMetadataErrorHandler((sender, ex) => Console.WriteLine($"Error to obtain file list: {ex.Message}"))
                .AddOnContentDownloadErrorHandler((sender, ex) => Console.WriteLine($"Item download error: {ex.Message}"))
                .AddOnSavedHandler((sender, content) => Console.WriteLine(content.ContentMetadata.Name))
                .AddOnSaveErrorHandler((sender, exeption) => Console.WriteLine($"Save error: {exeption.Message}"))
                .Download(consoleArgs.SourceRequest);
        }

            
        private static LoaderBase GetLoader(ConsoleArgs consoleArgs) =>
            DependencyResolver.ResolveAndGet<LoaderBase>(resolver =>
            {
                switch (consoleArgs.SourceName)
                {
                    case DanbooruSource:
                        resolver.RegisterType<DanbooruAdapter, IApiAdapter>()
                                .RegisterType<SynchronousLoader, LoaderBase>();
                        break;
                    case DavachSource:
                        resolver.RegisterType<DvachAdapter, IApiAdapter>()
                                .RegisterType<ParallelLoader, LoaderBase>();
                        break;
                    case AnonIbSource:
                        resolver.RegisterType<AnonIbAdapter, IApiAdapter>()
                                .RegisterType<ParallelLoader, LoaderBase>();
                        break;
                    case ImgurSource:
                        resolver.RegisterType<ImgurAdapter, IApiAdapter>()
                                .RegisterType<ParallelLoader, LoaderBase>();
                        break;
                    default:
                        Console.WriteLine("Unexpected source");
                        break;
                }

                var savePath = Path.Combine(consoleArgs.SavePath, consoleArgs.SourceRequest);

                resolver
                    .RegisterType<ContentMetadataRepository, IContentMetadataRepository>("dbpath", savePath)
                    .RegisterType<Downloader, IContentDownloader>()
                    .RegisterType<FileSaver, IContentSaver>("saveDir", savePath);
            });
        
    }
}