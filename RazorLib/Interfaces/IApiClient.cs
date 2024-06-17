using ModelLib.ApiDTOs;
using ModelLib.ApiDTOs.Pagination;
using ModelLib.DTOs;
using ModelLib.DTOs.Authentication;
using ModelLib.DTOs.CheckIns;
using ModelLib.DTOs.DogPark;
using ModelLib.DTOs.Instructors;
using ModelLib.DTOs.Places;
using ModelLib.DTOs.Posts;
using ModelLib.DTOs.Reviews;
using ModelLib.DTOs.Users;
using ModelLib.Repositories;
using System.Net;

namespace RazorLib.Interfaces
{
    public interface IApiClient
    {
        public Task<DogParkDetailedDTO> GetDogParkAsync(int id);

        #region Login
        public Task<SignInWithThirdPartyResult> FacebookLogin(string? accessToken = null);
        public Task<UserDetailedDTO> GetUserInfo();
        public Task<LoginResult?> Login(string email, string password);
        public Task<RegisterResultDTO> RegisterAsync(RegisterDTO registerDTO);
        public Task LogOut();
        public Task<(bool, UserDetailedDTO?)> IsAuthorized();
        public Task<bool> ConfirmEmailAsync(string email, string token);
        public Task<bool> UpdateUserAsync(UserUpdateDTO updateDTO);

        public Task<HttpResponseMessage> ForgotPassword(string email);

        public Task<HttpResponseMessage> ResetPassword(ResetPasswordDTO resetPasswordDTO);

        public Task<ChangePasswordResultDTO> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO);
        public Task<RequestChangeEmailResultDTO> RequestChangeEmailAsync(RequestChangeEmailDTO requestChangeEmailDTO);
        public Task<HttpResponseMessage> ChangeEmailAsync(ChangeEmailDTO dto); 
        #endregion

        public Task<HttpResponseMessage> SendFeedback(FeedbackCreateDTO dto);

        #region DogParks
        public Task<bool> CreateDogParkRequestAsync(DogParkRequestCreateDTO dto);
        public Task<DateTimePaginationResult<DogParkRequestDetailedDTO>> GetDogParkRequestsAsync(DateTimePaginationRequest dto);
        public Task<DateTimePaginationResult<DogParkListDTO>> GetApprovedDogParkRequestsAsync(DateTimePaginationRequest dto);
        public Task<IEnumerable<DogParkListDTO>> GetDogParkListByArea(SearchAreaDTO searchArea);
        public Task<DistancePaginationResult<DogParkListDTO>> GetDogParkList(DogParksDTOPaginationRequest request);
        #endregion

        #region Reviews

        public Task<ReviewDetailedDTO> GetReviewAsync(int userId, int placeId);
        public Task<PaginationResult<ReviewDetailedDTO>> GetReviewsAsync(ReviewsDTOPaginationRequest request);
        public Task<ReviewCreatedDTO?> CreateReviewAsync(ReviewCreateDTO reviewCreateDTO);
        #endregion

        #region Checkins
        public Task<int?> CheckIn(CheckInCreateDTO dto);
        public Task<int?> CheckOut();
        public Task<PaginationResult<CheckInListDTO>> GetCheckIns(CheckInListDTOPaginationRequest dto);
        public Task<CurrentlyCheckedInDTO?> GetCurrentCheckIn();
        public Task<CheckInStatisticsDetailedDTO> GetCheckInStatisticsAsync(int placeId);
        #endregion

        #region Friendships
        public Task AddFriend(int id);
        public Task RemoveFriend(int id);
        public Task AcceptFriendRequest(int id);
        public Task DeclineFriendRequest(int id);
        public Task<RepositoryEnums.FriendShipStatus> GetFriendStatus(int id);
        public Task<PaginationResult<UserListDTO>> GetFriends(StringPaginationRequest pagination);
        public Task<PaginationResult<UserListDTO>> GetFriendRequests(StringPaginationRequest pagination);
        #endregion

        #region Users
        public Task<PaginationResult<UserListDTO>> SearchUsers(UserSearchDTOPaginationRequest request);

        public Task<UserDetailedDTO> GetUserAsync(int id);
        #endregion

        #region DogInstructors
        public Task<int> CreateDogInstructor(InstructorCreateDTO dto);
        public Task<HttpResponseMessage> UpdateDogInstructor(InstructorUpdateDTO dto);
        public Task<InstructorDetailedDTO> GetDogInstructor(int dogInstructorId);
        public Task<DistancePaginationResult<DogTrainerListDTO>> GetDogTrainersAsync(DogTrainerRequest request);
        public Task<IEnumerable<DogTrainerListDTO>> GetDogInstructorListByArea(SearchAreaDTO searchArea);

        public Task<HttpResponseMessage> UnregisterDogTrainer();
        #endregion

        #region DogTraining
        Task<(HttpStatusCode, int)> CreateDogTrainingAsync(DogTrainingCreateDTO createDTO);
        Task<HttpStatusCode> UpdateDogTrainingAsync(DogTrainingUpdateDTO updateDto);
        Task<HttpStatusCode> DeleteDogTrainingAsync(int dogTrainingId);
        Task<DogTrainingDetailsDTO?> GetDogTrainingAsync(int id);
        Task<DistancePaginationResult<DogTrainingListDTO>> GetDogTrainingsAsync(DogTrainingRequest request);
        Task<IEnumerable<DogTrainingListDTO>> GetUpcommingDogTrainingsAsync(int instructorCompanyId);
        Task<AllDogTrainingsInstructorListDTO> GetAllDogTrainingsAsync(int instructorCompanyId);

        Task<Dictionary<int, string>> GetTrainerNames();
        #endregion

        #region Places
        public Task<PlaceListDTO?> GetPlaceAsync(int placeId);
        #endregion

        #region Posts
        public Task<DateTimePaginationResult<PostDetailedDTO>> GetPosts(PostDateTimePaginationRequest request);
        public Task<PostCreateResult> CreatePost(PostCreateDTO postCreateDTO);
        public Task<HttpResponseMessage> DeletePost(int postId);
        public Task<PostDetailedDTO?> GetPostAsync(int postId);
        public Task<HttpResponseMessage> LikePostAsync(int postId);

        public Task<int?> CreateCommentAsync(CommentCreateDTO commentCreateDTO);
        public Task<HttpResponseMessage> DeleteCommentAsync(int commentId);
        public Task<DateTimePaginationResult<CommentDetailedDTO>> GetCommentsAsync(CommentGetRequest request);
        public Task<CommentDetailedDTO?> GetCommentAsync(int commentId);
        public Task<HttpResponseMessage> LikeCommentAsync(int commentId);
        #endregion
    }
}
