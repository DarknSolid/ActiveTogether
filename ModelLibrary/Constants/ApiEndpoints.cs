namespace ModelLib.Constants
{
    public class ApiEndpoints
    {

        private const string BASE = "api/";

        public const string AUTHENTICATION = BASE + "authentication/";


        public const string AUTHENTICATION_FACEBOOK_LOGIN = "facebook-login/";
        public const string AUTHENTICATION_LOG_OUT = "log-out/";
        public const string AUTHENTICATION_LOGIN = "login/";
        public const string AUTHENTICATION_REGISTER = "register/";
        public const string AUTHENTICATION_USER_INFO = "user-info/";

        public const string DOG_PARKS_GET = "";

        public const string MAP_POINTS = "points/";

        public const string MAP = BASE + "map/";
        public const string DOG_PARKS = BASE + "dog-parks/";

        public const string REVIEWS = BASE + "reviews/";
        public const string REVIEWS_LIST = "";
        public const string REVIEWS_CREATE = "create/";

        public const string DOGS = BASE + "dogs/";
        public const string DOGS_CREATE = "";
        public const string DOGS_UPDATE = "";
        public const string DOGS_DELETE = "";
        public const string DOGS_LIST = "list/";
        public const string DOGS_DETAILED = "";

        public const string DOG_BREEDS = "breeds/";

        public const string CHECKINS = "checkins/";
        public const string CHECKINS_CHECK_IN = "";
        public const string CHECK_INS_CHECK_OUT = "check-out/";
        public const string CHECKINS_LIST = "list/";
        public const string CHECKINS_CURRENT_CHECK_IN = "current-check-in/";


        // full api endpoints for the client to use:
        public const string POST_FACEBOOK_LOGIN = AUTHENTICATION + AUTHENTICATION_FACEBOOK_LOGIN;
        public const string POST_LOGIN = AUTHENTICATION + AUTHENTICATION_LOGIN;
        public const string POST_REGISTER = AUTHENTICATION + AUTHENTICATION_REGISTER;
        public const string GET_LOG_OUT = AUTHENTICATION + AUTHENTICATION_LOG_OUT;
        public const string GET_USER_INFO = AUTHENTICATION + AUTHENTICATION_USER_INFO;

        public const string POST_MAP_POINTS = MAP + MAP_POINTS;

        public const string GET_DOG_PARK = DOG_PARKS + DOG_PARKS_GET;

        public const string POST_LIST_REVIEWS = REVIEWS + REVIEWS_LIST;
        public const string POST_CREATE_REVIEW = REVIEWS + REVIEWS_CREATE;

        public const string POST_CREATE_DOG = DOGS + DOGS_CREATE;
        public const string Update_DOG = DOGS + DOGS_UPDATE;
        public const string DELETE_DOG = DOGS + DOGS_DELETE;
        public const string GET_LIST_DOGS = DOGS + DOGS_LIST;
        public const string GET_DETAILED_DOG = DOGS + DOGS_DETAILED;
        public const string GET_DOG_BREEDS = DOGS + DOG_BREEDS;

        public const string POST_CHECK_IN = CHECKINS + CHECKINS_CHECK_IN;
        public const string PUT_CHECK_OUT = CHECKINS + CHECK_INS_CHECK_OUT;
        public const string GET_CHECKINS_LIST = CHECKINS + CHECKINS_LIST;
        public const string GET_CHECKINS_CURRENT_CHECKIN = CHECKINS + CHECKINS_CURRENT_CHECK_IN;


    }
}
