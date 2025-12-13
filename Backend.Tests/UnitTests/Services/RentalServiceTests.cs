using Xunit;
using Moq;
using FluentAssertions;
using Backend.Application.Services;
using Backend.Core.Entities;
using Backend.Core.Interfaces;
using Backend.Application.Factories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Tests.UnitTests.Services
{
    /// <summary>
    /// Tests unitaires pour le service de location (RentalService)
    /// Niveau: Test Unitaire
    /// Technique: Boîte noire - Classes d'équivalence et valeurs limites
    /// </summary>
    public class RentalServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRentalRepository> _mockRentalRepository;
        private readonly Mock<IVehicleRepository> _mockVehicleRepository;
        private readonly Mock<IPricingStrategyFactory> _mockPricingStrategyFactory;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly RentalService _rentalService;

        public RentalServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRentalRepository = new Mock<IRentalRepository>();
            _mockVehicleRepository = new Mock<IVehicleRepository>();
            _mockPricingStrategyFactory = new Mock<IPricingStrategyFactory>();
            
            // Mock UserManager
            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            _rentalService = new RentalService(
                _mockUnitOfWork.Object,
                _mockRentalRepository.Object,
                _mockVehicleRepository.Object,
                _mockPricingStrategyFactory.Object,
                _mockUserManager.Object);
        }

        [Fact]
        [Trait("Category", "Unit")]
        [Trait("TestCase", "TC001")]
        public async Task GetRentalByIdAsync_ExistingId_ReturnsRental()
        {
            // Arrange
            var rentalId = 1;
            var rental = new Rental 
            { 
                Id = rentalId, 
                VehicleId = 1,
                UserId = "user123",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(3),
                TotalCost = 100.0m,
                Status = RentalStatus.Reserved
            };
            
            _mockRentalRepository.Setup(r => r.GetByIdWithDetailsAsync(rentalId))
                .ReturnsAsync(rental);

            // Act
            var result = await _rentalService.GetRentalByIdAsync(rentalId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(rentalId);
            _mockRentalRepository.Verify(r => r.GetByIdWithDetailsAsync(rentalId), Times.Once);
        }

        [Fact]
        [Trait("Category", "Unit")]
        [Trait("TestCase", "TC002")]
        public async Task GetRentalByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            var rentalId = 999;
            
            _mockRentalRepository.Setup(r => r.GetByIdWithDetailsAsync(rentalId))
                .ReturnsAsync((Rental?)null);

            // Act
            var result = await _rentalService.GetRentalByIdAsync(rentalId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        [Trait("Category", "Unit")]
        [Trait("TestCase", "TC003")]
        public async Task GetAllRentalsAsync_ReturnsAllRentals()
        {
            // Arrange
            var rentals = new List<Rental>
            {
                new Rental { Id = 1, VehicleId = 1, UserId = "user1", TotalCost = 100, Status = RentalStatus.Active },
                new Rental { Id = 2, VehicleId = 2, UserId = "user2", TotalCost = 200, Status = RentalStatus.Completed }
            };
            
            _mockRentalRepository.Setup(r => r.GetAllWithDetailsAsync())
                .ReturnsAsync(rentals);

            // Act
            var result = await _rentalService.GetAllRentalsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        [Trait("Category", "Unit")]
        [Trait("TestCase", "TC004")]
        public async Task GetRentalsByUserAsync_ReturnsUserRentals()
        {
            // Arrange
            var userId = "user123";
            var rentals = new List<Rental>
            {
                new Rental { Id = 1, VehicleId = 1, UserId = userId, TotalCost = 100, Status = RentalStatus.Active }
            };
            
            _mockRentalRepository.Setup(r => r.GetRentalsByUserIdAsync(userId))
                .ReturnsAsync(rentals);

            // Act
            var result = await _rentalService.GetRentalsByUserAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().UserId.Should().Be(userId);
        }

        [Fact]
        [Trait("Category", "Unit")]
        [Trait("TestCase", "TC005")]
        public async Task CancelRentalAsync_ExistingRental_CancelsSuccessfully()
        {
            // Arrange
            var rentalId = 1;
            var rental = new Rental 
            { 
                Id = rentalId, 
                VehicleId = 1,
                UserId = "user123",
                Status = RentalStatus.Reserved,
                Vehicle = new Vehicle { Id = 1, Status = VehicleStatus.Rented }
            };
            
            _mockRentalRepository.Setup(r => r.GetByIdWithDetailsAsync(rentalId))
                .ReturnsAsync(rental);

            // Act
            var result = await _rentalService.CancelRentalAsync(rentalId);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(RentalStatus.Cancelled);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }
    }
}
