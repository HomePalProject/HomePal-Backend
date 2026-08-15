using HomePal.Application.Features.HouseholdManagement.DTOs;
using HomePal.Domain.Common;
using HomePal.Domain.Constants;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.HouseholdManagement.Mappers;

public static class HouseholdMapper
{
    public static Household ToEntity(this CreateHouseholdRequest request)
    {
        return new Household
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Address = request.Address?.Trim(),
            GovernorateId = request.GovernorateId,
            CityId = request.CityId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static HouseholdResponse ToResponse(this Household household, int membersCount, string? culture = null)
    {
        return new HouseholdResponse
        {
            Id = household.Id,
            Name = household.Name,
            Address = household.Address,
            GovernorateId = household.GovernorateId,
            Governorate = household.Governorate?.Name.Get(culture),
            CityId = household.CityId,
            City = household.City?.Name.Get(culture),
            CreatedAt = household.CreatedAt,
            MembersCount = membersCount
        };
    }

    public static HouseholdMember ToOfflineMemberEntity(this AddOfflineMemberRequest request, Guid householdId)
    {
        return new HouseholdMember
        {
            Id = Guid.NewGuid(),
            HouseholdId = householdId,
            UserId = null,
            FullName = request.FullName.Trim(),
            Gender = request.Gender,
            DateOfBirth = request.DateOfBirth,
            Role = Roles.HouseholdMember,
            JoinedAt = DateTime.UtcNow
        };
    }

    public static HouseholdMemberResponse ToResponse(this HouseholdMember member)
    {
        return new HouseholdMemberResponse
        {
            Id = member.Id,
            HouseholdId = member.HouseholdId,
            UserId = member.UserId,
            UserName = member.User?.UserName,
            FullName = member.FullName,
            Gender = member.Gender,
            DateOfBirth = member.DateOfBirth,
            Role = member.Role,
            JoinedAt = member.JoinedAt
        };
    }

    public static IReadOnlyCollection<HouseholdMemberResponse> ToResponseList(this IEnumerable<HouseholdMember> members)
    {
        return members.Select(m => m.ToResponse()).ToList();
    }

    public static HouseholdInvitationResponse ToResponse(this HouseholdInvitation invitation, string householdName, string inviterName)
    {
        return new HouseholdInvitationResponse
        {
            Id = invitation.Id,
            HouseholdId = invitation.HouseholdId,
            HouseholdName = householdName,
            InvitedEmail = invitation.InvitedEmail,
            InvitedUserName = invitation.InvitedUserName,
            InvitedById = invitation.InvitedById,
            InvitedByName = inviterName,
            Token = invitation.Token,
            Status = invitation.Status.ToString(),
            CreatedAt = invitation.CreatedAt
        };
    }

    public static IReadOnlyCollection<HouseholdInvitationResponse> ToResponseList(this IEnumerable<HouseholdInvitation> invitations)
    {
        return invitations.Select(i => i.ToResponse(
            i.Household?.Name ?? string.Empty,
            i.InvitedBy?.FullName ?? string.Empty
        )).ToList();
    }
}
