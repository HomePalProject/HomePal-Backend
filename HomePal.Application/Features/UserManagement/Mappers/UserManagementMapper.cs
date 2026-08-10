using HomePal.Application.Features.UserManagement.DTOs;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.UserManagement.Mappers;

public static class UserManagementMapper
{
    public static UserResponse ToResponse(this ApplicationUser user, IList<string> roles)
    {
        return new UserResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            Gender = user.Gender,
            Username = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
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

    public static ApplicationUser ToEntity(this CreateAdminRequest request)
    {
        return new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = request.Username,
            Email = request.Email,
            FullName = request.FullName,
            Gender = request.Gender,
            BirthDate = request.BirthDate,
            Governorate = request.Governorate,
            City = request.City,
            PhoneNumber = request.PhoneNumber,
            IsActive = true,
            IsProfileComplete = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };
    }
}
