using Contentful.Core;
using Contentful.Core.Models;
using Contentful.Core.Search;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheExampleApp.Models;
using TheExampleApp.Pages;
using Xunit;

namespace TheExampleApp.Tests.Pages
{
    public class IndexModelTests
    {
        [Fact]
        public async Task GettingIndexShouldSetLayoutCorrectly()
        {
            //Arrange
            var collection = new ContentfulCollection<Layout>();
            collection.Items = new List<Layout>()
            {
                new Layout()
                {
                    Title = "SomeTitle"
                }
            };
            var client = new Mock<IContentfulClient>();
            client.SetupGet(c => c.SerializerSettings).Returns(new JsonSerializerSettings());
            client.Setup(c => c.GetEntries(It.IsAny<QueryBuilder<Layout>>(), default(CancellationToken)))
                .Returns(Task.FromResult(collection));
            var model = new IndexModel(client.Object);
            //Act

            await model.OnGet();

            //Assert
            Assert.Equal("SomeTitle", model.IndexPage.Title);
        }
    }
}
