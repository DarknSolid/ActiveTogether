
namespace RazorLib.Interfaces
{
    public interface ISessionStorage
    {
        public Task SetItem<T>(string key, T obj);
        public Task DeleteItem(string key);
        public Task<T?> GetItem<T>(string key);
        public Task<bool> Exists(string key);
    }
}
