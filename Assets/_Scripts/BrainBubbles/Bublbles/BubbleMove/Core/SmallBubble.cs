//Author : _SourceCode
//CreateTime : 2026-01-30-18:19:41
//Version : 1.0
//UnityVersion : 2022.3.62f2c1

using MyFrame.BrainBubbles.Bubbles.BubbleMove.Interfaces;
using MyFrame.BrainBubbles.Bubbles.Refs;
using UnityEngine;

namespace MyFrame.BrainBubbles.Bubbles.BubbleMove.Core
{
    using UnityEngine;

    public sealed class SmallBubble
    {
        private readonly RectTransform _self;
        private readonly RectTransform _panel;
        private readonly RectTransform _target;

        private readonly string _id;

        // 速度向量（panel局部坐标系下，UI单位/秒）
        private Vector2 _vel;

        // 基础参数（以 referencePanelMin 下为调参基准）
        private readonly float _baseSpeed;
        private readonly float _referencePanelMin;

        // === 调参（基于 referencePanelMin） ===
        private const float ArriveRadius = 50f;

        // 最大速度（UI单位/秒）
        private const float MaxSpeed = 2000f;

        // 转向角速度（单位：rad/s）—— 注意：现在 Force 语义变了，不再是“转向加速度”
        private const float Force = 6f;

        // 推进加速度（UI单位/秒^2）
        private const float Throttle = 1000f;

        // 若 target 与 self 锚点/父级不同，建议开这个（统一到 panel 局部坐标）
        private readonly bool _useWorldToPanel = true;

        public bool IsAlive => _self != null && _target != null;
        public string Id => _id;

        public SmallBubble(
            string id,
            RectTransform self,
            RectTransform panel,
            RectTransform target,
            float baseSpeed,
            Vector2 dir,
            float referencePanelMin,
            bool useWorldToPanel = true)
        {
            _id = id;
            _self = self;
            _panel = panel != null ? panel : (self != null ? self.parent as RectTransform : null);
            _target = target;

            _baseSpeed = Mathf.Max(0f, baseSpeed);
            _referencePanelMin = Mathf.Max(1f, referencePanelMin);
            _useWorldToPanel = useWorldToPanel;

            if (dir.sqrMagnitude < 1e-6f) dir = Vector2.up;
            dir.Normalize();

            // 初速度：随机方向 * 初始速度
            _vel = dir * _baseSpeed;
        }

        public void OnUpdate(float dt)
        {
            if (!IsAlive) return;
            if (dt <= 0f) return;

            float k = GetPanelScaleK();

            float arriveR = ArriveRadius * k;
            float maxV = MaxSpeed * k;
            float cruiseV = _baseSpeed * k;

            // Force 现在代表“最大转向角速度”（rad/s）
            float turnRateRad = Force * k;

            // 推进加速度（单位/秒^2）
            float throttle = Throttle * k;

            // === 统一坐标（避免 anchor/pivot 不同导致 anchoredPosition 不可比）===
            // 计算“当前位置/目标位置”用 panel 局部坐标（推荐）
            Vector2 posLocal = GetSelfPosInPanel();
            Vector2 targetLocal = GetTargetPosInPanel();

            Vector2 to = targetLocal - posLocal;
            float dist = to.magnitude;

            if (dist <= arriveR)
            {
                Boom();
                return;
            }

            Vector2 dirTo = (dist > 1e-6f) ? (to / dist) : Vector2.zero;

            // --- 1) 方向：按最大角速度逐渐转向目标方向 ---
            Vector2 curDir = (_vel.sqrMagnitude > 1e-6f) ? _vel.normalized : dirTo;

            // SignedAngle 返回度
            float deltaDeg = Vector2.SignedAngle(curDir, dirTo);

            // 每帧最多转的角度（度）
            float maxTurnDeg = turnRateRad * Mathf.Rad2Deg * dt;

            float turnDeg = Mathf.Clamp(deltaDeg, -maxTurnDeg, maxTurnDeg);
            Vector2 newDir = Rotate(curDir, turnDeg);

            // --- 2) 速度大小：推进加速到目标速度 ---
            // 你可以用 cruiseV（巡航）或 maxV（持续加速到上限）
            float targetSpeed = maxV;

            float speed = _vel.magnitude;

            // 可选：靠近目标减速（让吸附更柔和）
            // 例如在 8 倍到达半径范围内开始压低目标速度
            float slowRange = arriveR * 8f;
            if (slowRange > 1e-3f)
            {
                float t = Mathf.Clamp01(dist / slowRange);
                targetSpeed = Mathf.Min(targetSpeed, Mathf.Lerp(0f, maxV, t));
                targetSpeed = Mathf.Max(targetSpeed, cruiseV * 0.2f); // 不要降到完全停住（可按喜好删）
            }

            // 本帧增加的速度量
            speed = Mathf.Min(speed + throttle * dt, targetSpeed);

            // 合成最终速度
            _vel = newDir * Mathf.Min(speed, maxV);

            // --- 3) 写回位置（使用 anchoredPosition 做移动） ---
            // 注意：移动最好直接用 anchoredPosition（小球就在 panel 下）
            Vector2 pos = _self.anchoredPosition;
            pos += _vel * dt;

            // 可选：限制在 panel 内
            // pos = ClampInsidePanel(pos, arriveR);

            _self.anchoredPosition = pos;
        }

        public void ForceBoom() => Boom();

        private void Boom()
        {
            if (_self != null)
                Object.DestroyImmediate(_self.gameObject);
        }

        private float GetPanelScaleK()
        {
            if (_panel == null) return 1f;

            Vector2 s = _panel.rect.size;
            float panelMin = Mathf.Max(1f, Mathf.Min(Mathf.Abs(s.x), Mathf.Abs(s.y)));
            return panelMin / _referencePanelMin;
        }

        // ======== 坐标统一：把 RectTransform 的中心点转换到 panel 局部坐标 ========
        private Vector2 GetSelfPosInPanel()
        {
            if (!_useWorldToPanel || _panel == null) return _self.anchoredPosition;
            return RectToPanelLocal(_self);
        }

        private Vector2 GetTargetPosInPanel()
        {
            if (!_useWorldToPanel || _panel == null) return _target.anchoredPosition;
            return RectToPanelLocal(_target);
        }

        private Vector2 RectToPanelLocal(RectTransform rt)
        {
            // Overlay: cam = null；Camera/WorldSpace: 用 canvas.worldCamera
            Camera cam = GetUICamera(_panel);

            Vector3 world = rt.TransformPoint(rt.rect.center);
            Vector2 screen = RectTransformUtility.WorldToScreenPoint(cam, world);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_panel, screen, cam, out var local);
            return local;
        }

        private static Camera GetUICamera(RectTransform any)
        {
            var canvas = any != null ? any.GetComponentInParent<Canvas>() : null;
            if (canvas == null) return null;
            return canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        }

        // ======== helper: 2D 向量旋转（角度制） ========
        private static Vector2 Rotate(Vector2 v, float deg)
        {
            float rad = deg * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
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

