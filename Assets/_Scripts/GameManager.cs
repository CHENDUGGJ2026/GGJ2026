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
using static BasePanelController;
using LunziSpace;

namespace MyFrame.BrainBubbles.Bubbles.Manager
{
    public class GameManager : MonoBehaviour
    {
        private IEventBusCore _eventBus;
        private RectTransform _frame;

         private BubblesInfo _bubblesInfo;
        private IBubbleManager _bubbleManager;//

        private BubbleFrame _bubbleFrame;//需要new,似乎是泡泡生成的框?
        private BrainSceneManager _brainSceneManager;


        private void Start()
        {
            // EventSystem Bulid
            _eventBus = new EventBusCore();
            
            _frame = GameObject.Find("Canvas").GetComponent<RectTransform>();//框的范围
            _brainSceneManager = new BrainSceneManager(_frame, new Vector2Int(-200, -100), new Vector2Int(600, 300));

            UIManager.Instance.DialogPanel.GetComponent<DialogController>().FightAction += BattleInit;//这里放了一个钩子,战斗开始时触发

            _brainSceneManager.Start();
        }

        private void Update()
        {
            _brainSceneManager?.OnUpdate();
        }

      /// <summary>
      /// 订阅FightAction
      /// </summary>
        public void BattleInit( )
        {

           
        }
    }
}
