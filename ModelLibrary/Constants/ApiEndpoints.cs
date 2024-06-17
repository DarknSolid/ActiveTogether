using System.Data;

namespace ModelLib.Constants
{
    public class ApiEndpoints
    {

        private const string BASE = "api/";

        public const string AUTHENTICATION = BASE + "authentication/";
        public const string ROLES = BASE + "roles/";

        public const string FEEDBACK = "feedback/";

        public const string POSTS = "posts/";
        public const string POSTS_CREATE = "create/";
        public const string POSTS_CREATE_COMMENT = "create-comment/";
        public const string POSTS_LIKE = "like/";
        public const string POSTS_COMMENT_LIKE = "like-comment/";
        public const string POSTS_DELETE_COMMENT = "delete-comments/";
        public const string POSTS_GET_COMMENTS = "get-comments";
        public const string POSTS_GET_COMMENT = "get-comment";


        public const string AUTHENTICATION_FACEBOOK_LOGIN = "facebook-login/";
        public const string AUTHENTICATION_LOG_OUT = "log-out/";
        public const string AUTHENTICATION_LOGIN = "login/";
        public const string AUTHENTICATION_REGISTER = "register/";
        public const string AUTHENTICATION_USER_INFO = "user-info/";
        public const string AUTHENTICATION_CONFIRM_EMAIL = "confirm-email/";
        public const string AUTHENTICATION_CHANGE_PASSWORD = "change-password/";
        public const string AUTHENTICATION_REQUEST_CHANGE_EMAIL = "request-change-email/";
        public const string AUTHENTICATION_CHANGE_EMAIL = "change-email/";
        public const string AUTHENTICATION_FORGOT_PASSWORD = "forgot-password/";
        public const string AUTHENTICATION_RESET_PASSWORD = "reset-password";

        public const string PLACES = "places/";
        public const string PLACES_GET = "";

        public const string DOG_PARKS_GET = "";
        public const string DOG_PARKS_CREATE_REQUEST = "request/";
        public const string DOG_PARKS_GET_REQUESTS = "my-requests/";
        public const string DOG_PARKS_GET_APPROVED_DOG_PARKS = "my-approved-parks/";
        public const string DOG_PARKS_APPROVE_DOG_PARK = "approve/";
        public const string DOG_PARKS_LIST = "list/";
        public const string DOG_PARKS_AREA = "area/";

        public const string MAP_POINTS = "points/";

        public const string MAP = BASE + "map/";
        public const string DOG_PARKS = BASE + "dog-parks/";

        public const string REVIEWS = BASE + "reviews/";
        public const string REVIEWS_LIST = "";
        public const string REVIEWS_DETAILS = "details/";
        public const string REVIEWS_CREATE = "create/";

        public const string DOGS = BASE + "dogs/";
        public const string DOGS_CREATE = "";
        public const string DOGS_UPDATE = "";
        public const string DOGS_DELETE = "";
        public const string DOGS_LIST = "list/";
        public const string DOGS_DETAILED = "";

        public const string DOG_BREEDS = "breeds/";

        public const string CHECKINS = BASE + "checkins/";
        public const string CHECKINS_CHECK_IN = "";
        public const string CHECKINS_CHECK_OUT = "check-out/";
        public const string CHECKINS_LIST = "list/";
        public const string CHECKINS_CURRENT_CHECK_IN = "current-checkin/";
        public const string CHECKINS_STATISTICS = "statistics/";

        public const string FRIENDSHIPS = BASE + "friendships/";
        public const string FRIENDSHIPS_ADD = "add/";
        public const string FRIENDSHIPS_REMOVE = "remove/";
        public const string FRIENDSHIPS_ACCEPT = "accept/";
        public const string FRIENDSHIPS_DECLINE = "decline/";
        public const string FRIENDSIPS_STATUS = "status/";
        public const string FRIENDSHIPS_ALL = "";
        public const string FRIENDSHIPS_REQUESTS = "requests/";

        public const string USERS = BASE + "users/";
        public const string USERS_SEARCH = "";
        public const string USERS_GET = "";
        public const string USERS_UPDATE = "";

