//Author : _SourceCode
//CreateTime : 2026-01-30-18:19:41
//Version : 1.0
//UnityVersion : 2022.3.62f2c1

using MyFrame.BrainBubbles.Bubbles.BubbleMove.Interfaces;
using MyFrame.BrainBubbles.Bubbles.BubbleMove.Refs;
using MyFrame.BrainBubbles.Bubbles.Refs;
using System;
using UnityEngine;

namespace MyFrame.BrainBubbles.Bubbles.BubbleMove.Core
{
    public class BubbleMoveController : IBubbleMoveController
    {
        private SmallBubbleInfo _info;
        private RectTransform _transform;

        private float _r;
        private const float _maxSpeed = 10;
        public void BoomOut(BubbleType type, BubblePos pos, ValueType values)
        {
            if(!_info.TryCreateSmallBubble(type, out var obj)) return;
            RectTransform rect = obj.GetComponent<RectTransform>();
            if(rect == null) return;

            rect.SetParent(_transform);
            float x = UnityEngine.Random.Range(-0.01f * _transform.rect.width, 0.01f * _transform.rect.width);
            float y = UnityEngine.Random.Range(-0.01f * _transform.rect.height, 0.01f * _transform.rect.height);
            
            float dir = UnityEngine.Random.Range(-1, 1);
            float speed = UnityEngine.Random.Range(0 , _maxSpeed);

            rect.SetLocalPositionAndRotation(new Vector3(pos.X + x,pos.Y + y),Quaternion.identity);


        }

        public void OnUpdate(float time)
        {
            throw new NotImplementedException();
        }
    }
}

