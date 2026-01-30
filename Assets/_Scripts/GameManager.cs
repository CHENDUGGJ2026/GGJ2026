//Author : _SourceCode
//CreateTime : 2026-01-30-18:19:41
//Version : 1.0
//UnityVersion : 2022.3.62f2c1


using MyFrame.BrainBubbles.Bubbles.Interfaces;
using MyFrame.BrainBubbles.Bubbles.Refs;
using MyFrame.BrainBubbles.Frame.Core;
using MyFrame.EventSystem.Core;
using MyFrame.EventSystem.Interfaces;
using UnityEngine;


namespace MyFrame.BrainBubbles.Bubbles.Manager
{
    public class GameManager : MonoBehaviour
    {
        private IEventBusCore _eventBus;
        private RectTransform _frame;
        private BubblesInfo _bubblesInfo;
        private IBubbleManager _bubbleManager;

        private BubbleFrame _bubbleFrame;
        private BrainSceneManager _brainSceneManager;

        private void Start()
        {
            // EventSystem Bulid
            _eventBus = new EventBusCore();
            
            _frame = GameObject.Find("Canvas").GetComponent<RectTransform>();
            _brainSceneManager = new BrainSceneManager(_frame, new Vector2Int(-200, -100), new Vector2Int(600, 300));
            

            _brainSceneManager.Start();
        }

        private void Update()
        {
            _brainSceneManager?.OnUpdate();
        }
    }
}
