using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using UrlImageSaver.Application.Models;
using UrlImageSaver.Application.Storage;
using Xunit;

namespace UrlImageSaver.UnitTests.Application
{
    public class LocalStorageTests
    {
        #region Store
        [Fact]
        public void Store_Test()
        {
            // Arrange
            var path = Path.Combine(Directory.GetCurrentDirectory(), "LocalStorageTest_" + Guid.NewGuid().ToString());

            var options = Options.Create(new LocalStorageOptions()
            {
                LocalPath = path,
                ShouldCreateFolder = true
            });             

            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources\\google_logo.png");
            var bytes = File.ReadAllBytes(logoPath);
            var localStorage = new LocalStorage(options, new LoggerFactory());

            // Act
            localStorage.Store("logo.png", bytes);

            // Assert
            Assert.True(Directory.Exists(path));

            var files = Directory.EnumerateFiles(path);
            Assert.True(files.Count() == 1);
            Assert.Contains("logo.png", files.First());

            var newBytes = File.ReadAllBytes(files.First());
            Assert.Equal(bytes, newBytes);

            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }
        #endregion
    }
}
