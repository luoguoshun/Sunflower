using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OHEXML.Infrastructure.HostedServices.QueuedHostedService
{
    public class QueuedServiceCenter: BackgroundService
    {
        #region 构造函数
        public IBackgroundTaskQueue _TaskQueue { get; set; }
        private readonly ILogger<QueuedServiceCenter> _logger;

        public QueuedServiceCenter(IBackgroundTaskQueue taskQueue, ILogger<QueuedServiceCenter> logger)
        {
            _TaskQueue = taskQueue;
            _logger = logger;
        }
        #endregion

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                $"排队的托管服务正在运行.{Environment.NewLine}" +
                $"后台队列.{Environment.NewLine}"
                );
            await BackgroundProcessing(stoppingToken);
        }
        /// <summary>
        ///返回等待的任务 取消排队并执行队列中的后台任
        /// </summary>
        /// <param name="stoppingToken">取消令牌</param>
        /// <returns></returns>
        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //服务在 StopAsync 中停止之前，将等待工作项
                Func<CancellationToken, ValueTask> workItem = await _TaskQueue.DequeueAsync(stoppingToken);
                try
                {
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "执行时出错 {WorkItem}.", nameof(workItem));
                }
            }
        }
        /// <summary>
        /// 当应用程序主机正在执行正常关机时触发
        /// </summary>
        /// <param name="stoppingToken">取消令牌：指示关闭进程不再正常</param>
        /// <returns></returns>
        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("排队的托管服务正在停止.");
            await base.StopAsync(stoppingToken);
        }
    }
}
