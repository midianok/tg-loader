using System;
using System.Collections.Generic;
using MultiLoader.Core.Model;

namespace MultiLoader.Core.Abstraction
{
    public interface IContentSaver
    {
        event EventHandler<Content> OnSave;
        event EventHandler<Exception> OnSaveError;
        void SaveContent(Content content);
        IEnumerable<string> GetFileNames(string request);
    }
}
