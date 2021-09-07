using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace OHEXML.Infrastructure.HostedServices.QueuedHostedService
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        //Channel为支持读写元素的通道提供基类
        private readonly Channel<Func<CancellationToken, ValueTask>> _queue;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="capacity">容量</param>
        public BackgroundTaskQueue(int capacity)
        {
            //容量应根据预期的应用程序负载和访问队列的并发线程数。
            //BoundedChannelFullMode.Wait将导致对WriteAsync()的调用返回任务，
            //只有在有空间的时候才能完成。这会导致背压，以防太多的发布者/呼叫开始累积。
            var options = new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(options);
        }
        /// <summary>
        /// 后台任务工作项
        /// </summary>
        /// <param name="workItem"></param>
        /// <returns></returns>
        public async ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }
            await _queue.Writer.WriteAsync(workItem);
        }
        /// <summary>
        /// 异步出列
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken)
        {
            //ReadAsync(取消令牌)从通道异步读取项
            Func<CancellationToken, ValueTask> workItem = await _queue.Reader.ReadAsync(cancellationToken);
            return workItem;
        }
    }
}
