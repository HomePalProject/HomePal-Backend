using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.Auth.DTOs;
using HomePal.Application.Features.Auth.Interfaces;
using HomePal.Application.Features.Auth.Options;
using HomePal.Application.Features.Auth.Services;
using HomePal.Domain.Constants;
using HomePal.Domain.Entities;
using HomePal.Domain.Enums;
using HomePal.Shared.Results;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace HomePal.UnitTests.Features.Auth;

public class AuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<RoleManager<IdentityRole<Guid>>> _roleManagerMock;
    private readonly Mock<ITokenProvider> _tokenProviderMock;
    private readonly Mock<IEmailSender> _emailSenderMock;
    private readonly Mock<IGoogleTokenValidator> _googleTokenValidatorMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly IOptions<ClientOptions> _clientOptions;

    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        var roleStoreMock = new Mock<IRoleStore<IdentityRole<Guid>>>();
        _roleManagerMock = new Mock<RoleManager<IdentityRole<Guid>>>(
            roleStoreMock.Object, null!, null!, null!, null!);

        _tokenProviderMock = new Mock<ITokenProvider>();
        _emailSenderMock = new Mock<IEmailSender>();
        _googleTokenValidatorMock = new Mock<IGoogleTokenValidator>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _clientOptions = Options.Create(new ClientOptions { BaseUrl = "http://localhost:3000" });

        _sut = new AuthService(
            _userManagerMock.Object,
            _roleManagerMock.Object,
            _tokenProviderMock.Object,
            _emailSenderMock.Object,
            _googleTokenValidatorMock.Object,
            _unitOfWorkMock.Object,
            _fileStorageServiceMock.Object,
            _clientOptions);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnConflict_WhenEmailAlreadyExists()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Username = "testuser",
            Password = "Password123!",
            FullName = "Test User"
        };

        _userManagerMock.Setup(u => u.FindByEmailAsync(request.Email))
            .ReturnsAsync(new ApplicationUser { Email = request.Email });

        // Act
        var result = await _sut.RegisterAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Conflict);
        result.Message.Should().Be(ErrorMessages.Auth.EmailExists);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnConflict_WhenUsernameAlreadyExists()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "new@example.com",
            Username = "existinguser",
            Password = "Password123!",
            FullName = "New User"
        };

        _userManagerMock.Setup(u => u.FindByEmailAsync(request.Email))
            .ReturnsAsync((ApplicationUser?)null);
        _userManagerMock.Setup(u => u.FindByNameAsync(request.Username))
            .ReturnsAsync(new ApplicationUser { UserName = request.Username });

        // Act
        var result = await _sut.RegisterAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Conflict);
        result.Message.Should().Be(ErrorMessages.Auth.UsernameExists);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnCreated_WhenDataIsValid()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "john@example.com",
            Username = "john_doe",
            Password = "Password123!",
            FullName = "John Doe",
            Gender = Gender.Male,
            BirthDate = new DateOnly(1995, 5, 15)
        };

        _userManagerMock.Setup(u => u.FindByEmailAsync(request.Email))
            .ReturnsAsync((ApplicationUser?)null);
        _userManagerMock.Setup(u => u.FindByNameAsync(request.Username))
            .ReturnsAsync((ApplicationUser?)null);

        _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
            .ReturnsAsync(IdentityResult.Success);

        _roleManagerMock.Setup(r => r.RoleExistsAsync(Roles.HouseholdManager))
            .ReturnsAsync(true);

        _userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), Roles.HouseholdManager))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(u => u.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync("sample_token");

        _userManagerMock.Setup(u => u.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(new List<string> { Roles.HouseholdManager });

        // Act
        var result = await _sut.RegisterAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Created);
        result.Data.Should().NotBeNull();
        result.Data!.Email.Should().Be("john@example.com");
        result.Data!.FullName.Should().Be("John Doe");
        result.Data!.Roles.Should().Contain(Roles.HouseholdManager);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnUnauthorized_WhenUserNotFound()
    {
        // Arrange
        var request = new LoginRequest
        {
            EmailOrUsername = "nonexistent@example.com",
            Password = "WrongPassword"
        };

        _userManagerMock.Setup(u => u.FindByEmailAsync(request.EmailOrUsername))
            .ReturnsAsync((ApplicationUser?)null);
        _userManagerMock.Setup(u => u.FindByNameAsync(request.EmailOrUsername))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Unauthorized);
        result.Message.Should().Be(ErrorMessages.Auth.InvalidCredentials);
    }
}
