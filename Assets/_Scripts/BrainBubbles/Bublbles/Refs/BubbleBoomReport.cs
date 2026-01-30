//Author : _SourceCode
//CreateTime : 2026-01-30-18:19:41
//Version : 1.0
//UnityVersion : 2022.3.62f2c1

using MyFrame.BrainBubbles.Bubbles.Core;

namespace MyFrame.BrainBubbles.Bubbles.Refs
{
    /// <summary>
    /// Bubble Boom Report
    /// Reason
    /// BubbleID
    /// </summary>
    public sealed record BubbleBoomReport(BubbleBoomReason Reason ,TypeValue BubbleType, string BubbleId ,string Message);
    
}

