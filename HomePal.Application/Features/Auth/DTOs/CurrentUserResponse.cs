using HomePal.Shared.Results;
using HomePal.Domain.Enums;

namespace HomePal.Application.Features.Auth.DTOs;

public class CurrentUserResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string Governorate { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool EmailConfirmed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
}
