using ModelLib.Constants;
using System.Net.Http.Json;
using RazorLib.Interfaces;
using ModelLib.DTOs;
using ModelLib.ApiDTOs;
using ModelLib.DTOs.DogPark;
using Newtonsoft.Json;
using ModelLib.DTOs.Reviews;
using ModelLib.DTOs.Dogs;
using ModelLib.DTOs.CheckIns;
using ModelLib.Repositories;
using ModelLib.DTOs.Authentication;
using ModelLib.ApiDTOs.Pagination;
using ModelLib.DTOs.Instructors;
using System.Net;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ModelLib.DTOs.Places;
using ModelLib.DTOs.Users;
using ModelLib.DTOs.Posts;
using EntityLib.Entities.PostsAndComments;

namespace RazorLib.Models
{
    public class ApiClient : IApiClient
    {

        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;
        private readonly IDialogService _dialogService;

        public ApiClient(HttpClient httpClient, NavigationManager navigationManager, IDialogService dialogService)
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
            _dialogService = dialogService;
        }

        #region BlazorLib Interface implementation:

        public async Task<(bool, UserDetailedDTO?)> IsAuthorized()
        {
            var response = await _httpClient.GetAsync(ApiEndpoints.GET_USER_INFO);
            ModelLib.DTOs.Authentication.UserDetailedDTO userInfo = null;
            if (response.IsSuccessStatusCode)
            {
                userInfo = JsonConvert.DeserializeObject<ModelLib.DTOs.Authentication.UserDetailedDTO>(await response.Content.ReadAsStringAsync());
            }

            return (response.IsSuccessStatusCode, userInfo);
        }

