using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OHEXML.Entity.Entities;
using OHEXML.Repository.UserModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OHEXML.Infrastructure.HostedServices.QueuedHostedService
{
    /// <summary>
    /// 监视器回路
    /// </summary>
    public class MonitorLoop
    {
        #region 构造函数
        private readonly IBackgroundTaskQueue _TaskQueue;
        private readonly ILogger _logger;
        private readonly CancellationToken _cancellationToken;
        private readonly IAdminRepository _adminRepository;
        public MonitorLoop(IBackgroundTaskQueue taskQueue,ILogger<MonitorLoop> logger,IHostApplicationLifetime applicationLifetime,IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
            _TaskQueue = taskQueue;
            _logger = logger;
            _cancellationToken = applicationLifetime.ApplicationStopping;
        }

        #endregion

        /// <summary>
        /// 开启监视器异步循环
        /// </summary>
        public void StartMonitorLoop()
        {
            _logger.LogInformation("监视器异步循环正在启动.");
            // 在后台线程中运行控制台用户输入循环
            Task.Run(async () => await MonitorAsync());
        }
        /// <summary>
        /// 监视器异步
        /// </summary>
        /// <returns></returns>
        private async ValueTask MonitorAsync()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                // 将工作项排入队列
                await _TaskQueue.QueueBackgroundWorkItemAsync(BuildWorkItem1);
                await _TaskQueue.QueueBackgroundWorkItemAsync(BuildWorkItem2);
                await Task.Delay(2000, _cancellationToken);
            }
        }
        /// <summary>
        /// 建立工作项1
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async ValueTask BuildWorkItem1(CancellationToken token)
        {

            Employee1<int> emp = new Employee1<int>();
            Console.WriteLine(emp.BuyBook(10, x => { return $"我需要买{x}本书"; }));
            Console.WriteLine("我是索引:" + emp[0]);
            _logger.LogInformation("队列后台任务查询数据正在启动.");
            List<AdminInfo> data = await _adminRepository.GetAllAsync();
            if (data is null)
            {
                _logger.LogInformation("暂无数据");
            }
            else
            {
                _logger.LogInformation(JsonConvert.SerializeObject(data.FirstOrDefault()));
            }
        }
        /// <summary>
        /// 建立工作项2
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async ValueTask BuildWorkItem2(CancellationToken token)
        {
            //模拟三个5秒钟的任务来完成
            //对于每个排队的工作项
            int delayLoop = 0;
            _logger.LogInformation("队列后台任务2正在启动.");
            while (!token.IsCancellationRequested && delayLoop < 3)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(3), token);
                }
                catch (OperationCanceledException)
                {
                    //如果延迟被取消，防止抛出
                    throw;
                }
                delayLoop++;
                _logger.LogInformation("队列后台任务2正在运行. " + "{DelayLoop}/3", delayLoop);
            }
            if (delayLoop == 3)
            {
                _logger.LogInformation("队列后台任务2正在完成.");
            }
            else
            {
                _logger.LogInformation("队列后台任务2正在取消.");
            }
        }
    }

    public class Employee1<TType>
    {
        readonly float[] temps = new float[10]
        {
            56.2F, 56.7F, 56.5F, 56.9F, 58.8F,
            61.3F, 65.9F, 62.1F, 59.2F, 57.5F
        };
        //索引器允许类或结构的实例就像数组一样进行索引
        // 定义索引器以允许客户端代码使用[]表示法。
        public float this[int index]
        {
            get => temps[index];
            set => temps[index] = value;
        }
        /// <summary>
        /// 委托人帮我T本书 Func/<T_string/>是买书的动作，传入参数T,返回参数String
        /// </summary>
        /// <param name="num">数据类型由决定 这里指数量</param>
        /// <param name="buy">传入参数为int,返回参数为string的泛型委托</param>
        public string BuyBook<T>(T num, Func<T, string> buy) where T:notnull
        {
            return buy(num);
        }
    }
}
