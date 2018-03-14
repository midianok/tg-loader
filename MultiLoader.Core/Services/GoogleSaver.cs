using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using MultiLoader.Core.Db;
using MultiLoader.Core.Infrustructure;
using MultiLoader.Core.Model;
using MultiLoader.Core.Tool;
using GoogleFile = Google.Apis.Drive.v3.Data.File;

namespace MultiLoader.Core.Services
{
    public class GoogleSaver : IContentSaver
    {
        public event EventHandler<Content> OnSave;
        public event EventHandler<Exception> OnSaveError;
        
        private const string User = "user";
        private readonly string[] _scopes = { DriveService.Scope.Drive };
        private readonly DriveService _driveService;
        private readonly string _folderName;
        private readonly string _folderId;
        private readonly string _appFilesPath;
        
        public GoogleSaver(string savePath)
        {
            _folderName = savePath;
            
            var secretPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "client_secret.json");
            var secrets = Disposable.Using(
                () => new FileStream(secretPath, FileMode.Open, FileAccess.Read),
                stream => GoogleClientSecrets.Load(stream).Secrets);

            _appFilesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ml");;
            var credFolder = new FileDataStore(_appFilesPath, true);
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

        public IRepository<ContentMetadata> GetContentMetadataRepository() => 
            new LiteDbRepository<ContentMetadata>(Path.Combine(_appFilesPath, "DriveMetadata"), $"{_folderName}.db");
    }
}