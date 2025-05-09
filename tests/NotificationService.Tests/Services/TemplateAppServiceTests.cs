using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using NotificationService.API.Persistence.Entities.DB;
using NotificationService.API.Services;
using NotificationService.API.Persistence.Entities.DB.Models;

namespace NotificationService.Tests.Services
{
    public class TemplateAppServiceTests
    {
        private DbContextOptions<NotificationDbContext> CreateInMemoryOptions(string dbName) =>
            new DbContextOptionsBuilder<NotificationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

        /// <summary>
        /// Tests that the correct template is returned if it exists.
        /// </summary>
        [Fact]
        public async Task GetTemplateAsync_ReturnsCorrectTemplate_WhenExists()
        {
            var options = CreateInMemoryOptions(nameof(GetTemplateAsync_ReturnsCorrectTemplate_WhenExists));
            using var context = new NotificationDbContext(options);

            var template = new Template
            {
                Name = "Welcome",
                Subject = "Hello",
                Text = "Welcome to the system!",
                Language = "en"
            };

            context.Templates.Add(template);
            await context.SaveChangesAsync();

            var service = new TemplateAppService(context);

            var result = await service.GetTemplateAsync("Welcome", "en");

            Assert.NotNull(result);
            Assert.Equal("Welcome", result.Name);
            Assert.Equal("Hello", result.Subject);
        }

        /// <summary>
        /// Tests that an exception is thrown if the template does not exist.
        /// </summary>
        [Fact]
        public async Task GetTemplateAsync_ThrowsException_WhenTemplateNotFound()
        {
            var options = CreateInMemoryOptions(nameof(GetTemplateAsync_ThrowsException_WhenTemplateNotFound));
            using var context = new NotificationDbContext(options);
            var service = new TemplateAppService(context);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetTemplateAsync("Missing", "en"));
        }

        /// <summary>
        /// Tests that all templates are returned.
        /// </summary>
        [Fact]
        public async Task GetAllTemplatesAsync_ReturnsAllTemplates()
        {
            var options = CreateInMemoryOptions(nameof(GetAllTemplatesAsync_ReturnsAllTemplates));
            using var context = new NotificationDbContext(options);

            context.Templates.AddRange(
                new Template { Name = "A", Subject = "S1", Text = "T1", Language = "en" },
                new Template { Name = "B", Subject = "S2", Text = "T2", Language = "cs" }
            );
            await context.SaveChangesAsync();

            var service = new TemplateAppService(context);
            var templates = await service.GetAllTemplatesAsync();

            Assert.Equal(2, templates.Count);
        }

        /// <summary>
        /// Tests that EditTemplateAsync updates the subject and text if the template exists.
        /// </summary>
        [Fact]
        public async Task EditTemplateAsync_UpdatesSubjectAndText()
        {
            var options = CreateInMemoryOptions(nameof(EditTemplateAsync_UpdatesSubjectAndText));
            using var context = new NotificationDbContext(options);

            var original = new Template
            {
                Name = "Reminder",
                Subject = "Old",
                Text = "Old Text",
                Language = "en"
            };
            context.Templates.Add(original);
            await context.SaveChangesAsync();

            var service = new TemplateAppService(context);

            var updatedTemplate = new Template
            {
                Name = "Reminder",
                Subject = "New Subject",
                Text = "Updated Text",
                Language = "en"
            };

            var result = await service.EditTemplateAsync(updatedTemplate);

            Assert.True(result);

            var reloaded = await context.Templates.FirstAsync();
            Assert.Equal("New Subject", reloaded.Subject);
            Assert.Equal("Updated Text", reloaded.Text);
        }

        /// <summary>
        /// Tests that EditTemplateAsync returns false if the template does not exist.
        /// </summary>
        [Fact]
        public async Task EditTemplateAsync_ReturnsFalse_WhenTemplateDoesNotExist()
        {
            var options = CreateInMemoryOptions(nameof(EditTemplateAsync_ReturnsFalse_WhenTemplateDoesNotExist));
            using var context = new NotificationDbContext(options);
            var service = new TemplateAppService(context);

            var missing = new Template
            {
                Name = "DoesNotExist",
                Subject = "Irrelevant",
                Text = "Irrelevant",
                Language = "en"
            };

            var result = await service.EditTemplateAsync(missing);

            Assert.False(result);
        }
    }
}
