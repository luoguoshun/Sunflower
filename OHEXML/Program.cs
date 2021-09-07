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

            #region ����ʱ������������ķ����Ա����г�ʼ������
            // ʹ�� IServiceScopeFactory.CreateScope ���� IServiceScope �Խ���Ӧ�÷�Χ�ڵ���������񡣴˷�����������������ʱ������������ķ����Ա����г�ʼ������
            //����ʾ����ʾ��η��ʷ�Χ�� IMyDependency ������ Program.Main �е����� WriteMessage ������
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
                         //logging.AddFilter("System", LogLevel.Error); //���˵�ϵͳĬ�ϵ�һЩ��־   
                         //logging.AddFilter("Microsoft", LogLevel.Error);//���˵�ϵͳĬ�ϵ�һЩ��־
                         ////������������ʾlog4net.config�������ļ�����Ӧ�ó����Ŀ¼�£�Ҳ����ָ�������ļ���·��
                         //logging.AddLog4Net(Path.Combine(Environment.CurrentDirectory, "Log4Net.config"));
                     });
                });

    }
}
