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
using System.Collections.Generic;
using System;

namespace MyFrame.BrainBubbles.Bubbles.Manager
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        private static object _lock = new object();
        public static GameManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    lock(_lock)
                    {
                        if(_instance == null)
                        {
                            var g = new GameObject("GameManager");
                            DontDestroyOnLoad(g);
                            _instance = g.AddComponent<GameManager>();  
                            _instance._eventBus = new EventBusCore();
                            _instance.ToUpdate = new Dictionary<string, Action<float>>();
                            _instance.ToRemoveUpdate = new List<string>();
                        }
                    }
                }
                return _instance;
            }
        }
        private GameManager()
        {
        }
        public IEventBusCore _eventBus;
        private Dictionary<string, Action<float>> ToUpdate { get; set; }
        private List<string> ToRemoveUpdate { get; set; }
        private void Update()
        {
            if(ToRemoveUpdate ?.Count >0 )
            {
                foreach(var item in ToRemoveUpdate)
                {
                    ToUpdate.Remove(item);
                }
            }
            foreach(var action in ToUpdate)
            {
                if(action.Value is null )
                {
                    RemoveUpdateListener(action.Key);
                    continue;
                }
                else action.Value(Time.deltaTime);
            }
        }
        /// <summary>
        /// key can not be the same
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        public void AddUpdateListener(string key,Action<float> action)
        {
            ToUpdate[key] = action;
        }

        public void RemoveUpdateListener(string key)
        {
            ToRemoveUpdate.Add(key);
        }

    }
}
