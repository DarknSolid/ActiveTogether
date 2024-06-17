using RazorLib.Interfaces;

namespace RazorLib.Models
{
    /// <summary>
    /// Manages storage of a specified type in the browser's session storage.
    /// It needs a topic broker to notify subscribers of changes made to the object being saved in the storage.
    /// This allows components to dynamically update. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StorageManager<T> : IStorageManager<T>
    {
        private readonly ISessionStorage _sessionStorage;
        private readonly ITopicBroker _topicBroker;

        public string BrokerTopicString { get; internal set; }

        public StorageManager(ISessionStorage sessionStorage, ITopicBroker broker)
        {
            _sessionStorage = sessionStorage;
            _topicBroker = broker;
            BrokerTopicString = typeof(T).Name;
        }

        public async Task Delete()
        {
            await _sessionStorage.DeleteItem(BrokerTopicString);
            await _topicBroker.Notify(BrokerTopicString);
        }

        public async Task<T?> Get()
        {
            if (await Exists())
            {
                return await _sessionStorage.GetItem<T>(BrokerTopicString);
            }
            return default;
        }

        public async Task<bool> Exists()
        {
            return await _sessionStorage.Exists(BrokerTopicString);
        }

        public async Task Set(T dto)
        {
            await _sessionStorage.SetItem(BrokerTopicString, dto);
            await _topicBroker.Notify(BrokerTopicString);
        }

        public void Subscribe(Func<Task> onNotificationReceived)
        {
            _topicBroker.Subscribe(BrokerTopicString, onNotificationReceived);
        }

        public void UnSubscribe(Func<Task> onNotificationReceived)
        {
            _topicBroker.Unsubscribe(BrokerTopicString, onNotificationReceived);
        }
    }
}
