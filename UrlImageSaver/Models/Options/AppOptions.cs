namespace UrlImageSaver.Application.Models
{
    class AppOptions
    {
        public static string SectionName => "Application";

        public bool UseLocal { get; set; }
        public bool UsePostgre { get; set; }    
    }
}
