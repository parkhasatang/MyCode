using UnityEngine;
using UnityEngine.Events;

namespace Platformer {
    public abstract class EventListener<T> : MonoBehaviour {
        [Header("# Event Listener System Info")]
        [SerializeField] protected EventChannel<T> eventChannel;
        [SerializeField] private UnityEvent<T> unityEvent;

        protected virtual void Awake() {
            eventChannel.Register(this);
        }
        
        protected void OnDestroy() {
            eventChannel.Deregister(this);
        }
        
        public void Raise(T value) {
            unityEvent?.Invoke(value);
        }
        
    }
}