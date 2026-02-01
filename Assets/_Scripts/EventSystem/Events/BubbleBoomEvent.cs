//Author : _SourceCode
//CreateTime : 2026-01-30-18:59:42
//Version : 1.0
//UnityVersion : 2022.3.62f2c1



using MyFrame.BrainBubbles.Bubbles.Core;
using MyFrame.BrainBubbles.Bubbles.Refs;
using MyFrame.EventSystem.Interfaces;
using System;

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

    public sealed class GameOverEvent : IEvent
    {
        private readonly bool _res;
        private readonly string _message ;
        private readonly GameValue _value;

        public GameOverEvent(bool res, GameValue value, string message = "")
        {
            _res = res;
            _value = value;
            _message = message;
        }

        public bool Res { get { return _res; } }
        public string Message { get { return _message; }}
        public GameValue Value { get { return _value; } }
    }

    public sealed class BubbleClickEvent : IEvent
    {
        private readonly string _id;
        private readonly BubblePos _pos;
        private readonly TypeValue _type;
        private readonly string _content;

        public BubbleClickEvent(string id, BubblePos pos, TypeValue type, string content)
        {
            _id = id;
            _pos = pos;
            _type = type;
            _content = content;
        }
        public string Id { get { return _id; }}
        public BubblePos Pos { get { return _pos; }}
        public TypeValue Type { get { return _type; }}
        public string Content { get { return _content; }}
    }

    public sealed class BallArriveEvent : IEvent
    {
        private readonly string _id;
        private readonly BubbleType _type;
        private readonly float _value;

        public BallArriveEvent(string id, BubbleType type, float value)
        {
            _id = id;
            _type = type;
            _value = value;
        }
        public string Id { get { return _id; } }
        public BubbleType Type { get { return _type; }}
        public float Value { get { return _value; }}
    }

}
