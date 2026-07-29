using HomePal.Application.Features.Auth.DTOs;
using HomePal.Domain.Constants;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.Auth.Mappers;

public static class AuthMapper
{
    public static ApplicationUser ToEntity(this RegisterRequest request)
    {
        return new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            Gender = request.Gender!.Value,
            UserName = request.Username.Trim(),
            Email = request.Email.Trim(),
            BirthDate = request.BirthDate!.Value,
            Governorate = request.Governorate.Trim(),
            City = request.City.Trim(),
            IsActive = true,
            IsProfileComplete = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static CurrentUserResponse ToCurrentUserResponse(this ApplicationUser user, IList<string> roles)
    {
        return new CurrentUserResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            Gender = user.Gender,
            Username = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            BirthDate = user.BirthDate,
            Governorate = user.Governorate,
            City = user.City,
            IsActive = user.IsActive,
            IsProfileComplete = user.IsProfileComplete,
            ProfileImageUrl = user.ProfileImageUrl,
            EmailConfirmed = user.EmailConfirmed,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            Roles = roles
        };
    }
}
