using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.PantryManagement.DTOs;
using HomePal.Application.Features.PantryManagement.Interfaces;
using HomePal.Application.Features.PantryManagement.Services;
using HomePal.Domain.Common;
using HomePal.Domain.Constants;
using HomePal.Domain.Entities;
using HomePal.Shared.Results;
using FluentAssertions;
using Moq;
using Xunit;

namespace HomePal.UnitTests.Features.PantryManagement;

public class PantryItemServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPantryScannerService> _scannerServiceMock;
    private readonly PantryItemService _sut;

    public PantryItemServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _scannerServiceMock = new Mock<IPantryScannerService>();
        _sut = new PantryItemService(_unitOfWorkMock.Object, _scannerServiceMock.Object);
    }

    [Fact]
    public async Task GetPantryItemsAsync_ShouldReturnFailure_WhenMemberNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.HouseholdMembers.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((HouseholdMember?)null);

        // Act
        var result = await _sut.GetPantryItemsAsync(userId);

        // Assert
        result.Success.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        result.Message.Should().Be(ErrorMessages.Pantry.NoHousehold);
    }

    [Fact]
    public async Task GetPantryItemsAsync_ShouldReturnItems_WhenHouseholdAndPantryExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var householdId = Guid.NewGuid();
        var member = new HouseholdMember { Id = Guid.NewGuid(), UserId = userId, HouseholdId = householdId };
        var pantry = new Pantry { Id = Guid.NewGuid(), HouseholdId = householdId };

        var category = new ProductCategory { Id = Guid.NewGuid(), Name = new List<LocalizedItem> { new LocalizedItem("en", "Dairy") } };
        var unit = new MeasuringUnit { Id = Guid.NewGuid(), Name = new List<LocalizedItem> { new LocalizedItem("en", "Kg") }, Symbol = new List<LocalizedItem> { new LocalizedItem("en", "kg") } };

        var pantryItem = new PantryItem
        {
            Id = Guid.NewGuid(),
            PantryId = pantry.Id,
            Name = "Milk",
            Quantity = 2,
            ExpireDate = DateTime.UtcNow.AddDays(5),
            Category = category,
            MeasuringUnit = unit
        };

        _unitOfWorkMock.Setup(u => u.HouseholdMembers.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);
        _unitOfWorkMock.Setup(u => u.Pantries.GetByHouseholdIdAsync(householdId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pantry);
        _unitOfWorkMock.Setup(u => u.PantryItems.GetByPantryIdAsync(pantry.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PantryItem> { pantryItem });

        // Act
        var result = await _sut.GetPantryItemsAsync(userId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(1);
        result.Data![0].Name.Should().Be("Milk");
        result.Data![0].Quantity.Should().Be(2);
    }

    [Fact]
    public async Task CreatePantryItemAsync_ShouldReturnForbidden_WhenUserIsNotManager()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var householdId = Guid.NewGuid();
        var member = new HouseholdMember
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            HouseholdId = householdId,
            Role = Roles.HouseholdMember // Regular member, not manager
        };
        var pantry = new Pantry { Id = Guid.NewGuid(), HouseholdId = householdId };

        _unitOfWorkMock.Setup(u => u.HouseholdMembers.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);
        _unitOfWorkMock.Setup(u => u.Pantries.GetByHouseholdIdAsync(householdId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pantry);

        var request = new CreatePantryItemRequest
        {
            Name = "Cheese",
            Quantity = 1,
            CategoryId = Guid.NewGuid(),
            MeasuringUnitId = Guid.NewGuid(),
            ExpireDate = DateTime.UtcNow.AddDays(10)
        };

        // Act
        var result = await _sut.CreatePantryItemAsync(userId, request);

        // Assert
        result.Success.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Forbidden);
        result.Message.Should().Be(ErrorMessages.Household.NotManager);
    }

    [Fact]
    public async Task CreatePantryItemAsync_ShouldReturnBadRequest_WhenMeasuringUnitNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var householdId = Guid.NewGuid();
        var member = new HouseholdMember
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            HouseholdId = householdId,
            Role = Roles.HouseholdManager
        };
        var pantry = new Pantry { Id = Guid.NewGuid(), HouseholdId = householdId };
        var unitId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.HouseholdMembers.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);
        _unitOfWorkMock.Setup(u => u.Pantries.GetByHouseholdIdAsync(householdId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pantry);
        _unitOfWorkMock.Setup(u => u.MeasuringUnits.GetByIdAsync(unitId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((MeasuringUnit?)null);

        var request = new CreatePantryItemRequest
        {
            Name = "Cheese",
            Quantity = 1,
            CategoryId = Guid.NewGuid(),
            MeasuringUnitId = unitId,
            ExpireDate = DateTime.UtcNow.AddDays(10)
        };

        // Act
        var result = await _sut.CreatePantryItemAsync(userId, request);

        // Assert
        result.Success.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
        result.Message.Should().Be(ErrorMessages.Catalog.MeasuringUnitNotFound);
    }

    [Fact]
    public async Task CreatePantryItemAsync_ShouldCreateItemSuccessfully_WhenRequestIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var householdId = Guid.NewGuid();
        var member = new HouseholdMember
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            HouseholdId = householdId,
            Role = Roles.HouseholdManager
        };
        var pantry = new Pantry { Id = Guid.NewGuid(), HouseholdId = householdId };

        var categoryId = Guid.NewGuid();
        var unitId = Guid.NewGuid();
        var category = new ProductCategory { Id = categoryId, Name = new List<LocalizedItem> { new LocalizedItem("en", "Cheese") } };
        var unit = new MeasuringUnit { Id = unitId, Name = new List<LocalizedItem> { new LocalizedItem("en", "Piece") }, Symbol = new List<LocalizedItem> { new LocalizedItem("en", "pc") } };

        _unitOfWorkMock.Setup(u => u.HouseholdMembers.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);
        _unitOfWorkMock.Setup(u => u.Pantries.GetByHouseholdIdAsync(householdId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pantry);
        _unitOfWorkMock.Setup(u => u.MeasuringUnits.GetByIdAsync(unitId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(unit);
        _unitOfWorkMock.Setup(u => u.ProductCategories.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        PantryItem? addedItem = null;
        _unitOfWorkMock.Setup(u => u.PantryItems.AddAsync(It.IsAny<PantryItem>(), It.IsAny<CancellationToken>()))
            .Callback<PantryItem, CancellationToken>((item, token) => addedItem = item)
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.PantryItems.GetByIdAndPantryIdAsync(It.IsAny<Guid>(), pantry.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, Guid pId, CancellationToken token) => new PantryItem
            {
                Id = id,
                PantryId = pId,
                Name = "Gouda",
                Quantity = 2,
                Category = category,
                MeasuringUnit = unit
            });

        var request = new CreatePantryItemRequest
        {
            Name = "Gouda",
            Quantity = 2,
            CategoryId = categoryId,
            MeasuringUnitId = unitId
        };

        // Act
        var result = await _sut.CreatePantryItemAsync(userId, request);

        // Assert
        result.Success.Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Created);
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Gouda");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
