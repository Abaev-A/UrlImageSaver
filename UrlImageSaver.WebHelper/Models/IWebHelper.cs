using System.Collections.Generic;
using System.Threading.Tasks;

namespace UrlImageSaver.Web
{
    public interface IWebHelper
    {
        List<string> GetImageLinks(string uri);
        byte[] GetResource(string url);
        Task<byte[]> GetResourceAsync(string url);
    }
}
