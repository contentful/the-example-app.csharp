using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using TheExampleApp.TagHelpers;
using Xunit;

namespace TheExampleApp.Tests.TagHelpers
{
    public class MarkdownTagHelperTests
    {

        private Func<bool, HtmlEncoder, Task<TagHelperContent>> GetChildContent(string childContent)
        {
            var content = new DefaultTagHelperContent();
            var tagHelperContent = content.SetContent(childContent);
            return (b, encoder) => Task.FromResult(tagHelperContent);
        }

        [Fact]
        public async Task MarkdownTagWithChildContentShouldReturnRenderedMarkdown()
        {
            //Arrange
            var helper = new MarkdownTagHelper();
            var context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), Guid.NewGuid().ToString());
            var output = new TagHelperOutput("markdown", new TagHelperAttributeList(), GetChildContent("## Banana"));

            //Act
            await helper.ProcessAsync(context, output);

            //Assert
            Assert.Null(output.TagName);
            Assert.StartsWith("<h2>Banana</h2>", output.Content.GetContent());
        }

        [Fact]
        public async Task DivWithAttributeShouldReturnRenderedMarkdown()
        {
            //Arrange
            var helper = new MarkdownTagHelper();
            var context = new TagHelperContext(new TagHelperAttributeList { new TagHelperAttribute("markdown") }, new Dictionary<object, object>(), Guid.NewGuid().ToString());
            var output = new TagHelperOutput("div", new TagHelperAttributeList { new TagHelperAttribute("markdown") }, GetChildContent("# Mr French"));

            //Act
            await helper.ProcessAsync(context, output);

            //Assert
            Assert.Equal("div", output.TagName);
            Assert.StartsWith("<h1>Mr French</h1>", output.Content.GetContent());
        }

    }
}
