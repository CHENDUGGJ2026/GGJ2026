//Author : _SourceCode
//CreateTime : 2026-01-30-18:19:41
//Version : 1.0
//UnityVersion : 2022.3.62f2c1

using MyFrame.BrainBubbles.Bubbles.Interfaces;
using MyFrame.BrainBubbles.Bubbles.Refs;
using MyFrame.EventSystem.Events;
using MyFrame.EventSystem.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyFrame.BrainBubbles.Bubbles.Core
{
    [Serializable]
    public sealed class TypeValue
    {
        
        private readonly Dictionary<BubbleType, float> _values;

        public TypeValue(Dictionary<BubbleType, float> values)
        {
            _values = values;
        }

        public float GetValue(BubbleType type)
        {
            if (!_values.TryGetValue(type, out var value)) return 0;
            return value;
        }
        public KeyValuePair<BubbleType, float>[] GetValues()
        {
            List<KeyValuePair<BubbleType, float>> list = new();
            foreach (var kvp in _values.Keys)
            {
                list.Add(new KeyValuePair<BubbleType, float>(kvp, _values[kvp]));
            }
            return list.ToArray();
        }
    }
    public abstract class BubbleBase : IBubble
    {
        protected readonly string _id;
        protected readonly TypeValue _values;
        protected readonly IEventBusCore _eventBusCore;

        public string ID => _id;
        public BubbleBase(string id, TypeValue values, IEventBusCore eventBusCore)
        {
            _id = id;
            _values = values;
            _eventBusCore = eventBusCore;
        }

        public virtual BubbleBoomReport Boom(BubbleBoomReason reason, string message = "")
        {
            Debug.Log($"Boom! Reason: {reason}  Id: {_id}");
            _eventBusCore.Publish(new BubbleBoomEvent(_id, reason,_values, message));
            return new BubbleBoomReport(reason,_values, _id , message);
        }
        public abstract void Init();
        public abstract void OnUpdate(float deltaTime);

    }
}

