using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OHEXML.SignalR
{
   public interface IChatClientServer
    {
        /// <summary>
        /// 添加信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="connectionId"></param>
        void AddUser(string id, string connectionId);
        /// <summary>
        /// 移除信息
        /// </summary>
        /// <param name="id"></param>
        void RemoveUser(string id);
        /// <summary>
        /// 修改用户连接ID
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="connectionId"></param>
        void UpdateUser(string UserID, string connectionId);
        /// <summary>
        /// 获取用户连接ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetClientConnectionId(string id);
    }
}
