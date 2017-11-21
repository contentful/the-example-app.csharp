using Microsoft.Extensions.FileProviders;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using TheExampleApp.Configuration;
using Xunit;

namespace TheExampleApp.Tests.Configuration
{
    public class LocalizationConfigurationTests
    {
        [Fact]
        public void ViewLocalizerShouldReadJsonFilesCorrectly()
        {
            //Arrange
            var file = new Mock<IFileInfo>();
            file.SetupGet(c => c.Name).Returns("wwwroot.locales.sv-SE.json");
            file.Setup(c => c.CreateReadStream()).Returns(GenerateStreamFromString(@"{""helloLabel"": ""Hallå!""}"));
            var directory = new Mock<IDirectoryContents>();
            directory.Setup(d => d.GetEnumerator()).Returns(new List<IFileInfo> { file.Object }.GetEnumerator());
            var provider = new Mock<IFileProvider>();
            provider.Setup(p => p.GetDirectoryContents("")).Returns(directory.Object);
            var viewLocalizer = new JsonViewLocalizer(provider.Object);
            Thread.CurrentThread.CurrentCulture = new CultureInfo("sv-SE");
            //Act
            var res = viewLocalizer["helloLabel"];
            //Assert
            Assert.Equal("Hallå!", res.Value);
        }

        [Fact]
        public void ViewLocalizerShouldReadJsonFilesCorrectlyWithGetString()
        {
            //Arrange
            var file = new Mock<IFileInfo>();
            file.SetupGet(c => c.Name).Returns("wwwroot.locales.sv-SE.json");
            file.Setup(c => c.CreateReadStream()).Returns(GenerateStreamFromString(@"{""helloLabel"": ""Hallå!""}"));
            var directory = new Mock<IDirectoryContents>();
            directory.Setup(d => d.GetEnumerator()).Returns(new List<IFileInfo> { file.Object }.GetEnumerator());
            var provider = new Mock<IFileProvider>();
            provider.Setup(p => p.GetDirectoryContents("")).Returns(directory.Object);
            var viewLocalizer = new JsonViewLocalizer(provider.Object);
            Thread.CurrentThread.CurrentCulture = new CultureInfo("sv-SE");
            //Act
            var res = viewLocalizer.GetString("helloLabel");
            //Assert
            Assert.Equal("Hallå!", res.Value);
        }

        private static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
