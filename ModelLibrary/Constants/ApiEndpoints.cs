namespace ModelLib.Constants
{
    public class ApiEndpoints
    {

        public const string AUTHENTICATION_FACEBOOK = "facebook-login";
        public const string AUTHENTICATION_LOG_OUT = "log-out";
        public const string AUTHENTICATION_USER_INFO = "user-info";

        private const string BASE = "api/";
        public const string AUTHENTICATION = BASE + "authentication/";

        public const string FACEBOOK_LOGIN = AUTHENTICATION + AUTHENTICATION_FACEBOOK;
        public const string LOG_OUT = AUTHENTICATION + AUTHENTICATION_LOG_OUT;
        public const string USER_INFO = AUTHENTICATION + AUTHENTICATION_USER_INFO;
    }
}
