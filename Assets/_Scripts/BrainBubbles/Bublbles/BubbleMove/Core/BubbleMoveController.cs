//Author : _SourceCode
//CreateTime : 2026-01-30-18:19:41
//Version : 1.0
//UnityVersion : 2022.3.62f2c1

using MyFrame.BrainBubbles.Bubbles.BubbleMove.Interfaces;
using MyFrame.BrainBubbles.Bubbles.BubbleMove.Refs;
using MyFrame.BrainBubbles.Bubbles.Refs;
using System;

namespace MyFrame.BrainBubbles.Bubbles.BubbleMove.Core
{
    using MyFrame.BrainBubbles.Bubbles.Core;
    using MyFrame.EventSystem.Events;
    using MyFrame.EventSystem.Interfaces;
    using System.Collections.Generic;
    using UnityEngine;

    public class BubbleMoveController : IBubbleMoveController
    {
        [SerializeField] private SmallBubbleInfo _info;
        [SerializeField] private RectTransform _transform; // Panel
        [SerializeField] private RectTransform _target;    // 目标

        private readonly IEventBusCore _eventBus;

        private readonly List<SmallBubble> _bubbles = new();
        private readonly Dictionary<string, (BubbleType,float)> _values = new();

        // 生成时给的速度上限（参考尺寸下）
        private const float _maxSpawnSpeed = 100f;

        // 你目标分辨率 3840x2160 => min=2160（用它作为“观感速度一致”的参考）
        private const float _referencePanelMin = 1080f;

        private long _id = 0;

        public BubbleMoveController() { }

        public BubbleMoveController(IEventBusCore eventBus, RectTransform panel, RectTransform target)
        {
            _eventBus = eventBus;
            _transform = panel;
            _target = target;

            _info = Resources.Load<SmallBubbleInfo>("BubblesInfo/SmallBubbleData");
        }

        public void SetRefs(SmallBubbleInfo info, RectTransform panel, RectTransform target)
        {
            _info = info;
            _transform = panel;
            _target = target;
        }
        private void NewSmallBubble(BubbleType type , BubblePos pos, float value)
        {
            if (_info == null || _transform == null || _target == null) return;
            if (!_info.TryCreateSmallBubble(type, out var obj)) return;
            if (obj == null) return;

            var rect = obj.GetComponent<RectTransform>();
            if (rect == null) return;

            // 1) 作为 UI 子物体放到 Panel 下（worldPositionStays=false 非常重要）
            rect.SetParent(_transform, false);

            // 2) 统一 anchor/pivot，确保 anchoredPosition 以 Panel 中心为原点更好控
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.0f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            // 3) 生成点在 pos 附近做少量随机扰动（按 Panel 尺寸比例）
            float x = Random.Range(-0.01f * _transform.rect.width, 0.01f * _transform.rect.width);
            float y = Random.Range(-0.01f * _transform.rect.height, 0.01f * _transform.rect.height);

            rect.localPosition = new Vector2(pos.X + x, pos.Y + y);
            rect.localRotation = Quaternion.identity;

            // 4) 随机初始方向与速度
            Vector2 dir = Random.insideUnitCircle;
            if (dir.sqrMagnitude < 1e-6f) dir = Vector2.up;
            dir.Normalize();

            float speed = Random.Range(0.2f, _maxSpawnSpeed); // 给个最小值，避免不动

            // 5) 创建运行时小球并加入管理
            _bubbles.Add(new SmallBubble(_id.ToString(), rect, _transform, _target, speed, dir, _referencePanelMin));
            _values[_id.ToString()] = (type, value);
            _id++;

        }
        public void BoomOut(BubblePos pos, TypeValue values)
        {
            foreach(var v in values.GetValues())
            {
                if(v.Value != 0)
                {
                    NewSmallBubble(v.Key,pos,v.Value);
                }
            }
        }

        public void OnUpdate(float dt)
        {
            if (_bubbles.Count == 0) return;

            // 倒序遍历方便移除
            for (int i = _bubbles.Count - 1; i >= 0; i--)
            {
                var b = _bubbles[i];

                // 目标/自身被销毁时清掉
                if (b == null || !b.IsAlive)
                {
                    _eventBus.Publish(new BallArriveEvent(b.Id, _values[b.Id].Item1, _values[b.Id].Item2));
                    _bubbles.RemoveAt(i);
                    continue;
                }

                b.OnUpdate(dt);

                if (!b.IsAlive)
                {
                    _eventBus.Publish(new BallArriveEvent(b.Id, _values[b.Id].Item1, _values[b.Id].Item2));
                    _bubbles.RemoveAt(i);
                }
                    
            }
        }

        // 你如果有需要，可加一个清理接口
        public void ClearAll()
        {
            for (int i = _bubbles.Count - 1; i >= 0; i--)
            {
                _bubbles[i]?.ForceBoom();
            }
            

            for (int i = _bubbles.Count - 1; i >= 0; i--)
            {
                var b = _bubbles[i];

                // 目标/自身被销毁时清掉
                if (b == null || !b.IsAlive)
                {
                    _eventBus.Publish(new BallArriveEvent(b.Id, _values[b.Id].Item1, _values[b.Id].Item2));
                    _bubbles.RemoveAt(i);
                    continue;
                }
            }
            _bubbles.Clear();
            _values.Clear();
        }
    }

}

