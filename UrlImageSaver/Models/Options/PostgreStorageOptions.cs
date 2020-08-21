namespace UrlImageSaver.Application.Models
{
    class PostgreStorageOptions
    {
        public static string SectionName => "Application:PostgreStore";

        public string ConnectionString { get; set; }
        public string TableName { get; set; }
        public bool UseProcedure { get; set; }
        public string ProcedureName { get; set; }

    }
}
