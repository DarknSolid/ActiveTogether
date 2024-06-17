using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;
using Azure.Storage.Files.DataLake.Specialized;
using Azure.Storage.Sas;
using ModelLib.ApiDTOs;
using System.Web;
using static ModelLib.Repositories.RepositoryEnums;
using static System.Net.Mime.MediaTypeNames;

namespace ModelLib.Repositories
{
    public interface IBlobStorageRepository
    {
        Task<(ResponseType, Uri?)> GetPublicImageUrl(string? path);
        Task<(ResponseType, string?)> UploadPublicImageAsync(FileDetailedDTO file, string path, int entityId);
        Task<ResponseType> DeletePublicImageAsync(string path, int entityId);
        Task<ResponseType> DeletePublicImageAsync(string pathWithExtension);
        Task<IList<string>> UploadPublicPostMedia(FileDetailedDTO[] files, int postId);
        Task<IList<(string, string)>> CreatePostFileBlobUploadUrls(FileListDTO[] files, int postId);
        Task DeletePublicPostAsync(int postId);

        public string GetAccountName();
    }

    public class BlobStorageRepository : IBlobStorageRepository
    {
        public static readonly string CompanyCoverImagePath = "company-cover-images/";
        public static readonly string PlaceProfileImagePath = "place-profile-images/";
        public static readonly string TrainingCoverImagePath = "training-cover-images/";
        public static readonly string UserProfilePicture = "user-profile-images/";
        public static readonly string DogProfilePicture = "dog-profile-images/";

        public static readonly string PublicContainerName = "public";

        private static readonly string PostMediaPath = "post-media/";


        private readonly DataLakeServiceClient _dataLakeServiceClient;
        private readonly DataLakeFileSystemClient _publicFileSystemClient;
        private readonly DelegationKeyHelper _delegationKeyHelper;
        private readonly StorageSharedKeyCredential _credentials;

        public BlobStorageRepository(DataLakeServiceClient dataLakeServiceClient, StorageSharedKeyCredential credentials)
        {
            _dataLakeServiceClient = dataLakeServiceClient;
            _publicFileSystemClient = _dataLakeServiceClient.GetFileSystemClient(PublicContainerName);
            _delegationKeyHelper = new(_dataLakeServiceClient);
            _credentials = credentials;
            //_delegationKeyHelper.RequestUserDelegationKeyAsync();
        }

        public string GetAccountName()
        {
            return _dataLakeServiceClient.AccountName;
        }

