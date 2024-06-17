
namespace ModelLib
{
    public static class MediaUtils
    {
        private static HashSet<string> _videoExtensionsSet = new();
        private static HashSet<string> _imageExtensionsSet = new();
        private static HashSet<string> _allMediaExtensionSet = new();

        //https://blog.filestack.com/thoughts-and-knowledge/complete-image-file-extension-list/
        public static readonly string AcceptedImages = ".jpg,.jpeg,.jpe.jif,.jfif,.jfi,.png,.gif,.webp,.raw,.arw,.cr2,.nrw,.k25,.heif,.heic";

        //https://blog.filestack.com/thoughts-and-knowledge/complete-list-audio-video-file-formats/
        public static readonly string _acceptedVideos = ".mp4,.m4p,.m4v,.mp3,.webm,.mpg,.mp2,.mpeg,.mpe,.mpv,.avi,.wmv";

        public static readonly long TWENTY_MEGABYTES_IN_BYTES = 20000000;

        public static readonly long FORTY_MEGABYTES_IN_BYTES = 20000000 * 2;

        public static HashSet<string> GetVideoExtensionsSet()
        {
            lock (_videoExtensionsSet)
            {
                if (!_videoExtensionsSet.Any())
                {
                    _videoExtensionsSet = _acceptedVideos.Split(",").ToHashSet();
                }
            }
            return _videoExtensionsSet;
        }

        public static HashSet<string> GetImageExtensionsSet()
        {
            lock (_imageExtensionsSet)
            {
                if (!_imageExtensionsSet.Any())
                {
                    _imageExtensionsSet = AcceptedImages.Split(",").ToHashSet();
                }
            }
            return _imageExtensionsSet;
        }

        public static HashSet<string> GetAllMediaExtensionsSet()
        {
            lock(_allMediaExtensionSet)
            {
                if (!_allMediaExtensionSet.Any())
                {
                    var videos = GetVideoExtensionsSet().ToList();
                    var images = GetImageExtensionsSet().ToList();
                    videos.AddRange(images);
                    _allMediaExtensionSet = videos.ToHashSet();
                }
            }
            return _allMediaExtensionSet;
        }

        public static bool IsVideo(string url)
        {
            return url.Split("/").Last().EndsWith("-video");
        }
    }
}
