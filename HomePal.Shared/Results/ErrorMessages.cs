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
    }
}
