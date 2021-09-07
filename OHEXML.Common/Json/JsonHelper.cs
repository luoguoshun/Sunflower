using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OHEXML.Common.Json
{
    /// <summary>
    /// 成功返回
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonSuccess<T>
    {
        public JsonSuccess(string message, T data)
        {
            Success = true;
            Message = message;
            Data = data;
        }
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

    }
    /// <summary>
    /// 失败返回
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonFail<T>
    {
        public JsonFail(string message, T data)
        {
            Success = false;
            Message = message;
            Data = data;
        }
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

}
