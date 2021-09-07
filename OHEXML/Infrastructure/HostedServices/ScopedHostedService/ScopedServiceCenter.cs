using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OHEXML.Contracts.CommunicationModule;
using OHEXML.Entity.Context;
using OHEXML.Entity.Entities;
using OHEXML.Repository.CommunicationModelu;
using OHEXML.Repository.UserModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OHEXML.Infrastructure.HostedServices.ScopedHostedService
{
    public class ScopedServiceCenter : BackgroundService
    {
        private readonly ILogger _logger;
        //private readonly IMessageRepository _messageRepository;
        private readonly OHEsystemContext _dbContext;
        private readonly IUserRepository _userRepository;
        public ScopedServiceCenter(ILogger<ScopedServiceCenter> logger, OHEsystemContext dbContext, IMessageRepository messageRepository, IUserRepository userRepository)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userRepository = userRepository;
            //_messageRepository = messageRepository;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await PushNoticeToUserAsync(stoppingToken);
                await Task.Delay(5000, stoppingToken);
            }
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("推行通知服务正在关闭");
            return Task.CompletedTask;
        }
        public async Task PushNoticeToUserAsync(CancellationToken stoppingToken)
        {
            try
            {
                List<MessageInfo> data = await _dbContext.Set<MessageInfo>().ToListAsync();
                string result = JsonConvert.SerializeObject(data);
                if (string.IsNullOrEmpty(result))
                {
                    _logger.LogInformation("暂无数据");
                }
                else
                {
                    _logger.LogInformation(result);
                    await Task.Delay(3000, stoppingToken);
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
                await Task.Delay(2000, stoppingToken);
            }

        }
    }
}