        public async Task<IList<string>> UploadPublicPostMedia(FileDetailedDTO[] files, int postId)
        {
            var filepath = PostMediaPath + postId + "/";
            var filePaths = files.Select(f => filepath + GetHashSHA1(f.Bytes)).ToList();

            var result = new List<string>();
            for (int i = 0; i < filePaths.Count; i++)
            {
                var (response, blobPath) = await UploadPublicImageAsync(filePaths[i], files[i]);
                if (blobPath is not null)
                {
                    result.Add(blobPath);
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        /// <param name="postId"></param>
        /// <returns>List of tuple of uris without sas tokens and sas tokens</returns>
        public async Task<IList<(string, string)>> CreatePostFileBlobUploadUrls(FileListDTO[] files, int postId)
        {
            var directory = PostMediaPath + postId + "/";
            await _publicFileSystemClient.GetDirectoryClient(directory).CreateIfNotExistsAsync();

            var uris = new List<(string, string)>();
            foreach (var f in files)
            {
                var filePath = GetMediaFilePath(directory + Guid.NewGuid(), f);
                var fileClient = _publicFileSystemClient.GetFileClient(filePath);

                var sasbuilder = new DataLakeSasBuilder
                {
                    FileSystemName = fileClient.GetParentFileSystemClient().Name,
                    Path = fileClient.Path,
                    StartsOn = DateTimeOffset.UtcNow.AddMinutes(-30),
                    ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(60),
                    Resource = "b",
                    Protocol = SasProtocol.Https,
                };

                sasbuilder.SetPermissions(DataLakeSasPermissions.All); //("racwld");

                // Add the SAS token to the blob URI
                var candenerate = _dataLakeServiceClient.CanGenerateAccountSasUri;
                var can = fileClient.CanGenerateSasUri;
                var uri = fileClient.GenerateSasUri(sasbuilder);
                //var dataLakeUriBuilder = new DataLakeUriBuilder(fileClient.Uri)
                //{
                //    Sas = sasbuilder.ToSasQueryParameters(
                //        await _delegationKeyHelper.RequestUserDelegationKeyAsync(),
                //        fileClient.GetParentFileSystemClient()
                //            .AccountName
                //    )
                //};
                //var uri = dataLakeUriBuilder.ToUri();
                uris.Add((filePath, uri.ToString()));
            }
            return uris;

        }

        public async Task DeletePublicPostAsync(int postId)
        {
            //https://learn.microsoft.com/en-us/dotnet/api/overview/azure/storage.files.datalake-readme?view=azure-dotnet
            await _dataLakeServiceClient.GetFileSystemClient("public").GetDirectoryClient(PostMediaPath + postId + "/").DeleteIfExistsAsync();
        }

        public async Task<(ResponseType, string?)> UploadPublicImageAsync(FileDetailedDTO file, string path, int entityId)
        {
            return await UploadPublicImageAsync(path + entityId, file);
        }

        public async Task<ResponseType> DeletePublicImageAsync(string pathWithExtension)
        {
            var blobClient = _publicFileSystemClient.GetFileClient(pathWithExtension);
            await blobClient.DeleteIfExistsAsync();
            return ResponseType.Deleted;
        }

        public async Task<ResponseType> DeletePublicImageAsync(string path, int entityId)
        {
            var blobClient = _publicFileSystemClient.GetFileClient(path + entityId);
            await blobClient.DeleteIfExistsAsync();
            return ResponseType.Deleted;
        }

        private async Task<(ResponseType, string?)> UploadPublicImageAsync(string filePath, FileDetailedDTO file)
        {
            var responseType = ResponseType.Created;
            filePath = GetMediaFilePath(filePath, file);

            var blobClient = _publicFileSystemClient.GetFileClient(filePath);
            if (await blobClient.DeleteIfExistsAsync())
            {
                responseType = ResponseType.Updated;
            }

            using (var ms = new MemoryStream(file.Bytes))
            {
                var httpHeaders = new PathHttpHeaders
                {
                    ContentType = file.ContentType,
                    CacheControl = "no-cache" // the client will use the etag 
                };
                await blobClient.UploadAsync(content: ms, httpHeaders: httpHeaders);
            }

            return (responseType, filePath);
        }

        private static string GetMediaFilePath(string filePath, FileListDTO file)
        {
            var fileExtension = "." + file.ContentType.Split("/").Last().ToLower();
            if (MediaUtils.GetVideoExtensionsSet().Contains(fileExtension))
            {
                filePath = filePath + "-video";
            }
            else if (MediaUtils.GetImageExtensionsSet().Contains(fileExtension))
            {
                filePath = filePath + "-image";
            }

            return filePath;
        }

        public async Task<(ResponseType, Uri?)> GetPublicImageUrl(string? path)
        {
            if (path is null)
            {
                return (ResponseType.NotFound, null);
            }
            else if (Uri.IsWellFormedUriString(path, UriKind.Absolute))
            {
                return (ResponseType.Ok, new Uri(path));
            }
            var blobClient = _publicFileSystemClient.GetFileClient(path);
            try
            {
                //var uri = await CreateUserDelegationSASBlob(blobClient);
                return (ResponseType.Ok, blobClient.Uri);
            }
            catch (Exception)
            {
                return (ResponseType.NotFound, null);
            }
        }


        public static string GetHashSHA1(byte[] data)
        {
            using (var sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider())
            {
                return string.Concat(sha1.ComputeHash(data).Select(x => x.ToString("X2")));
            }
        }

        private class DelegationKeyHelper
        {
            private readonly DataLakeServiceClient _datalakeClient;
            private UserDelegationKey? _userDelegationKey;
            static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

            public DelegationKeyHelper(DataLakeServiceClient datalakeClient)
            {
                _datalakeClient = datalakeClient;
            }

            public async Task<UserDelegationKey> RequestUserDelegationKeyAsync()
            {
                await semaphoreSlim.WaitAsync();
                try
                {
                    // Critical section:
                    if (_userDelegationKey is null || DateTime.UtcNow >= _userDelegationKey.SignedExpiresOn.UtcDateTime.AddMinutes(-15))
                    {
                        _userDelegationKey =
                            await _datalakeClient.GetUserDelegationKeyAsync(
                                DateTimeOffset.UtcNow.AddMinutes(-30), // to avoid clocks not being synchronized on machines: https://learn.microsoft.com/en-us/azure/storage/common/storage-sas-overview
                                DateTimeOffset.UtcNow.AddDays(1));
                    }
                }
                finally
                {
                    semaphoreSlim.Release();
                }

                return _userDelegationKey;
            }
        }
    }
}
