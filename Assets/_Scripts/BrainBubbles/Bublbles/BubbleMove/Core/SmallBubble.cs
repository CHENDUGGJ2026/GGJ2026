//Author : _SourceCode
//CreateTime : 2026-01-30-18:19:41
//Version : 1.0
//UnityVersion : 2022.3.62f2c1

using MyFrame.BrainBubbles.Bubbles.BubbleMove.Interfaces;
using MyFrame.BrainBubbles.Bubbles.Refs;
using UnityEngine;

namespace MyFrame.BrainBubbles.Bubbles.BubbleMove.Core
{
    using MyFrame.EventSystem.Events;
    using MyFrame.EventSystem.Interfaces;
    using UnityEngine;

    public sealed class SmallBubble
    {
        private readonly RectTransform _self;
        private readonly RectTransform _panel;
        private readonly RectTransform _target;

        private readonly string _id;

        private Vector2 _vel;

        // 基础参数（以 referencePanelMin 下为调参基准）
        private readonly float _baseSpeed;
        private readonly float _referencePanelMin;

        private const float ArriveRadius = 5f;
        private const float MaxSpeed = 500f;
        private const float Force = 10f;

        public bool IsAlive => _self != null && _target != null;
        public string Id => _id;

        public SmallBubble(string id, RectTransform self, RectTransform panel, RectTransform target,
            float baseSpeed, Vector2 dir, float referencePanelMin)
        {
            _id = id;
            _self = self;
            _panel = panel;
            _target = target;
            _baseSpeed = Mathf.Max(0f, baseSpeed);
            _referencePanelMin = Mathf.Max(1f, referencePanelMin);

            if (dir.sqrMagnitude < 1e-6f) dir = Vector2.up;
            dir.Normalize();

            _vel = dir * _baseSpeed;
        }

        public void OnUpdate(float dt)
        {
            Debug.Log("SmallBubble Alive:  " + IsAlive);
            if (!IsAlive) return;

            float k = GetPanelScaleK();
            float arriveR = ArriveRadius * k;
            float maxV = MaxSpeed * k;
            float cruiseV = _baseSpeed * k;
            float steer = Force * k;

            Vector2 pos = _self.anchoredPosition;

            // 这里假设 target 与 panel 在同一坐标系更稳。
            // 如果 target 不在同一 panel 下，建议改成：把 target 世界点转换到 panel 局部坐标（我之前给过 WorldToPanel 的写法）
            Vector2 targetPos = _target.anchoredPosition;

            Vector2 to = targetPos - pos;
            float dist = to.magnitude;

            if (dist <= arriveR)
            {
                Boom();
                return;
            }

            Vector2 desiredVel = (dist > 1e-6f) ? (to / dist) * cruiseV : Vector2.zero;
            Vector2 accel = (desiredVel - _vel) * steer;

            _vel += accel * dt;
            Debug.Log("SmallBubble Vel:  " + _vel);
            float v = _vel.magnitude;
            if (v > maxV) _vel = _vel / v * maxV;

            pos += _vel * dt;

            // 可选：限制在 panel 内（避免跑出去）
            pos = ClampInsidePanel(pos, arriveR);

            _self.anchoredPosition = pos;
        }

        public void ForceBoom() => Boom();

        private void Boom()
        {
            if (_self != null)
                Object.Destroy(_self.gameObject);
        }

        private float GetPanelScaleK()
        {
            if (_panel == null) return 1f;

            Vector2 s = _panel.rect.size;
            float panelMin = Mathf.Max(1f, Mathf.Min(Mathf.Abs(s.x), Mathf.Abs(s.y)));
            return panelMin / _referencePanelMin;
        }

        private Vector2 ClampInsidePanel(Vector2 pos, float padding)
        {
            if (_panel == null) return pos;

            Rect r = _panel.rect;
            pos.x = Mathf.Clamp(pos.x, r.xMin + padding, r.xMax - padding);
            pos.y = Mathf.Clamp(pos.y, r.yMin + padding, r.yMax - padding);
            return pos;
        }
    }

}

