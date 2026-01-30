//Author : _SourceCode
//CreateTime : 2025-09-10-14:16:19
//Version : 0.1
//UnityVersion : 2022.3.62f1c1

using System;

namespace MyFrame.EventSystem.Interfaces
{
    /// <summary>
    /// 异常处理策略接口。通过注入自定义实现可接入日志/告警系统。
    /// </summary>
    public interface IEventErrorHandler
    {
        void OnPublishException(object evt, Exception ex);
        void OnHandlerException(object evt, Delegate handler, Exception ex);
    }

}
