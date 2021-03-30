#region
using System;
using System.Collections.Generic;
using UnityEngine;
#endregion
namespace Events
{

    public static class ObjectExt
    {
        public static Subscriber<T> Subscribe<T>(this object _, Action<T> cb) where T : Message
        {
            EventManager<T>.Subscribe(cb);
            return new Subscriber<T>(cb);
        }

        public static void Unsubscribe<T>(this object _, Action<T> cb) where T : Message
        {
            EventManager<T>.Unsubscribe(cb);
        }

        public static void SendEvent<T>(this object _, T data = null) where T : Message
        {
            EventManager<T>.Invoke(data);
        }

        public static void SubscribeOnce<T>(this object @this, Action<T> action) where T : Message
        {
            void WrapperAction(T data)
            {
                action?.Invoke(data);
                EventManager<T>.Unsubscribe(WrapperAction);
            }

            EventManager<T>.Subscribe(WrapperAction);
        }

        public static void Unsubscribe<TMessage, TKey>(this object _, Action<TMessage> action, TKey key) where TMessage : Message
        {
            TopicEventManager<TMessage, TKey>.Unsubscribe(action, key);
        }
        public static Subscriber<TMessage, TKey> Subscribe<TMessage, TKey>(this object _, Action<TMessage> action, TKey key) where TMessage : Message
        {
            TopicEventManager<TMessage, TKey>.Subscribe(action, key);
            return new Subscriber<TMessage, TKey>(action, key);
        }
        public static void SendEvent<TMessage, TKey>(this object _, TMessage data, TKey key) where TMessage : Message
        {
            TopicEventManager<TMessage, TKey>.Invoke(data, key);
        }
        public class DestroyActionContainer : MonoBehaviour
        {
            private Action _unsubscription;
            private void OnDestroy()
            {
                _unsubscription();
            }
            public void Init(Action action)
            {
                _unsubscription = action;
            }
        }

        public class Subscriber<TMessage, TKey> where TMessage : Message
        {
            private readonly Action<TMessage> _action;
            private TKey _key;
            public Subscriber(Action<TMessage> action, TKey key)
            {
                _action = action;
                _key = key;
            }

            public void BindTo(MonoBehaviour mb)
            {
                mb.gameObject.AddComponent<DestroyActionContainer>().Init(
                    () => this.Unsubscribe(_action, _key)
                );
            }
        }

        public class Subscriber<T> where T : Message
        {
            private readonly Action<T> _action;
            public Subscriber(Action<T> action)
            {
                _action = action;
            }

            public void BindTo(MonoBehaviour mb)
            {
                mb.gameObject.AddComponent<DestroyActionContainer>().Init(
                    () => this.Unsubscribe(_action)
                );
            }
        }
    }

    public abstract class Message
    {
    }

    public abstract class BoolMessage : Message
    {
        public bool active { get; set; }
    }

    public abstract class BaseValueMessage<T> : Message
    {
        public T value { get; set; }
    }

    internal class EventContainer<T>
    {
        public event Action<T> Event;

        public void Invoke(T msg)
        {
            Event?.Invoke(msg);
        }
    }

    internal static class TopicEventManager<TMessage, TKey> where TMessage : Message
    {
        private static readonly Dictionary<TKey, EventContainer<TMessage>> _events = new Dictionary<TKey, EventContainer<TMessage>>();

        public static void Subscribe(Action<TMessage> action, TKey topic)
        {
            if (_events.TryGetValue(topic, out var container) == false) _events[topic] = container = new EventContainer<TMessage>();

            container.Event += action;
        }

        public static void Unsubscribe(Action<TMessage> action, TKey topic)
        {
            if (_events.TryGetValue(topic, out var container)) container.Event -= action;
        }

        public static void Invoke(TMessage message, TKey key)
        {
            if (_events.TryGetValue(key, out var container)) container.Invoke(message);
        }
    }

    internal static class EventManager<T> where T : Message
    {
        public static event Action<T> Event;
        public static void Subscribe(Action<T> cb)
        {
            Event += cb;
        }

        public static void Unsubscribe(Action<T> cb)
        {
            Event -= cb;
        }

        public static void Invoke(T data = null)
        {
            Event?.Invoke(data);
        }
    }


}
