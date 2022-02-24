namespace ModelLib.Constants
{
    public class ApiEndpoints
    {

        public const string AUTHENTICATION_FACEBOOK = "facebook-login";
        public const string AUTHENTICATION_LOG_OUT = "log-out";
        public const string AUTHENTICATION_USER_INFO = "user-info";

        public const string MAP_POINTS = "points/";

        private const string BASE = "api/";
        public const string AUTHENTICATION = BASE + "authentication/";
        public const string MAP = BASE + "map/";

        // full api endpoints for the client to use:
        public const string POST_FACEBOOK_LOGIN = AUTHENTICATION + AUTHENTICATION_FACEBOOK;
        public const string GET_LOG_OUT = AUTHENTICATION + AUTHENTICATION_LOG_OUT;
        public const string GET_USER_INFO = AUTHENTICATION + AUTHENTICATION_USER_INFO;

        public const string POST_MAP_POINTS = MAP + MAP_POINTS;
    }
}
