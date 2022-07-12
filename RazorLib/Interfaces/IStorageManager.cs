using ModelLib.DTOs.CheckIns;

namespace RazorLib.Interfaces
{
    public interface IStorageManager<T>
    {
        public Task SetCurrentCheckIn(T dto);
        public Task DeleteCheckIn();
        public Task<bool> HasCheckIn();
    }
}
