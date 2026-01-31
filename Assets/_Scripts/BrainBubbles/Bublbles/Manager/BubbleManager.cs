//Author : _SourceCode
//CreateTime : 2026-01-30-18:19:41
//Version : 1.0
//UnityVersion : 2022.3.62f2c1

using MyFrame.BrainBubbles.Bubbles.Core;
using MyFrame.BrainBubbles.Bubbles.Interfaces;
using MyFrame.BrainBubbles.Bubbles.Refs;
using MyFrame.EventSystem.Events;
using MyFrame.EventSystem.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyFrame.BrainBubbles.Bubbles.Manager
{
    /// <summary>
    /// Engine Logic
    /// </summary>
    public class BubbleManager : IBubbleManager
    {
        private const float _minBubbleShowTime = 1.5f;
        private const float _maxBubbleShowTime = 3f;

        private const float _minBubbleZoom = 0.7f;
        private const float _maxBubbleZoom = 1.5f;

        private IEventBusCore _eventBus;
        private RectTransform _frame;


        private Dictionary<string, BubbleBase> _bubbles;
        private List<string> _toRemove;

        private BubblesInfo _info;
        private ulong _id = 0;

        private List<int> _remain;
        private System.IDisposable _bubbleBoomEventDis;
        private System.IDisposable _gameOverEventDis;

        ~BubbleManager() { 
            _bubbleBoomEventDis?.Dispose();
            _gameOverEventDis?.Dispose();
        } 
        public BubbleManager(IEventBusCore eventBus, RectTransform frame, BubblesInfo info)
        {
            _eventBus = eventBus;
            _frame = frame;
            _info = info;
            _bubbles = new Dictionary<string, BubbleBase>();
            _remain = new List<int>();
            _toRemove = new();
            OnStart();

            _bubbleBoomEventDis = _eventBus.Subscribe<BubbleBoomEvent>(OnBubbleBoomEvent);
            _bubbleBoomEventDis = _eventBus.Subscribe<GameOverEvent>(OnGameOverEvent);
        }

        private void OnGameOverEvent(GameOverEvent evt)
        {
            Over(evt.Message);
        }

        private void Over(string message = "")
        {
            foreach (var bubble in _bubbles.Values)
            {
                if(bubble is BrainBubble b)
                {
                    b.Over(BubbleBoomReason.GameOver,message);
                }
                else bubble.Boom(BubbleBoomReason.GameOver,message);
            }
            _remain.Clear();
            _bubbles.Clear();
            _toRemove.Clear();
        }
        public void OnStart()
        {
            _remain.Clear();
            _bubbles.Clear();
            _toRemove.Clear();
            for (int i = 0; i < _info.Count; i++)
            {
                _remain.Add(i);
            }
            
        }

        private void OnBubbleBoomEvent(BubbleBoomEvent evt)
        {
            RemoveBubble(evt.Id);
        }
        public bool NewBubble(BubblePos pos , out BubbleBase b)
        {
            b = null;
            if (_remain.Count == 0) return false;
            if (!_info.TryCreateBubbleObject(_frame, pos, out var obj)) return false;
            obj.name = $"Bubble {_id} ({pos.X},{pos.Y})";


            Button button = obj.GetComponent<Button>();
            if (button == null) return false;

            float random = Random.Range(_minBubbleShowTime, _maxBubbleShowTime);
            int random_index = Random.Range(0, _remain.Count);

            // Bubble Zoom
            float zoom = Random.Range(_minBubbleZoom, _maxBubbleZoom);
            obj.transform.localScale = new Vector3(zoom, zoom, 1);

            if (_info.TryGetValue(_remain[random_index], out string content, out TypeValue value))
            {
                var bubble = new BrainBubble(random, pos, content, button, obj, _frame, _id.ToString(), value, _eventBus);
                _bubbles[_id.ToString()] = bubble;

                bubble.Init();
                _id++;

                b = bubble;
            }
            _remain.RemoveAt(random_index);

            return true;
        }

        public void OnUpdate(float deltaTime)
        {
            foreach(var id in _toRemove)
            {
                _bubbles.Remove(id);
            }
            foreach (var bubble in _bubbles.Values)
            {
                bubble.OnUpdate(deltaTime);
            }
        }

        public void RemoveBubble(string bubbleId)
        {
            _toRemove.Add(bubbleId);
        }




    }
}
