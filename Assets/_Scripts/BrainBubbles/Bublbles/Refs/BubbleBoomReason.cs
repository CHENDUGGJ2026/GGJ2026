//Author : _SourceCode
//CreateTime : 2026-01-30-18:19:41
//Version : 1.0
//UnityVersion : 2022.3.62f2c1

namespace MyFrame.BrainBubbles.Bubbles.Refs
{
    public enum BubbleBoomReason
    {
        None = 0,
        Click = 1 << 0,
        TimeOff = 1 << 1,
    }

    public enum BubbleType
    {
        None = 0,
        Happy = 1<<0,
        Angry = 1<<1,
        Sad = 1<<2,
        Scared = 1<<3,
    }
}

