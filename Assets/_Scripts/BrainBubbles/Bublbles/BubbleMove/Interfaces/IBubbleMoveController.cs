//Author : _SourceCode
//CreateTime : 2026-01-30-18:19:41
//Version : 1.0
//UnityVersion : 2022.3.62f2c1

using MyFrame.BrainBubbles.Bubbles.Refs;
using System;

namespace MyFrame.BrainBubbles.Bubbles.BubbleMove.Interfaces
{
    public interface IBubbleMoveController
    {
        void BoomOut(BubbleType type,BubblePos pos, ValueType values);
        void OnUpdate(float time);
    }
}

