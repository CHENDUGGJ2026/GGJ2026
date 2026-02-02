//Author : _SourceCode
//CreateTime : 2026-01-30-18:19:41
//Version : 1.0
//UnityVersion : 2022.3.62f2c1

using System;

namespace MyFrame.BrainBubbles.Bubbles.Refs
{
    public readonly struct BubblePos
    {
        public int X { get; }
        public int Y { get; }

        public BubblePos(int x, int y)
        {
            X = x; Y = y;
        }
    }
}

