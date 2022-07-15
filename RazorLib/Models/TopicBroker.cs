using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RazorLib.Models.ITopicBroker;

namespace RazorLib.Models
{
    public interface ITopicBroker
    {
        public void Subscribe(string topic, Action onNotificationReceived);
        public Task Notify(string topic);
    }

    public class TopicBroker : ITopicBroker
    {
        private readonly IDictionary<string, List<Action>> _subscribers;

        public TopicBroker()
        {
            _subscribers = new Dictionary<string, List<Action>>();
        }

        public async Task Notify(string topic)
        {
            var exists = _subscribers.TryGetValue(topic, out List<Action>? subscribers);
            if (exists)
            {
                foreach (var action in subscribers) {
                    action.Invoke();
                }
            }
        }

        public void Subscribe(string topic, Action onNotificationReceived)
        {
            var exists = _subscribers.TryGetValue(topic, out List<Action>? subscribers);
            if (!exists)
            {
                _subscribers.Add(topic, new List<Action>() { onNotificationReceived });
            }
            else
            {
                subscribers.Add(onNotificationReceived);
            }
        }
    }
}
