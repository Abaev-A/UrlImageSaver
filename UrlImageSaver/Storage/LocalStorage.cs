using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using UrlImageSaver.Application.Models;

namespace UrlImageSaver.Application.Storage
{
    class LocalStorage : IStorage
    {
        #region Fields
        private readonly LocalStorageOptions _options;
        private readonly ILogger<LocalStorage> _logger;
        #endregion

        #region ctor
        public LocalStorage(IOptions<LocalStorageOptions> localStoreOptions, ILoggerFactory loggerFactory)
        {
            _options = localStoreOptions.Value;
            _logger = loggerFactory.CreateLogger<LocalStorage>();
        }
        #endregion

        #region IStorage
        public void Store(string name, byte[] content)
        {
            if (!Directory.Exists(_options.LocalPath))
            {
                if (_options.ShouldCreateFolder)
                    Directory.CreateDirectory(_options.LocalPath);
                else
                {
                    _logger.LogError($"Directore \"{_options.LocalPath}\" does not exists");
                    return;
                }
            }

            var filePath = Path.Combine(_options.LocalPath, name);

            if (File.Exists(filePath))
                filePath = Path.Combine(_options.LocalPath, Guid.NewGuid().ToString() + Path.GetExtension(name));

            using (var fileStream = new FileStream(filePath, FileMode.CreateNew))             
                fileStream.Write(content, 0, content.Length);             

            _logger.LogDebug($"File saved successfully {filePath}");
        }
        #endregion
    }
}
