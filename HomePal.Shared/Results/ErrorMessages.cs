namespace HomePal.Shared.Results;

public static class ErrorMessages
{
    public const string General = "Error.General";

    public static class Server
    {
        public const string InternalError = "Server.InternalError";
    }

    public static class Validation
    {
        public const string General = "Validation.General";
        public const string Required = "Validation.Required";
        public const string Email = "Validation.Email";
        public const string PasswordFormat = "Validation.PasswordFormat";
        public const string Compare = "Validation.Compare";
        public const string InvalidValue = "Validation.InvalidValue";
    }

    public static class Auth
    {
        public const string EmailExists = "Auth.EmailExists";
        public const string UsernameExists = "Auth.UsernameExists";
        public const string RegistrationFailed = "Auth.RegistrationFailed";
        public const string InvalidCredentials = "Auth.InvalidCredentials";
        public const string AccountInactive = "Auth.AccountInactive";
        public const string InvalidGoogleToken = "Auth.InvalidGoogleToken";
        public const string GoogleRegistrationFailed = "Auth.GoogleRegistrationFailed";
        public const string InvalidRefreshToken = "Auth.InvalidRefreshToken";
        public const string InactiveRefreshToken = "Auth.InactiveRefreshToken";
        public const string UserNotFound = "Auth.UserNotFound";
        public const string ResetPasswordFailed = "Auth.ResetPasswordFailed";
        public const string ChangePasswordFailed = "Auth.ChangePasswordFailed";
        public const string ConfirmEmailFailed = "Auth.ConfirmEmailFailed";
        public const string EmailAlreadyConfirmed = "Auth.EmailAlreadyConfirmed";
        public const string UpdateProfileFailed = "Auth.UpdateProfileFailed";
        public const string EmailNotConfirmed = "Auth.EmailNotConfirmed";
        public const string InvalidImageFile = "Auth.InvalidImageFile";
        public const string ImageSizeExceeded = "Auth.ImageSizeExceeded";
        public const string ProfileImageUploadFailed = "Auth.ProfileImageUploadFailed";
    }

    public static class Household
    {
        public const string AlreadyInHousehold = "Household.AlreadyInHousehold";
        public const string UserAlreadyInHousehold = "Household.UserAlreadyInHousehold";
        public const string UserNotFound = "Household.UserNotFound";
        public const string HouseholdNotFound = "Household.HouseholdNotFound";
        public const string NotManager = "Household.NotManager";
        public const string MemberNotFound = "Household.MemberNotFound";
        public const string InvitationNotFound = "Household.InvitationNotFound";
        public const string PendingInvitationExists = "Household.PendingInvitationExists";
        public const string InvitationExpiredOrInvalid = "Household.InvitationExpiredOrInvalid";
        public const string InvitationNotForUser = "Household.InvitationNotForUser";
        public const string OnlyPendingCanBeCancelled = "Household.OnlyPendingCanBeCancelled";
        public const string MemberRemovalUnauthorized = "Household.MemberRemovalUnauthorized";
        public const string CannotRemoveOnlyManager = "Household.CannotRemoveOnlyManager";
        public const string ManagerCannotRemoveSelf = "Household.ManagerCannotRemoveSelf";
        public const string PreferenceNotFound = "Household.PreferenceNotFound";
        public const string PreferenceAlreadyExists = "Household.PreferenceAlreadyExists";
        public const string PreferenceManagementUnauthorized = "Household.PreferenceManagementUnauthorized";
        public const string CategoryNotFound = "Household.CategoryNotFound";
        public const string CategoryAlreadyExists = "Household.CategoryAlreadyExists";
        public const string CategoryHasPreferences = "Household.CategoryHasPreferences";
    }

    public static class Pantry
    {
        public const string PantryNotFound = "PantryNotFound";
        public const string ItemNotFound = "PantryItemNotFound";
        public const string InvalidItemData = "Pantry.InvalidItemData";
    }

    public static class Scan
    {
        public const string NoImageUploaded = "NoImageUploaded";
        public const string InvalidImage = "InvalidImage";
        public const string ScanFailed = "ScanFailed";
    }
}
