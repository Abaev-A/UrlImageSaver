using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using UrlImageSaver.Application.Models;

namespace UrlImageSaver.Application.Storage
{
    class StorageManager : IStorageManager
    {
        #region Fields
        private readonly AppOptions _appOptions;
        private readonly IServiceProvider _serviceProvider;
        #endregion

        #region ctor
        public StorageManager(IOptions<AppOptions> appOptions, IServiceProvider serviceProvider)
        {
            _appOptions = appOptions.Value; 
            _serviceProvider = serviceProvider;
        }
        #endregion

        #region IStorageManager
        public List<IStorage> GetStorages()
        {
            var result = new List<IStorage>();
 
            if (_appOptions.UseLocal)
                result.Add(_serviceProvider.GetRequiredService<LocalStorage>());

            if (_appOptions.UsePostgre)
                result.Add(_serviceProvider.GetRequiredService<PostgreStorage>());

            return result;
        }
        #endregion
    }
}
