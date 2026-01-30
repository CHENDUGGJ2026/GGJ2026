//Author : _SourceCode
//CreateTime : 2025-09-10-14:16:19
//Version : 0.1
//UnityVersion : 2022.3.62f1c1

using System;
namespace MyFrame.EventSystem.Interfaces
{
    /// <summary>
    /// 核心事件总线接口：仅保留最基础能力。
    /// </summary>
    public interface IEventBusCore
    {
        /// <summary>订阅指定类型的事件，返回可释放的退订句柄。</summary>
        IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent;
        /// <summary>发布事件（同步、立即分发）。</summary>
        void Publish<TEvent>(TEvent evt) where TEvent : IEvent;
    }

}
