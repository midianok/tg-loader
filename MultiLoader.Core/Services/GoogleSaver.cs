using System;
using System.IO;
using System.Linq;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using MultiLoader.Core.Db;
using MultiLoader.Core.Infrustructure;
using MultiLoader.Core.Model;
using MultiLoader.Core.Tool;
using File = System.IO.File;
using GoogleFile = Google.Apis.Drive.v3.Data.File;

namespace MultiLoader.Core.Services
{
    public class GoogleSaver : IContentSaver
    {
        private const string User = "user";
        private readonly string[] _scopes = { DriveService.Scope.Drive };
        private readonly DriveService _driveService;
        private readonly string _tempFolderPath;
        private readonly string _folderName;
        private readonly string _folderId;
        
        public GoogleSaver(string savePath)
        {
            _folderName = savePath.Split('\\').Last();
            _tempFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Temp", _folderName);
            
            var secrets =Disposable.Using(
                () => new FileStream("client_secret.json", FileMode.Open, FileAccess.Read),
                stream => GoogleClientSecrets.Load(stream).Secrets);

            var credFolder = new FileDataStore(Directory.GetCurrentDirectory(), true);
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(secrets, _scopes, User, CancellationToken.None, credFolder).Result;
            
            _driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Simple Password Manager",
            });
            
            var folderRequest = _driveService.Files.List();
            folderRequest.Q = $"mimeType = 'application/vnd.google-apps.folder' and name = '{_folderName}'";
            var folderRequsetResult = folderRequest.Execute();

            
            if (!folderRequsetResult.Files.Any()) 
            {
                var id = _driveService.Files.GenerateIds().Execute().Ids.First();
                var newFolder = new GoogleFile
                {
                    Id = id,
                    Name = _folderName,
                    MimeType = "application/vnd.google-apps.folder"
                };
                _driveService.Files.Create(newFolder).Execute();
                _folderId = newFolder.Id;
            }
            else
            {
                _folderId = folderRequsetResult.Files.First().Id;
            }
            
        }

        public event EventHandler<Content> OnSave;
        public event EventHandler<Exception> OnSaveError;
        
        public void SaveContent(Content content)
        {
            var file = new GoogleFile
            {
                Name = content.ContentMetadata.Name,
                MimeType = "image/jpeg",
                Parents = new[] {_folderId}
            };
            using (var ms = new MemoryStream(content.Data))
            {
                _driveService.Files.Create(file, ms, "image/jpeg").Upload();
            }
            
            OnSave?.Invoke(this, content);
        }

        public IRepository<ContentMetadata> GetContentMetadataRepository()
        {
            var contentMetadataStorageRequest = _driveService.Files.List();
            contentMetadataStorageRequest.Q = $"'{_folderId}' in parents and name = 'metadata.db'";
            var contentMetadataStorageResult = contentMetadataStorageRequest.Execute();

            if (!contentMetadataStorageResult.Files.Any())
                return CreateNewDb();
                
            var dbFile = contentMetadataStorageResult.Files.FirstOrDefault();
            if (dbFile != null)
            {
                var getFileResult = _driveService.Files.Get(dbFile.Id);
            
                using (var ms = new MemoryStream())
                {
                    getFileResult.Download(ms);
                    if (!Directory.Exists(_tempFolderPath))
                        Directory.CreateDirectory(_tempFolderPath);
                    
                    File.WriteAllBytes($"{_tempFolderPath}\\{dbFile.Name}", ms.ToArray());
                }
            }
            return new LiteDbRepository<ContentMetadata>(_tempFolderPath);
        }

        private IRepository<ContentMetadata> CreateNewDb()
        {
            var dbFile = new GoogleFile
            {
                Name = "metadata.db",
                Parents = new[] {_folderId}
            };
            _driveService.Files.Create(dbFile).Execute();
            return new LiteDbRepository<ContentMetadata>(_tempFolderPath);
        }

        public void SaveMetadata()
        {            
            var contentMetadataStorageRequest = _driveService.Files.List();
            contentMetadataStorageRequest.Q = $"'{_folderId}' in parents and name = 'metadata.db'";
            var contentMetadataStorageResult = contentMetadataStorageRequest.Execute();
            var dbFile = contentMetadataStorageResult.Files.FirstOrDefault();
            
            var db = File.ReadAllBytes($"{_tempFolderPath}\\metadata.db");
            using (var ms = new MemoryStream(db))
            {
               var updateRequest = _driveService.Files.Update(new GoogleFile(), dbFile.Id, ms, "application/octet-stream");
                updateRequest.Upload();
            }
            Directory.Delete(_tempFolderPath, true);
            
        }
    }
}