//Author : _SourceCode
//CreateTime : 2026-01-30-18:19:41
//Version : 1.0
//UnityVersion : 2022.3.62f2c1

using MyFrame.BrainBubbles.Bubbles.Interfaces;
using MyFrame.BrainBubbles.Bubbles.Refs;
using MyFrame.BrainBubbles.Frame.Core;
using MyFrame.EventSystem.Core;
using MyFrame.EventSystem.Events;
using MyFrame.EventSystem.Interfaces;
using UnityEngine;

namespace MyFrame.BrainBubbles.Bubbles.Manager
{
    public class BrainSceneManager : IBrainSceneManager
    {
        private IBubbleManager _bubbleManager;
        private IEventBusCore _eventBusCore;
        private BubbleFrame _bubbleFrame;

        private GameValue _gameValue;

        private bool _start = false;

        private float _BubbleMinTime = 1f;
        private float _BubbleMaxTime = 5f;

        private float _bubbleTimer = 0f;
        private float _bubbleTime = 1f;

        private int _BubbleMinCount = 1;
        private int _BubbleMaxCount = 5;

        private System.IDisposable _bubbleBoomEventDis;

        ~BrainSceneManager()
        {
            _bubbleBoomEventDis?.Dispose();
        }

        public BrainSceneManager(RectTransform transform, Vector2Int pos, Vector2Int size, IEventBusCore eventBusCore = null)
        {
            BubblesData bubblesData = Resources.Load<BubblesData>("BubblesInfo/Data");
            GameObject frame = Resources.Load<GameObject>("BubblesInfo/Frame");
            _eventBusCore = eventBusCore ?? new EventBusCore();

            _bubbleFrame = new BubbleFrame(transform,frame,pos,size);
            _bubbleManager = new BubbleManager(_eventBusCore, _bubbleFrame.RectTransform, BubblesInfo.Bulid(bubblesData));

            _gameValue = new GameValue();
            _start = false;

            _bubbleBoomEventDis = _eventBusCore.Subscribe<BubbleBoomEvent>(OnBubbleBoomEvent);
        }

        private void OnBubbleBoomEvent(BubbleBoomEvent evt)
        {
            if (evt.Reason == BubbleBoomReason.Click)
            {
                string message = "";
                foreach (var v in evt.Value.GetValues())
                {
                    if (_gameValue.TryGetValue(v.Key, out var value))
                    {
                        _gameValue.SetValue(v.Key, value + v.Value);
                    }

                }
                foreach (var v in _gameValue.GetValues())
                {
                    message += $"{v.Key} {v.Value}\n";
                }
                Debug.Log($"Current Value:\n{message}");
            }
        }
        public void Start()
        {
            _start = true;
        }
        public void Stop()
        {
            _start = false;
        }
        public GameValue GameOver()
        {
            Stop();
            return _gameValue;
        }

        public void OnUpdate()
        {
            if (!_start) return;
            _bubbleManager.OnUpdate(Time.deltaTime);

            if (_bubbleTimer < _bubbleTime)
            {
                _bubbleTimer += Time.deltaTime;
            }
            else
            {
                _bubbleTimer = 0f;
                int count = Random.Range(_BubbleMinCount, _BubbleMaxCount + 1);
                for (int i = 0; i < count; i++)
                {
                    float x = Random.Range(_bubbleFrame.Xmin, _bubbleFrame.Xmax);
                    float y = Random.Range(_bubbleFrame.Ymin, _bubbleFrame.Ymax);
                    _bubbleManager.NewBubble(new BubblePos((int)x, (int)y));
                }
                _bubbleTime = Random.Range(_BubbleMinTime, _BubbleMaxTime);
            }
        }
    }
}
