using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RazorLib.Models.ITopicBroker;
using System.Collections.Concurrent;

namespace RazorLib.Models
{
    public interface ITopicBroker
    {
        public void Subscribe(string topic, Func<Task> onNotificationReceived);
        public void Unsubscribe(string topic, Func<Task> onNotificationReceived);
        public Task Notify(string topic);
    }

    public class TopicBroker : ITopicBroker
    {
        private readonly IDictionary<string, ConcurrentDictionary<Func<Task>, object?>> _subscribers;

        public TopicBroker()
        {
            _subscribers = new Dictionary<string, ConcurrentDictionary<Func<Task>, object?>>();
        }

        public async Task Notify(string topic)
        {
            var exists = _subscribers.TryGetValue(topic, out ConcurrentDictionary<Func<Task>, object?>? subscribers);
            if (exists)
            {
                foreach (var action in subscribers)
                {
                    await action.Key.Invoke();
                }
            }
        }

        public void Subscribe(string topic, Func<Task> onNotificationReceived)
        {
            lock (this)
            {
                var exists = _subscribers.TryGetValue(topic, out ConcurrentDictionary<Func<Task>, object?>? subscribers);
                if (!exists)
                {
                    var x = new ConcurrentDictionary<Func<Task>, object>();
                    x.TryAdd(onNotificationReceived, null);
                    _subscribers.Add(topic, x);
                }
                else
                {
                    subscribers.TryAdd(onNotificationReceived,null);
                }
            }
        }

        public void Unsubscribe(string topic, Func<Task> onNotificationReceived)
        {
            lock (this)
            {
                var exists = _subscribers.TryGetValue(topic, out ConcurrentDictionary<Func<Task>, object?>? subscribers);
                if (exists)
                {
                    subscribers.Remove(onNotificationReceived, out object x);
                }
            }
        }
    }
}
