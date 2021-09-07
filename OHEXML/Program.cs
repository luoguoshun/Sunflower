using System;
using System.IO;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OHEXML.Infrastructure.HostedServices.QueuedHostedService;

namespace OHEXML
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var host = CreateHostBuilder(args).Build();
            //var monitorLoop = host.Services.GetRequiredService<MonitorLoop>();
            //monitorLoop.StartMonitorLoop();
            //host.Run();
            CreateHostBuilder(args).Build().Run();

            #region 启动时访问有作用域的服务以便运行初始化任务。
            // 使用 IServiceScopeFactory.CreateScope 创建 IServiceScope 以解析应用范围内的作用域服务。此方法可以用于在启动时访问有作用域的服务以便运行初始化任务。
            //以下示例演示如何访问范围内 IMyDependency 服务并在 Program.Main 中调用其 WriteMessage 方法：
            //var host = CreateHostBuilder(args).Build();

            //using (var serviceScope = host.Services.CreateScope())
            //{
            //    var services = serviceScope.ServiceProvider;

            //    try
            //    {
            //        var myDependency = services.GetRequiredService<IMyDependency>();
            //        myDependency.WriteMessage("Call services from main");
            //    }
            //    catch (Exception ex)
            //    {
            //        var logger = services.GetRequiredService<ILogger<Program>>();
            //        logger.LogError(ex, "An error occurred.");
            //    }
            //}

            //host.Run();
            #endregion
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args).UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                    .ConfigureLogging((hostingContext, logging) =>
                     {
                         //logging.AddFilter("System", LogLevel.Error); //过滤掉系统默认的一些日志   
                         //logging.AddFilter("Microsoft", LogLevel.Error);//过滤掉系统默认的一些日志
                         ////不带参数：表示log4net.config的配置文件就在应用程序根目录下，也可以指定配置文件的路径
                         //logging.AddLog4Net(Path.Combine(Environment.CurrentDirectory, "Log4Net.config"));
                     });
                });

    }
}
