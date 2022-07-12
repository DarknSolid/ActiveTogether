using ModelLib.DTOs.CheckIns;

namespace RazorLib.Interfaces
{
    public interface ICheckInStorageManager
    {
        public Task SetCurrentCheckIn(CurrentlyCheckedInDTO dto);
        public Task DeleteCheckIn();
        public Task<bool> HasCheckIn();
    }
}
