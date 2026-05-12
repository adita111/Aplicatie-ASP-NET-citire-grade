using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProiectFlorea.Data;
using Xunit;

namespace ProiectFlorea.Tests
{
    public class TemperatureEndpointTests
    {
        private HttpClient CreateClient()
        {
            var factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll<DbContextOptions<AppDbContext>>();

                        services.AddDbContext<AppDbContext>(options =>
                        {
                            options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                        });
                    });
                });

            return factory.CreateClient();
        }

        [Fact]
        public async Task PostTemperature_WithValidData_ReturnsOk()
        {
            var client = CreateClient();

            var payload = new
            {
                temperature_c = 23.5,
                voltage_mv = 300,
                latitude = 45.7983,
                longitude = 24.1256
            };

            var response = await client.PostAsJsonAsync("/api/temperature", payload);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostTemperature_WithInvalidTemperature_ReturnsBadRequest()
        {
            var client = CreateClient();

            var json = """
            {
                "temperature_c": "abc",
                "voltage_mv": 300,
                "latitude": 45.7983,
                "longitude": 24.1256
            }
            """;

            var response = await client.PostAsync(
                "/api/temperature",
                new StringContent(json, System.Text.Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PostTemperature_WithoutTemperature_ReturnsBadRequest()
        {
            var client = CreateClient();

            var json = """
            {
                "voltage_mv": 300,
                "latitude": 45.7983,
                "longitude": 24.1256
            }
            """;

            var response = await client.PostAsync(
                "/api/temperature",
                new StringContent(json, System.Text.Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}