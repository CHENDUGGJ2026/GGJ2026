//Author : _SourceCode
//CreateTime : 2026-01-30-18:19:41
//Version : 1.0
//UnityVersion : 2022.3.62f2c1

using LunziSpace;
using luoyu;
using MyFrame.BrainBubbles.Bubbles.BubbleMove.Core;
using MyFrame.BrainBubbles.Bubbles.BubbleMove.Interfaces;
using MyFrame.BrainBubbles.Bubbles.Interfaces;
using MyFrame.BrainBubbles.Bubbles.Refs;
using MyFrame.BrainBubbles.Frame.Core;
using MyFrame.BrainBubbles.Frame.Score;
using MyFrame.EventSystem.Core;
using MyFrame.EventSystem.Events;
using MyFrame.EventSystem.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace MyFrame.BrainBubbles.Bubbles.Manager
{
    public interface IGameOver
    {
        /// <summary>
        /// outPut Info 
        /// </summary>
        /// <param name="_gameValue"></param>
        void GameOver(GameValue _gameValue);
    }
    public class GameOverAdaptor : IGameOver
    {
        private readonly IResultInformastion _resultInformastion;

        public GameOverAdaptor(IResultInformastion resultInformastion)
        {
            _resultInformastion = resultInformastion;
        }

        public void GameOver(GameValue _gameValue)
        {
            var value = new OverInformation();
            foreach(var v in _gameValue.GetValues())
            {
                switch(v.Key)
                {
                    case BubbleType.Happy:
                        value.happy =(int) v.Value;
                        break;
                    case BubbleType.Scared:
                        value.afraid =(int) v.Value; break;
                    case BubbleType.Sad:
                        value.sad =(int) v.Value;
                        break;
                    case BubbleType.Angry:
                        value.angry =(int) v.Value;
                        break;
                }     
            }
            var c = UIManager.Instance.DialogPanel.GetComponent<DialogController>();
            var face = c?.TargetValue();
            var condition = c?.GetConditionValue();
            var res = _resultInformastion.ResultJudgment(value, face.Value, condition.Value);
            GameManager.Instance._eventBus.Publish(new GameOverEvent(res, _gameValue ,"GameOver"));
            Debug.Log($"GameOver: If Win? {res}");
        }
    }
    public class BrainSceneManager : IBrainSceneManager
    {
        private IBubbleManager _bubbleManager;
        private IEventBusCore _eventBusCore;
        private BubbleFrame _bubbleFrame;
         
        private IGameOver _gameOver;

        private IScoreUI _scoreUI;
        private IScoreController _scoreController;

        private IBubbleMoveController _bubbleMoveController;


        private bool _start = false;

        private const float _gameTime = 13f;
        private float _time = 0;

        private const float _BubbleMinTime = 1f;
        private const float _BubbleMaxTime = 2f;

        private float _bubbleTimer = 1f;
        private float _bubbleTime = 2f;

        private const int _BubbleMinCount = 3;
        private const int _BubbleMaxCount = 7;

        private int _clickCount = 0;
        private const int _maxClickCount = 6;

        private const int _maxRandomTimes = 2;
        private const float _minBubbleDistance = 80f;
        private List<string> _toRemove;

        private System.IDisposable _bubbleBoomEventDis;

        private Dictionary<string, BubblePos> _bubblePos;
        ~BrainSceneManager()
        {
            _bubbleBoomEventDis?.Dispose();
        }

        public BrainSceneManager(RectTransform transform, Vector2Int pos, Vector2Int size,IGameOver gameOver ,IEventBusCore eventBusCore = null)
        {
            BubblesData bubblesData = Resources.Load<BubblesData>("BubblesInfo/Data");
            GameObject frame = Resources.Load<GameObject>("BubblesInfo/Frame");
            _eventBusCore = eventBusCore ?? new EventBusCore();
            _gameOver = gameOver;

           

            _bubbleFrame = new BubbleFrame(transform,frame,pos,size);
            _bubbleManager = new BubbleManager(_eventBusCore, _bubbleFrame.RectTransform, BubblesInfo.Bulid(bubblesData));

            _bubbleMoveController = new BubbleMoveController(_eventBusCore, _bubbleFrame.RectTransform, _bubbleFrame.SmallBubbleTarget);

            _bubbleBoomEventDis = _eventBusCore.Subscribe<BubbleBoomEvent>(OnBubbleBoomEvent);

            _scoreUI = new ScoreUI(_bubbleFrame);
            _scoreController = new ScoreController(_eventBusCore, _scoreUI);

            _start = false;
            _time = _gameTime;
            _bubblePos = new Dictionary<string, BubblePos>();
            _toRemove = new List<string>();
            
        }

        private void OnBubbleBoomEvent(BubbleBoomEvent evt)
        {
            _toRemove.Add(evt.Id);
            if (evt.Reason == BubbleBoomReason.Click)
            {
                if(_bubblePos.TryGetValue(evt.Id, out BubblePos pos))
                {
                    _bubbleMoveController.BoomOut(pos, evt.Value);
                }
                foreach (var v in evt.Value.GetValues())
                {
                    if (_scoreController.TryGetValue(v.Key, out var value))
                    {
                        _scoreController.TrySetValue(v.Key, value + v.Value);
                    }

                }

                _clickCount++;
                if(_clickCount >= _maxClickCount)
                {
                    GameOver(GameOverReason.ClickTimesOn, $"ClickCount: {_clickCount}");
                }
                string message = "";
                foreach (var v in _scoreController.GetAll().GetValues())
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
        public void GameOver(GameOverReason reason, string message = "")
        {
            _bubbleManager.Over(message);
            Stop();
            _gameOver.GameOver(_scoreController.GetAll());
            _scoreController.Clear();
            _bubblePos = new Dictionary<string, BubblePos>();
            _toRemove.Clear();
            _time = _gameTime;

            Debug.Log("Current BrainScene : " + _start);

            return;
        }

        private void TimeUpdate(float time)
        {
            if(_time > 0)
            {
                _time -= time;
                _bubbleFrame.ChangeTimeSlider(1-_time/_gameTime);
            }
            else
            {
                GameOver(GameOverReason.TimeOff,"TimeOff");
            }
        }

        public void OnUpdate(float time)
        {
            if (!_start) return;
            TimeUpdate(time);
            _bubbleMoveController.OnUpdate(time);
            _bubbleManager.OnUpdate(time);
            if(_toRemove.Count > 0)
            {
                foreach (var v in _toRemove)
                {
                    _bubblePos.Remove(v);
                }
            }
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
                    float x, y;
                    int randomTime = 0;
                    while(randomTime < _maxRandomTimes)
                    {
                        x = Random.Range(_bubbleFrame.Xmin, _bubbleFrame.Xmax);
                        y = Random.Range(_bubbleFrame.Ymin, _bubbleFrame.Ymax);

                        bool ok = true;
                        foreach (var b in _bubblePos.Values)
                        {
                            if((x - b.X) * (x - b.X) + (y - b.Y) * (y - b.Y) < _minBubbleDistance * _minBubbleDistance)
                            {
                                ok = false;
                            }
                        }
                        if(ok)
                        {
                            if (_bubbleManager.NewBubble(new BubblePos((int)x, (int)y), out var bubble)) _bubblePos[bubble.ID] = new BubblePos((int)x, (int)y);
                            break;
                        }
                    }
                    
                }
                _bubbleTime = Random.Range(_BubbleMinTime, _BubbleMaxTime);
            }
        }
    }
}
