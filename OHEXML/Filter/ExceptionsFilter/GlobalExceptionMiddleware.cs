using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OHEXML.Common.Json;
using OHEXML.Common.LogHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OHEXML.Filter.ExceptionsFilter
{
    /// <summary>
    /// 全局异常拦截器 GlobalExceptionsFilter无法拦截的在此拦截
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        #region 构造函数依赖注入
        private readonly RequestDelegate _next;
        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        #endregion

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception e)
        {
            int sysId = 1;//可以配置
            string ip = context.Connection.RemoteIpAddress.ToString();
            var uri = context.Request.Path.Value;
            Log4NetHelper.LogErr("Middleware全局异常", $"系统编号：{sysId},主机IP:{ip}\r\r<br>异常Uri：{uri}\r\r<br>异常描述：{e.Message}\r\r<br>堆栈信息：\r\r<br>{e.StackTrace}");
            context.Response.ContentType = "application/json;charset=utf-8";
            await context.Response.WriteAsync("Middleware全局异常拦截：" + e.Message);
        }
    }
}
