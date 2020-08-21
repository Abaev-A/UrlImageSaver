using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UrlImageSaver.Web.Utils;

namespace UrlImageSaver.Web
{
    public class WebHelper : IWebHelper
    {
        #region Fields
        private readonly ILogger<WebHelper> _logger;
        private readonly string ValidHostnameRegex = @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$|^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)+([A-Za-z]|[A-Za-z][A-Za-z0-9\-]*[A-Za-z0-9])$";
        #endregion

        #region ctor
        public WebHelper(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<WebHelper>();
        }
        #endregion

        #region IWebHelper
        public byte[] GetResource(string url)
        {
            var web = new WebClient();
            var result = web.DownloadData(url);
            _logger.LogDebug($"Url data loaded ({Formatter.BytesToString(result.Count())}). Url: {url}");

            return result;
        }

        async public Task<byte[]> GetResourceAsync(string url)
        {
            var web = new WebClient();
            var result = await web.DownloadDataTaskAsync(new Uri(url));
            _logger.LogDebug($"Url data loaded ({Formatter.BytesToString(result.Count())}). Url: {url}");

            return result;
        }

        public List<string> GetImageLinks(string url)
        {
            var uri = new UriBuilder(url).Uri;

            HtmlDocument doc;
            try
            {
                _logger.LogInformation($"Loading web page {url}");
                var web = new HtmlWeb();
                doc = web.Load(uri);
                uri = web.ResponseUri; // google.com -> https://www.google.com/
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unhandled exception while loading web page");
                return null;
            }
            
            return ParseImageLinks(doc, uri);
        }
        #endregion

        #region private
        private List<string> ParseImageLinks(HtmlDocument document, Uri uri)
        {
            var result = new List<string>();

            _logger.LogInformation($"Parsing html document");

            var nodeCollection = document.DocumentNode.SelectNodes("//img | //a");

            if (nodeCollection == null)
                return result;

            foreach (HtmlNode node in nodeCollection)
            {
                var src = node.Attributes["src"]?.Value;
                if (src == null)
                    continue;

                src = src.TrimStart(new char[] { '/' });

                Uri nodeUri;

                if (!Uri.TryCreate(src, UriKind.RelativeOrAbsolute, out nodeUri))
                    continue;

                if (Regex.Match(src.Split(new char[] { '/' })[0], ValidHostnameRegex).Success)
                    nodeUri = new UriBuilder(uri.Scheme + "://" + src).Uri;
                else if (!nodeUri.IsAbsoluteUri)
                    nodeUri = new UriBuilder(uri.Scheme, uri.Host, uri.Port, src).Uri;

                result.Add(nodeUri.ToString());
            }

            return result;
        }
        #endregion
    }
}
