namespace UrlImageSaver.Application.Models
{
    class LocalStorageOptions
    {
        public static string SectionName => "Application:LocalStorage";

        public string LocalPath { get; set; }
        public bool ShouldCreateFolder { get; set; }
    }
}
