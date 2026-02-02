//Author : _SourceCode
//CreateTime : 2026-01-30-18:19:41
//Version : 1.0
//UnityVersion : 2022.3.62f2c1

using MyFrame.BrainBubbles.Bubbles.Refs;
using System.Collections.Generic;

namespace MyFrame.BrainBubbles.Bubbles.Interfaces
{
    /// <summary>
    /// Bubble: 
    /// Core£º
    /// Boom
    /// The Reason May Cause Boom:On Click£¬Time Off
    /// </summary>
    public interface IBubble
    {
        BubbleBoomReport Boom(BubbleBoomReason reason , string message = null);
    }



    
}
