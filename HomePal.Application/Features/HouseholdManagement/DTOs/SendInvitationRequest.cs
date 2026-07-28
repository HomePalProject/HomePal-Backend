using System.ComponentModel.DataAnnotations;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.HouseholdManagement.DTOs;

public class SendInvitationRequest
{
    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [StringLength(100, MinimumLength = 3, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public string InvitedUserNameOrEmail { get; set; } = string.Empty;
}
