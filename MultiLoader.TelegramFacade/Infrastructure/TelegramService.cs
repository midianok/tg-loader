using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
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
            var multiPart = fileInfoLists.Count > 1;
            foreach (var fileInfoList in fileInfoLists)
            {
                var zipTempFilePath = Path.Combine(Directory.GetCurrentDirectory(), ContentPath, $"{contentName}");
                
                if (multiPart)
                    zipTempFilePath += $"_{counter}";

                zipTempFilePath += ".zip";
                    
                if (IOFile.Exists(zipTempFilePath)) 
                    IOFile.Delete(zipTempFilePath);

                using (var archive = ZipFile.Open(zipTempFilePath, ZipArchiveMode.Create))
                {
                    foreach (var fileInfo in fileInfoList)
                    {
                        archive.CreateEntryFromFile(fileInfo.FullName, fileInfo.Name, CompressionLevel.NoCompression);
                    }
                }

                var zipTempFileName = zipTempFilePath.Split("/").Last();
                using (var zipFile = new FileStream(zipTempFilePath, FileMode.Open))
                   await _telegramClient.SendDocumentAsync(message.Chat.Id, new InputOnlineFile(zipFile, zipTempFileName));
                
                counter++;
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
            var fileCount = 0;
            var downloadedCounter = 0;
            Message progressMessage = null;
            
            try
            {
                Loader.CreateLoader(message.Text, ContentPath)?
                    .AddOnAlreadyExistItemsFilteredHandler( async (sender, count) =>
                    {
                        if (count != 0)
                        {
                            fileCount = count;
                            await _telegramClient.SendTextMessageAsync(userId, $"{count} files to download");
                            progressMessage = await _telegramClient.SendTextMessageAsync(userId, $"{downloadedCounter}/{fileCount} files downloaded 📂");
                        }
                        else
                            await _telegramClient.SendTextMessageAsync(userId, "Nothing to download 👐");

                    })
                    .AddOnGetContentMetadataErrorHandler((sender, ex) => _telegramClient.SendTextMessageAsync(userId, $"Error to obtain file list: {ex.Message}"))
                    .AddOnContentDownloadErrorHandler((sender, ex) => _telegramClient.SendTextMessageAsync(userId, $"Item download error: {ex.Message}"))
                    .AddOnSaveErrorHandler((sender, exception) => _telegramClient.SendTextMessageAsync(userId, $"Save error: {exception.Message}"))
                    .AddOnDownloadFinishedHandler((sender, args) => _telegramClient.EditMessageTextAsync(userId, progressMessage.MessageId, $"{message.Text} download done 🎉"))
                    .AddOnSavedHandler( async (sender, content) =>
                        {
                            if (progressMessage == null)
                                return;
                            
                            Interlocked.Increment(ref downloadedCounter);
                            await _telegramClient.EditMessageTextAsync(userId, progressMessage.MessageId, $"{downloadedCounter}/{fileCount} files downloaded 📂");
                        })
                    .Download();
            }
            catch (ArgumentException argumentException)
            {
                _telegramClient.SendTextMessageAsync(userId, argumentException.Message);
            }
        }
    }
}
