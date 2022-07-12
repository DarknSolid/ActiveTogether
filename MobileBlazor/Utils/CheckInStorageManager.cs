using ModelLib.DTOs.CheckIns;
using RazorLib.Interfaces;

namespace MobileBlazor.Utils
{
    public class CheckInStorageManager : ICheckInStorageManager
    {
        private readonly ISessionStorage _sessionStorage;
        private readonly string _key;

        public CheckInStorageManager(ISessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
            _key = "CurrentCheckInStatus";
        }

        public async Task DeleteCheckIn()
        {
            await _sessionStorage.DeleteItem(_key);
        }

        public async Task<bool> HasCheckIn()
        {
            return await _sessionStorage.Exists(_key);
        }

        public async Task SetCurrentCheckIn(CurrentlyCheckedInDTO dto)
        {
            await _sessionStorage.SetItem(_key, dto);
        }
    }
}