        public const string DOG_INSTRUCTORS = BASE + "dog-instructors/";
        public const string DOG_INSTRUCTORS_CREATE = "create/";
        public const string DOG_INSTRUCTORS_UPDATE = "update/";
        public const string DOG_INSTRUCTORS_GET = "";
        public const string DOG_INSTRUCTORS_LIST = "list/";
        public const string DOG_INSTRUCTORS_AREA = "area/";
        public const string DOG_INSTRUCTOR_DELETE = "delete/";
        public const string TRAINER_NAMES = "names/";

        public const string DOG_TRAINING = "training/";
        public const string DOG_TRAINING_GET = DOG_TRAINING;
        public const string DOG_TRAINING_UPDATE = DOG_TRAINING;
        public const string DOG_TRAINING_CREATE = DOG_TRAINING + "create/";
        public const string DOG_TRAINING_LIST = DOG_TRAINING + "list/";
        public const string DOG_TRAINING_LIST_ALL = DOG_TRAINING + "list/all/";


        // full api endpoints for the client to use:
        public const string POST_FACEBOOK_LOGIN = AUTHENTICATION + AUTHENTICATION_FACEBOOK_LOGIN;
        public const string POST_LOGIN = AUTHENTICATION + AUTHENTICATION_LOGIN;
        public const string POST_REGISTER = AUTHENTICATION + AUTHENTICATION_REGISTER;
        public const string GET_LOG_OUT = AUTHENTICATION + AUTHENTICATION_LOG_OUT;
        public const string GET_USER_INFO = AUTHENTICATION + AUTHENTICATION_USER_INFO;
        public const string POST_CONFIRM_EMAIL = AUTHENTICATION + AUTHENTICATION_CONFIRM_EMAIL;
        public const string UPDATE_CHANGE_PASSWORD = AUTHENTICATION + AUTHENTICATION_CHANGE_PASSWORD;
        public const string POST_REQUEST_CHANGE_EMAIL = AUTHENTICATION + AUTHENTICATION_REQUEST_CHANGE_EMAIL;
        public const string POST_CHANGE_EMAIL = AUTHENTICATION + AUTHENTICATION_CHANGE_EMAIL;
        public const string GET_FORGOT_PASSWORD = AUTHENTICATION + AUTHENTICATION_FORGOT_PASSWORD;
        public const string POST_RESET_PASSWORD = AUTHENTICATION + AUTHENTICATION_RESET_PASSWORD;

        public const string POST_FEEDBACK = FEEDBACK;

        public const string GET_PLACE = PLACES + PLACES_GET;

        public const string POST_MAP_POINTS = MAP + MAP_POINTS;

        public const string GET_DOG_PARK = DOG_PARKS + DOG_PARKS_GET;
        public const string POST_CREATE_DOG_PARK_REQUEST = DOG_PARKS + DOG_PARKS_CREATE_REQUEST;
        public const string POST_GET_MY_DOG_PARK_REQUESTS = DOG_PARKS + DOG_PARKS_GET_REQUESTS;
        public const string POST_GET_MY_APPROVED_DOG_PARKS = DOG_PARKS + DOG_PARKS_GET_APPROVED_DOG_PARKS;
        public const string POST_APPROVE_DOG_PARK = DOG_PARKS + DOG_PARKS_APPROVE_DOG_PARK;
        public const string POST_DOG_PARK_GET_LIST = DOG_PARKS + DOG_PARKS_LIST;
        public const string POST_DOG_PARK_GET_AREA = DOG_PARKS + DOG_PARKS_AREA;

        public const string GET_REVIEW = REVIEWS + REVIEWS_DETAILS;
        public const string POST_LIST_REVIEWS = REVIEWS + REVIEWS_LIST;
        public const string POST_CREATE_REVIEW = REVIEWS + REVIEWS_CREATE;

        public const string POST_CREATE_DOG = DOGS + DOGS_CREATE;
        public const string Update_DOG = DOGS + DOGS_UPDATE;
        public const string DELETE_DOG = DOGS + DOGS_DELETE;
        public const string GET_LIST_DOGS = DOGS + DOGS_LIST;
        public const string GET_DETAILED_DOG = DOGS + DOGS_DETAILED;
        public const string GET_DOG_BREEDS = DOGS + DOG_BREEDS;

