//Author : _SourceCode
//CreateTime : 2026-02-01-01:26:19
//Version : 0.1
//UnityVersion : 2022.3.62f1c1

using MyFrame.BrainBubbles.Bubbles.Refs;
using MyFrame.BrainBubbles.Frame.Core;
using MyFrame.EventSystem.Events;
using MyFrame.EventSystem.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace MyFrame.BrainBubbles.Frame.Score
{
    public interface IScoreController
    {
        bool TrySetValue(BubbleType type, float value);
        bool TryGetValue(BubbleType type, out float value);
        GameValue GetAll();
        void Clear();
    }
    public class ScoreController : IScoreController
    {
        private GameValue _value;
        private IEventBusCore _eventBus;
        
        private IScoreUI _uI;



        public ScoreController( IEventBusCore eventBus, IScoreUI uI)
        {
            _value = new();
            _eventBus = eventBus;
            _uI = uI;

            _eventBus.Subscribe<BallArriveEvent>(OnBallArriveEvent);
        }
        private void OnBallArriveEvent(BallArriveEvent evt)
        {
            _uI.ScoreUIValueAdd(evt.Type, evt.Value);
        }

        public bool TrySetValue(BubbleType type, float value)
        {
            if(_value == null) return false;
            if(!_value.TryGetValue(type,out var v)) return false;
            _value.SetValue(type, value);
            return true;
        }

        public bool TryGetValue(BubbleType type, out float value)
        {
            value = 0f;
            if(_value == null) return false;
            if (!_value.TryGetValue(type, out var v)) return false;
            value = v;
            return true;
        }

        public GameValue GetAll()
        {
            var v = new GameValue();
            foreach(var va in _value.GetValues())
            {
                v.SetValue(va.Key, va.Value);
            }
            return v;
        }

        public void Clear()
        {
            _value = new GameValue();
        }
    }

    public interface IScoreUI
    {
        void ScoreUIValueChange(BubbleType type , float value);
        void ScoreUIValueAdd(BubbleType type , float value);
    }

    public class ScoreUI : IScoreUI
    {
        private BubbleFrame _frame;
        private Dictionary<BubbleType, Slider> _scoreUI;
        public ScoreUI(BubbleFrame frame)
        {
            _frame = frame;
            var t = _frame.RectTransform.Find("ScoreBoard");
            _scoreUI[BubbleType.Happy] = t.Find("Happy").GetComponent<Slider>();
            _scoreUI[BubbleType.Sad] = t.Find("Sad").GetComponent<Slider>();
            _scoreUI[BubbleType.Angry] = t.Find("Angry").GetComponent<Slider>();
            _scoreUI[BubbleType.Scared] = t.Find("Scared").GetComponent<Slider>();

        }
        public void ScoreUIValueChange(BubbleType type, float value)
        {
            var slider = _scoreUI[type];
            slider.value = value;
        }
        public void ScoreUIValueAdd(BubbleType type , float value)
        {
            _scoreUI[BubbleType.Happy].value += value;
        }
    }
}