using log4net;
using System;
using System.Reflection;

namespace OHEXML.Common.LogHelper
{
    public static class Log4NetHelper
    {
        //log4net日志初始化 这里就读取多个配置节点
        //GetLogger()参数说明：参数一=》//存储组件：用于查找存储库的程序集。参数二=》要检索的记录器的名称。       
        //private static string path => Environment.CurrentDirectory + @"\Log4NetRepository";
        private static readonly ILog _logError = LogManager.GetLogger(Assembly.GetCallingAssembly(), "LogError");
        private static readonly ILog _logNormal = LogManager.GetLogger(Assembly.GetCallingAssembly(), "LogNormal");
        private static readonly ILog _logAOP = LogManager.GetLogger(Assembly.GetCallingAssembly(), "LogAOP");

        public static void LogErrException(string info, Exception ex)
        {
            //Error(object message,Exception exception)
            // 参数一==>要记录的消息对象:object message
            //参数二==>日志的异常，包括其堆栈跟踪：Exception exception
            if (_logError.IsErrorEnabled)
            {
                _logError.Error(info, ex);
            }
        }
        #region Error()记录程序出错后返回的错误信息
        public static void LogErr(string message)
        {
            if (_logError.IsErrorEnabled)
            {
                _logError.Error(message);
            }
        }
        public static void LogErr(string errkey, string errmsg)
        {
            if (_logError.IsErrorEnabled)
            {
                _logError.Error(errkey + ":" + errmsg);
            }
        }
        public static void LogErr(string message, Exception ex)
        {
            if (_logError.IsErrorEnabled)
            {
                _logError.Error(message, ex);
            }
        }
        #endregion
        public static void LogNormal(string info)
        {
            if (_logNormal.IsInfoEnabled)
            {
                _logNormal.Info(info);
            }
        }

        public static void LogAOP(string key, string info)
        {
            if (_logAOP.IsInfoEnabled)
            {
                _logAOP.Info($"{key}:{info}");
            }
        }
        public static void LogAOP(string info)
        {
            if (_logAOP.IsInfoEnabled)
            {
                _logAOP.Info(info);
            }
        }
    }
}
