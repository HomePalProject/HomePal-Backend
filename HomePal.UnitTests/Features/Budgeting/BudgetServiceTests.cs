using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.Budgeting.DTOs;
using HomePal.Application.Features.Budgeting.Services;
using HomePal.Domain.Constants;
using HomePal.Domain.Entities;
using HomePal.Shared.Results;
using FluentAssertions;
using Moq;
using Xunit;

namespace HomePal.UnitTests.Features.Budgeting;

public class BudgetServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly BudgetService _sut;

    public BudgetServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new BudgetService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetMonthlyBudgetSummaryAsync_ShouldReturnBadRequest_WhenUserHasNoHousehold()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.HouseholdMembers.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((HouseholdMember?)null);

        // Act
        var result = await _sut.GetMonthlyBudgetSummaryAsync(userId, 2026, 8);

        // Assert
        result.Success.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
        result.Message.Should().Be(ErrorMessages.Pantry.NoHousehold);
    }

    [Fact]
    public async Task GetMonthlyBudgetSummaryAsync_ShouldReturnSummary_WithCurrentMonthBudgetAndExpenses()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var householdId = Guid.NewGuid();
        var member = new HouseholdMember { Id = Guid.NewGuid(), UserId = userId, HouseholdId = householdId };
        int year = 2026;
        int month = 8;

        var budget = new HouseholdMonthlyBudget
        {
            Id = Guid.NewGuid(),
            HouseholdId = householdId,
            BudgetDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc),
            Amount = 5000m,
            Notes = "Monthly Grocery & Bills"
        };

        var expenses = new List<HouseholdExpense>
        {
            new HouseholdExpense { Id = Guid.NewGuid(), HouseholdId = householdId, Amount = 1200m, Title = "Supermarket" },
            new HouseholdExpense { Id = Guid.NewGuid(), HouseholdId = householdId, Amount = 800m, Title = "Electricity" }
        };

        _unitOfWorkMock.Setup(u => u.HouseholdMembers.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);
        _unitOfWorkMock.Setup(u => u.MonthlyBudgets.GetByHouseholdAndPeriodAsync(householdId, year, month, It.IsAny<CancellationToken>()))
            .ReturnsAsync(budget);
        _unitOfWorkMock.Setup(u => u.HouseholdExpenses.GetByHouseholdAndPeriodAsync(householdId, year, month, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expenses);

        // Act
        var result = await _sut.GetMonthlyBudgetSummaryAsync(userId, year, month);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.BudgetAmount.Should().Be(5000m);
        result.Data!.TotalSpent.Should().Be(2000m);
        result.Data!.RemainingAmount.Should().Be(3000m);
        result.Data!.TotalExpensesCount.Should().Be(2);
    }

    [Fact]
    public async Task GetMonthlyBudgetSummaryAsync_ShouldCarryOverPreviousBudget_WhenCurrentMonthBudgetIsNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var householdId = Guid.NewGuid();
        var member = new HouseholdMember { Id = Guid.NewGuid(), UserId = userId, HouseholdId = householdId };
        int year = 2026;
        int month = 8;

        var previousBudget = new HouseholdMonthlyBudget
        {
            Id = Guid.NewGuid(),
            HouseholdId = householdId,
            BudgetDate = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc),
            Amount = 4500m
        };

        _unitOfWorkMock.Setup(u => u.HouseholdMembers.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);
        _unitOfWorkMock.Setup(u => u.MonthlyBudgets.GetByHouseholdAndPeriodAsync(householdId, year, month, It.IsAny<CancellationToken>()))
            .ReturnsAsync((HouseholdMonthlyBudget?)null);
        _unitOfWorkMock.Setup(u => u.MonthlyBudgets.GetLatestBeforePeriodAsync(householdId, year, month, It.IsAny<CancellationToken>()))
            .ReturnsAsync(previousBudget);
        _unitOfWorkMock.Setup(u => u.HouseholdExpenses.GetByHouseholdAndPeriodAsync(householdId, year, month, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<HouseholdExpense>());

        // Act
        var result = await _sut.GetMonthlyBudgetSummaryAsync(userId, year, month);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.BudgetAmount.Should().Be(4500m);
        result.Data!.TotalSpent.Should().Be(0m);
        result.Data!.RemainingAmount.Should().Be(4500m);
    }

    [Fact]
    public async Task SetMonthlyBudgetAsync_ShouldReturnBadRequest_WhenYearOrMonthIsInvalid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var householdId = Guid.NewGuid();
        var member = new HouseholdMember { Id = Guid.NewGuid(), UserId = userId, HouseholdId = householdId };

        _unitOfWorkMock.Setup(u => u.HouseholdMembers.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);

        var request = new SetMonthlyBudgetRequest
        {
            Year = 1999, // Invalid year
            Month = 13,   // Invalid month
            TargetAmount = 5000m
        };

        // Act
        var result = await _sut.SetMonthlyBudgetAsync(userId, request);

        // Assert
        result.Success.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
        result.Message.Should().Be(ErrorMessages.Budget.InvalidYearOrMonth);
    }

    [Fact]
    public async Task SetMonthlyBudgetAsync_ShouldCreateNewBudget_WhenNoneExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var householdId = Guid.NewGuid();
        var member = new HouseholdMember { Id = Guid.NewGuid(), UserId = userId, HouseholdId = householdId };
        int year = 2026;
        int month = 9;

        _unitOfWorkMock.Setup(u => u.HouseholdMembers.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);
        _unitOfWorkMock.Setup(u => u.MonthlyBudgets.GetByHouseholdAndPeriodAsync(householdId, year, month, It.IsAny<CancellationToken>()))
            .ReturnsAsync((HouseholdMonthlyBudget?)null);
        _unitOfWorkMock.Setup(u => u.HouseholdExpenses.GetByHouseholdAndPeriodAsync(householdId, year, month, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<HouseholdExpense>());

        HouseholdMonthlyBudget? addedBudget = null;
        _unitOfWorkMock.Setup(u => u.MonthlyBudgets.AddAsync(It.IsAny<HouseholdMonthlyBudget>(), It.IsAny<CancellationToken>()))
            .Callback<HouseholdMonthlyBudget, CancellationToken>((b, t) => addedBudget = b)
            .Returns(Task.CompletedTask);

        var request = new SetMonthlyBudgetRequest
        {
            Year = year,
            Month = month,
            TargetAmount = 7000m,
            Notes = "September Budget"
        };

        // Act
        var result = await _sut.SetMonthlyBudgetAsync(userId, request);

        // Assert
        result.Success.Should().BeTrue();
        addedBudget.Should().NotBeNull();
        addedBudget!.Amount.Should().Be(7000m);
        addedBudget.BudgetDate.Year.Should().Be(year);
        addedBudget.BudgetDate.Month.Should().Be(month);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
