using System.IO;
using System.IO.Compression;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MultiLoader.Core.Infrustructure;
using MultiLoader.TelegramFacade.Filters;
using MultiLoader.TelegramFacade.Infrastructure;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using IOFile = System.IO.File;

namespace MultiLoader.TelegramFacade.Controllers
{
    public class TelegramBotController : Controller
    {
        private const string ContentPath = "Content";
        private const string ListCommand = "list";
        private readonly string[] _content;
        private readonly string _allowedUser;

        public TelegramBotController(IOptions<Config> settings)
        {
            _allowedUser = settings.Value.TelegramUserAccess;
            _content = Directory
                .GetDirectories(ContentPath)
                .Select(x => x.Remove(0, ContentPath.Length + 1))
                .ToArray();
        }

        [HttpPost]
        [IpWhitelist("149.154.167.197", "149.154.167.233")]
        public IActionResult Message([FromBody]Update update)
        {
            if (_allowedUser != update.Message.Chat.Username &&
                update.Message.Type != MessageType.TextMessage) return Ok();

            var command = update.Message.Text.Replace("/", string.Empty);
            switch (command)
            {
                case var str when _content.Contains(str):
                    SendContent(update);
                    break;
                case ListCommand:
                    ShowContent(update);
                    break;
                default:
                     Download(update);
                    break;
            }

            return Ok();
        }

        private async void SendContent(Update update)
        {
            var contentName = update.Message.Text.Replace("/", string.Empty);
            var contentFolder = Path.Combine(Directory.GetCurrentDirectory(), ContentPath, contentName);
            var zipTempFile = Path.Combine(Directory.GetCurrentDirectory(), ContentPath, contentName + ".zip");

            if (IOFile.Exists(zipTempFile)) IOFile.Delete(zipTempFile);

            ZipFile.CreateFromDirectory(contentFolder, zipTempFile);
            using (var zipFile = new FileStream(zipTempFile, FileMode.Open))
                await Bot.Api.SendDocumentAsync(update.Message.Chat.Id, new FileToSend($"{contentName}", zipFile));

            IOFile.Delete(zipTempFile);
        }

        private void ShowContent(Update update)
        {
            var contentItemAsCommand = _content.Select(x => "/" + x);
            var message = string.Join("\n", contentItemAsCommand);
            Bot.Api.SendTextMessageAsync(update.Message.Chat.Id, message);
        }

        private void Download(Update update)
        {
            var userId = update.Message.Chat.Id;
            var loader = Loader.CreateLoader(update.Message.Text, ContentPath);
            if (loader == null)
            {
                Bot.Api.SendTextMessageAsync(userId, $"Bad request: {Request}");
                return;
            }

            loader
                .AddOnAlreadyExistItemsFilteredHandler((sender, count) =>
                {
                    if (count != 0)
                        Bot.Api.SendTextMessageAsync(userId, $"{count} files to download");
                    else
                        Bot.Api.SendTextMessageAsync(userId, "Nothing to download");

                })
                .AddOnGetContentMetadataErrorHandler((sender, ex) => Bot.Api.SendTextMessageAsync(userId, $"Error to obtain file list: {ex.Message}"))
                .AddOnContentDownloadErrorHandler((sender, ex) => Bot.Api.SendTextMessageAsync(userId, $"Item download error: {ex.Message}"))
                .AddOnSaveErrorHandler((sender, exeption) => Bot.Api.SendTextMessageAsync(userId, $"Save error: {exeption.Message}"))
                .AddOnDownloadFinishedHandler((sender, args) => Bot.Api.SendTextMessageAsync(userId, $"{update.Message.Text} download done"))
                .Download();
        }
    }
}
