//Author : _SourceCode
//CreateTime : 2025-09-10-14:01:26
//Version : 0.1
//UnityVersion : 2022.3.62f1c1

#nullable enable
using MyFrame.EventSystem.Events;
using MyFrame.EventSystem.Interfaces;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MyFrame.EventSystem.Core
{
    /// <summary>
    /// 内存事件总线
    /// </summary>
    public class EventBusCore : IEventBusCore
    {
        protected readonly object Gate = new();
        protected readonly Dictionary<Type, List<Delegate>> Subscribers = new();
        protected readonly IEventErrorHandler ErrorHandler;

        /// <summary>
        /// 可注入异常处理器。
        /// </summary>
        public EventBusCore(IEventErrorHandler? errorHandler = null)
        {
            ErrorHandler = errorHandler ?? new DefaultErrorHandler();
        }

        /// <summary>
        /// 订阅指定类型的事件，返回可释放的退订句柄。
        /// </summary>
        /// <typeparam name="TEvent">事件类，实现 IEvent 事件标记接口</typeparam>
        /// <param name="handler">监听器</param>
        /// <returns>退订句柄</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            lock (Gate)
            {
                var key = typeof(TEvent);
                if (!Subscribers.TryGetValue(key, out var list))
                {
                    list = new List<Delegate>();
                    Subscribers[key] = list;
                }
                list.Add(handler);
            }

            return new Unsubscriber(this, typeof(TEvent), handler);
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <param name="evt">事件</param>
        /// <exception cref="ArgumentNullException">参数为空，抛出异常</exception>
        public virtual void Publish<TEvent>(TEvent evt) where TEvent : IEvent
        {
            if (evt == null) throw new ArgumentNullException(nameof(evt));

            List<Delegate>? snapshot = null;
            try
            {
                // 快照以避免分发过程中集合被修改导致的并发问题
                lock (Gate)
                {
                    if (Subscribers.TryGetValue(typeof(TEvent), out var list) && list.Count > 0)
                        snapshot = list.ToList();
                }

                if (snapshot == null) return;

                foreach (var d in snapshot)
                {
                    try
                    {
                        // 强类型调度：Action<TEvent>
                        if (d is Action<TEvent> typed)
                        {
                            typed(evt);
                        }
                        else
                        {
                            // 兼容边界（一般不会触发）：
                            d.DynamicInvoke(evt);
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler.OnHandlerException(evt!, d, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.OnPublishException(evt!, ex);
            }
        }

        private sealed class Unsubscriber : IDisposable
        {
            private readonly EventBusCore _bus;
            private readonly Type _type;
            private readonly Delegate _handler;
            private int _disposed; // 0/1

            public Unsubscriber(EventBusCore bus, Type type, Delegate handler)
            { _bus = bus; _type = type; _handler = handler; }

            public void Dispose()
            {
                if (Interlocked.Exchange(ref _disposed, 1) == 1) return;
                lock (_bus.Gate)
                {
                    if (_bus.Subscribers.TryGetValue(_type, out var list))
                    {
                        list.Remove(_handler);
                        if (list.Count == 0) _bus.Subscribers.Remove(_type);
                    }
                }
            }
        }
    }


}

