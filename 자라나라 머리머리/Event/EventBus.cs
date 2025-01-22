using System;
using System.Collections.Generic;
using System.Linq;

public static class EventBus
{
    private static readonly Dictionary<Type, List<Delegate>> Subscribers = new Dictionary<Type, List<Delegate>>();

    public static void Subscribe<T>(Action<T> callback)
    {
        Type eventType = typeof(T);
        if (!Subscribers.ContainsKey(eventType))
        {
            Subscribers[eventType] = new List<Delegate>();
        }
        Subscribers[eventType].Add(callback);
    }

    public static void Unsubscribe<T>(Action<T> callback)
    {
        Type eventType = typeof(T);
        if (Subscribers.ContainsKey(eventType))
        {
            Subscribers[eventType].Remove(callback);
        }
    }

    public static void Publish<T>(T publishedEvent)
    {
        Type eventType = typeof(T);
        if (Subscribers.ContainsKey(eventType))
        {
            foreach (var subscriber in Subscribers[eventType].ToList())
            {
                ((Action<T>)subscriber)(publishedEvent);
            }
        }
    }
}