        public async Task<DogParkDetailedDTO> GetDogParkAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<DogParkDetailedDTO>(ApiEndpoints.GET_DOG_PARK + id);
        }

        public async Task<IEnumerable<DogParkListDTO>> GetDogParkListByArea(SearchAreaDTO searchArea)
        {
            var url = ApiEndpoints.POST_DOG_PARK_GET_AREA;
            var (response, parks) = await PostAsync<SearchAreaDTO, IEnumerable<DogParkListDTO>>(url, searchArea);
            return parks;
        }

        public async Task<DistancePaginationResult<DogParkListDTO>> GetDogParkList(DogParksDTOPaginationRequest request)
        {
            var url = ApiEndpoints.POST_DOG_PARK_GET_LIST;
            var (response, parks) = await PostAsync<DogParksDTOPaginationRequest, DistancePaginationResult<DogParkListDTO>>(url, request);
            return parks;
        }

        public async Task<ReviewDetailedDTO?> GetReviewAsync(int userId, int placeId)
        {
            var (_, dto) = await PostAsync<ReviewGetDTO, ReviewDetailedDTO?>(ApiEndpoints.GET_REVIEW, new ReviewGetDTO
            {
                UserId = userId,
                PlaceId = placeId
            });

            return dto;
        }

        public async Task<PaginationResult<ReviewDetailedDTO>> GetReviewsAsync(ReviewsDTOPaginationRequest request)
        {
            var url = ApiEndpoints.POST_LIST_REVIEWS;

            var (response, obj) = await PostAsync<ReviewsDTOPaginationRequest, PaginationResult<ReviewDetailedDTO>>(url, request);
            return obj;
        }

        public async Task<ReviewCreatedDTO?> CreateReviewAsync(ReviewCreateDTO reviewCreateDTO)
        {
            var url = ApiEndpoints.POST_CREATE_REVIEW;
            var (response, obj) = await PostAsync<ReviewCreateDTO, ReviewCreatedDTO?>(url, reviewCreateDTO);
            return obj;
        }

        public async Task<List<DogListDTO>> GetMyDogsAsync()
        {
            var user = await GetUserInfo();
            return await GetDogsAsync(user.Id);
        }

        public async Task<DogDetailedDTO> GetDogDetailedAsync(int id)
        {
            var url = ApiEndpoints.GET_DETAILED_DOG + id;
            return await _httpClient.GetFromJsonAsync<DogDetailedDTO>(url);
        }

        public async Task<int?> CreateDogAsync(DogCreateDTO dto)
        {
            var url = ApiEndpoints.POST_CREATE_DOG;
            var (response, obj) = await PostAsync<DogCreateDTO, int>(url, dto);
            return obj;
        }

        public async Task<IDictionary<int, string>> GetDogBreedsAsync()
        {
            var url = ApiEndpoints.GET_DOG_BREEDS;
            return await _httpClient.GetFromJsonAsync<IDictionary<int, string>>(url);
        }

        public async Task<bool> UpdateDogAsync(DogUpdateDTO dto)
        {
            var url = ApiEndpoints.Update_DOG;
            var result = await PutAsync<DogUpdateDTO, object>(url, dto);
            return result.Item1.IsSuccessStatusCode;
        }

        public async Task<List<DogListDTO>> GetDogsAsync(int userId)
        {
            var url = ApiEndpoints.GET_LIST_DOGS + userId;
            return await GetAsJsonAsync<List<DogListDTO>>(url) ?? new List<DogListDTO>();
        }

        public async Task<bool> DeleteDogAsync(int dogId)
        {
            var url = ApiEndpoints.DELETE_DOG + dogId;
            var response = await DeleteAsync(url);
            return response.IsSuccessStatusCode;
        }

        public async Task<int?> CheckIn(CheckInCreateDTO dto)
        {
            var url = ApiEndpoints.POST_CHECK_IN;
            var (response, id) = await PostAsync<CheckInCreateDTO, int>(url, dto);
            return id;
        }

        public async Task<int?> CheckOut()
        {
            var url = ApiEndpoints.PUT_CHECK_OUT;
            var (result, id) = await PutAsync<int>(url);
            return id;
        }

        public async Task<PaginationResult<CheckInListDTO>> GetCheckIns(CheckInListDTOPaginationRequest dto)
        {
            var url = ApiEndpoints.POST_CHECKINS_LIST;
            var (response, obj) = await PostAsync<CheckInListDTOPaginationRequest, PaginationResult<CheckInListDTO>>(url, dto);
            return obj;
        }

        public async Task<CurrentlyCheckedInDTO?> GetCurrentCheckIn()
        {
            var url = ApiEndpoints.GET_CHECKINS_CURRENT_CHECKIN;
            return await GetAsJsonAsync<CurrentlyCheckedInDTO>(url);
        }

        public async Task<PaginationResult<UserListDTO>> SearchUsers(UserSearchDTOPaginationRequest request)
        {
            var url = ApiEndpoints.POST_SEARCH_USERS;
            var (response, obj) = await PostAsync<UserSearchDTOPaginationRequest, PaginationResult<UserListDTO>>(url, request);
            return obj;
        }

        public async Task<UserDetailedDTO> GetUserAsync(int id)
        {
            var url = ApiEndpoints.GET_USER + id;
            return await GetAsJsonAsync<UserDetailedDTO>(url);
        }

        public async Task AddFriend(int id)
        {
            var url = ApiEndpoints.POST_FRIENDSHIPS_ADD + id;
            await _httpClient.PostAsync(url, null);
        }

        public async Task RemoveFriend(int id)
        {
            var url = ApiEndpoints.DELETE_FRIENDSHIPS_REMOVE + id;
            await _httpClient.DeleteAsync(url);
        }

        public async Task AcceptFriendRequest(int id)
        {
            var url = ApiEndpoints.UPDATE_FRIENDSHIPS_ACCEPT + id;
            await _httpClient.PutAsync(url, null);
        }

        public async Task DeclineFriendRequest(int id)
        {
            var url = ApiEndpoints.UPDATE_FRIENDSHIPS_DECLINE + id;
            await _httpClient.PutAsync(url, null);
        }

        public async Task<RepositoryEnums.FriendShipStatus> GetFriendStatus(int id)
        {
            var url = ApiEndpoints.GET_FRIENDSHIPS_STATUS + id;
            return await GetAsJsonAsync<RepositoryEnums.FriendShipStatus>(url);
        }

        public async Task<PaginationResult<UserListDTO>> GetFriends(StringPaginationRequest pagination)
        {
            var url = ApiEndpoints.POST_FRIENDSHIPS;
            var (response, obj) = await PostAsync<StringPaginationRequest, PaginationResult<UserListDTO>>(url, pagination);
            return obj;
        }

        public async Task<PaginationResult<UserListDTO>> GetFriendRequests(StringPaginationRequest pagination)
        {
            var url = ApiEndpoints.POST_FRIENDSHIPS_REQUESTS;
            var (response, obj) = await PostAsync<StringPaginationRequest, PaginationResult<UserListDTO>>(url, pagination);
            return obj;
        }

        public async Task<CheckInStatisticsDetailedDTO> GetCheckInStatisticsAsync(int placeId)
        {
            var url = ApiEndpoints.GET_CHECKINS_STATISTICS + placeId;
            return await GetAsJsonAsync<CheckInStatisticsDetailedDTO>(url);
        }

        public async Task<bool> CreateDogParkRequestAsync(DogParkRequestCreateDTO dto)
        {
            var url = ApiEndpoints.POST_CREATE_DOG_PARK_REQUEST;
            var (response, obj) = await PostAsync<DogParkRequestCreateDTO, object>(url, dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<DateTimePaginationResult<DogParkRequestDetailedDTO>> GetDogParkRequestsAsync(DateTimePaginationRequest dto)
        {
            var url = ApiEndpoints.POST_GET_MY_DOG_PARK_REQUESTS;
            var (response, obj) = await PostAsync<DateTimePaginationRequest, DateTimePaginationResult<DogParkRequestDetailedDTO>>(url, dto);
            return obj;
        }

        public async Task<DateTimePaginationResult<DogParkListDTO>> GetApprovedDogParkRequestsAsync(DateTimePaginationRequest dto)
        {
            var url = ApiEndpoints.POST_GET_MY_APPROVED_DOG_PARKS;
            var (response, obj) = await PostAsync<DateTimePaginationRequest, DateTimePaginationResult<DogParkListDTO>>(url, dto);
            return obj;
        }

        public async Task<int> CreateDogInstructor(InstructorCreateDTO dto)
        {
            var url = ApiEndpoints.POST_CREATE_DOG_INSTRUCTOR;
            var (response, id) = await PostAsync<InstructorCreateDTO, int>(url, dto);
            return id;
        }

        public async Task<HttpResponseMessage> UpdateDogInstructor(InstructorUpdateDTO dto)
        {
            var url = ApiEndpoints.PUT_UPDATE_DOG_INSTRUCTOR;
            var (response, obj) = await PutAsync<InstructorUpdateDTO, object>(url, dto);
            return response;
        }

        public async Task<InstructorDetailedDTO> GetDogInstructor(int dogInstructorId)
        {
            var url = ApiEndpoints.GET_GET_DOG_INSTRUCTOR + dogInstructorId;
            var instructor = await GetAsJsonAsync<InstructorDetailedDTO>(url);
            return instructor;
        }

        public async Task<DistancePaginationResult<DogTrainerListDTO>> GetDogTrainersAsync(DogTrainerRequest request)
        {
            var url = ApiEndpoints.POST_DOG_INSTRUCTOR_GET_LIST;
            var (resopnse, list) = await PostAsync<DogTrainerRequest, DistancePaginationResult<DogTrainerListDTO>>(url, request);
            return list;
        }

        public async Task<IEnumerable<DogTrainerListDTO>> GetDogInstructorListByArea(SearchAreaDTO searchArea)
        {
            var url = ApiEndpoints.POST_DOG_INSTRUCTOR_GET_AREA;
            var (response, list) = await PostAsync<SearchAreaDTO, IEnumerable<DogTrainerListDTO>>(url, searchArea);
            return list;
        }

        #endregion

        #region Posts
        public async Task<DateTimePaginationResult<PostDetailedDTO>> GetPosts(PostDateTimePaginationRequest request)
        {
            var (_, result) = await PostAsync<PostDateTimePaginationRequest, DateTimePaginationResult<PostDetailedDTO>>(ApiEndpoints.POST_GET_POSTS, request);
            return result;
        }

        public async Task<PostCreateResult> CreatePost(PostCreateDTO postCreateDTO)
        {
            var (_, result) = await PostAsync<PostCreateDTO, PostCreateResult>(ApiEndpoints.POST_POSTS_CREATE_POST, postCreateDTO);
            return result;

        }

        public async Task<HttpResponseMessage> DeletePost(int postId)
        {
            return await DeleteAsync(ApiEndpoints.DELETE_POSTS_POST + postId);
        }

        public async Task<PostDetailedDTO?> GetPostAsync(int postId)
        {
            return await GetAsJsonAsync<PostDetailedDTO?>(ApiEndpoints.GET_POSTS_POST + postId);
        }

        public async Task<HttpResponseMessage> LikePostAsync(int postId)
        {
            return await GetAsync(ApiEndpoints.GET_POSTS_LIKE_POST + postId);
        }

        public async Task<int?> CreateCommentAsync(CommentCreateDTO commentCreateDTO)
        {
            var (response, id) = await PostAsync<CommentCreateDTO, int?>(ApiEndpoints.POST_POSTS_CREATE_COMMENT, commentCreateDTO);
            return id;
        }
        public Task<HttpResponseMessage> DeleteCommentAsync(int commentId)
        {
            return DeleteAsync(ApiEndpoints.DELETE_POSTS_COMMENT + commentId);
        }
        public async Task<DateTimePaginationResult<CommentDetailedDTO>> GetCommentsAsync(CommentGetRequest request)
        {
            var (response, result) = await PostAsync<CommentGetRequest, DateTimePaginationResult<CommentDetailedDTO>>(ApiEndpoints.POST_POSTS_GET_COMMENTS, request);
            return result;
        }

        public async Task<CommentDetailedDTO?> GetCommentAsync(int commentId)
        {
            return await GetAsJsonAsync<CommentDetailedDTO?>(ApiEndpoints.GET_POSTS_COMMENT + commentId);
        }
        public async Task<HttpResponseMessage> LikeCommentAsync(int commentId)
        {
            return await GetAsync(ApiEndpoints.GET_POSTS_LIKE_COMMENT + commentId);
        }
        #endregion

        #region Dog Training
        public async Task<(HttpStatusCode, int)> CreateDogTrainingAsync(DogTrainingCreateDTO createDTO)
        {
            var url = ApiEndpoints.POST_CREATE_DOG_TRAINING;
            var (result, id) = await PostAsync<DogTrainingCreateDTO, int>(url, createDTO);
            return (result.StatusCode, id);
        }

        public async Task<HttpStatusCode> UpdateDogTrainingAsync(DogTrainingUpdateDTO updateDto)
        {
            var url = ApiEndpoints.PUT_UPDATE_DOG_TRAINING;
            var result = await _httpClient.PutAsJsonAsync(url, updateDto);
            return result.StatusCode;
        }

        public async Task<HttpStatusCode> DeleteDogTrainingAsync(int dogTrainingId)
        {
            var url = ApiEndpoints.DELETE_DOG_TRAINING + dogTrainingId;
            var response = await _httpClient.DeleteAsync(url);
            return response.StatusCode;
        }

        public async Task<DogTrainingDetailsDTO?> GetDogTrainingAsync(int id)
        {
            var url = ApiEndpoints.GET_DOG_TRAINING + id;
            return await GetAsJsonAsync<DogTrainingDetailsDTO>(url);
        }

        public async Task<DistancePaginationResult<DogTrainingListDTO>> GetDogTrainingsAsync(DogTrainingRequest request)
        {
            var url = ApiEndpoints.POST_DOG_TRAINING_GET_LIST;
            var (response, dto) = await PostAsync<DogTrainingRequest, DistancePaginationResult<DogTrainingListDTO>>(url, request);
            return dto;
        }

        public async Task<IEnumerable<DogTrainingListDTO>> GetUpcommingDogTrainingsAsync(int instructorCompanyId)
        {
            var url = ApiEndpoints.GET_DOG_TRAINING_GET_LIST + instructorCompanyId;
            var dto = await GetAsJsonAsync<IEnumerable<DogTrainingListDTO>>(url);
            if (dto == null)
            {
                return new List<DogTrainingListDTO>();
            }
            return dto;
        }

        public async Task<AllDogTrainingsInstructorListDTO> GetAllDogTrainingsAsync(int instructorCompanyId)
        {
            var url = ApiEndpoints.GET_DOG_TRAINING_GET_LIST_ALL + instructorCompanyId;
            var dto = await GetAsJsonAsync<AllDogTrainingsInstructorListDTO>(url);
            if (dto == null)
            {
                return new AllDogTrainingsInstructorListDTO();
            }
            return dto;
        }
        public async Task<Dictionary<int, string>> GetTrainerNames()
        {
            return await GetAsJsonAsync<Dictionary<int, string>>(ApiEndpoints.GET_TRAINER_NAMES);
        }

        public async Task<HttpResponseMessage> UnregisterDogTrainer()
        {
            var url = ApiEndpoints.DELETE_DOG_TRAINER;
            var result = await _httpClient.DeleteAsync(url);
            return result;
        }
        #endregion

        #region Login

        public async Task<SignInWithThirdPartyResult> FacebookLogin(string? accessToken = null)
        {

#if __ANDROID__
            Xamarin.Facebook.Login.LoginManager.Instance.LogInWithReadPermissions(MainActivity.Instance, new List<string> { "public_profile", "email" });
            while (MainActivity.Instance.FacebookAccessToken == null)
            {
                await Task.Delay(500);
            }
            accessToken = MainActivity.Instance.FacebookAccessToken;
#endif

            var url = ApiEndpoints.POST_FACEBOOK_LOGIN;
            var response = await _httpClient.PostAsJsonAsync(url, new FacebookLoginDTO() { AccessToken = accessToken });
            if (response.IsSuccessStatusCode)
            {
                var dto = JsonConvert.DeserializeObject<SignInWithThirdPartyResult>(await response.Content.ReadAsStringAsync());
                return dto;
            }

            return new SignInWithThirdPartyResult();
        }

        public async Task<UserDetailedDTO?> GetUserInfo()
        {
            return await GetAsJsonAsync<UserDetailedDTO>(ApiEndpoints.GET_USER_INFO);
        }

        public async Task<LoginResult?> Login(string email, string password)
        {
            var login = new LoginDTO() { Email = email, Password = password };
            var (response, userInfo) = await PostAsync<LoginDTO, LoginResult?>(ApiEndpoints.POST_LOGIN, login);
            return userInfo;
        }

        public async Task<RegisterResultDTO> RegisterAsync(RegisterDTO registerDTO)
        {
            var (_, dto) = await PostAsync<RegisterDTO, RegisterResultDTO>(ApiEndpoints.POST_REGISTER, registerDTO);
            return dto;
        }


        public async Task LogOut()
        {
            await _httpClient.GetAsync(ApiEndpoints.GET_LOG_OUT);
        }

        public async Task<PlaceListDTO?> GetPlaceAsync(int placeId)
        {
            return await GetAsJsonAsync<PlaceListDTO?>(ApiEndpoints.GET_PLACE + placeId);
        }

        #endregion

        #region Helpers

        private async Task<T?> GetAsJsonAsync<T>(string url)
        {
            var result = await _httpClient.GetAsync(url);
            await InterceptResponse(result);
            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                return default;
            }
            return JsonConvert.DeserializeObject<T>(await result.Content.ReadAsStringAsync());
        }
        private async Task<HttpResponseMessage> GetAsync(string url)
        {
            var result = await _httpClient.GetAsync(url);
            await InterceptResponse(result);
            return result;
        }
        private async Task<(HttpResponseMessage, O?)> PostAsync<I, O>(string url, I body)
        {
            var result = await _httpClient.PostAsJsonAsync(url, body);
            await InterceptResponse(result);
            var str = await result.Content.ReadAsStringAsync();
            O? jsonObject = default;
            if (result.IsSuccessStatusCode)
            {
                jsonObject = JsonConvert.DeserializeObject<O?>(str);
            }
            return (result, jsonObject);
        }

        private async Task<HttpResponseMessage> PostAsync<I>(string url, I body)
        {
            var result = await _httpClient.PostAsJsonAsync(url, body);
            await InterceptResponse(result);
            var str = await result.Content.ReadAsStringAsync();
            return result;
        }

        private async Task<(HttpResponseMessage, O?)> PutAsync<I, O>(string url, I body)
        {
            var result = await _httpClient.PutAsJsonAsync(url, body);
            await InterceptResponse(result);
            var str = await result.Content.ReadAsStringAsync();
            O? jsonObject = default;
            if (result.IsSuccessStatusCode)
            {
                jsonObject = JsonConvert.DeserializeObject<O?>(str);
            }
            return (result, jsonObject);
        }

        private async Task<(HttpResponseMessage, O?)> PutAsync<O>(string url)
        {
            var result = await _httpClient.PutAsync(url, null);
            await InterceptResponse(result);
            var str = await result.Content.ReadAsStringAsync();
            O? jsonObject = default;
            if (result.IsSuccessStatusCode)
            {
                jsonObject = JsonConvert.DeserializeObject<O?>(str);
            }
            return (result, jsonObject);
        }

        private async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            var result = await _httpClient.DeleteAsync(url);
            await InterceptResponse(result);
            return result;
        }

        private async Task InterceptResponse(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                _navigationManager.NavigateToLogin();
            }
            else if ((int)response.StatusCode >= 500 && (int)response.StatusCode <= 599)
            {
                await DialogServiceExtender.ShowApiInternalError(_dialogService);
            }
        }

        public async Task<bool> ConfirmEmailAsync(string email, string token)
        {
            var (_, success) = await PostAsync<ConfirmEmailDTO, bool>(ApiEndpoints.POST_CONFIRM_EMAIL, new ConfirmEmailDTO
            {
                Email = email,
                Token = token
            });
            return success;
        }

        public async Task<HttpResponseMessage> ForgotPassword(string email)
        {
            return await _httpClient.GetAsync(ApiEndpoints.GET_FORGOT_PASSWORD + email);
        }

        public async Task<HttpResponseMessage> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            return await _httpClient.PostAsJsonAsync(ApiEndpoints.POST_RESET_PASSWORD, resetPasswordDTO);
        }

        public async Task<bool> UpdateUserAsync(UserUpdateDTO updateDTO)
        {
            var (response, success) = await PutAsync<UserUpdateDTO, bool>(ApiEndpoints.UPDATE_USER, updateDTO);
            return success;
        }

        public async Task<ChangePasswordResultDTO> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO)
        {
            var (response, dto) = await PutAsync<ChangePasswordDTO, ChangePasswordResultDTO>(ApiEndpoints.UPDATE_CHANGE_PASSWORD, changePasswordDTO);
            return dto;
        }
        public async Task<RequestChangeEmailResultDTO> RequestChangeEmailAsync(RequestChangeEmailDTO requestChangeEmailDTO)
        {
            var (resopnse, dto) = await PostAsync<RequestChangeEmailDTO, RequestChangeEmailResultDTO>(ApiEndpoints.POST_REQUEST_CHANGE_EMAIL, requestChangeEmailDTO);
            return dto;
        }
        public async Task<HttpResponseMessage> ChangeEmailAsync(ChangeEmailDTO dto)
        {
            var responseMessage = await PostAsync<ChangeEmailDTO>(ApiEndpoints.POST_CHANGE_EMAIL, dto);
            return responseMessage;
        }

        public async Task<HttpResponseMessage> SendFeedback(FeedbackCreateDTO dto)
        {
            return await PostAsync(ApiEndpoints.POST_FEEDBACK, dto);
        }
        #endregion
    }
}
