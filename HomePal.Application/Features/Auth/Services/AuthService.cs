using HomePal.Application.Features.Auth.DTOs;
using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.Auth.Interfaces;
using HomePal.Application.Features.Auth.Mappers;
using HomePal.Application.Features.Auth.Options;
using HomePal.Domain.Constants;
using HomePal.Domain.Entities;
using HomePal.Domain.Enums;
using HomePal.Shared.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace HomePal.Application.Features.Auth.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly ITokenProvider _tokenProvider;
    private readonly IEmailSender _emailSender;
    private readonly IGoogleTokenValidator _googleTokenValidator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    private readonly ClientOptions _clientOptions;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        ITokenProvider tokenProvider,
        IEmailSender emailSender,
        IGoogleTokenValidator googleTokenValidator,
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService,
        IOptions<ClientOptions> clientOptions)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenProvider = tokenProvider;
        _emailSender = emailSender;
        _googleTokenValidator = googleTokenValidator;
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
        _clientOptions = clientOptions.Value;
    }

    public async Task<Result<CurrentUserResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var existingEmail = await _userManager.FindByEmailAsync(request.Email);
        if (existingEmail != null)
        {
            return Result<CurrentUserResponse>.Fail(ErrorMessages.Auth.EmailExists, ResultStatus.Conflict);
        }

        var existingUsername = await _userManager.FindByNameAsync(request.Username);
        if (existingUsername != null)
        {
            return Result<CurrentUserResponse>.Fail(ErrorMessages.Auth.UsernameExists, ResultStatus.Conflict);
        }

        var user = request.ToEntity();
        var createResult = await _userManager.CreateAsync(user, request.Password);

        if (!createResult.Succeeded)
        {
            var errors = createResult.Errors.Select(e => new Error(e.Description)).ToList();
            return Result<CurrentUserResponse>.Fail(ErrorMessages.Auth.RegistrationFailed, ResultStatus.BadRequest, errors);
        }

        if (!await _roleManager.RoleExistsAsync(Roles.HouseholdManager))
        {
            await _roleManager.CreateAsync(new IdentityRole<Guid>(Roles.HouseholdManager));
        }
        await _userManager.AddToRoleAsync(user, Roles.HouseholdManager);

        try
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"{_clientOptions.BaseUrl.TrimEnd('/')}/confirm-email.html?userId={user.Id}&token={Uri.EscapeDataString(token)}";
            await _emailSender.SendConfirmationEmailAsync(user.Email!, user.FullName, confirmationLink, cancellationToken);
        }
        catch
        {
            // Email sending failure shouldn't abort user registration
        }

        var roles = await _userManager.GetRolesAsync(user);
        return Result<CurrentUserResponse>.Ok(user.ToCurrentUserResponse(roles), SuccessMessages.Auth.Register, ResultStatus.Created);
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.EmailOrUsername)
                   ?? await _userManager.FindByNameAsync(request.EmailOrUsername);

        if (user == null)
        {
            return Result<LoginResponse>.Fail(ErrorMessages.Auth.InvalidCredentials, ResultStatus.Unauthorized);
        }

        if (!user.IsActive)
        {
            return Result<LoginResponse>.Fail(ErrorMessages.Auth.AccountInactive, ResultStatus.Forbidden);
        }

        if (!user.EmailConfirmed)
        {
            return Result<LoginResponse>.Fail(ErrorMessages.Auth.EmailNotConfirmed, ResultStatus.Forbidden);
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
        {
            return Result<LoginResponse>.Fail(ErrorMessages.Auth.InvalidCredentials, ResultStatus.Unauthorized);
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);
        var tokenResponse = await _tokenProvider.GenerateTokensAsync(user, roles, cancellationToken);

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = tokenResponse.RefreshToken,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = tokenResponse.RefreshTokenExpiresAt
        };

        await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new LoginResponse
        {
            User = user.ToCurrentUserResponse(roles),
            Tokens = tokenResponse
        };

        return Result<LoginResponse>.Ok(response, SuccessMessages.Auth.Login);
    }

    public async Task<Result<LoginResponse>> GoogleLoginAsync(GoogleLoginRequest request, CancellationToken cancellationToken = default)
    {
        var payload = await _googleTokenValidator.ValidateTokenAsync(request.IdToken, cancellationToken);
        if (payload == null)
        {
            return Result<LoginResponse>.Fail(ErrorMessages.Auth.InvalidGoogleToken, ResultStatus.Unauthorized);
        }

        var user = await _userManager.FindByEmailAsync(payload.Email);

        if (user == null)
        {
            var username = payload.Email.Split('@')[0] + "_" + Guid.NewGuid().ToString("N")[..4];
            user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FullName = string.IsNullOrWhiteSpace(payload.Name) ? payload.Email.Split('@')[0] : payload.Name,
                UserName = username,
                Email = payload.Email,
                EmailConfirmed = payload.EmailVerified,
                IsActive = true,
                IsProfileComplete = false,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow,
                Governorate = "Not Specified",
                City = "Not Specified",
                Gender = Gender.Male,
                BirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20))
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                var errors = createResult.Errors.Select(e => new Error(e.Description)).ToList();
                return Result<LoginResponse>.Fail(ErrorMessages.Auth.GoogleRegistrationFailed, ResultStatus.BadRequest, errors);
            }

            if (!await _roleManager.RoleExistsAsync(Roles.HouseholdManager))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(Roles.HouseholdManager));
            }
            await _userManager.AddToRoleAsync(user, Roles.HouseholdManager);

            try
            {
                await _emailSender.SendWelcomeEmailAsync(user.Email!, user.FullName, cancellationToken);
            }
            catch
            {
                // Ignore email failure on registration
            }
        }
        else
        {
            if (!user.IsActive)
            {
                return Result<LoginResponse>.Fail(ErrorMessages.Auth.AccountInactive, ResultStatus.Forbidden);
            }

            if (!user.EmailConfirmed && payload.EmailVerified)
            {
                user.EmailConfirmed = true;
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var tokenResponse = await _tokenProvider.GenerateTokensAsync(user, roles, cancellationToken);

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = tokenResponse.RefreshToken,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = tokenResponse.RefreshTokenExpiresAt
        };

        await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var loginResponse = new LoginResponse
        {
            User = user.ToCurrentUserResponse(roles),
            Tokens = tokenResponse
        };

        return Result<LoginResponse>.Ok(loginResponse, SuccessMessages.Auth.GoogleLogin);
    }

    public async Task<Result<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var existingToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (existingToken == null)
        {
            return Result<TokenResponse>.Fail(ErrorMessages.Auth.InvalidRefreshToken, ResultStatus.Unauthorized);
        }

        if (!existingToken.IsActive)
        {
            return Result<TokenResponse>.Fail(ErrorMessages.Auth.InactiveRefreshToken, ResultStatus.Unauthorized);
        }

        var user = await _userManager.FindByIdAsync(existingToken.UserId.ToString());
        if (user == null || !user.IsActive)
        {
            return Result<TokenResponse>.Fail(ErrorMessages.Auth.UserNotFound, ResultStatus.NotFound);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var newTokens = await _tokenProvider.GenerateTokensAsync(user, roles, cancellationToken);

        existingToken.RevokedAt = DateTime.UtcNow;
        existingToken.ReplacedByToken = newTokens.RefreshToken;
        _unitOfWork.RefreshTokens.Update(existingToken);

        var newRefreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = newTokens.RefreshToken,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = newTokens.RefreshTokenExpiresAt
        };

        await _unitOfWork.RefreshTokens.AddAsync(newRefreshTokenEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<TokenResponse>.Ok(newTokens, SuccessMessages.Auth.RefreshToken);
    }

    public async Task<Result> LogoutAsync(Guid userId, string? refreshToken = null, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            var tokenEntity = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken, cancellationToken);
            if (tokenEntity != null && tokenEntity.UserId == userId && tokenEntity.IsActive)
            {
                tokenEntity.RevokedAt = DateTime.UtcNow;
                _unitOfWork.RefreshTokens.Update(tokenEntity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
        else
        {
            await _unitOfWork.RefreshTokens.RevokeAllUserTokensAsync(userId, cancellationToken: cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Ok(SuccessMessages.Auth.Logout);
    }

    public async Task<Result> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !user.IsActive)
        {
            // For security reasons, don't disclose whether email exists
            return Result.Ok(SuccessMessages.Auth.ForgotPassword);
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = $"{_clientOptions.BaseUrl.TrimEnd('/')}/reset-password.html?email={Uri.EscapeDataString(user.Email!)}&token={Uri.EscapeDataString(token)}";

        await _emailSender.SendResetPasswordEmailAsync(user.Email!, user.FullName, resetLink, cancellationToken);
        return Result.Ok(SuccessMessages.Auth.ForgotPassword);
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !user.IsActive)
        {
            return Result.Fail(ErrorMessages.Auth.UserNotFound, ResultStatus.NotFound);
        }

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => new Error(e.Description)).ToList();
            return Result.Fail(ErrorMessages.Auth.ResetPasswordFailed, ResultStatus.BadRequest, errors);
        }

        await _userManager.UpdateSecurityStampAsync(user);
        await _unitOfWork.RefreshTokens.RevokeAllUserTokensAsync(user.Id, cancellationToken: cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(SuccessMessages.Auth.ResetPassword);
    }

    public async Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null || !user.IsActive)
        {
            return Result.Fail(ErrorMessages.Auth.UserNotFound, ResultStatus.NotFound);
        }

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => new Error(e.Description)).ToList();
            return Result.Fail(ErrorMessages.Auth.ChangePasswordFailed, ResultStatus.BadRequest, errors);
        }

        await _userManager.UpdateSecurityStampAsync(user);
        await _unitOfWork.RefreshTokens.RevokeAllUserTokensAsync(user.Id, cancellationToken: cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(SuccessMessages.Auth.ChangePassword);
    }

    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            return Result.Fail(ErrorMessages.Auth.UserNotFound, ResultStatus.NotFound);
        }

        if (user.EmailConfirmed)
        {
            return Result.Fail(ErrorMessages.Auth.EmailAlreadyConfirmed, ResultStatus.Conflict);
        }

        var result = await _userManager.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => new Error(e.Description)).ToList();
            return Result.Fail(ErrorMessages.Auth.ConfirmEmailFailed, ResultStatus.BadRequest, errors);
        }

        return Result.Ok(SuccessMessages.Auth.ConfirmEmail);
    }

    public async Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result.Ok(SuccessMessages.Auth.ResendConfirmation); // Silent return for security
        }

        if (!user.IsActive)
        {
            return Result.Fail(ErrorMessages.Auth.AccountInactive, ResultStatus.Forbidden);
        }

        if (user.EmailConfirmed)
        {
            return Result.Fail(ErrorMessages.Auth.EmailAlreadyConfirmed, ResultStatus.Conflict);
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = $"{_clientOptions.BaseUrl.TrimEnd('/')}/confirm-email.html?userId={user.Id}&token={Uri.EscapeDataString(token)}";

        await _emailSender.SendConfirmationEmailAsync(user.Email!, user.FullName, confirmationLink, cancellationToken);
        return Result.Ok(SuccessMessages.Auth.ResendConfirmation);
    }

    public async Task<Result<CurrentUserResponse>> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result<CurrentUserResponse>.Fail(ErrorMessages.Auth.UserNotFound, ResultStatus.NotFound);
        }

        var roles = await _userManager.GetRolesAsync(user);
        return Result<CurrentUserResponse>.Ok(user.ToCurrentUserResponse(roles), SuccessMessages.Auth.GetCurrentUser);
    }

    public async Task<Result<CurrentUserResponse>> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result<CurrentUserResponse>.Fail(ErrorMessages.Auth.UserNotFound, ResultStatus.NotFound);
        }

        if (!user.IsActive)
        {
            return Result<CurrentUserResponse>.Fail(ErrorMessages.Auth.AccountInactive, ResultStatus.Forbidden);
        }

        user.FullName = request.FullName.Trim();
        user.Gender = request.Gender!.Value;
        user.BirthDate = request.BirthDate!.Value;
        user.Governorate = request.Governorate.Trim();
        user.City = request.City.Trim();
        user.IsProfileComplete = true;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return Result<CurrentUserResponse>.Fail(ErrorMessages.Auth.UpdateProfileFailed, ResultStatus.BadRequest);
        }

        var roles = await _userManager.GetRolesAsync(user);
        return Result<CurrentUserResponse>.Ok(user.ToCurrentUserResponse(roles), SuccessMessages.Auth.UpdateProfile);
    }

    public async Task<Result<CurrentUserResponse>> UpdateProfileImageAsync(Guid userId, Microsoft.AspNetCore.Http.IFormFile imageFile, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result<CurrentUserResponse>.Fail(ErrorMessages.Auth.UserNotFound, ResultStatus.NotFound);
        }

        if (!user.IsActive)
        {
            return Result<CurrentUserResponse>.Fail(ErrorMessages.Auth.AccountInactive, ResultStatus.Forbidden);
        }

        string newImageUrl;
        try
        {
            newImageUrl = await _fileStorageService.SaveFileAsync(imageFile, "profiles", cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            return Result<CurrentUserResponse>.Fail(ex.Message, ResultStatus.BadRequest);
        }
        catch
        {
            return Result<CurrentUserResponse>.Fail(ErrorMessages.Auth.ProfileImageUploadFailed, ResultStatus.BadRequest);
        }

        if (!string.IsNullOrWhiteSpace(user.ProfileImageUrl))
        {
            await _fileStorageService.DeleteFileAsync(user.ProfileImageUrl);
        }

        user.ProfileImageUrl = newImageUrl;
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return Result<CurrentUserResponse>.Fail(ErrorMessages.Auth.UpdateProfileFailed, ResultStatus.BadRequest);
        }

        var roles = await _userManager.GetRolesAsync(user);
        return Result<CurrentUserResponse>.Ok(user.ToCurrentUserResponse(roles), SuccessMessages.Auth.UpdateProfileImage);
    }

    public async Task<Result<CurrentUserResponse>> DeleteProfileImageAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result<CurrentUserResponse>.Fail(ErrorMessages.Auth.UserNotFound, ResultStatus.NotFound);
        }

        if (!user.IsActive)
        {
            return Result<CurrentUserResponse>.Fail(ErrorMessages.Auth.AccountInactive, ResultStatus.Forbidden);
        }

        if (!string.IsNullOrWhiteSpace(user.ProfileImageUrl))
        {
            await _fileStorageService.DeleteFileAsync(user.ProfileImageUrl);
            user.ProfileImageUrl = null;
            await _userManager.UpdateAsync(user);
        }

        var roles = await _userManager.GetRolesAsync(user);
        return Result<CurrentUserResponse>.Ok(user.ToCurrentUserResponse(roles), SuccessMessages.Auth.DeleteProfileImage);
    }
}
