using System;
using MultiLoader.Core;
using MultiLoader.Core.Adapter;
using MultiLoader.Core.Db;
using MultiLoader.Core.Abstraction;
using System.IO;
using MultiLoader.Core.Model;

namespace MultiLoader.ConsoleFacade
{
    class Program
    {
        private const string DavachSource = "2ch";
        private const string DanbooruSource = "danbooru";

        static void Main(string[] args1)
        {
            var args = @"2ch e_268534 C:\Users\Midian\test".Split();

            if (!ConsoleArgs.ParseArgs(args, out ConsoleArgs consoleArgs))
                Console.WriteLine(consoleArgs.ValidationMessage);

            var loader = GetLoader(consoleArgs);

            loader
                .AddOnContentSavedHandler((sender, content) => Console.WriteLine(content.ContentMetadata.Name))
                .AddOnSaveErrorHandler((sender, content) => Console.WriteLine("something happend"))
                .Download(consoleArgs.SourceRequest);
        }

        private static Loader GetLoader(ConsoleArgs consoleArgs) =>
            DependencyResolver.ResolveAndGet<Loader>(resolver =>
            {
                switch (consoleArgs.SourceName)
                {
                    case DanbooruSource:
                        resolver.RegisterType<DanbooruAdapter, IApiAdapter>();
                        break;
                    case DavachSource:
                        resolver.RegisterType<DvachAdapter, IApiAdapter>();
                        break;
                    default:
                        Console.WriteLine("Unexpected source");
                        break;
                }

                var savePath = Path.Combine(consoleArgs.SavePath, consoleArgs.SourceRequest);
                resolver
                    .RegisterType<Downloader, IContentDownloader>()
                    .RegisterType<ContentMetadataRepository, IContentMetadataRepository>("dbpath", savePath)
                    .RegisterType<FileSaver, IContentSaver>("saveDir", savePath)
                    .RegisterType<Loader>();
            });
        
    }
}