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
        public const string CannotPromoteOfflineMember = "Household.CannotPromoteOfflineMember";
        public const string CannotDemoteSelfAsOnlyManager = "Household.CannotDemoteSelfAsOnlyManager";
        public const string ManagerCannotRemoveSelf = "Household.ManagerCannotRemoveSelf";
        public const string PreferenceNotFound = "Household.PreferenceNotFound";
        public const string PreferenceAlreadyExists = "Household.PreferenceAlreadyExists";
        public const string PreferenceManagementUnauthorized = "Household.PreferenceManagementUnauthorized";
        public const string CategoryNotFound = "Household.CategoryNotFound";
        public const string CategoryAlreadyExists = "Household.CategoryAlreadyExists";
        public const string CategoryHasPreferences = "Household.CategoryHasPreferences";
    }

    public static class Catalog
    {
        public const string ProductCategoryNotFound = "Catalog.ProductCategoryNotFound";
        public const string MeasuringUnitNotFound = "Catalog.MeasuringUnitNotFound";
        public const string SupermarketNotFound = "Catalog.SupermarketNotFound";
        public const string CanonicalProductNotFound = "Catalog.CanonicalProductNotFound";
        public const string OfferNotFound = "Catalog.OfferNotFound";

        public const string ImageUploadFailed = "Catalog.ImageUploadFailed";
        public const string ScrapeJobInProgress = "Catalog.ScrapeJobInProgress";
    }

    public static class Pantry
    {
        public const string PantryNotFound = "Pantry.PantryNotFound";
        public const string PantryItemNotFound = "Pantry.PantryItemNotFound";
        public const string InvalidQuantity = "Pantry.InvalidQuantity";
        public const string NoHousehold = "Pantry.NoHousehold";
        public const string ScanFailed = "Pantry.ScanFailed";
    }

    public static class UserManagement
    {
        public const string UserNotFound = "UserManagement.UserNotFound";
        public const string AdminNotFound = "UserManagement.AdminNotFound";
        public const string CannotDeleteProtectedAdmin = "UserManagement.CannotDeleteProtectedAdmin";
        public const string CannotDeactivateProtectedAdmin = "UserManagement.CannotDeactivateProtectedAdmin";
        public const string UserIsNotAdmin = "UserManagement.UserIsNotAdmin";
        public const string AddAdminFailed = "UserManagement.AddAdminFailed";
        public const string UpdateAdminFailed = "UserManagement.UpdateAdminFailed";
    }

    public static class Budget
    {
        public const string BudgetNotFound = "Budget.BudgetNotFound";
        public const string ExpenseNotFound = "Budget.ExpenseNotFound";
        public const string InvalidYearOrMonth = "Budget.InvalidYearOrMonth";
        public const string InvalidAmount = "Budget.InvalidAmount";
        public const string InvalidTitle = "Budget.InvalidTitle";
    }

    public static class AgentChat
    {
        public const string ChatNotFound = "AgentChat.ChatNotFound";
        public const string MessageRequired = "AgentChat.MessageRequired";
        public const string ToolCallIdRequired = "AgentChat.ToolCallIdRequired";
        public const string ChatCleared = "AgentChat.ChatCleared";
        public const string GetSessionSuccess = "AgentChat.GetSessionSuccess";
    }

    public static class MealPlan
    {
        public const string MealPlanNotFound = "MealPlan.MealPlanNotFound";
        public const string InvalidDates = "MealPlan.InvalidDates";
        public const string NoHousehold = "MealPlan.NoHousehold";
    }

    public static class Locations
    {
        public const string GovernorateNotFound = "Locations.GovernorateNotFound";
        public const string CityNotFound = "Locations.CityNotFound";
    }

    public static class Subscriptions
    {
        public const string PlanNotFound = "Subscriptions.PlanNotFound";
        public const string PlanInactive = "Subscriptions.PlanInactive";
        public const string AlreadySubscribed = "Subscriptions.AlreadySubscribed";
        public const string PaymentInitiationFailed = "Subscriptions.PaymentInitiationFailed";
        public const string TransactionNotFound = "Subscriptions.TransactionNotFound";
        public const string InvalidWebhookPayload = "Subscriptions.InvalidWebhookPayload";
        public const string HmacVerificationFailed = "Subscriptions.HmacVerificationFailed";
        public const string OrderIdMissing = "Subscriptions.OrderIdMissing";
        public const string ActiveSubscriptionRequired = "Subscriptions.ActiveSubscriptionRequired";
    }
}


