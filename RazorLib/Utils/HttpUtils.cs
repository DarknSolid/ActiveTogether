using System.Net;

namespace RazorLib.Utils
{
    public static class HttpUtils
    {
        public static bool IsSuccessStatus(HttpStatusCode statusCode)
        {
            return (int)statusCode >= 200 && (int)statusCode < 300;
        }
    }
}
