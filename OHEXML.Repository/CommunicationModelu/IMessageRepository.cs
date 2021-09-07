using OHEXML.Entity.Entities;
using OHEXML.Repository.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OHEXML.Repository.CommunicationModelu
{
    public interface IMessageRepository: IRepository<MessageInfo>
    {
        /// <summary>
        /// 获取消息列表
        /// </summary>
        /// <param name="recevierId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<IEnumerable<MessageInfo>> GetMessagesAsync(string senderId, string receiverId, string time);
    }
}
