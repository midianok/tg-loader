using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using MihaZupan;
using MultiLoader.TelegramFacade.Infrastructure;
using Telegram.Bot;

namespace MultiLoader.TelegramFacade
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            
            var settings = configuration.Get<Settings>();
            
            var telegramClient = string.IsNullOrWhiteSpace(settings.Proxy.Host) ?
                new TelegramBotClient(settings.TelegramBotToken) :
                new TelegramBotClient(
                    settings.TelegramBotToken, 
                    new HttpToSocks5Proxy(settings.Proxy.Host, settings.Proxy.Port, settings.Proxy.User, settings.Proxy.Password)
                );
            
            var telegramService = new TelegramService(telegramClient, settings.TelegramUsersAccess);

            telegramClient.OnMessage += (sender, update) => telegramService.ProcessMessageAsync(update.Message);
            telegramClient.StartReceiving();
            Console.WriteLine("Started");
            
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
