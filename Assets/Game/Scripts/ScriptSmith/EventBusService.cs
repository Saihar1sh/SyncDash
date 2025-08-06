using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arixen.ScriptSmith
{
    public static class EventBusService
    {
        public delegate void EventDelegate<T>(T e) where T : IGameEventData;

        private delegate void EventDelegate(IGameEventData e);

        private static Dictionary<Type, EventDelegate> m_events_dictionary;

        static EventBusService()
        {
            m_events_dictionary = new Dictionary<Type, EventDelegate>();
        }

        public static void Subscribe<T>(EventDelegate<T> listener) where T : IGameEventData
        {
            EventDelegate eve = (e) => listener((T)e);
            if (!m_events_dictionary.TryAdd(typeof(T), eve))
            {
                m_events_dictionary[typeof(T)] += eve;
            }
        }

        public static void UnSubscribe<T>(EventDelegate<T> listener) where T : IGameEventData
        {
            EventDelegate eve = (e) => listener((T)e);
            if (m_events_dictionary.TryGetValue(typeof(T), out EventDelegate _event))
            {
                _event -= eve;
            }
        }

        public static void UnSubscribeAll<T>() where T : IGameEventData
        {
            m_events_dictionary.Remove(typeof(T));
        }

        public static void InvokeEvent(IGameEventData gameEventData)
        {
            if (m_events_dictionary.TryGetValue(gameEventData.GetType(), out EventDelegate _event))
            {
                _event.Invoke(gameEventData);
            }
        }
    }

    public interface IGameEventData
    {
        
    }
}