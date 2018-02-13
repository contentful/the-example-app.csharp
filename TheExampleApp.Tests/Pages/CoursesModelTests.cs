using Contentful.Core;
using Contentful.Core.Models;
using Contentful.Core.Search;
using Microsoft.AspNetCore.Mvc.Localization;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheExampleApp.Configuration;
using TheExampleApp.Models;
using TheExampleApp.Pages;
using Xunit;

namespace TheExampleApp.Tests.Pages
{
    public class CoursesModelTests
    {

        [Fact]
        public async Task CoursesModelShouldLoadAllCoursesWhenNoCategoryIsSelected()
        {
            //Arrange
            var categories = new ContentfulCollection<Category>();
            categories.Items = new List<Category>()
            {
                new Category()
                {
                    Title = "Cat1",
                    Slug = "sluggish"
                },
                new Category()
                {
                    Title = "Cat5",
                    Slug = "another-slug"
                }
            };

            var courses = new ContentfulCollection<Course>();
            courses.Items = new List<Course>()
            {
                new Course()
                {
                    Title = "Course1",
                    Slug = "bah-slug"
                },
                new Course()
                {
                    Title = "Cruiser2",
                    Slug = "kah-slug"
                }
            };

            var client = new Mock<IContentfulClient>();
            client.SetupGet(c => c.SerializerSettings).Returns(new JsonSerializerSettings());
            client.Setup(c => c.GetEntries(It.IsAny<QueryBuilder<Course>>(), default(CancellationToken)))
                .Returns(Task.FromResult(courses));
            client.Setup(c => c.GetEntriesByType("category", It.IsAny<QueryBuilder<Category>>(), default(CancellationToken)))
                .Returns(Task.FromResult(categories));
            var crumbs = new Mock<IBreadcrumbsManager>();
            var localizer = new Mock<IViewLocalizer>();

            var model = new CoursesModel(client.Object, crumbs.Object, localizer.Object);
            //Act
            await model.OnGet(null);

            //Assert
            Assert.Equal(courses, model.Courses);
        }

        [Fact]
        public async Task CoursesModelShouldFilterCoursesWhenCategoryIsSelected()
        {
            //Arrange
            var categories = new ContentfulCollection<Category>();
            categories.Items = new List<Category>()
            {
                new Category()
                {
                    Title = "Cat1",
                    Slug = "sluggish"
                },
                new Category()
                {
                    Title = "Cat5",
                    Slug = "another-slug"
                }
            };

            var courses = new ContentfulCollection<Course>();
            courses.Items = new List<Course>()
            {
                new Course()
                {
                    Title = "Course1",
                    Slug = "bah-slug",
                    Categories = new List<Category>()
                    {
                        new Category()
                        {
                            Title = "Cat1",
                            Slug = "sluggish"
                        }
                    }
                },
                new Course()
                {
                    Title = "Cruiser2",
                    Slug = "kah-slug",
                    Categories = new List<Category>()
                    {
                        new Category()
                        {
                            Title = "Cat5",
                            Slug = "another-slug"
                        }
                    }
                }
            };

            var client = new Mock<IContentfulClient>();
            client.SetupGet(c => c.SerializerSettings).Returns(new JsonSerializerSettings());
            client.Setup(c => c.GetEntries(It.IsAny<QueryBuilder<Course>>(), default(CancellationToken)))
                .Returns(Task.FromResult(courses));
            client.Setup(c => c.GetEntriesByType("category", It.IsAny<QueryBuilder<Category>>(), default(CancellationToken)))
                .Returns(Task.FromResult(categories));
            var crumbs = new Mock<IBreadcrumbsManager>();
            var localizer = new Mock<IViewLocalizer>();

            var model = new CoursesModel(client.Object, crumbs.Object, localizer.Object);
            //Act
            await model.OnGet("another-sluG");

            //Assert
            Assert.Collection(model.Courses, (c) => { Assert.Equal("Cruiser2", c.Title); });
        }
    }
}
