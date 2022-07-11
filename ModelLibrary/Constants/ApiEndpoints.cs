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
        public const string DOGS_CREATE = "create/";
        public const string DOGS_UPDATE = "update/";
        public const string DOGS_LIST = "list/";
        public const string DOGS_DETAILED = "";

        public const string DOG_BREEDS = "breeds/";


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
        public const string POST_UPDATE_DOG = DOGS + DOGS_UPDATE;
        public const string GET_LIST_DOGS = DOGS + DOGS_LIST;
        public const string GET_DETAILED_DOG = DOGS + DOGS_DETAILED;
        public const string GET_DOG_BREEDS = DOGS + DOG_BREEDS;
    }
}
