//Author : _SourceCode
//CreateTime : 2026-01-30-18:59:42
//Version : 1.0
//UnityVersion : 2022.3.62f2c1



using MyFrame.BrainBubbles.Bubbles.Core;
using MyFrame.BrainBubbles.Bubbles.Refs;
using MyFrame.EventSystem.Interfaces;

namespace MyFrame.EventSystem.Events
{
    public sealed class BubbleBoomEvent : IEvent
    {
        private readonly string _id;
        private readonly BubbleBoomReason _reason;
        private readonly TypeValue _value;
        private readonly string _message;
        public BubbleBoomEvent(string id, BubbleBoomReason reason, TypeValue value ,string message = "")
        {
            _id = id;
            _reason = reason;
            _value = value;
            _message = message;
        }

        public string Id { get { return _id; } }
        public BubbleBoomReason Reason { get { return _reason; } }

        public string Message { get { return _message; } }
        public TypeValue Value { get { return _value; } }
    }

    public sealed class BubbleMoveEvent : IEvent
    {
        private readonly string _id;
        private readonly BubblePos _oldPos;
        private readonly BubblePos _newPos;
        private readonly string _message;

        public BubbleMoveEvent(string id, BubblePos oldPos, BubblePos newPos, string message = "")
        {
            _id = id;
            _oldPos = oldPos;
            _newPos = newPos;
            _message = message;
        }

        public string Id { get { return _id; } }
        public BubblePos OldPos { get { return _oldPos; } }
        public BubblePos NewPos { get { return _newPos; } }
        public string Message { get { return _message; } }
    }
}
