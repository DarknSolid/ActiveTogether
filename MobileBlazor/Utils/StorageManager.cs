using RazorLib.Interfaces;

namespace MobileBlazor.Utils
{
    public class StorageManager<T> : IStorageManager<T>
    {
        private readonly ISessionStorage _sessionStorage;
        private readonly string _key;

        public StorageManager(ISessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
            _key = nameof(this.GetType);
        }

        public async Task DeleteCheckIn()
        {
            await _sessionStorage.DeleteItem(_key);
        }

        public async Task<bool> HasCheckIn()
        {
            return await _sessionStorage.Exists(_key);
        }

        public async Task SetCurrentCheckIn(T dto)
        {
            await _sessionStorage.SetItem(_key, dto);
        }
    }
}
