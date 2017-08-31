using System.IO;
using Microsoft.AspNetCore.Hosting;
using MultiLoader.TelegramFacade.Infrastructure;

namespace MultiLoader.TelegramFacade
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseUrls("http://localhost:5001")
                .Build();
            host.Run();
            Bot.Api.SetWebhookAsync().Wait();
        }
    }
}
