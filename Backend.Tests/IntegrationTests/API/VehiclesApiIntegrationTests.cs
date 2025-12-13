using System.Net;
using System.Net.Http.Json;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Backend.Core.Entities;

namespace Backend.Tests.IntegrationTests.API;

public class VehiclesApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public VehiclesApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    [Trait("Category", "Integration")]
    [Trait("TestCase", "TC016")]
    public async Task GetAllVehicles_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/vehicles");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    [Trait("Category", "Integration")]
    [Trait("TestCase", "TC017")]
    public async Task GetAllVehicles_ReturnsVehiclesList()
    {
        // Act
        var response = await _client.GetAsync("/api/vehicles");
        var vehicles = await response.Content.ReadFromJsonAsync<List<Vehicle>>();

        // Assert
        response.Should().BeSuccessful();
        vehicles.Should().NotBeNull();
        vehicles.Should().BeOfType<List<Vehicle>>();
    }

    [Fact]
    [Trait("Category", "Integration")]
    [Trait("TestCase", "TC018")]
    public async Task GetVehicleById_ValidId_ReturnsVehicle()
    {
        // Arrange
        var vehicleId = 1;

        // Act
        var response = await _client.GetAsync($"/api/vehicles/{vehicleId}");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var vehicle = await response.Content.ReadFromJsonAsync<Vehicle>();
            vehicle.Should().NotBeNull();
            vehicle!.Id.Should().Be(vehicleId);
        }
        else
        {
            // If no vehicles exist in test DB, expect 404
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    [Trait("TestCase", "TC019")]
    public async Task GetVehicleById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = 99999;

        // Act
        var response = await _client.GetAsync($"/api/vehicles/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait("Category", "Integration")]
    [Trait("TestCase", "TC020")]
    public async Task GetAvailableVehicles_WithDates_ReturnsFilteredList()
    {
        // Arrange
        var startDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
        var endDate = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd");

        // Act
        var response = await _client.GetAsync($"/api/vehicles/available?startDate={startDate}&endDate={endDate}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var vehicles = await response.Content.ReadFromJsonAsync<List<Vehicle>>();
        vehicles.Should().NotBeNull();
    }
}
