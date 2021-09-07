using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OHEXML.SignalR
{
    /// <summary>
    /// 防止使用 magic 字符串导致的问题，因为 Hub<T> 只能提供对 接口中定义的方法的访问。
    /// </summary>
    public class ChatClientServer : IChatClientServer
    {
        #region 构造函数
        private readonly List<HubUserDTO> Users;
        private readonly object _obj;
        public ChatClientServer()
        {
            Users = new List<HubUserDTO>();
            _obj = new object();
        }
        #endregion

        /// <summary>
        /// 添加信息
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="connectionId"></param>
        public void AddUser(string UserID, string connectionId)
        {
            //持有 lock 时持有lock的线程可以再次获取并释放 lock。阻止任何其他线程获取 lock 并等待释放 lock。
            lock (_obj)
            {
                Users.Add(new HubUserDTO
                {
                    UserID = UserID,
                    ConnectionId = connectionId,
                });
            }
        }
        /// <summary>
        /// 移除信息
        /// </summary>
        /// <param name="id"></param>
        public void RemoveUser(string UserID)
        {
            lock (_obj)
            {
                var index = Users.Select(x => x.UserID).ToList().IndexOf(UserID);
                if (index != -1 && Users.Select(x => x.UserID).Contains(UserID))
                {
                    Users.RemoveAt(index);
                }
            }
        }
        /// <summary>
        /// 修改用户连接ID
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="connectionId"></param>
        public void UpdateUser(string UserID, string connectionId)
        {
            lock (_obj)
            {
                HubUserDTO user = Users.Where(x => x.UserID == UserID).FirstOrDefault();
                if (user != null)
                {
                    user.ConnectionId = connectionId;
                }
            }
        }
        /// <summary>
        /// 获取用户连接ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetClientConnectionId(string id)
        {
            return Users.Where(x => x.UserID == id).FirstOrDefault()?.ConnectionId;
        }
    }
}
