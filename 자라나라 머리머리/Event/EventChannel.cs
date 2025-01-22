using System.Collections.Generic;
using UnityEngine;

namespace Platformer {
    public abstract class EventChannel<T> : ScriptableObject {
        readonly HashSet<EventListener<T>> observers = new();

        public void Invoke(T value) {
            foreach (var observer in observers) {
                observer.Raise(value);
            }
        }
        
        public void Register(EventListener<T> observer) => observers.Add(observer);
        public void Deregister(EventListener<T> observer) => observers.Remove(observer);
    }


    [CreateAssetMenu(menuName = "Events/QuestChannel")]
    public class QuestEventChannel : EventChannel<QuestSystem> { }

    [CreateAssetMenu(menuName = "Events/InitChannel")]
    public class InitialEventChannel : EventChannel<InitialSystem> { }
}