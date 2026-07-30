namespace HomePal.Shared.Results;

public static class SuccessMessages
{
    public const string General = "Success.General";

    public static class Auth
    {
        public const string Register = "Auth.RegisterSuccess";
        public const string Login = "Auth.LoginSuccess";
        public const string GoogleLogin = "Auth.GoogleLoginSuccess";
        public const string RefreshToken = "Auth.RefreshTokenSuccess";
        public const string Logout = "Auth.LogoutSuccess";
        public const string ForgotPassword = "Auth.ForgotPasswordSuccess";
        public const string ResetPassword = "Auth.ResetPasswordSuccess";
        public const string ChangePassword = "Auth.ChangePasswordSuccess";
        public const string ConfirmEmail = "Auth.ConfirmEmailSuccess";
        public const string ResendConfirmation = "Auth.ResendConfirmationSuccess";
        public const string GetCurrentUser = "Auth.GetCurrentUserSuccess";
        public const string UpdateProfile = "Auth.UpdateProfileSuccess";
        public const string UpdateProfileImage = "Auth.UpdateProfileImageSuccess";
        public const string DeleteProfileImage = "Auth.DeleteProfileImageSuccess";
    }

    public static class Household
    {
        public const string Create = "Household.CreateSuccess";
        public const string Get = "Household.GetSuccess";
        public const string Update = "Household.UpdateSuccess";
        public const string Delete = "Household.DeleteSuccess";
        public const string GetMembers = "Household.GetMembersSuccess";
        public const string GetMember = "Household.GetMemberSuccess";
        public const string AddMember = "Household.AddMemberSuccess";
        public const string UpdateMember = "Household.UpdateMemberSuccess";
        public const string RemoveMember = "Household.RemoveMemberSuccess";
        public const string SendInvitation = "Household.SendInvitationSuccess";
        public const string GetInvitations = "Household.GetInvitationsSuccess";
        public const string CancelInvitation = "Household.CancelInvitationSuccess";
        public const string AcceptInvitation = "Household.AcceptInvitationSuccess";
        public const string DeclineInvitation = "Household.DeclineInvitationSuccess";
        public const string GetAllPreferences = "Household.GetAllPreferencesSuccess";
        public const string SearchPreferences = "Household.SearchPreferencesSuccess";
        public const string GetPreference = "Household.GetPreferenceSuccess";
        public const string GetPreferences = "Household.GetPreferencesSuccess";
        public const string SetPreferences = "Household.SetPreferencesSuccess";
        public const string AddPreference = "Household.AddPreferenceSuccess";
        public const string UpdatePreference = "Household.UpdatePreferenceSuccess";
        public const string DeletePreference = "Household.DeletePreferenceSuccess";
        public const string RemovePreference = "Household.RemovePreferenceSuccess";
        public const string GetAllCategories = "Household.GetAllCategoriesSuccess";
        public const string SearchCategories = "Household.SearchCategoriesSuccess";
        public const string GetCategory = "Household.GetCategorySuccess";
        public const string AddCategory = "Household.AddCategorySuccess";
        public const string UpdateCategory = "Household.UpdateCategorySuccess";
        public const string DeleteCategory = "Household.DeleteCategorySuccess";
    }

    public static class Pantry
    {
        public const string GetItems = "PantryItemsGet";
        public const string GetItem = "PantryItemGet";
        public const string CreateItem = "PantryItemCreated";
        public const string UpdateItem = "PantryItemUpdated";
        public const string DeleteItem = "PantryItemDeleted";
        public const string PantryCreated = "PantryCreated";
        public const string PantryUpdated = "PantryUpdated";
        public const string PantryDeleted = "PantryDeleted";
    }

    public static class Scan
    {
        public const string ScanSuccess = "ScanSuccess";
    }
}
