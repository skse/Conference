using System;
using System.Collections.Generic;
using System.Linq;
using ConferenceApi.Controllers;
using ConferenceApi.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ConferenceApi.Tests
{
    /// <summary>
    /// Tests dedicated to app controller methods.
    /// </summary>
    /// <tip>
    /// When unit testing controller logic,
    /// only the contents of a single action is tested, not the behavior of its dependencies or of the
    /// framework itself. As you unit test your controller actions, make sure you focus only on its behavior.
    /// A controller unit test avoids things like filters, routing, or model binding.
    /// However, unit tests don't detect issues in the interaction between components,
    /// which is the purpose of integration tests.
    /// </tip>
    public class ConferencesControllerUnitTests
    {
        /// <summary>
        /// Db create and initialize.
        /// </summary>
        /// <returns></returns>
        private ConferenceContext GetContextWithData()
        {
            var options = new DbContextOptionsBuilder<ConferenceContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ConferenceContext(options);

            #region Db Seeding

            context.ConferenceItems.Add(new ConferenceItem
            {
                Section = "GIS",
                Info = new InfoItem
                {
                    Name = "Geoinformation Systems",
                    City = "Tomsk",
                    Location = "Lenina 2, 404"
                }
            });
            context.ConferenceItems.Add(new ConferenceItem
            {
                Section = "CS",
                Info = new InfoItem
                {
                    Name = "Computer Science",
                    City = "Tomsk",
                    Location = "Lenina 30, 206"
                }
            });
            context.ConferenceItems.Add(new ConferenceItem
            {
                Section = "CF",
                Info = new InfoItem
                {
                    Name = "CodeFest",
                    City = "Novosibirsk",
                    Location = "Expocenter"
                }
            });

            #endregion

            context.SaveChanges();

            return context;
        }

        [Fact]
        public void Conferences_Get_All()
        {
            // Arrange
            using (var context = GetContextWithData())
            using (var controller = new ConferenceController(context))
            {
                // Act
                var result = controller.GetAll();

                // Assert
                var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
                var items = okResult.Value.Should().BeAssignableTo<IEnumerable<ConferenceItem>>().Subject;

                items.Count().Should().Be(3);
            }
        }

        [Fact]
        public void Info_Get_Specific()
        {
            // Arrange
            using (var context = GetContextWithData())
            using (var controller = new ConferenceController(context))
            {
                // Act
                var result = controller.GetById("GIS");

                // Assert
                var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
                var item = okResult.Value.Should().BeAssignableTo<InfoItem>().Subject;

                item.Name.Should().Be("Geoinformation Systems");
            }
        }

        [Fact]
        public void Info_Get_Specific_Invalid()
        {
            // Arrange
            using (var context = GetContextWithData())
            using (var controller = new ConferenceController(context))
            {
                // Act
                var result = controller.GetById("USSR");

                // Assert
                result.Should().BeOfType<NotFoundResult>();
            }
        }

        [Fact]
        public void Conference_Add()
        {
            // Arrange
            string section = "DPRO";
            var info = new InfoItem
            {
                Name = "DevPRO",
                City = "Tomsk",
                Location = "ul. Naberejnaya reki Ushaiky, 12 (IAM TSU)"
            };

            using (var context = GetContextWithData())
            using (var controller = new ConferenceController(context))
            {
                // Act
                var result = controller.Create(section, info);

                // Assert
                var createdResult = result.Should().BeOfType<CreatedAtRouteResult>().Subject;
                var item = createdResult.Value.Should().BeAssignableTo<ConferenceItem>().Subject;

                item.Section.Should().Be("DPRO");
                item.Info.Name.Should().Be("DevPRO");
                item.Info.City.Should().Be("Tomsk");
                item.Info.Location.Should().Be("ul. Naberejnaya reki Ushaiky, 12 (IAM TSU)");
            }
        }

        /// <summary>
        /// Check for ModelState.AddModelError() behavior.
        /// </summary>
        [Fact]
        public void Conference_Add_Invalid()
        {
            // Arrange
            string section = "DPRODPRO";
            var info = new InfoItem
            {
                Name = "DevPRO",
                City = "Tomsk",
                Location = "ul. Naberejnaya reki Ushaiky, 12 (IAM TSU)"
            };

            using (var context = GetContextWithData())
            using (var controller = new ConferenceController(context))
            {
                // Act
                var result = controller.Create(section, info);

                // Assert   
                var createdResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
                var item = createdResult.Value.Should().BeAssignableTo<SerializableError>().Subject;
                item.ContainsKey("Section").Should().Be(true);
            }
        }
    }
}