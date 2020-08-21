using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using UrlImageSaver.Application.Models;
using UrlImageSaver.Application.Storage;
using UrlImageSaver.Web;

[assembly: InternalsVisibleTo("UrlImageSaver.UnitTests")]
namespace UrlImageSaver.Application
{
    class Program
    {
        private static IServiceProvider _serviceProvider;

        static void Main(string[] args)
        {
            ConfigureServices();

            var logger = _serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<Program>();

            try
            {
                logger.LogInformation("Entering the application");
                _serviceProvider.GetRequiredService<App>().Run(args);
            }
            catch (Exception e)
            {
                logger.LogCritical(e, "Unhandled exception");
            }
            finally
            {
                logger.LogInformation("Shutting down the application");
                DisposeServices();
                Console.ReadKey();
            }
        }


        private static void ConfigureServices()
        {
            ServiceCollection serviceCollection = new ServiceCollection();

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();

            IConfigurationSection appSection = configuration.GetSection(AppOptions.SectionName);
            IConfigurationSection localStoreSection = configuration.GetSection(LocalStorageOptions.SectionName);
            IConfigurationSection dbStoreSection = configuration.GetSection(PostgreStorageOptions.SectionName);
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConfiguration(configuration).AddConsole());

            serviceCollection.Configure<AppOptions>(appSection);
            serviceCollection.Configure<LocalStorageOptions>(localStoreSection);
            serviceCollection.Configure<PostgreStorageOptions>(dbStoreSection);

            serviceCollection.AddSingleton<App>();
            serviceCollection.AddSingleton<IConfigurationRoot>(configuration);
            serviceCollection.AddSingleton<ILoggerFactory>(loggerFactory);
            serviceCollection.AddSingleton<IWebHelper, WebHelper>();
            serviceCollection.AddSingleton<IStorageManager, StorageManager>();
            serviceCollection.AddSingleton<LocalStorage>();
            serviceCollection.AddSingleton<PostgreStorage>();

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private static void DisposeServices()
        {
            if (_serviceProvider != null && _serviceProvider is IDisposable)
                ((IDisposable)_serviceProvider).Dispose();
        }

    }
}
