namespace HomePal.Domain.Constants;

public static class Roles
{
    public const string Admin = "Admin";
    public const string HouseholdManager = "Household Manager";
    public const string HouseholdMember = "Household Member";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        Admin,
        HouseholdManager,
        HouseholdMember
    };
}
