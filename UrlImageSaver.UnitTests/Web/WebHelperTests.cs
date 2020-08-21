using Microsoft.Extensions.Logging;
using System.IO;
using UrlImageSaver.Web;
using Xunit;

namespace UrlImageSaver.UnitTests.Web
{
    public class WebHelperTests
    {
        #region GetLinks     
        [Fact]
        public void GetLinks_Google_Test()
        {
            // Arrange
            var webHelper = new WebHelper(new LoggerFactory());

            // Act
            var result = webHelper.GetImageLinks("google.com");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 2);
            Assert.Collection(result, item => Assert.Contains("images/srpr/nav_logo230.png", item),
                              item => Assert.Contains("images/branding/googlelogo/1x/googlelogo_white_background_color_272x92dp.png", item));
        }

        [Fact]
        public void GetLinks_NoLinks_Test()
        {
            // Arrange
            var webHelper = new WebHelper(new LoggerFactory());

            // Act
            var result = webHelper.GetImageLinks("axentrealty.com/robots.txt");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
        }

        [Fact]
        public void GetLinks_InvalidPage_Test()
        {
            // Arrange
            var webHelper = new WebHelper(new LoggerFactory());

            // Act
            var result = webHelper.GetImageLinks("something.com");

            // Assert
            Assert.Null(result);
        }
        #endregion

        #region GetResource
        [Fact]
        public void GetResource_Test()
        {
            // Arrange
            var webHelper = new WebHelper(new LoggerFactory());
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources\\google_logo.png");
            var bytes = File.ReadAllBytes(logoPath);

            // Act
            var result = webHelper.GetResource("https://www.google.com/images/branding/googlelogo/2x/googlelogo_color_272x92dp.png");

            // Assert
            Assert.Equal(bytes, result);
        }

        [Fact]
        public void GetResource_WebException_Test()
        {
            // Arrange
            var webHelper = new WebHelper(new LoggerFactory());

            // Act + Assert
            Assert.Throws<System.Net.WebException>(() => webHelper.GetResource("https://vk.com/error3.png"));
        }
        #endregion

        #region GetResourceAsync
        [Fact]
        public async void GetResourceAsync_Test()
        {
            // Arrange
            var webHelper = new WebHelper(new LoggerFactory());
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources\\google_logo.png");
            var bytes = File.ReadAllBytes(logoPath);

            // Act
            var result = await webHelper.GetResourceAsync("https://www.google.com/images/branding/googlelogo/2x/googlelogo_color_272x92dp.png");

            // Assert
            Assert.Equal(bytes, result);
        }

        [Fact]
        public async void GetResourceAsync_WebException_Test()
        {
            // Arrange
            var webHelper = new WebHelper(new LoggerFactory());

            // Act + Assert
            await Assert.ThrowsAsync<System.Net.WebException>(() => webHelper.GetResourceAsync("https://vk.com/error3.png"));
        }
        #endregion
    }
}
