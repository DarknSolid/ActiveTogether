using Microsoft.AspNetCore.Components.Forms;

namespace RazorLib.Utils
{
    public static class BrowserFileExtensions
    {
        public static async Task<byte[]> ToByteArray(this IBrowserFile browser, long maxAllowedSize)
        {
            using (var browserStream = browser.OpenReadStream(maxAllowedSize: maxAllowedSize))
            {
                using (var ms = new MemoryStream())
                {
                    await browserStream.CopyToAsync(ms);
                    return ms.ToArray();
                }
            }
        }

        public static async Task<string> ToDataURL(this IBrowserFile browser, long maxAllowedFileSizeBytes)
        {
            return $"data:{browser.ContentType};base64,{await browser.OpenReadStream(maxAllowedSize:maxAllowedFileSizeBytes).ConvertToBase64Async()}";
        }

        public static async Task<string> ConvertToBase64Async(this Stream stream)
        {
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                bytes = memoryStream.ToArray();
            }

            return Convert.ToBase64String(bytes);
        }
    }
}
