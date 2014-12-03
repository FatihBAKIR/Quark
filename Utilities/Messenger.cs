﻿using System;
using System.Collections.Generic;
using Quark;
using System.Text.RegularExpressions;

public delegate void Callback();
public delegate void Callback<T>(T arg1);
public delegate void Callback<T,U>(T arg1,U arg2);
public delegate void Callback<T,U,V>(T arg1,U arg2,V arg3);

public enum MessengerMode
{
    DONT_REQUIRE_LISTENER,
    REQUIRE_LISTENER,
}


static public class MessengerInternal
{
    static public EventDictionary eventTable = new EventDictionary();
    static public readonly MessengerMode DEFAULT_MODE = MessengerMode.DONT_REQUIRE_LISTENER;

    static public void OnListenerAdding(string eventType, Delegate listenerBeingAdded)
    {
        if (!eventTable.ContainsKey(eventType))
        {
            eventTable.Add(eventType, null);
        }
		
        Delegate d = eventTable[eventType];
        if (d != null && d.GetType() != listenerBeingAdded.GetType())
        {
            throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
        }
    }

    static public void OnListenerRemoving(string eventType, Delegate listenerBeingRemoved)
    {
        if (eventTable.ContainsKey(eventType))
        {
            Delegate d = eventTable[eventType];
			
            if (d == null)
            {
                throw new ListenerException(string.Format("Attempting to remove listener with for event type {0} but current listener is null.", eventType));
            }
            else if (d.GetType() != listenerBeingRemoved.GetType())
            {
                throw new ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
            }
        }
        else
        {
            throw new ListenerException(string.Format("Attempting to remove listener for type {0} but Messenger doesn't know about this event type.", eventType));
        }
    }

    static public void OnListenerRemoved(string eventType)
    {
        if (eventTable[eventType] == null)
        {
            eventTable.Remove(eventType);
        }
    }

    static public void OnBroadcasting(string eventType, MessengerMode mode)
    {
        if (mode == MessengerMode.REQUIRE_LISTENER && !eventTable.ContainsKey(eventType))
        {
            throw new MessengerInternal.BroadcastException(string.Format("Broadcasting message {0} but no listener found.", eventType));
        }
    }

