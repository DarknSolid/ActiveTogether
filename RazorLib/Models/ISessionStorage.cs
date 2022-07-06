using Microsoft.JSInterop;
using System.Text.Json;

namespace RazorLib.Models
{
    public interface ISessionStorage
    {
        public Task SetItem<T>(string key, T obj);
        public Task<T> GetItem<T>(string key);
        public Task<bool> Exists(string key);
    }
}
