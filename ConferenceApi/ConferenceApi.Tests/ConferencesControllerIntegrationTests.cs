using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ConferenceApi.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Xunit;

namespace ConferenceApi.Tests
{
    /// <summary>
    /// Tests dedicated to controller integration (server-client).
    /// </summary>
    /// <tip>
    /// Generally, anything you can test with a unit test, you can also test
    /// with an integration test, but the reverse isn't true. However, integration
    /// tests tend to be much slower than unit tests. Thus, it's best to test whatever
    /// you can with unit tests, and use integration tests for scenarios that involve
    /// multiple collaborators.
    /// </tip>
    public class ConferencesControllerIntegrationTests
    {
        private readonly HttpClient _client;

        /// <summary>
        /// Create test server and client.
        /// </summary>
        public ConferencesControllerIntegrationTests()
        {
            // Arrange
            var server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());
            _client = server.CreateClient();
        }

        [Fact]
        public async Task Conferences_Get_All()
        {
            // Act
            var response = await _client.GetAsync("/conference/info");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            var conferences = JsonConvert.DeserializeObject<IEnumerable<ConferenceItem>>(responseString);
            conferences.Count().Should().Be(3);
        }

        [Fact]
        public async Task Conferences_Get_Specific()
        {
            // Act
            var response = await _client.GetAsync("/conference/GIS/info");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            var item = JsonConvert.DeserializeObject<InfoItem>(responseString);
            item.Name.Should().Be("Geoinformation Systems");
        }

        [Fact]
        public async Task Conferences_Get_Specific_Invalid()
        {
            // Act
            var response = await _client.GetAsync("/conference/USSR/info");

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Conference_Post()
        {
            // Arrange
            string section = "DPRO";
            var info = new InfoItem
            {
                Name = "DevPRO",
                City = "Tomsk",
                Location = "ul. Naberejnaya reki Ushaiky, 12 (IAM TSU)"
            };
            var content = JsonConvert.SerializeObject(info);
            var stringContent = new StringContent(content, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/conference/" + section + "/info", stringContent);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var conference = JsonConvert.DeserializeObject<ConferenceItem>(responseString);
            conference.Section.Should().Be("DPRO");
            conference.Info.Name.Should().Be("DevPRO");
            conference.Info.City.Should().Be("Tomsk");
            conference.Info.Location.Should().Be("ul. Naberejnaya reki Ushaiky, 12 (IAM TSU)");
        }

        [Fact]
        public async Task Conference_Post_Invalid()
        {
            // Arrange
            string section = "FURFURFUR";
            var info = new InfoItem
            {
                Name = "loremipsumloremipsumloremipsumloremipsum",
                Location = "Odddfgd"
            };

            var content = JsonConvert.SerializeObject(info);
            var stringContent = new StringContent(content, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/conference/" + section + "/info", stringContent);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Contain("The City field is required")
                .And.Contain("The field Name must be a string with a maximum length of 20")
                .And.Contain("The field Section must be a string with a maximum length of 5");
        }
    }
}