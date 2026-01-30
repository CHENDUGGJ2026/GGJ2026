//Author : _SourceCode
//CreateTime : 2025-09-10-14:01:26
//Version : 0.1
//UnityVersion : 2022.3.62f1c1

#nullable enable
using MyFrame.EventSystem.Interfaces;
using System;
using System.Collections.Generic;

namespace MyFrame.EventSystem.Events
{
    public sealed class EventBuffer
    {
        private readonly IEventBusCore _eventBus;

        private readonly List<(IEvent evt, Action<IEventBusCore, IEvent> publish)> _events = new(32);

        public EventBuffer(IEventBusCore eventBus)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public bool TryAdd<TEvent>(TEvent e) where TEvent : IEvent
        {
            if (e is null) return false;
            _events.Add((e, StaticPublisher<TEvent>.Publish));
            return true;
        }

        public void Release()
        {
            if (_events.Count == 0) return;

            try
            {
                for (int i = 0; i < _events.Count; i++)
                {
                    var (evt, publish) = _events[i];
                    publish(_eventBus, evt);
                }
            }
            finally
            {
                _events.Clear();
            }
        }

        public void Clear() => _events.Clear();

        private static class StaticPublisher<TEvent> where TEvent : IEvent
        {
            public static readonly Action<IEventBusCore, IEvent> Publish =
                static (bus, evt) => bus.Publish((TEvent)evt);
        }
    }


}

