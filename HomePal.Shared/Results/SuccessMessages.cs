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

    public static class Catalog
    {
        public const string GetAllProductCategories = "Catalog.GetAllProductCategoriesSuccess";
        public const string GetProductCategory = "Catalog.GetProductCategorySuccess";
        public const string CreateProductCategory = "Catalog.CreateProductCategorySuccess";
        public const string UpdateProductCategory = "Catalog.UpdateProductCategorySuccess";
        public const string DeleteProductCategory = "Catalog.DeleteProductCategorySuccess";

        public const string GetAllMeasuringUnits = "Catalog.GetAllMeasuringUnitsSuccess";
        public const string GetMeasuringUnit = "Catalog.GetMeasuringUnitSuccess";
        public const string CreateMeasuringUnit = "Catalog.CreateMeasuringUnitSuccess";
        public const string UpdateMeasuringUnit = "Catalog.UpdateMeasuringUnitSuccess";
        public const string DeleteMeasuringUnit = "Catalog.DeleteMeasuringUnitSuccess";

        public const string GetAllSupermarkets = "Catalog.GetAllSupermarketsSuccess";
        public const string GetSupermarket = "Catalog.GetSupermarketSuccess";
        public const string CreateSupermarket = "Catalog.CreateSupermarketSuccess";
        public const string UpdateSupermarket = "Catalog.UpdateSupermarketSuccess";
        public const string DeleteSupermarket = "Catalog.DeleteSupermarketSuccess";

        public const string GetCanonicalProducts = "Catalog.GetCanonicalProductsSuccess";
        public const string GetCanonicalProduct = "Catalog.GetCanonicalProductSuccess";
        public const string CreateCanonicalProduct = "Catalog.CreateCanonicalProductSuccess";
        public const string UpdateCanonicalProduct = "Catalog.UpdateCanonicalProductSuccess";
        public const string DeleteCanonicalProduct = "Catalog.DeleteCanonicalProductSuccess";

        public const string GetOffers = "Catalog.GetOffersSuccess";
        public const string GetOffer = "Catalog.GetOfferSuccess";
        public const string CreateOffer = "Catalog.CreateOfferSuccess";
        public const string UpdateOffer = "Catalog.UpdateOfferSuccess";
        public const string DeleteOffer = "Catalog.DeleteOfferSuccess";

        public const string UploadSupermarketLogo = "Catalog.UploadSupermarketLogoSuccess";
        public const string UploadCanonicalProductImage = "Catalog.UploadCanonicalProductImageSuccess";
        public const string UploadOfferImage = "Catalog.UploadOfferImageSuccess";
        public const string UploadProductCategoryImage = "Catalog.UploadProductCategoryImageSuccess";

        public const string DeleteSupermarketLogo = "Catalog.DeleteSupermarketLogoSuccess";
        public const string DeleteCanonicalProductImage = "Catalog.DeleteCanonicalProductImageSuccess";
        public const string DeleteOfferImage = "Catalog.DeleteOfferImageSuccess";
        public const string DeleteProductCategoryImage = "Catalog.DeleteProductCategoryImageSuccess";
        public const string ScrapeJobStarted = "Catalog.ScrapeJobStarted";
    }

    public static class Pantry
    {
        public const string GetItems = "Pantry.GetItemsSuccess";
        public const string GetItem = "Pantry.GetItemSuccess";
        public const string CreateItem = "Pantry.CreateItemSuccess";
        public const string UpdateItem = "Pantry.UpdateItemSuccess";
        public const string UpdateEntireItems = "Pantry.UpdateEntireItemsSuccess";
        public const string DeleteItem = "Pantry.DeleteItemSuccess";
        public const string Scan = "Pantry.ScanSuccess";
        public const string BulkAdd = "Pantry.BulkAddSuccess";
    }

    public static class UserManagement
    {
        public const string GetUsers = "UserManagement.GetUsersSuccess";
        public const string GetUser = "UserManagement.GetUserSuccess";
        public const string AddAdmin = "UserManagement.AddAdminSuccess";
        public const string UpdateAdmin = "UserManagement.UpdateAdminSuccess";
        public const string DeleteUser = "UserManagement.DeleteUserSuccess";
        public const string DeactivateAccount = "UserManagement.DeactivateAccountSuccess";
        public const string DeleteAdmin = "UserManagement.DeleteAdminSuccess";
    }

    public static class MealPlan
    {
        public const string Create = "MealPlan.CreateSuccess";
        public const string Get = "MealPlan.GetSuccess";
        public const string GetAll = "MealPlan.GetAllSuccess";
        public const string GetLast = "MealPlan.GetLastSuccess";
        public const string Update = "MealPlan.UpdateSuccess";
        public const string Delete = "MealPlan.DeleteSuccess";
    }

    public static class Locations
    {
        public const string GetAllGovernorates = "Locations.GetAllGovernoratesSuccess";
        public const string GetGovernorate = "Locations.GetGovernorateSuccess";
        public const string GetAllCities = "Locations.GetAllCitiesSuccess";
        public const string GetCity = "Locations.GetCitySuccess";
    }

    public static class Analytics
    {
        public const string Fetch = "Analytics.FetchSuccess";
    }

    public static class Reports
    {
        public const string Fetch = "Reports.FetchSuccess";
    }

    public static class Subscriptions
    {
        public const string GetPlans = "Subscriptions.GetPlansSuccess";
        public const string GetCurrentSubscription = "Subscriptions.GetCurrentSubscriptionSuccess";
        public const string InitiatePayment = "Subscriptions.InitiatePaymentSuccess";
        public const string GetPaymentHistory = "Subscriptions.GetPaymentHistorySuccess";
        public const string WebhookProcessed = "Subscriptions.WebhookProcessedSuccess";
    }
}

