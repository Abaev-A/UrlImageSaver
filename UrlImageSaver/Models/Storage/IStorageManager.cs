using System.Collections.Generic;

namespace UrlImageSaver.Application.Models
{
    interface IStorageManager
    {
        List<IStorage> GetStorages();
    }
}