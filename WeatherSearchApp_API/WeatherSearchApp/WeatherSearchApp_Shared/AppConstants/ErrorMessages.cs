namespace WeatherSearchApp_Shared.AppConstants
{
    public static class ErrorMessages
    {
        public const string UserAlreadyExist = "User Already Exist!!";

        public const string InvalidEmailOrPassword = "Incorrect email or password!!";

        public const string GenericError = "Oops! Something went wrong!";

        public const string InvalidCityName = "Invalid city name. Please enter a valid city name.";

        public static string InvalidUser = "Invalid User";

        public static string ValidateAccount = "Account not verified. Please validate your account.";

        public static string SecurityCodeExpired = "Security Code invalid or expired. Please request a new Security Code.";

        public static string TooManyAttepmts = "Too Many Attempts";
        public static string GenericErrorControllerMessage = "An error occurred while processing the request.";

        public static string AccountLocked = "Your account has been locked due to too many login attempts. Please try again later or contact support.";

        public static string FailedAccountVerification = "Account verification failed. The verification link may be expired or incorrect. Please request a new verification link.";
        public static string FailedUpdateOfTwoFactorAuth = "Failed to update the two factor auth status";

    }
}
