using System;
using System.Collections.Generic;
using MultiLoader.Core.Model;

namespace MultiLoader.Core.Infrustructure
{
    public interface IContentSaver
    {
        event EventHandler<Content> OnSave;
        event EventHandler<Exception> OnSaveError;
        void SaveContent(Content content);
        IRepository<ContentMetadata> GetContentMetadataRepository();
        void SaveMetadata();
    }
}
