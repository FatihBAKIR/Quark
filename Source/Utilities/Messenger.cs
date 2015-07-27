using System;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable 1591

namespace Quark.Utilities
{
    public delegate void Callback();

    public delegate void Callback<T>(T arg1);

    public delegate void Callback<T, U>(T arg1, U arg2);

    public delegate void Callback<T, U, V>(T arg1, U arg2, V arg3);

    public enum MessengerMode
    {
        DONT_REQUIRE_LISTENER,
        REQUIRE_LISTENER,
    }

    internal static class MessengerInternal
    {
        public static readonly Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();
        public static readonly MessengerMode DEFAULT_MODE = MessengerMode.REQUIRE_LISTENER;

        public static void AddListener(string eventType, Delegate callback)
        {
            MessengerInternal.OnListenerAdding(eventType, callback);
            eventTable[eventType] = Delegate.Combine(eventTable[eventType], callback);
        }

        public static void RemoveListener(string eventType, Delegate handler)
        {
            MessengerInternal.OnListenerRemoving(eventType, handler);
            eventTable[eventType] = Delegate.Remove(eventTable[eventType], handler);
            MessengerInternal.OnListenerRemoved(eventType);
        }

        public static T[] GetInvocationList<T>(string eventType)
        {
            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                if (d != null)
                {
                    return d.GetInvocationList().Cast<T>().ToArray();
                }
                else
                {
                    throw MessengerInternal.CreateBroadcastSignatureException(eventType);
                }
            }
            return null;
        }

