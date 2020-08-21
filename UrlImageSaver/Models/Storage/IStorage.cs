namespace UrlImageSaver.Application.Models
{
    interface IStorage
    {
        void Store(string name, byte[] content);
    }
}
