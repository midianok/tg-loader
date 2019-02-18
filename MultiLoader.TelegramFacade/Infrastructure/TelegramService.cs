using System;
using System.Collections.Generic;
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
        private const long MaxByteFileSize = 50 * 1000000;
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
            var files = Directory.GetFiles(contentFolder).Select(x => new FileInfo(x));

            var fileInfoLists = new List<List<FileInfo>>{new List<FileInfo>()};
            foreach (var file in files)
            {
                if (file.Length > MaxByteFileSize)
                    continue;
                
                var fileInfoList = fileInfoLists.Last();
                var size = fileInfoList.Sum(x => x.Length);
                if (file.Length < MaxByteFileSize - size)
                {
                    fileInfoList.Add(file);
                }
                else
                {
                    var newFileInfoList = new List<FileInfo>{file};
                    fileInfoLists.Add(newFileInfoList);
                    
                }
            }

            var counter = 1;
            foreach (var fileInfoList in fileInfoLists)
            {
                var zipTempFile = Path.Combine(Directory.GetCurrentDirectory(), ContentPath, $"{contentName}_{counter}.zip");
                
                if (IOFile.Exists(zipTempFile)) 
                    IOFile.Delete(zipTempFile);
                
                using (var archive = ZipFile.Open(zipTempFile, ZipArchiveMode.Create))
                {
                    foreach (var fileInfo in fileInfoList)
                    {
                        archive.CreateEntryFromFile(fileInfo.FullName, fileInfo.Name, CompressionLevel.NoCompression);
                    }
                }
                
                counter++;
                
                using (var zipFile = new FileStream(zipTempFile, FileMode.Open))
                   await _telegramClient.SendDocumentAsync(message.Chat.Id, new InputOnlineFile(zipFile, $"{contentName}_{counter}.zip"));
            }
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
            try
            {
                Loader.CreateLoader(message.Text, ContentPath)?
                    .AddOnAlreadyExistItemsFilteredHandler((sender, count) =>
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
            catch (ArgumentException argumentException)
            {
                _telegramClient.SendTextMessageAsync(userId, argumentException.Message);
            }
            
            
        }
    }
}