        public const string POST_CHECK_IN = CHECKINS + CHECKINS_CHECK_IN;
        public const string PUT_CHECK_OUT = CHECKINS + CHECKINS_CHECK_OUT;
        public const string POST_CHECKINS_LIST = CHECKINS + CHECKINS_LIST;
        public const string GET_CHECKINS_CURRENT_CHECKIN = CHECKINS + CHECKINS_CURRENT_CHECK_IN;
        public const string GET_CHECKINS_STATISTICS = CHECKINS + CHECKINS_STATISTICS;

        public const string POST_FRIENDSHIPS_ADD = FRIENDSHIPS + FRIENDSHIPS_ADD;
        public const string DELETE_FRIENDSHIPS_REMOVE = FRIENDSHIPS + FRIENDSHIPS_REMOVE;
        public const string UPDATE_FRIENDSHIPS_ACCEPT = FRIENDSHIPS + FRIENDSHIPS_ACCEPT;
        public const string UPDATE_FRIENDSHIPS_DECLINE = FRIENDSHIPS + FRIENDSHIPS_DECLINE;
        public const string GET_FRIENDSHIPS_STATUS = FRIENDSHIPS + FRIENDSIPS_STATUS;
        public const string POST_FRIENDSHIPS = FRIENDSHIPS + FRIENDSHIPS_ALL;
        public const string POST_FRIENDSHIPS_REQUESTS = FRIENDSHIPS + FRIENDSHIPS_REQUESTS;

        public const string POST_SEARCH_USERS = USERS + USERS_SEARCH;
        public const string GET_USER = USERS + USERS_GET;
        public const string UPDATE_USER = USERS + USERS_UPDATE;

        public const string POST_CREATE_DOG_INSTRUCTOR = DOG_INSTRUCTORS + DOG_INSTRUCTORS_CREATE;
        public const string GET_GET_DOG_INSTRUCTOR = DOG_INSTRUCTORS + DOG_INSTRUCTORS_GET;
        public const string PUT_UPDATE_DOG_INSTRUCTOR = DOG_INSTRUCTORS + DOG_INSTRUCTORS_UPDATE;
        public const string POST_DOG_INSTRUCTOR_GET_LIST = DOG_INSTRUCTORS + DOG_INSTRUCTORS_LIST;
        public const string POST_DOG_INSTRUCTOR_GET_AREA = DOG_INSTRUCTORS + DOG_INSTRUCTORS_AREA;
        public const string DELETE_DOG_TRAINER = DOG_INSTRUCTORS + DOG_INSTRUCTOR_DELETE;
        public const string GET_TRAINER_NAMES = DOG_INSTRUCTORS + TRAINER_NAMES;

        public const string POST_CREATE_DOG_TRAINING = DOG_INSTRUCTORS + DOG_TRAINING_CREATE;
        public const string PUT_UPDATE_DOG_TRAINING = DOG_INSTRUCTORS + DOG_TRAINING_UPDATE;
        public const string DELETE_DOG_TRAINING = DOG_INSTRUCTORS + DOG_TRAINING;
        public const string GET_DOG_TRAINING = DOG_INSTRUCTORS + DOG_TRAINING_GET;
        public const string POST_DOG_TRAINING_GET_LIST = DOG_INSTRUCTORS + DOG_TRAINING_LIST;
        public const string GET_DOG_TRAINING_GET_LIST = DOG_INSTRUCTORS + DOG_TRAINING_LIST;
        public const string GET_DOG_TRAINING_GET_LIST_ALL = DOG_INSTRUCTORS + DOG_TRAINING_LIST_ALL;

        public const string POST_GET_POSTS = POSTS;
        public const string POST_POSTS_CREATE_POST = POSTS + POSTS_CREATE;
        public const string DELETE_POSTS_POST = POSTS;
        public const string GET_POSTS_POST = POSTS;
        public const string GET_POSTS_LIKE_POST = POSTS + POSTS_LIKE;
        public const string POST_POSTS_CREATE_COMMENT = POSTS + POSTS_CREATE_COMMENT;
        public const string DELETE_POSTS_COMMENT = POSTS + POSTS_DELETE_COMMENT;
        public const string GET_POSTS_LIKE_COMMENT = POSTS + POSTS_COMMENT_LIKE;
        public const string POST_POSTS_GET_COMMENTS = POSTS + POSTS_GET_COMMENTS;
        public const string GET_POSTS_COMMENT = POSTS + POSTS_GET_COMMENT;

    }
}
