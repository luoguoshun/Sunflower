using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OHEXML.Server.HostedModule
{
    public interface IBackgroundTaskQueue
    {
        /// <summary>
        /// 后台任务工作项异步，返回异步操作的等待结果 
        /// </summary>
        /// <param name="workItem">泛型委托，传入参数为CancellationToken，返回参数为ValueTask的方法</param>
        /// <returns></returns>
        ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem);
        /// <summary>
        /// 异步出列
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
    }
}
