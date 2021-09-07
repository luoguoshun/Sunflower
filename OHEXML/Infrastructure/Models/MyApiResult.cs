using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OHEXML.Infrastructure.Models
{
    /// <summary>
    /// 通用返回信息类
    /// </summary>
    public class MyApiResult
    {
        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public object Data { get; set; }
    }
}
