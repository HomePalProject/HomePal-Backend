using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.ShoppingList.DTOs;
using HomePal.Application.Features.ShoppingList.Services;
using HomePal.Domain.Constants;
using HomePal.Domain.Entities;
using HomePal.Shared.Results;
using FluentAssertions;
using Moq;
using Xunit;

namespace HomePal.UnitTests.Features.ShoppingList;

public class ShoppingListServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ShoppingListService _sut;

    public ShoppingListServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new ShoppingListService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetShoppingListAsync_ShouldReturnBadRequest_WhenUserHasNoHousehold()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.HouseholdMembers.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((HouseholdMember?)null);

        // Act
        var result = await _sut.GetShoppingListAsync(userId);

        // Assert
        result.Success.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
        result.Message.Should().Be(ErrorMessages.Pantry.NoHousehold);
    }

    [Fact]
    public async Task AddCustomItemAsync_ShouldReturnBadRequest_WhenUnitIdIsInvalid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var householdId = Guid.NewGuid();
        var member = new HouseholdMember { Id = Guid.NewGuid(), UserId = userId, HouseholdId = householdId };
        var shoppingList = new Domain.Entities.ShoppingList { Id = Guid.NewGuid(), HouseholdId = householdId };
        var invalidUnitId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.HouseholdMembers.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);
        _unitOfWorkMock.Setup(u => u.ShoppingLists.GetOrCreateByHouseholdIdAsync(householdId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingList);
        _unitOfWorkMock.Setup(u => u.MeasuringUnits.GetByIdAsync(invalidUnitId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((MeasuringUnit?)null);

        var request = new CreateShoppingListItemRequest
        {
            Name = "Bread",
            UnitId = invalidUnitId,
            Quantity = 2
        };

        // Act
        var result = await _sut.AddCustomItemAsync(userId, request);

        // Assert
        result.Success.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
        result.Message.Should().Be(ErrorMessages.Catalog.MeasuringUnitNotFound);
    }

    [Fact]
    public async Task AddCustomItemAsync_ShouldAddItem_WhenInputIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var householdId = Guid.NewGuid();
        var member = new HouseholdMember { Id = Guid.NewGuid(), UserId = userId, HouseholdId = householdId };
        var shoppingList = new Domain.Entities.ShoppingList { Id = Guid.NewGuid(), HouseholdId = householdId };

        _unitOfWorkMock.Setup(u => u.HouseholdMembers.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);
        _unitOfWorkMock.Setup(u => u.ShoppingLists.GetOrCreateByHouseholdIdAsync(householdId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingList);

        ShoppingListItem? savedItem = null;
        _unitOfWorkMock.Setup(u => u.ShoppingListItems.AddAsync(It.IsAny<ShoppingListItem>(), It.IsAny<CancellationToken>()))
            .Callback<ShoppingListItem, CancellationToken>((item, token) => savedItem = item)
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.ShoppingListItems.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken token) => new ShoppingListItem
            {
                Id = id,
                ShoppingListId = shoppingList.Id,
                Name = "Apples",
                Quantity = 5,
                IsPurchased = false
            });

        var request = new CreateShoppingListItemRequest
        {
            Name = "Apples",
            Quantity = 5
        };

        // Act
        var result = await _sut.AddCustomItemAsync(userId, request);

        // Assert
        result.Success.Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Created);
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Apples");
        result.Data!.Quantity.Should().Be(5);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task TogglePurchasedAsync_ShouldToggleIsPurchasedFlag()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var householdId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        var member = new HouseholdMember { Id = Guid.NewGuid(), UserId = userId, HouseholdId = householdId };
        var shoppingList = new Domain.Entities.ShoppingList { Id = Guid.NewGuid(), HouseholdId = householdId };
        var item = new ShoppingListItem
        {
            Id = itemId,
            ShoppingListId = shoppingList.Id,
            Name = "Rice",
            Quantity = 1,
            IsPurchased = false
        };

        _unitOfWorkMock.Setup(u => u.HouseholdMembers.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);
        _unitOfWorkMock.Setup(u => u.ShoppingLists.GetByHouseholdIdAsync(householdId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingList);
        _unitOfWorkMock.Setup(u => u.ShoppingListItems.GetByIdWithDetailsAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        // Act
        var result = await _sut.TogglePurchasedAsync(userId, itemId);

        // Assert
        result.Success.Should().BeTrue();
        item.IsPurchased.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
