//Author : _SourceCode
//CreateTime : 2026-01-30-18:19:41
//Version : 1.0
//UnityVersion : 2022.3.62f2c1

using MyFrame.BrainBubbles.Bubbles.BubbleMove.Interfaces;
using MyFrame.BrainBubbles.Bubbles.Refs;
using UnityEngine;

namespace MyFrame.BrainBubbles.Bubbles.BubbleMove.Core
{

    public class SmallBubble : ISmallBubble
    {
        private readonly RectTransform _self;      // 小球 RectTransform
        private readonly RectTransform _panel;     // 小球所在 Panel（父物体）
        private readonly RectTransform _target;    // 目标 RectTransform

        private Vector2 _vel;                      // 当前速度（anchored单位/秒）
        private readonly float _baseSpeed;         // 参考尺寸下巡航速度
        private readonly Vector2 _initDir;

        // 以下常量以“参考面板尺寸”下为调参基准
        private const float ArriveRadius = 5f;
        private const float MaxSpeed = 500f;
        private const float Force = 10f;

        // 参考面板最小边（你目标 3840x2160，min=2160）
        private const float ReferencePanelMin = 2160f;

        // 如果你希望：目标不在同一父级，也能正确吸引
        // true：把目标世界点转到 panel 的局部坐标，再作为目标点
        // false：直接用 target.anchoredPosition（要求同父级/同坐标系）
        private readonly bool _useWorldToPanel = true;

        public SmallBubble(RectTransform self, RectTransform target, float speed, Vector2 dir)
        {
            _self = self;
            _panel = self != null ? self.parent as RectTransform : null;
            _target = target;

            _baseSpeed = Mathf.Max(0f, speed);
            _initDir = (dir.sqrMagnitude < 1e-6f) ? Random.insideUnitCircle : dir.normalized;

            // 初速度（每帧会再按 panel 缩放补偿）
            _vel = _initDir * _baseSpeed;
        }

        public void OnUpdate(float dt)
        {
            if (_self == null || _target == null) return;

            // 1) 根据 Panel 大小缩放参数：保证“相对面板尺度”的观感速度一致
            float k = GetPanelScaleK();
            float arriveR = ArriveRadius * k;
            float maxV = MaxSpeed * k;
            float cruiseV = _baseSpeed * k;
            float steer = Force * k;

            Vector2 pos = _self.anchoredPosition;
            Vector2 targetPos = GetTargetPosInPanel();

            Vector2 to = targetPos - pos;
            float dist = to.magnitude;

            // 2) 到达范围：销毁/回收
            if (dist <= arriveR)
            {
                Boom();
                return;
            }

            // 3) Steering：逐步转向目标（更丝滑）
            Vector2 desiredVel = (dist > 1e-6f) ? (to / dist) * cruiseV : Vector2.zero;
            Vector2 accel = (desiredVel - _vel) * steer;

            _vel += accel * dt;

            // 4) 限速
            float v = _vel.magnitude;
            if (v > maxV) _vel = _vel / v * maxV;

            // 5) 移动
            pos += _vel * dt;

            // 6) 可选：限制在 Panel 内
            pos = ClampInsidePanel(pos, arriveR);

            _self.anchoredPosition = pos;
        }

        public void Boom()
        {
            if (_self != null)
            {
                // 如果你用对象池，把 Destroy 换成 pool.Release(...)
                Object.Destroy(_self.gameObject);
            }
        }

        /// <summary>
        /// k = 当前Panel最小边 / 参考Panel最小边
        /// Panel越大 -> k越大 -> 速度/力/半径越大 -> 相对尺度观感一致
        /// </summary>
        private float GetPanelScaleK()
        {
            if (_panel == null) return 1f;

            Vector2 size = _panel.rect.size;
            float panelMin = Mathf.Max(1f, Mathf.Min(Mathf.Abs(size.x), Mathf.Abs(size.y)));
            return panelMin / Mathf.Max(1f, ReferencePanelMin);
        }

        /// <summary>
        /// 获取目标点在 Panel 局部坐标系下的位置（与 self.anchoredPosition 同坐标系）
        /// </summary>
        private Vector2 GetTargetPosInPanel()
        {
            if (!_useWorldToPanel)
            {
                // 要求：target 与 self 同坐标系（通常同一个 panel 下）
                return _target.anchoredPosition;
            }

            // 更稳：把目标的世界点投到 panel 局部坐标
            // 注意：UI 的 panel 通常在 ScreenSpace-Overlay，需要用 RectTransformUtility
            Vector3 world = _target.TransformPoint(_target.rect.center);
            return WorldToPanelLocal(world);
        }

        private Vector2 WorldToPanelLocal(Vector3 worldPoint)
        {
            if (_panel == null) return _self.anchoredPosition;

            var canvas = _panel.GetComponentInParent<Canvas>();
            Camera cam = null;

            // ScreenSpace-Overlay: cam = null
            // ScreenSpace-Camera / WorldSpace: cam = canvas.worldCamera
            if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                cam = canvas.worldCamera;

            Vector2 screen = RectTransformUtility.WorldToScreenPoint(cam, worldPoint);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_panel, screen, cam, out var local);
            return local;
        }

        private Vector2 ClampInsidePanel(Vector2 pos, float padding)
        {
            if (_panel == null) return pos;

            Rect r = _panel.rect;
            float left = r.xMin + padding;
            float right = r.xMax - padding;
            float bottom = r.yMin + padding;
            float top = r.yMax - padding;

            pos.x = Mathf.Clamp(pos.x, left, right);
            pos.y = Mathf.Clamp(pos.y, bottom, top);
            return pos;
        }
    }
}

