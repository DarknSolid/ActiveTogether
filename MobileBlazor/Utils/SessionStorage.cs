using Microsoft.JSInterop;
using RazorLib.Interfaces;
using System.Text.Json;

namespace MobileBlazor.Utils
{
    public class SessionStorage : ISessionStorage
    {
        private readonly IJSRuntime _jSRuntime;

        public SessionStorage(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
        }

        public async Task SetItem<T>(string key, T obj)
        {
            string jsonString = JsonSerializer.Serialize(obj);
            await _jSRuntime.InvokeVoidAsync("sessionStorage.setItem", key, jsonString);
        }

        public async Task<T> GetItem<T>(string key)
        {
            var json = await _jSRuntime.InvokeAsync<string>("sessionStorage.getItem", key);
            return JsonSerializer.Deserialize<T>(json);
        }

        public async Task<bool> Exists(string key)
        {
            return (await _jSRuntime.InvokeAsync<string>("sessionStorage.getItem", key)) != null;
        }

        public async Task DeleteItem(string key)
        {
            await _jSRuntime.InvokeVoidAsync("sessionStorage.removeItem", key);
        }
    }
}
