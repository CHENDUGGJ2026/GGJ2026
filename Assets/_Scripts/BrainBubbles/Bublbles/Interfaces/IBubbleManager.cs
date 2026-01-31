//Author : _SourceCode
//CreateTime : 2026-01-30-18:19:41
//Version : 1.0
//UnityVersion : 2022.3.62f2c1

using MyFrame.BrainBubbles.Bubbles.Core;
using MyFrame.BrainBubbles.Bubbles.Refs;

namespace MyFrame.BrainBubbles.Bubbles.Interfaces
{
    public interface IBubbleManager
    {
        bool NewBubble(BubblePos pos,out BubbleBase bubble);
        void OnStart();
        void OnUpdate(float deltaTime);
        void RemoveBubble(string id);
    }
}