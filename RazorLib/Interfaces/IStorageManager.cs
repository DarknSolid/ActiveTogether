using ModelLib.DTOs.CheckIns;

namespace RazorLib.Interfaces
{
    public interface IStorageManager<T>
    {
        public Task Set(T dto);
        public Task Delete();
        public Task<bool> Exists();
        public Task<T?> Get();

        public void Subscribe(Func<Task> onNotificationReceived);
        public void UnSubscribe(Func<Task> onNotificationReceived);
    }
}
