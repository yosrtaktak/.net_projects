using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Backend.Controllers;
using Backend.Core.Entities;
using Backend.Core.Interfaces;

namespace Backend.Tests.UnitTests.Controllers;

public class VehiclesControllerTests
{
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly VehiclesController _controller;

    public VehiclesControllerTests()
    {
        _mockVehicleRepository = new Mock<IVehicleRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _controller = new VehiclesController(_mockVehicleRepository.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "TC006")]
    public async Task GetAllVehicles_ReturnsOkWithVehicles()
    {
        // Arrange
        var vehicles = new List<Vehicle>
        {
            new Vehicle { Id = 1, Brand = "Toyota", Model = "Camry", Status = VehicleStatus.Available },
            new Vehicle { Id = 2, Brand = "Honda", Model = "Accord", Status = VehicleStatus.Available }
        };
        
        _mockVehicleRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(vehicles);

        // Act
        var result = await _controller.GetAllVehicles();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedVehicles = okResult.Value.Should().BeAssignableTo<IEnumerable<Vehicle>>().Subject;
        returnedVehicles.Should().HaveCount(2);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "TC007")]
    public async Task GetVehicle_ExistingId_ReturnsOk()
    {
        // Arrange
        var vehicleId = 1;
        var vehicle = new Vehicle 
        { 
            Id = vehicleId, 
            Brand = "Toyota", 
            Model = "Camry",
            Status = VehicleStatus.Available
        };
        
        _mockVehicleRepository.Setup(r => r.GetByIdAsync(vehicleId))
            .ReturnsAsync(vehicle);

        // Act
        var result = await _controller.GetVehicle(vehicleId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedVehicle = okResult.Value.Should().BeOfType<Vehicle>().Subject;
        returnedVehicle.Id.Should().Be(vehicleId);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "TC008")]
    public async Task GetVehicle_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var vehicleId = 999;
        
        _mockVehicleRepository.Setup(r => r.GetByIdAsync(vehicleId))
            .ReturnsAsync((Vehicle?)null);

        // Act
        var result = await _controller.GetVehicle(vehicleId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "TC009")]
    public async Task CreateVehicle_ValidData_ReturnsCreated()
    {
        // Arrange
        var newVehicle = new Vehicle 
        { 
            Brand = "Toyota", 
            Model = "Camry", 
            Year = 2024,
            Status = VehicleStatus.Available,
            DailyRate = 50,
            CategoryId = 1
        };
        
        var createdVehicle = new Vehicle 
        { 
            Id = 1, 
            Brand = "Toyota", 
            Model = "Camry", 
            Year = 2024,
            Status = VehicleStatus.Available,
            DailyRate = 50,
            CategoryId = 1
        };

        _mockVehicleRepository.Setup(r => r.AddAsync(It.IsAny<Vehicle>()))
            .Callback<Vehicle>(v => v.Id = 1)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.CreateVehicle(newVehicle);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(_controller.GetVehicle));
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "TC010")]
    public async Task DeleteVehicle_ExistingId_ReturnsNoContent()
    {
        // Arrange
        var vehicleId = 1;
        var vehicle = new Vehicle 
        { 
            Id = vehicleId,
            Brand = "Toyota",
            Model = "Camry",
            Status = VehicleStatus.Available,
            Rentals = new List<Rental>()
        };
        
        _mockVehicleRepository.Setup(r => r.GetByIdWithHistoryAsync(vehicleId))
            .ReturnsAsync(vehicle);

        // Act
        var result = await _controller.DeleteVehicle(vehicleId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockVehicleRepository.Verify(r => r.Remove(vehicle), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }
}
