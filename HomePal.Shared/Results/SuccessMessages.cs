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
    }
}
