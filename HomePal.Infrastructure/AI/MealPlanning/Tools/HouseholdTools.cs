using System.ComponentModel;
using HomePal.Application.Features.HouseholdManagement.Interfaces;
using HomePal.Infrastructure.AI.Common;

namespace HomePal.Infrastructure.AI.MealPlanning.Tools;

/// <summary>
/// AI Agent Tool for retrieving household members and their dietary preferences/allergies via application services.
/// </summary>
public class HouseholdTools
{
    private readonly IHouseholdMemberService _householdMemberService;
    private readonly IMemberPreferenceService _memberPreferenceService;
    private readonly AgentUserContext _userContext;

    public HouseholdTools(
        IHouseholdMemberService householdMemberService,
        IMemberPreferenceService memberPreferenceService,
        AgentUserContext userContext)
    {
        _householdMemberService = householdMemberService;
        _memberPreferenceService = memberPreferenceService;
        _userContext = userContext;
    }

    [Description("Gets all members in the user's household along with their dietary preferences, allergies, health restrictions, age, gender, and roles. Preferences include food allergies — treat any registered allergy as a hard safety constraint that must never be violated. IMPORTANT: Always call this tool before generating any meal recommendations or recipe suggestions.")]
    public async Task<object> GetHouseholdMembersWithPreferencesAsync(CancellationToken cancellationToken = default)
    {
        var userId = _userContext.GetCurrentUserId();
        if (userId == Guid.Empty)
            return new { success = false, error = "User is not authenticated." };

        var membersResult = await _householdMemberService.GetHouseholdMembersAsync(userId, cancellationToken);
        if (!membersResult.Success || membersResult.Data == null)
            return new { success = false, error = membersResult.Message };

        var memberList = new List<object>();

        foreach (var member in membersResult.Data)
        {
            var preferencesResult = await _memberPreferenceService.GetMemberPreferencesAsync(userId, member.Id, cancellationToken);
            var preferences = preferencesResult.Success && preferencesResult.Data != null
                ? preferencesResult.Data.Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    category = p.CategoryName,
                    description = p.Description
                })
                : [];

            int? age = member.DateOfBirth.HasValue
                ? DateTime.UtcNow.Year - member.DateOfBirth.Value.Year - (DateTime.UtcNow.DayOfYear < member.DateOfBirth.Value.DayOfYear ? 1 : 0)
                : null;

            memberList.Add(new
            {
                id = member.Id,
                fullName = member.FullName,
                userName = member.UserName,
                gender = member.Gender.ToString(),
                role = member.Role,
                dateOfBirth = member.DateOfBirth?.ToString("yyyy-MM-dd"),
                age,
                preferences
            });
        }

        return new
        {
            success = true,
            totalMembers = memberList.Count,
            members = memberList
        };
    }
}
