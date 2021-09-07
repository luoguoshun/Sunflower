using System;
using System.Collections.Generic;
using System.Text;

namespace OHEXML.Common.Context
{
    /// <summary>
    /// 上下文
    /// </summary>
    public class ApiContext
    {
        public ApiContext()
        {
            User = new ApiUser();
        }
        public ApiUser User { get; set; }
    }
}