        public static void OnListenerAdding(string eventType, Delegate listenerBeingAdded)
        {
            if (!eventTable.ContainsKey(eventType))
            {
                eventTable.Add(eventType, null);
            }

            var d = eventTable[eventType];
            if (d != null && d.GetType() != listenerBeingAdded.GetType())
            {
                throw new ListenerException(
                    string.Format(
                        "Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}",
                        eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
            }
        }

        public static void OnListenerRemoving(string eventType, Delegate listenerBeingRemoved)
        {
            if (eventTable.ContainsKey(eventType))
            {
                var d = eventTable[eventType];

                if (d == null)
                {
                    throw new ListenerException(
                        string.Format(
                            "Attempting to remove listener with for event type {0} but current listener is null.",
                            eventType));
                }
                else if (d.GetType() != listenerBeingRemoved.GetType())
                {
                    throw new ListenerException(
                        string.Format(
                            "Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}",
                            eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
                }
            }
            else
            {
                throw new ListenerException(
                    string.Format(
                        "Attempting to remove listener for type {0} but Messenger doesn't know about this event type.",
                        eventType));
            }
        }

        public static void OnListenerRemoved(string eventType)
        {
            if (eventTable[eventType] == null)
            {
                eventTable.Remove(eventType);
            }
        }

        public static void OnBroadcasting(string eventType, MessengerMode mode)
        {
            if (mode == MessengerMode.REQUIRE_LISTENER && !eventTable.ContainsKey(eventType))
            {
                throw new MessengerInternal.BroadcastException(
                    string.Format("Broadcasting message {0} but no listener found.", eventType));
            }
        }

        public static BroadcastException CreateBroadcastSignatureException(string eventType)
        {
            return
                new BroadcastException(
                    string.Format(
                        "Broadcasting message {0} but listeners have a different signature than the broadcaster.",
                        eventType));
        }

        public class BroadcastException : Exception
        {
            public BroadcastException(string msg)
                : base(msg)
            {
            }
        }

        public class ListenerException : Exception
        {
            public ListenerException(string msg)
                : base(msg)
            {
            }
        }
    }

// No parameters
    public static class Messenger
    {
        public static void AddListener(string eventType, Action handler)
        {
            MessengerInternal.AddListener(eventType, handler);
        }

        public static void AddListener<TReturn>(string eventType, Func<TReturn> handler)
        {
            MessengerInternal.AddListener(eventType, handler);
        }

        public static void RemoveListener(string eventType, Action handler)
        {
            MessengerInternal.RemoveListener(eventType, handler);
        }

        public static void RemoveListener<TReturn>(string eventType, Func<TReturn> handler)
        {
            MessengerInternal.RemoveListener(eventType, handler);
        }

        public static void Broadcast(string eventType)
        {
            Broadcast(eventType, MessengerInternal.DEFAULT_MODE);
        }

        public static void Broadcast<TReturn>(string eventType, Action<TReturn> returnCall)
        {
            Broadcast(eventType, returnCall, MessengerInternal.DEFAULT_MODE);
        }

        public static void Broadcast(string eventType, MessengerMode mode)
        {
            MessengerInternal.OnBroadcasting(eventType, mode);
            var invocationList = MessengerInternal.GetInvocationList<Action>(eventType);

            foreach (var callback in invocationList)
                callback.Invoke();
        }

        public static void Broadcast<TReturn>(string eventType, Action<TReturn> returnCall, MessengerMode mode)
        {
            MessengerInternal.OnBroadcasting(eventType, mode);
            var invocationList = MessengerInternal.GetInvocationList<Func<TReturn>>(eventType);

            foreach (var result in invocationList.Select(del => del.Invoke()).Cast<TReturn>())
            {
                returnCall.Invoke(result);
            }
        }
    }

// One parameter
    public static class Messenger<T>
    {
        public static void AddListener(string eventType, Action<T> handler)
        {
            MessengerInternal.AddListener(eventType, handler);
        }

        public static void AddListener<TReturn>(string eventType, Func<T, TReturn> handler)
        {
            MessengerInternal.AddListener(eventType, handler);
        }

        public static void RemoveListener(string eventType, Action<T> handler)
        {
            MessengerInternal.RemoveListener(eventType, handler);
        }

        public static void RemoveListener<T, TReturn>(string eventType, Func<T, TReturn> handler)
        {
            MessengerInternal.RemoveListener(eventType, handler);
        }

        public static void Broadcast(string eventType, T arg1)
        {
            Broadcast(eventType, arg1, MessengerInternal.DEFAULT_MODE);
        }

        public static void Broadcast<TReturn>(string eventType, T arg1, Action<TReturn> returnCall)
        {
            Broadcast(eventType, arg1, returnCall, MessengerInternal.DEFAULT_MODE);
        }

        public static void Broadcast(string eventType, T arg1, MessengerMode mode)
        {
            MessengerInternal.OnBroadcasting(eventType, mode);
            var invocationList = MessengerInternal.GetInvocationList<Action<T>>(eventType);

            foreach (var callback in invocationList)
                callback.Invoke(arg1);
        }

        public static void Broadcast<TReturn>(string eventType, T arg1, Action<TReturn> returnCall, MessengerMode mode)
        {
            MessengerInternal.OnBroadcasting(eventType, mode);
            var invocationList = MessengerInternal.GetInvocationList<Func<T, TReturn>>(eventType);

            foreach (var result in invocationList.Select(del => del.Invoke(arg1)).Cast<TReturn>())
            {
                returnCall.Invoke(result);
            }
        }
    }


// Two parameters
    public static class Messenger<T, U>
    {
        public static void AddListener(string eventType, Action<T, U> handler)
        {
            MessengerInternal.AddListener(eventType, handler);
        }

        public static void AddListener<TReturn>(string eventType, Func<T, U, TReturn> handler)
        {
            MessengerInternal.AddListener(eventType, handler);
        }

        public static void RemoveListener(string eventType, Action<T, U> handler)
        {
            MessengerInternal.RemoveListener(eventType, handler);
        }

        public static void RemoveListener<TReturn>(string eventType, Func<T, U, TReturn> handler)
        {
            MessengerInternal.RemoveListener(eventType, handler);
        }

        public static void Broadcast(string eventType, T arg1, U arg2)
        {
            Broadcast(eventType, arg1, arg2, MessengerInternal.DEFAULT_MODE);
        }

        public static void Broadcast<TReturn>(string eventType, T arg1, U arg2, Action<TReturn> returnCall)
        {
            Broadcast(eventType, arg1, arg2, returnCall, MessengerInternal.DEFAULT_MODE);
        }

        public static void Broadcast(string eventType, T arg1, U arg2, MessengerMode mode)
        {
            MessengerInternal.OnBroadcasting(eventType, mode);
            var invocationList = MessengerInternal.GetInvocationList<Action<T, U>>(eventType);

            foreach (var callback in invocationList)
                callback.Invoke(arg1, arg2);
        }

        public static void Broadcast<TReturn>(string eventType, T arg1, U arg2, Action<TReturn> returnCall,
            MessengerMode mode)
        {
            MessengerInternal.OnBroadcasting(eventType, mode);
            var invocationList = MessengerInternal.GetInvocationList<Func<T, U, TReturn>>(eventType);

            foreach (var result in invocationList.Select(del => del.Invoke(arg1, arg2)).Cast<TReturn>())
            {
                returnCall.Invoke(result);
            }
        }
    }


// Three parameters
    public static class Messenger<T, U, V>
    {
        public static void AddListener(string eventType, Action<T, U, V> handler)
        {
            MessengerInternal.AddListener(eventType, handler);
        }

        public static void AddListener<TReturn>(string eventType, Func<T, U, V, TReturn> handler)
        {
            MessengerInternal.AddListener(eventType, handler);
        }

        public static void RemoveListener(string eventType, Action<T, U, V> handler)
        {
            MessengerInternal.RemoveListener(eventType, handler);
        }

        public static void RemoveListener<TReturn>(string eventType, Func<T, U, V, TReturn> handler)
        {
            MessengerInternal.RemoveListener(eventType, handler);
        }

        public static void Broadcast(string eventType, T arg1, U arg2, V arg3)
        {
            Broadcast(eventType, arg1, arg2, arg3, MessengerInternal.DEFAULT_MODE);
        }

        public static void Broadcast<TReturn>(string eventType, T arg1, U arg2, V arg3, Action<TReturn> returnCall)
        {
            Broadcast(eventType, arg1, arg2, arg3, returnCall, MessengerInternal.DEFAULT_MODE);
        }

        public static void Broadcast(string eventType, T arg1, U arg2, V arg3, MessengerMode mode)
        {
            MessengerInternal.OnBroadcasting(eventType, mode);
            var invocationList = MessengerInternal.GetInvocationList<Action<T, U, V>>(eventType);

            foreach (var callback in invocationList)
                callback.Invoke(arg1, arg2, arg3);
        }

        public static void Broadcast<TReturn>(string eventType, T arg1, U arg2, V arg3, Action<TReturn> returnCall,
            MessengerMode mode)
        {
            MessengerInternal.OnBroadcasting(eventType, mode);
            var invocationList = MessengerInternal.GetInvocationList<Func<T, U, V, TReturn>>(eventType);

            foreach (var result in invocationList.Select(del => del.Invoke(arg1, arg2, arg3)).Cast<TReturn>())
            {
                returnCall.Invoke(result);
            }
        }
    }
}