    static public BroadcastException CreateBroadcastSignatureException(string eventType)
    {
        return new BroadcastException(string.Format("Broadcasting message {0} but listeners have a different signature than the broadcaster.", eventType));
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
static public class Messenger
{
    private static EventDictionary eventTable = MessengerInternal.eventTable;

    static public void AddListener(string eventType, Callback handler)
    {
        eventType = Regex.Escape(eventType).Replace(@"\*", ".*").Replace(@"\?", ".");
        MessengerInternal.OnListenerAdding(eventType, handler);
        eventTable[eventType] = (Callback)eventTable[eventType] + handler;
    }

    static public void RemoveListener(string eventType, Callback handler)
    {
        eventType = Regex.Escape(eventType).Replace(@"\*", ".*").Replace(@"\?", ".");
        MessengerInternal.OnListenerRemoving(eventType, handler);	
        eventTable[eventType] = (Callback)eventTable[eventType] - handler;
        MessengerInternal.OnListenerRemoved(eventType);
    }

    static public void Broadcast(string eventType)
    {
        Broadcast(eventType, MessengerInternal.DEFAULT_MODE);
    }

    static public void Broadcast(string eventType, MessengerMode mode)
    {
        MessengerInternal.OnBroadcasting(eventType, mode);
        Delegate[] handlers = eventTable.GetHandlers(eventType);
        foreach (Delegate d in handlers)
        {
            Callback callback = (Callback)d;
            callback();
        }
    }
}

// One parameter
static public class Messenger<T>
{
    private static EventDictionary eventTable = MessengerInternal.eventTable;

    static public void AddListener(string eventType, Callback<T> handler)
    {
        eventType = Regex.Escape(eventType).Replace(@"\*", ".*").Replace(@"\?", ".");
        MessengerInternal.OnListenerAdding(eventType, handler);
        eventTable[eventType] = (Callback<T>)eventTable[eventType] + handler;
    }

    static public void RemoveListener(string eventType, Callback<T> handler)
    {
        eventType = Regex.Escape(eventType).Replace(@"\*", ".*").Replace(@"\?", ".");
        MessengerInternal.OnListenerRemoving(eventType, handler);
        eventTable[eventType] = (Callback<T>)eventTable[eventType] - handler;
        MessengerInternal.OnListenerRemoved(eventType);
    }

    static public void Broadcast(string eventType, T arg1)
    {
        Broadcast(eventType, arg1, MessengerInternal.DEFAULT_MODE);
        if (arg1 is Quark.Identifiable)
        {
            Messenger.Broadcast(((Quark.Identifiable)arg1).Identifier() + "." + eventType);
        }
    }

    static public void Broadcast(string eventType, T arg1, MessengerMode mode)
    {
        MessengerInternal.OnBroadcasting(eventType, mode);
        Delegate[] handlers = eventTable.GetHandlers(eventType);
        foreach (Delegate d in handlers)
        {
            Callback<T> callback = d as Callback<T>;
            callback(arg1);
        }
    }
}


// Two parameters
static public class Messenger<T, U>
{
    private static EventDictionary eventTable = MessengerInternal.eventTable;

    static public void AddListener(string eventType, Callback<T, U> handler)
    {
        eventType = Regex.Escape(eventType).Replace(@"\*", ".*").Replace(@"\?", ".");
        MessengerInternal.OnListenerAdding(eventType, handler);
        eventTable[eventType] = (Callback<T, U>)eventTable[eventType] + handler;
    }

    static public void RemoveListener(string eventType, Callback<T, U> handler)
    {
        eventType = Regex.Escape(eventType).Replace(@"\*", ".*").Replace(@"\?", ".");
        MessengerInternal.OnListenerRemoving(eventType, handler);
        eventTable[eventType] = (Callback<T, U>)eventTable[eventType] - handler;
        MessengerInternal.OnListenerRemoved(eventType);
    }

    static public void Broadcast(string eventType, T arg1, U arg2)
    {
        Broadcast(eventType, arg1, arg2, MessengerInternal.DEFAULT_MODE);
        if (arg1 is Quark.Identifiable)
        {
            Messenger<U>.Broadcast(((Quark.Identifiable)arg1).Identifier() + "." + eventType, arg2);
        }
    }

    static public void Broadcast(string eventType, T arg1, U arg2, MessengerMode mode)
    {
        MessengerInternal.OnBroadcasting(eventType, mode);
        Delegate[] handlers = eventTable.GetHandlers(eventType);
        foreach (Delegate d in handlers)
        {
            Callback<T, U> callback = d as Callback<T, U>;
            callback(arg1, arg2);
        }
    }
}


// Three parameters
static public class Messenger<T, U, V>
{
    private static EventDictionary eventTable = MessengerInternal.eventTable;

    static public void AddListener(string eventType, Callback<T, U, V> handler)
    {
        eventType = Regex.Escape(eventType).Replace(@"\*", ".*").Replace(@"\?", ".");
        MessengerInternal.OnListenerAdding(eventType, handler);
        eventTable[eventType] = (Callback<T, U, V>)eventTable[eventType] + handler;
    }

    static public void RemoveListener(string eventType, Callback<T, U, V> handler)
    {
        eventType = Regex.Escape(eventType).Replace(@"\*", ".*").Replace(@"\?", ".");
        MessengerInternal.OnListenerRemoving(eventType, handler);
        eventTable[eventType] = (Callback<T, U, V>)eventTable[eventType] - handler;
        MessengerInternal.OnListenerRemoved(eventType);
    }

    static public void Broadcast(string eventType, T arg1, U arg2, V arg3)
    {
        Broadcast(eventType, arg1, arg2, arg3, MessengerInternal.DEFAULT_MODE);
        if (arg1 is Quark.Identifiable)
        {
            Messenger<U, V>.Broadcast(((Quark.Identifiable)arg1).Identifier() + "." + eventType, arg2, arg3);
        }
    }

    static public void Broadcast(string eventType, T arg1, U arg2, V arg3, MessengerMode mode)
    {
        MessengerInternal.OnBroadcasting(eventType, mode);
        Delegate[] handlers = eventTable.GetHandlers(eventType);
        foreach (Delegate d in handlers)
        {
            Callback<T, U, V> callback = d as Callback<T, U, V>;
            callback(arg1, arg2, arg3);
        }
    }
}