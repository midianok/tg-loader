using System.IO;
using System.IO.Compression;
using System.Linq;
using MultiLoader.Core.Infrustructure;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using IOFile = System.IO.File;

namespace MultiLoader.TelegramFacade.Infrastructure
{
    public class TelegramService
    {
        private readonly TelegramBotClient _telegramClient;
        
        private const string ContentPath = "Content";
        private const string ListCommand = "list";
        private readonly string _allowedUser;

        public TelegramService(TelegramBotClient telegramClient, string allowedUser)
        {
            _telegramClient = telegramClient;
            _allowedUser = allowedUser;
            if (!Directory.Exists(ContentPath))
                Directory.CreateDirectory(ContentPath);
        }

        public void ProcessMessageAsync(Message message)
        {
            if (_allowedUser != message.Chat.Username ||
                message.Type != MessageType.Text) return;

            var command = message.Text.Replace("/", string.Empty);
            
            var content = Directory
                .GetDirectories(ContentPath)
                .Select(x => x.Remove(0, ContentPath.Length + 1))
                .ToArray();
            
            switch (command)
            {
                case var str when content.Contains(str):
                    SendContent(message);
                    break;
                case ListCommand:
                    ShowContent(message, content);
                    break;
                default:
                    Download(message);
                    break;
            }
        }
        
        private async void SendContent(Message message)
        {
            var contentName = message.Text.Replace("/", string.Empty);
            var contentFolder = Path.Combine(Directory.GetCurrentDirectory(), ContentPath, contentName);
            var zipTempFile = Path.Combine(Directory.GetCurrentDirectory(), ContentPath, contentName + ".zip");

            if (IOFile.Exists(zipTempFile)) 
                IOFile.Delete(zipTempFile);

            ZipFile.CreateFromDirectory(contentFolder, zipTempFile);
            using (var zipFile = new FileStream(zipTempFile, FileMode.Open))
                await _telegramClient.SendDocumentAsync(message.Chat.Id, new InputOnlineFile(zipFile, $"{contentName}.zip"));

            IOFile.Delete(zipTempFile);
        }

        private void ShowContent(Message message, string[] content)
        {
            var contentItemAsCommand = content.Select(x => "/" + x);
            var messageText = string.Join("\n", contentItemAsCommand);
            _telegramClient.SendTextMessageAsync(message.Chat.Id, messageText);
        }

        private void Download(Message message)
        {
            var userId = message.Chat.Id;
            var loader = Loader.CreateLoader(message.Text, ContentPath);

            loader?.AddOnAlreadyExistItemsFilteredHandler((sender, count) =>
                {
                    if (count != 0)
                        _telegramClient.SendTextMessageAsync(userId, $"{count} files to download");
                    else
                        _telegramClient.SendTextMessageAsync(userId, "Nothing to download");

                })
                .AddOnGetContentMetadataErrorHandler((sender, ex) => _telegramClient.SendTextMessageAsync(userId, $"Error to obtain file list: {ex.Message}"))
                .AddOnContentDownloadErrorHandler((sender, ex) => _telegramClient.SendTextMessageAsync(userId, $"Item download error: {ex.Message}"))
                .AddOnSaveErrorHandler((sender, exception) => _telegramClient.SendTextMessageAsync(userId, $"Save error: {exception.Message}"))
                .AddOnDownloadFinishedHandler((sender, args) => _telegramClient.SendTextMessageAsync(userId, $"{message.Text} download done"))
                .Download();
        }
    }
}
