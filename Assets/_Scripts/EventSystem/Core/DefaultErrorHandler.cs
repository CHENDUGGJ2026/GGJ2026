//Author : _SourceCode
//CreateTime : 2025-09-10-14:01:26
//Version : 0.1
//UnityVersion : 2022.3.62f1c1

#nullable enable
using MyFrame.EventSystem.Interfaces;
using System;

namespace MyFrame.EventSystem.Events
{
    /// <summary>
    /// 默认异常处理：开发期打印，生产可替换为 ILogger / Sentry / 自研监控。
    /// </summary>
    public sealed class DefaultErrorHandler : IEventErrorHandler
    {
        public void OnPublishException(object evt, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EventBus] Publish error for {evt.GetType().Name}: {ex}");
        }
        public void OnHandlerException(object evt, Delegate handler, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EventBus] Handler {handler.Method.DeclaringType?.Name}.{handler.Method.Name} error: {ex}");
        }
    }


}

