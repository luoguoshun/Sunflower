using System;
using System.Collections.Generic;
using System.Text;

namespace OHEXML.SignalR
{
    internal class HubUserDTO
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 连接ID
        /// </summary>
        public string ConnectionId { get; set; }
    }
}
