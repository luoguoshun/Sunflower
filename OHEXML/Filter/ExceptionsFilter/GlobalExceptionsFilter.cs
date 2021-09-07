using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OHEXML.Common.Json;
using OHEXML.Common.LogHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OHEXML.Filter.ExceptionsFilter
{
    /// <summary>
    /// 群居异常出路过滤器(只能管到Controller 之外的异常无法捕获)
    /// </summary>
    public class GlobalExceptionsFilter : IAsyncExceptionFilter
    {
        public Task OnExceptionAsync(ExceptionContext context)
        {
            if (context.ExceptionHandled == false)
            {
                Exception ex = context.Exception;
                //这里给系统分配标识，监控异常肯定不止一个系统。
                int sysId = 1;
                //这里获取服务器ip时，需要考虑如果是使用nginx做了负载，这里要兼容负载后的ip，
                //监控了ip方便定位到底是那台服务器出故障了
                string ip = context.HttpContext.Connection.RemoteIpAddress.ToString();
                var uri = context.HttpContext.Request.Path.Value;
                Log4NetHelper.LogErr("Controller全局异常"+ $"系统编号：{sysId},主机IP:{ip}\r\r<br>异常Uri：{uri}\r\r<br>异常描述：{ex.Message}\r\r<br>堆栈信息：{ex.StackTrace}");

                context.Result = new JsonResult("Controller全局异常拦截：" + ex.Message);
            }
            context.ExceptionHandled = true; //异常已处理了

            return Task.CompletedTask;
        }
    }
}
