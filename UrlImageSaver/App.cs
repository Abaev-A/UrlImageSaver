using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using UrlImageSaver.Application.Models;
using UrlImageSaver.Web;

namespace UrlImageSaver.Application
{
    class App
    {
        #region Fields
        private readonly ILogger<App> _logger;
        private readonly AppOptions _appOptions;
        private readonly IStorageManager _storageManager;
        private readonly IWebHelper _webHelper;
        #endregion

        #region ctor
        public App(
            ILoggerFactory loggerFactory,
            IOptions<AppOptions> appOptions,
            IStorageManager storageManager,
            IWebHelper webHelper)
        {
            _logger = loggerFactory.CreateLogger<App>();
            _appOptions = appOptions.Value;
            _storageManager = storageManager;
            _webHelper = webHelper;
        }
#endregion

        public void Run(string[] args)
        {
            if (args.Count() == 0)
            {
                _logger.LogError("No input parameters");
                return;
            }

            if (!Uri.IsWellFormedUriString(args[0], UriKind.RelativeOrAbsolute))
            {
                _logger.LogError($"Invalid URL: {args[0]}");
                return;
            }

            if (!_appOptions.UseLocal && !_appOptions.UsePostgre)
            {
                _logger.LogInformation("Image saving method not selected. Set true to the \"Application:UseLocalStore\" or \"Application:UseDbStore\" parameter");
                return;
            }

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var links = _webHelper.GetImageLinks(args[0])?.Distinct();
            watch.Stop();

            if (links == null)
            {
                _logger.LogError($"Error while getting images list");
                return;
            }

            var storages = _storageManager.GetStorages();

            _logger.LogInformation($"Loading and parsing page succeed. Timer: {watch.ElapsedMilliseconds} ms");
            _logger.LogInformation($"{links.Count()} unique links found");
            _logger.LogInformation($"Start loading and storing images using {string.Join(", ", storages.Select(s => s.GetType().Name))}");

            watch = System.Diagnostics.Stopwatch.StartNew();

            links.AsParallel().ForAll(link =>
            {
                var content =  _webHelper.GetResource(link);
                var name = Path.GetFileName(new Uri(link).LocalPath);

                storages.AsParallel().ForAll(storage => storage.Store(name, content));
            });

            watch.Stop();

            _logger.LogInformation($"Storing succeed. Timer: {watch.ElapsedMilliseconds} ms");
        }
    }
}

