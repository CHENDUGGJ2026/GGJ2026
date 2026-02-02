//Author : _SourceCode
//CreateTime : 2026-01-30-18:19:41
//Version : 1.0
//UnityVersion : 2022.3.62f2c1

namespace MyFrame.BrainBubbles.Bubbles.Refs
{
    public record BubbleMoveReport(string BubbleId,BubblePos OldPos,BubblePos NewPos, string Message = "");
}

