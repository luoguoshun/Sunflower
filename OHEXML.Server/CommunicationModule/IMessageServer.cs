using OHEXML.Contracts.CommunicationModule;
using System.Threading.Tasks;

namespace OHEXML.Server.CommunicationModule
{
    public interface IMessageServer
    {
        /// <summary>
        /// 发送信息给指定用户
        /// </summary>
        /// <param name="senderId">发送人</param>
        /// <param name="receiverId">接收人</param>
        /// <param name="context">内容</param>
        /// <returns></returns>
        Task<MessageDTO> SendMessageToPrivateAsync(string senderId, string receiverId, string context);
        /// <summary>
        /// 发送通告给所有用户
        /// </summary>
        Task SendMessageToAllAsync(string user, string message);
    }
}
